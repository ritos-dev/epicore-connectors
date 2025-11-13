using RTS.Service.Connector.DTOs;

namespace RTS.Service.Connector.Infrastructure.Tracelink
{
    public static class TracelinkOrderFactory
    {
        public static CompleteTracelinkDto Create(TracelinkOrderListDto list, TracelinkOrderDto details, TracelinkCustomerDto customer, TracelinkCRMDto crm)
        {
            return new CompleteTracelinkDto
            {
                // order list
                OrderId = list.OrderId,
                OrderNumber = list.Number,
                CustomerName = list.Name,

                // order details
                Description = details.Description,
                State = details.State,
                StartDate = details.StartDate,
                DeadlineDate = details.DeadlineDate,
                UpdatedAt = details.UpdatedAt,

                // customer list
                CustomerId = customer.CustomerId,
                CustomerAddress = customer.CustomerAdress,
                CustomerCity = customer.CustomerCity,
                CustomerPostalCode = customer.CustomerPostalcode,

                // crm list
                CrmNumber = crm.CrmNumber
            };
        }
    }
}
