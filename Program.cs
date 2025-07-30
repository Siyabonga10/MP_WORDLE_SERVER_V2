using Microsoft.EntityFrameworkCore;
using MP_WORDLE_SERVER_V2.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using MP_WORDLE_SERVER_V2.Services;
using Azure.Identity;
using Serilog;


var builder = WebApplication.CreateBuilder(args);

Console.WriteLine("Beginning app building");

if (!builder.Environment.IsDevelopment())
{
    var keyVaultUri = new Uri($"https://mpwordle.vault.azure.net/");
    builder.Configuration.AddAzureKeyVault(
        keyVaultUri,
        new DefaultAzureCredential()
    );
}

builder.Services.AddDbContextFactory<GameCache>(
    options => options.UseInMemoryDatabase("temp"),
    ServiceLifetime.Singleton
);

Console.WriteLine("Helllooooo");

builder.Host.UseSerilog((context, configuration) =>
    configuration.ReadFrom.Configuration(context.Configuration)
);

builder.Services.AddDbContextFactory<GameDb>(
    options => options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"))
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