using RTS.Service.Connector.Domain.Orders.Entities;
using RTS.Service.Connector.DTOs;

namespace RTS.Service.Connector.Infrastructure.Services
{
    public class TracelinkPersistenceService
    {
        private readonly AppDbContext _context;
        private readonly ILogger<TracelinkPersistenceService> _logger;

        public TracelinkPersistenceService(AppDbContext context, ILogger<TracelinkPersistenceService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task SaveOrderAsync(TracelinkOrderDto dto)
        {
            try
            {
                var order = TracelinkMapper.ToEntity(dto);

                _context.TracelinkOrders.Add(order);
                await _context.SaveChangesAsync();

                _logger.LogInformation("[Database] Order information for order {OrderNumber} has been saved.", order.OrderNumber);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[Database] Failed to save order information");
                throw;
            }
        }
        public static class TracelinkMapper
        {
            public static Orders ToEntity(TracelinkOrderDto dto)
            {
                return new Orders
                {
                    CompanyId = dto.Company,
                    OrderNumber = int.TryParse(dto.Number, out var number) ? number : 0,
                    CustomerName = dto.Name,
                    CrmId = dto.OrderSrcData?.Number
                };
            }
        }
    }
}
