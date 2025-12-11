using Backend.Models;
using Microsoft.EntityFrameworkCore;

namespace BackEnd.Models;

public static class ApplyMigration
{
    public static void ApplyMigrations(this IApplicationBuilder app)
    {
        using (var scope = app.ApplicationServices.CreateScope())
        {
            var context = scope.ServiceProvider.GetService<Context>();
            context!.Database.Migrate();
        }
    }
}