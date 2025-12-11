using Backend.Models.Entities;

namespace Backend.Repository;

public interface ITelemetryRepository
{
    Task<int> AddAsync(TelemetryEntity entity);
    Task<List<TelemetryEntity>> GetAllAsync();
}
