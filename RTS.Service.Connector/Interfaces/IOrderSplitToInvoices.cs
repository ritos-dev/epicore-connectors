using RTS.Service.Connector.Domain.Enums;
using RTS.Service.Connector.Infrastructure.InvoiceSplit;

namespace RTS.Service.Connector.Interfaces
{
    public interface IOrderSplitToInvoices
    {
        List<InvoicePart> Split(decimal totalAmount, CustomerType customerType);
    }
}
