using Microsoft.Extensions.DependencyInjection;
using Starwars.YodaQuotes.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Starwars.Core.Services
{
    public static class ServiceModule 
    {
        public static void ConfigureServices(IServiceCollection services)
        {

            var sp = services.BuildServiceProvider();
            var httpClientFactory = sp.GetRequiredService<IHttpClientFactory>();

            //TODO: Change baseUrl via appSettings
            services.AddTransient<YodaQuotesClient>(
                        serviceProvider => new YodaQuotesClient(baseUrl: "http://localhost:5023",
                                                                httpClient: httpClientFactory.CreateClient()));

            services.AddTransient<QuotesService>();


        }
    }
}
