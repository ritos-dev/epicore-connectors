using Microsoft.Extensions.Options;
using RTS.Service.Connector.Interfaces;
using System.Text;
using System.Text.Json;

namespace RTS.Service.Connector.Infrastructure.Economic
{
    public sealed class EconomicClient : IEconomicClient
    {
        private readonly HttpClient _client;

        public EconomicClient(IHttpClientFactory factory, IOptions<EconomicOptions> options)
        {
            _client = factory.CreateClient("Economic");
        }

        public async Task<string?> GetOrderDraftIfExistsAsync(string orderNumber, CancellationToken cancellationToken = default)
        {
            // Temporary mapping for testing
            if (orderNumber == "24056")
            {
                Console.WriteLine("Overriding TraceLink order nr. 24056 → Economic order nr. 30092");
                orderNumber = "30097";
            }

            var url = $"orders/drafts/{orderNumber}";
            Console.WriteLine($"[Economic] Fetching order draft {orderNumber}...");

            var response = await _client.GetAsync(url, cancellationToken);
            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine($"[Economic] Order {orderNumber} not found → HTTP {response.StatusCode}");
                return null;
            }

            var json = await response.Content.ReadAsStringAsync(cancellationToken);
            Console.WriteLine($"[Economic] Order draft {orderNumber} fetched successfully.");
            return json;
        }

        // Create an invoice draft
        public async Task<bool> CreateInvoiceDraftAsync(string orderJson, CancellationToken cancellationToken = default)
        {
            Console.WriteLine("[Economic] Building invoice draft payload from order data...");

            using var orderDoc = JsonDocument.Parse(orderJson);
            var root = orderDoc.RootElement;

            // Extracting key fields from the order
            var currency = root.GetProperty("currency").GetString();
            var customerNumber = root.GetProperty("customer").GetProperty("customerNumber").GetInt32();
            var paymentTermsNumber = root.GetProperty("paymentTerms").GetProperty("paymentTermsNumber").GetInt32();
            var layoutNumber = root.GetProperty("layout").GetProperty("layoutNumber").GetInt32();

            var recipient = root.GetProperty("recipient");
            var recipientName = recipient.GetProperty("name").GetString();
            var vatZoneNumber = recipient.GetProperty("vatZone").GetProperty("vatZoneNumber").GetInt32();

            // Minimal valid invoice draft JSON
            var invoice = new
            {
                date = DateTime.UtcNow.ToString("yyyy-MM-dd"),
                currency,
                customer = new { customerNumber },
                paymentTerms = new { paymentTermsNumber },
                layout = new { layoutNumber },
                recipient = new
                {
                    name = recipientName,
                    vatZone = new { vatZoneNumber }
                },
                references = new { other = "TraceLink" },
                lines = new object[] { } // empty for now
            };

            var jsonBody = JsonSerializer.Serialize(invoice);
            Console.WriteLine($"[Economic] Sending invoice draft payload: {jsonBody}");

            // Send to /invoices/drafts
            var response = await _client.PostAsync(
                "invoices/drafts",
                new StringContent(jsonBody, Encoding.UTF8, "application/json"),
                cancellationToken);

            // Handle the response
            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine($"[Economic] Failed to create invoice draft → HTTP {response.StatusCode}");
                var error = await response.Content.ReadAsStringAsync(cancellationToken);
                Console.WriteLine($"[Economic] Response: {error}");
                return false;
            }

            var result = await response.Content.ReadAsStringAsync(cancellationToken);

            try
            {
                using var doc = JsonDocument.Parse(result);
                if (doc.RootElement.TryGetProperty("draftInvoiceNumber", out var draftNumber))
                {
                    Console.WriteLine($"[Economic] Draft invoice created successfully → Invoice number: {draftNumber.GetInt32()}");
                }
                else
                {
                    Console.WriteLine("[Economic] Draft invoice created, but number not found in response.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Economic] Could not parse invoice response: {ex.Message}");
            }

            return true;
        }
    }
}
