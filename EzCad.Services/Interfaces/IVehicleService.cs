using EzCad.Database.Entities;

namespace EzCad.Services.Interfaces;

public interface IVehicleService
{
    Task<Vehicle?> GetVehicleAsync(User user, string id, bool noCache = false,  CancellationToken cancellationToken = default);

    Task<Vehicle?> CreateVehicleAsync(Identity identity, Vehicle vehicle,
        CancellationToken cancellationToken = default);

    Task UpdateVehicleAsync(User user, string vehicleId, Vehicle newVehicle,
        CancellationToken cancellationToken = default);

    Task DeleteVehicleAsync(User user, string vehicleId, CancellationToken cancellationToken = default);
    Task<List<Vehicle>?> GetVehiclesAsync(User user, string identityId, bool noCache = false,  CancellationToken cancellationToken = default);
    Task<List<Vehicle>?> GetVehiclesAsync(string identityId, bool noCache = false,  CancellationToken cancellationToken = default);
    Task<Vehicle?> GetVehicleByLicenceAsync(string licensePlate, bool noCache = false, 
        CancellationToken cancellationToken = default);
}