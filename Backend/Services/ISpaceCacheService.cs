using Backend.Models.Entities;

public interface ISpaceCacheService
{
    Task<SpaceCache?> GetLatestAsync(string source);
}
