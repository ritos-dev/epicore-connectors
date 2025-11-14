using Microsoft.IdentityModel.Tokens;
using RTS.Service.Connector.Domain.Enums;

namespace RTS.Service.Connector.Infrastructure.InvoiceSplit
{
    public class CustomerTypeClassifier
    {
        public static CustomerType Classify(string? companyDesc)
        {
            if(string.IsNullOrEmpty(companyDesc))
            {
                return CustomerType.Unknown;
            }

            // When a companyDesc containt words "privat" then return customer type as "Private".
            // Has to be like this for now, because companyDesc in TL contains both danish and english version of the word. 
            var type = companyDesc.ToLowerInvariant();
            
            if(type.Contains("privat"))
            {
                return CustomerType.Private;
            }

            return CustomerType.Unknown;
        }
    }
}
