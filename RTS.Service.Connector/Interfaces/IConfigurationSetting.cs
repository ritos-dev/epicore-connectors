using RTS.Service.Connector.Domain.Configuration;
using RTS.Service.Connector.Domain.Enums;

namespace RTS.Service.Connector.Interfaces
{
    public interface IConfigurationSettingRepository
    {
        Task<ConfigurationSetting?> GetActiveBySystemAsync(IntegrationSystem system);
        Task AddAsync(ConfigurationSetting setting);
        Task UpdateAsync(ConfigurationSetting setting);
    }
}
