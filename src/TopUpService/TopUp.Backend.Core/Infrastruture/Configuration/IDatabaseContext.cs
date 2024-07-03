using Microsoft.EntityFrameworkCore;

namespace Backend.TopUp.Core.Infrastruture.Configuration
{
    // todo: move to Infrastructure/Configuration
    public interface IDatabaseContext : IDisposable
    {
        DbSet<TEntity> Set<TEntity>() where TEntity : class;
        Task<int> SaveChangesAsync();
    }
}
