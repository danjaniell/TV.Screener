using Api.Core;
using Api.Services;
using Binance.Net.Clients;
using Microsoft.Extensions.Options;
using Oakton;
using Wolverine;
using Wolverine.Http;

var builder = WebApplication.CreateBuilder(args);
builder.Host.UseWolverine();

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.Configure<WatchList>(builder.Configuration.GetSection("WatchList"));
builder.Services.AddScoped(x => new DataProvider());
builder.Services.AddScoped(x => new BinanceSocketClient());
builder.Services.AddScoped(
    x =>
        new ScreenerService(
            x.GetRequiredService<DataProvider>(),
            x.GetRequiredService<BinanceSocketClient>(),
            x.GetRequiredService<IOptionsMonitor<WatchList>>()
        )
);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.MapWolverineEndpoints();
return await app.RunOaktonCommands(args);
