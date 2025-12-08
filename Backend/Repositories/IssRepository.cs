using Backend.Models;
using Backend.Models.Entities;
using Microsoft.EntityFrameworkCore;
namespace Backend.Repository;
public class IssRepository : IIssRepository
{
    private readonly Context context;

    public IssRepository(Context context)
    {
        this.context = context;
    }

    public async Task<IssFetchLog> AddAsync(IssFetchLog log)
    {
        context.IssFetchLogs.Add(log);
        await context.SaveChangesAsync();
        return log;
    }

    public async Task<IssFetchLog?> GetLastAsync()
    {
        return await context.IssFetchLogs
            .OrderByDescending(l => l.Id)
            .FirstOrDefaultAsync();
    }

    public async Task<List<IssFetchLog>> GetLastNAsync(int n)
    {
        return await context.IssFetchLogs
            .OrderByDescending(l => l.Id)
            .Take(n)
            .ToListAsync();
    }
}
