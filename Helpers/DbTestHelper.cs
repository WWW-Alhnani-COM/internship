// InternshipManagement.API/Helpers/DbTestHelper.cs
using Microsoft.EntityFrameworkCore;
using InternshipManagement.API.Data;

namespace InternshipManagement.API.Helpers;

public static class DbTestHelper
{
    public static async Task<bool> CanConnectToDatabase(IServiceProvider services)
    {
        try
        {
            using var scope = services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            await context.Database.CanConnectAsync();
            return true;
        }
        catch
        {
            return false;
        }
    }
}