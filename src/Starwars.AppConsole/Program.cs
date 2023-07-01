using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Sinks.SystemConsole.Themes;
using Starwars.Core.Services;


var host = new HostBuilder()
    .ConfigureServices((hostContext, services) =>
    {
        services.AddHttpClient();

        services.AddSingleton<ILogger>(provider => new LoggerConfiguration()
            .WriteTo.Console(theme: AnsiConsoleTheme.Code)
            .CreateLogger());

        ServiceModule.ConfigureServices(services);

    })
    .UseSerilog((hostContext, loggerConfiguration) =>
    {
        loggerConfiguration
            .MinimumLevel.Verbose()
            .WriteTo.Console();
    })
    .Build();

using var serviceScope = host.Services.CreateScope();
var serviceProvider = serviceScope.ServiceProvider;
var logger = serviceProvider.GetRequiredService<ILogger>();

logger.Information("-------------------------------------");
logger.Information("Starwars Quotes!");
logger.Information("-------------------------------------");


var quoteService = serviceProvider.GetRequiredService<QuotesService>();

int index = 0;

while (true)
{
    
    index++;

    try
	{
        //var quote = await quoteService.GetQuoteAsync();
        //var quote = await quoteService.GetQuoteWithErrorAsync();
        //var quote = await quoteService.GetQuoteWithErrorRetryAsync();
        var quote = await quoteService.GetQuoteWithErrorCircuitBreakerAsync();
        //var quote = await quoteService.GetQuoteWithSuperPowerAsync();


        logger.Information($"#{index} > {quote.Quote} [{quote.Timestamp}]");
    }
	catch (Exception ex) 
	{
        logger.Error($"#{index} >[ERROR] {ex.Message}");
    }

    Console.ReadKey();
}


