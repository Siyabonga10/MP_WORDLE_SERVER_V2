using Microsoft.EntityFrameworkCore;
using MP_WORDLE_SERVER_V2.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using MP_WORDLE_SERVER_V2.Services;
using Serilog;
using MP_WORDLE_SERVER_V2.Constants;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContextFactory<GameCache>(
    options => options.UseInMemoryDatabase("temp"),
    ServiceLifetime.Singleton
);

builder.Host.UseSerilog((context, configuration) =>
    configuration.ReadFrom.Configuration(context.Configuration)
);

var conn_string = builder.Configuration.GetConnectionString("DefaultConnection");

if (builder.Environment.IsProduction())
{
    conn_string = Environment.GetEnvironmentVariable("APPSETTING_DB_ConnectionString");
    if(conn_string == null)
        throw new("Could not connect to database");
}

builder.Services.AddDbContextFactory<GameDb>(
    options => options.UseNpgsql(conn_string)
);

builder.Services.AddOpenApi();

var jwt_key = Environment.GetEnvironmentVariable("JWT_KEY") ?? "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa";
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = "MpWordle.com",
            ValidAudience = "MpWordle.com",
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt_key))
        };
        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                if (string.IsNullOrEmpty(context.Token))
                {
                    context.Token = context.Request.Cookies["jwt_token"];
                }
                return Task.CompletedTask;  
            }
        };
    });
builder.Services.AddAuthorization();

builder.Services.AddScoped<PlayerService>();
builder.Services.AddSingleton<GameManagementService>();
builder.Services.AddSingleton<IWordManager, TestWordManager>();


builder.Services.AddControllers(); 

var app = builder.Build();

app.UseHttpsRedirection();
app.MapControllers();
app.UseSerilogRequestLogging();

app.Run();