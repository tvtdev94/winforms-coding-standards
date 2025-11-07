// Template: Repository Pattern with EF Core
// Replace: YourEntity, YourRepository, AppDbContext

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace YourNamespace.Data.Repositories
{
    /// <summary>
    /// Repository interface for YourEntity data access.
    /// </summary>
    public interface IYourRepository
    {
        Task<List<YourEntity>> GetAllAsync();
        Task<YourEntity?> GetByIdAsync(int id);
        Task<bool> SaveAsync(YourEntity entity);
        Task<bool> DeleteAsync(int id);
    }

    /// <summary>
    /// EF Core implementation of YourEntity repository.
    /// </summary>
    public class YourRepository : IYourRepository
    {
        private readonly AppDbContext _context;

        public YourRepository(AppDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<List<YourEntity>> GetAllAsync()
        {
            return await _context.YourEntities
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<YourEntity?> GetByIdAsync(int id)
        {
            return await _context.YourEntities
                .AsNoTracking()
                .FirstOrDefaultAsync(e => e.Id == id);
        }

        public async Task<bool> SaveAsync(YourEntity entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            if (entity.Id == 0)
            {
                // Insert
                await _context.YourEntities.AddAsync(entity);
            }
            else
            {
                // Update
                _context.YourEntities.Update(entity);
            }

            var changes = await _context.SaveChangesAsync();
            return changes > 0;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var entity = await _context.YourEntities.FindAsync(id);

            if (entity == null)
                return false;

            _context.YourEntities.Remove(entity);
            var changes = await _context.SaveChangesAsync();
            return changes > 0;
        }
    }
}
