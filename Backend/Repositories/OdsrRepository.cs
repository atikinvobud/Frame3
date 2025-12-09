
using Backend.Models;
using Backend.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace Backend.Repository;


public class OsdrRepository : IOsdrRepository
{
    private readonly Context context;
    public OsdrRepository(Context context)
    {
        this.context = context;
    }

    public async Task<int> UpsertManyAsync(IEnumerable<OsdrItem> items, CancellationToken ct = default)
    {
        var written = 0;
        foreach (var item in items)
        {
            if (!string.IsNullOrEmpty(item.DatasetId))
            {
                var exist = await context.OsdrItems.SingleOrDefaultAsync(x => x.DatasetId == item.DatasetId, ct);
                if (exist != null)
                {
                    exist.Title = item.Title;
                    exist.Status = item.Status;
                    exist.UpdatedAt = item.UpdatedAt;
                    exist.Raw = item.Raw;
                    context.OsdrItems.Update(exist);
                }
                else
                {
                    await context.OsdrItems.AddAsync(item, ct);
                }
            }
            else
            {
                await context.OsdrItems.AddAsync(item, ct);
            }
            written++;
        }
        await context.SaveChangesAsync(ct);
        return written;
    }

    public Task<List<OsdrItem>> ListLatestAsync(int limit = 20, CancellationToken ct = default)
    {
        return context.OsdrItems.OrderByDescending(x => x.InsertedAt).Take(limit).ToListAsync(ct);
    }
     public Task<List<OsdrItem>> GetAll(CancellationToken ct = default)
    {
        return context.OsdrItems.OrderByDescending(x => x.InsertedAt).ToListAsync(ct);
    }
    
}

