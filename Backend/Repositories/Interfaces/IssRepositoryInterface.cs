using Backend.Models.Entities;
namespace Backend.Repository;
public interface IIssRepository
{
    Task<IssFetchLog> AddAsync(IssFetchLog log);
    Task<IssFetchLog?> GetLastAsync();
    Task<List<IssFetchLog>> GetLastNAsync(int n);
}