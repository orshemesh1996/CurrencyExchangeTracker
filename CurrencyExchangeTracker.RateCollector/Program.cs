using Microsoft.EntityFrameworkCore;
using CurrencyExchangeTracker.Core.Contracts;
using CurrencyExchangeTracker.Infrastructure.Database;
using CurrencyExchangeTracker.Infrastructure.ExternalApis;
using CurrencyExchangeTracker.Infrastructure.Services;
using CurrencyExchangeTracker.RateCollector.Workers;

var builder = Host.CreateApplicationBuilder(args);


builder.Services.Configure<CollectorSettings>(
    builder.Configuration.GetSection("CurrencyCollector"));


builder.Services.AddDbContext<CurrencyDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));


builder.Services.AddHttpClient<ICurrencyApiClient, ExchangeRateApiClient>();
builder.Services.AddScoped<IExchangeRateService, ExchangeRateService>();
builder.Services.AddHostedService<CurrencyDataCollector>();

var app = builder.Build();


using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<CurrencyDbContext>();
    await db.Database.EnsureCreatedAsync();
}

app.Run();
