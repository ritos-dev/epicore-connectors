using RTS.Service.Connector.DTOs;

namespace RTS.Service.Connector.Infrastructure.Tracelink
{
    public static class TracelinkTotalCalculator
    {
        public static decimal CalculateTotal(CompleteTracelinkDto dto)
        {
            return dto.Items.Sum(i => i.ItemAmount * i.Price); 
        }
    }
}
