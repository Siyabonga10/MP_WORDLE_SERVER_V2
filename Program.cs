using Microsoft.EntityFrameworkCore;
using MP_WORDLE_SERVER_V2.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using MP_WORDLE_SERVER_V2.Services;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();

var connectionString = builder.Configuration.GetConnectionString("Game") ?? "Data Source=Game.db";
builder.Services.AddSqlite<GameDb>(connectionString);
builder.Services.AddOpenApi();

var jwt_key = Environment.GetEnvironmentVariable("JWT_KEY") ?? "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa"; // 256 bits
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

builder.Services.AddScoped<GameDb>();
builder.Services.AddScoped<PlayerService>();

builder.Services.AddAuthorization();

var app = builder.Build();
app.UseHttpsRedirection();
app.MapControllers();
app.Run();