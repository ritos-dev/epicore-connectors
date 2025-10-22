namespace Connector.Domain.Integration.Aggregates.IntegrationJob;

public class Mapping
{
    public Guid Id { get; private set; }
    public string Field { get; private set; }          // this is something like "CustomerNumber" or "ItemSKU"
    public string TraceLinkValue { get; private set; } // value from TraceLink
    public string EconomicValue { get; private set; }  // value from e-conomic
    public DateTime CreatedAt { get; private set; }

    internal Mapping(string field, string traceLinkValue, string economicValue)
    {
        Id = Guid.NewGuid();
        Field = field;
        TraceLinkValue = traceLinkValue;
        EconomicValue = economicValue;
        CreatedAt = DateTime.UtcNow;
    }
}
