using Backend.Models.Entities;
using Backend.Repository;

public class SpaceCacheService : ISpaceCacheService
{
    private readonly ISpaceCacheRepository repo;
    public SpaceCacheService(ISpaceCacheRepository repo)
    {
        this.repo = repo;
    }

    public Task<SpaceCache?> GetLatestAsync(string source)
    {
        return repo.GetBySource(source);
    }
}
