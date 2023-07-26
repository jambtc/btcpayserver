using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using BTCPayServer.Rating;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace BTCPayServer.Services.Rates
{
    public class SwaggyRateProvider : IRateProvider
    {
        public RateSourceInfo RateSourceInfo => new("swaggy", "Swaggy", "https://api-stage.swaggyapp.com/currency/v1/rates/official/BTC?quote=EUR");
        private readonly HttpClient _httpClient;
        public SwaggyRateProvider(HttpClient httpClient)
        {
            _httpClient = httpClient ?? new HttpClient();
        }

        public async Task<PairRate[]> GetRatesAsync(CancellationToken cancellationToken)
        {
            var response = await _httpClient.GetAsync("https://api-stage.swaggyapp.com/currency/v1/rates/official/BTC?quote=EUR", cancellationToken);
            response.EnsureSuccessStatusCode();
            var jobj = await response.Content.ReadAsAsync<JObject>(cancellationToken);
            var results = jobj["rate"];
            var list = new List<PairRate>();
            
            var value = results.Value<decimal>();
            list.Add(new PairRate(new CurrencyPair("BTC", "EUR"), new BidAsk(value)));
        
            return list.ToArray();
        }
    }
}
