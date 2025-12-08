using Backend.Models.Entities;

namespace Backend.Repository;

public interface ISpaceCacheRepository
{
    Task AddAsync(SpaceCache spaceCache);
    Task<SpaceCache?> GetBySource(string source);
}