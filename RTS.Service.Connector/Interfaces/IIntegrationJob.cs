using RTS.Service.Connector.Domain.Integration;

namespace RTS.Service.Connector.Interfaces
{
    public interface IIntegrationJob
    {
        Task<IntegrationJob?> GetByIdAsync(Guid id);
        Task AddAsync(IntegrationJob job);
        Task UpdateAsync(IntegrationJob job);
    }
}
