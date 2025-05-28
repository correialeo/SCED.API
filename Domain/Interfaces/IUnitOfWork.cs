using SCED.API.Infrasctructure.Context;

namespace SCED.API.Domain.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IAlertRepository Alerts { get; }
        IDeviceRepository Devices { get; }
        IShelterRepository Shelters { get; }
        IResourceRepository Resources { get; }
        IDeviceDataRepository DeviceData { get; }

        DatabaseContext Context { get; }
        
        Task<int> SaveChangesAsync();
        Task BeginTransactionAsync();
        Task CommitTransactionAsync();
        Task RollbackTransactionAsync();
    }
}