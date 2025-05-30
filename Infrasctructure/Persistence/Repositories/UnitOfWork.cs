using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using SCED.API.Domain.Interfaces;
using SCED.API.Infrasctructure.Context;

namespace SCED.API.Infrastructure.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly DatabaseContext _context;
        private IDbContextTransaction? _transaction;

        public UnitOfWork(DatabaseContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public DatabaseContext Context => _context;

        private IAlertRepository? _alerts;
        private IDeviceRepository? _devices;
        private IShelterRepository? _shelters;
        private IResourceRepository? _resources;
        private IDeviceDataRepository? _deviceData;

        public IAlertRepository Alerts => _alerts ??= new AlertRepository(_context);
        public IDeviceRepository Devices => _devices ??= new DeviceRepository(_context);
        public IShelterRepository Shelters => _shelters ??= new ShelterRepository(_context);
        public IResourceRepository Resources => _resources ??= new ResourceRepository(_context);
        public IDeviceDataRepository DeviceData => _deviceData ??= new DeviceDataRepository(_context);

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public async Task BeginTransactionAsync()
        {
            _transaction = await _context.Database.BeginTransactionAsync();
        }

        public async Task CommitTransactionAsync()
        {
            if (_transaction != null)
            {
                await _transaction.CommitAsync();
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }

        public async Task RollbackTransactionAsync()
        {
            if (_transaction != null)
            {
                await _transaction.RollbackAsync();
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }

        public void Dispose()
        {
            _transaction?.Dispose();
            _context?.Dispose();
        }
    }
}