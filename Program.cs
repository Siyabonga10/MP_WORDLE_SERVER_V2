using Microsoft.EntityFrameworkCore;
using MP_WORDLE_SERVER_V2.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("Game") ?? "Data Source=Game.db";
builder.Services.AddSqlite<GameDb>(connectionString);
builder.Services.AddOpenApi();
var jwt_key = Environment.GetEnvironmentVariable("JWT_KEY") ?? "no_key";
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = "MpWordle.com",
            ValidAudience = "MpWordle.com",
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt_key))
        };
    });
builder.Services.AddAuthorization();
var app = builder.Build();

app.UseHttpsRedirection();
app.Run();