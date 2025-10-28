using Microsoft.Extensions.Options;
using RTS.Service.Connector.Interfaces;

namespace RTS.Service.Connector.Infrastructure.Economic
{
    public sealed class EconomicClient : IEconomicClient
    {
        private readonly HttpClient _client;

        public EconomicClient(IHttpClientFactory factory, IOptions<EconomicOptions> options)
        {
            _client = factory.CreateClient("Economic");
        }

        public async Task<bool> OrderExistsAsync(string orderNumber, CancellationToken cancellationToken = default)
        {
            var url = $"orders?filter=orderNumber$eq:{orderNumber}";
            Console.WriteLine($"[Economic] Checking order {orderNumber}...");

            var response = await _client.GetAsync(url, cancellationToken);
            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine($"[Economic] HTTP {response.StatusCode}");
                return false;
            }

            var json = await response.Content.ReadAsStringAsync(cancellationToken);
            var exists = json.Contains("\"orderNumber\"");
            Console.WriteLine(exists
                ? $"[Economic] Order {orderNumber} found."
                : $"[Economic] Order {orderNumber} not found.");

            return exists;
        }
    }
}
