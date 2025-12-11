using Backend.Models.Entities;

namespace Backend.Services;

public interface IIssApiService
{
    Task<string?> FetchCurrentAsync();
    Task<IssFetchLog?> GetLastAsync();
    Task<object> GetTrendAsync();
}
