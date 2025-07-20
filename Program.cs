using Microsoft.EntityFrameworkCore;
using MP_WORDLE_SERVER_V2.Data;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("Game") ?? "Data Source=Game.db";
builder.Services.AddSqlite<GameDb>(connectionString);

builder.Services.AddOpenApi();
var app = builder.Build();

app.UseHttpsRedirection();
app.Run();