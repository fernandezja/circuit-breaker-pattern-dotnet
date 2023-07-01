using Polly;
using Polly.CircuitBreaker;
using Polly.Retry;
using Polly.Wrap;
using Starwars.YodaQuotes.Client;
using System.Threading;

namespace Starwars.Core.Services
{
    public class QuotesService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly Serilog.ILogger _logger;
        private YodaQuotesClient _yodaQuotesClient { get; set; }

        private AsyncRetryPolicy _retryPolicy;
        private AsyncCircuitBreakerPolicy _circuitBreakerPolicy;
        private AsyncPolicyWrap _superPowerPolicy;

        //public QuotesService(YodaQuotesClient yodaQuotesClient)
        //{
        //    _yodaQuotesClient = yodaQuotesClient;
        //}

        public QuotesService(IHttpClientFactory httpClientFactory,
                             Serilog.ILogger logger,
                             YodaQuotesClient yodaQuotesClient)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
            _yodaQuotesClient = yodaQuotesClient;


            _retryPolicy = Policy
              .Handle<Exception>()
              .WaitAndRetryAsync(
                retryCount: 2, 
                sleepDurationProvider: retryAttempt => {
                    var timeToWait = TimeSpan.FromSeconds(Math.Pow(2, retryAttempt));
                    _logger.Information($"Waiting {timeToWait.TotalSeconds} seconds");
                    return timeToWait;
              });

            
            _circuitBreakerPolicy = Policy.Handle<Exception>()
                .CircuitBreakerAsync(
                        exceptionsAllowedBeforeBreaking: 1, 
                        durationOfBreak: TimeSpan.FromSeconds(30), //TimeSpan.FromMinutes(1),
                        onBreak: (ex, t) =>
                        {
                            _logger.Information("Circuit broken!");
                        },
                        onReset: () =>
                        {
                            _logger.Information("Circuit reset!");
                        });



            _superPowerPolicy = Policy.WrapAsync(_retryPolicy, 
                                                 _circuitBreakerPolicy);


        }

        public async Task<YodaQuote> GetQuoteAsync() { 

            return await _yodaQuotesClient.GetYodaQuoteAsync();

        }


        public async Task<YodaQuote> GetQuoteWithErrorAsync()
        {
            return await GetQuoteWithErrorAsync(4);

        }

        public async Task<YodaQuote> GetQuoteWithErrorAsync(int attempIndex)
        {
            return await _yodaQuotesClient.GetYodaQuoteWithErrorAsync(attempIndex);

        }


        public async Task<YodaQuote> GetQuoteWithErrorRetryAsync()
        {
            return await _retryPolicy.ExecuteAsync<YodaQuote>(async () => await GetQuoteWithErrorAsync());

        }


        public async Task<YodaQuote> GetQuoteWithErrorCircuitBreakerAsync()
        {
            try
            {
                _logger.Information($"Circuit State: {_circuitBreakerPolicy.CircuitState}");

                return await _circuitBreakerPolicy.ExecuteAsync<YodaQuote>(async () =>
                {
                    return await GetQuoteWithErrorAsync();
                });
            }
            catch (Exception ex)
            {
                _logger.Error($"[ERROR] {ex.Message}");
                throw ex;
            }
        }


        public async Task<YodaQuote> GetQuoteWithSuperPowerAsync()
        {
            try
            {

                return await _superPowerPolicy.ExecuteAsync<YodaQuote>(async () =>
                {
                    return await GetQuoteWithErrorAsync();
                });
            }
            catch (Exception ex)
            {
                _logger.Error($"[ERROR] {ex.Message}");
                throw ex;
            }
        }

    }
}