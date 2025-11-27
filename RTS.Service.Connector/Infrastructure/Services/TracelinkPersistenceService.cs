using RTS.Service.Connector.DTOs;
using RTS.Service.Connector.Domain.Orders.Entities;

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

        public async Task SaveOrderAsync(CompleteTracelinkDto dto)
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
            public static Orders ToEntity(CompleteTracelinkDto dto)
            {
                return new Orders
                {
                    CustomerName = dto.CustomerName,
                    CustomerId = dto.CustomerId,
                    CrmNumber = dto.CrmNumber,
                    OrderNumber = int.TryParse(dto.OrderNumber, out var number) ? number : 0
                };
            }
        }
    }
}
