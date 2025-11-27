using RTS.Service.Connector.Domain.Enums;

namespace RTS.Service.Connector.Infrastructure.InvoiceSplit
{
    public class CustomerTypeClassifier
    {
        public static CustomerType Classify(string? companyDesc)
        {
            if(string.IsNullOrEmpty(companyDesc))
            {
                return CustomerType.Company;
            }

            // When a companyDesc contains at least the word "privat" then return customer type as "Private",
            // companyDesc in TL contains both danish and english version of the word. 
            var type = companyDesc.ToLowerInvariant();
            
            if(type.Contains("privat"))
            {
                return CustomerType.Private;
            }

            return CustomerType.Unknown;
        }
    }
}
