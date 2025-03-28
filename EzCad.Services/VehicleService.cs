using EzCad.Database;
using EzCad.Database.Entities;
using EzCad.Redis.Interfaces;
using EzCad.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EzCad.Services;

public class VehicleService : IVehicleService
{
    private readonly EzCadDataContext _dataContext;
    private readonly IRedisCachingService _redis;

    public VehicleService(EzCadDataContext dataContext, IRedisCachingService redis)
    {
        _dataContext = dataContext;
        _redis = redis;
    }

    public async Task<List<Vehicle>?> GetVehiclesAsync(User user, string identityId, bool noCache = false,
        CancellationToken cancellationToken = default)
    {
        return await _redis.GetOrSetRecordAsync($"{identityId}_vehicles", async () =>
        {
            return await _dataContext.Vehicles
                .Where(x => x.HostIdentity.Id == identityId && x.HostIdentity.HostUser.Id == user.Id)
                .ToListAsync(cancellationToken);
        }, noCache, cancellationToken);
    }
    
    public async Task<List<Vehicle>?> GetVehiclesAsync(string identityId, bool noCache = false,
        CancellationToken cancellationToken = default)
    {
        return await _redis.GetOrSetRecordAsync($"{identityId}_vehicles", async () =>
        {
            return await _dataContext.Vehicles
                .Where(x => x.HostIdentity.Id == identityId)
                .ToListAsync(cancellationToken);
        }, noCache, cancellationToken);
    }

    public async Task<Vehicle?> GetVehicleByLicenceAsync(string licensePlate, bool noCache = false,
        CancellationToken cancellationToken = default)
    {
        return await _redis.GetOrSetRecordAsync($"vehicle_{licensePlate}", async () =>
        {
            return await _dataContext.Vehicles.SingleOrDefaultAsync(
                x => x.LicensePlate.ToLower().Replace(" ", string.Empty) ==
                     licensePlate.ToLower().Replace(" ", string.Empty), cancellationToken);
        }, noCache, cancellationToken);
    }

    public async Task<Vehicle?> GetVehicleAsync(User user, string id, bool noCache = false,
        CancellationToken cancellationToken = default)
    {
        return await _redis.GetOrSetRecordAsync($"vehicle_{id}", async () =>
        {
            return await _dataContext.Vehicles.SingleOrDefaultAsync(
                x => x.Id == id && x.HostIdentity.HostUser.Id == user.Id, cancellationToken);
        }, noCache, cancellationToken);
    }
    
    public async Task<Vehicle?> CreateVehicleAsync(Identity identity, Vehicle vehicle,
        CancellationToken cancellationToken = default)
    {
        vehicle.HostIdentity = identity;

        await _dataContext.AddAsync(vehicle, cancellationToken);
        await _dataContext.SaveChangesAsync(cancellationToken);

        // Update vehicles cache
        await _redis.RemoveRecordAsync<List<Vehicle>>($"{vehicle.HostIdentity.Id}_vehicles", cancellationToken);

        return vehicle;
    }

    public async Task UpdateVehicleAsync(User user, string vehicleId, Vehicle newVehicle,
        CancellationToken cancellationToken = default)
    {
        var vehicle = await GetVehicleAsync(user, vehicleId, true, cancellationToken);
        if (vehicle is null) return;

        await _redis.RemoveRecordAsync<Vehicle>($"vehicle_{vehicle.LicensePlate}", cancellationToken);

        vehicle.Manufacturer = newVehicle.Manufacturer;
        vehicle.Model = newVehicle.Model;
        vehicle.LicensePlate = newVehicle.LicensePlate;
        vehicle.IsStolen = newVehicle.IsStolen;
        vehicle.HostIdentity = newVehicle.HostIdentity;
        vehicle.InsuranceState = newVehicle.InsuranceState;
        vehicle.MotState = newVehicle.MotState;

        await _dataContext.SaveChangesAsync(cancellationToken);

        // Remove any records
        await _redis.UpdateRecordAsync($"vehicle_{vehicleId}", vehicle, cancellationToken: cancellationToken);
        await _redis.SetRecordAsync($"vehicle_{newVehicle.LicensePlate}", vehicle,
            cancellationToken: cancellationToken);

        // Update vehicles cache
        await _redis.RemoveRecordAsync<List<Vehicle>>($"{vehicle.HostIdentity.Id}_vehicles", cancellationToken);
    }

    public async Task DeleteVehicleAsync(User user, string vehicleId, CancellationToken cancellationToken = default)
    {
        var vehicle = await GetVehicleAsync(user, vehicleId, true, cancellationToken);
        if (vehicle is null) return;

        _dataContext.Remove(vehicle);
        await _dataContext.SaveChangesAsync(cancellationToken);

        // Remove all potentially cached records
        await _redis.RemoveRecordAsync<Vehicle>($"vehicle_{vehicleId}", cancellationToken);
        await _redis.RemoveRecordAsync<Vehicle>($"vehicle_{vehicle.LicensePlate}", cancellationToken);
        await _redis.RemoveRecordAsync<List<Vehicle>>($"{vehicle.HostIdentity.Id}_vehicles", cancellationToken);
    }
}