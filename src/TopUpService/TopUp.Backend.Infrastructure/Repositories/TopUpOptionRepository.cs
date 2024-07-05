using Backend.TopUp.Core.Contracts;
using Backend.TopUp.Core.Entities;
using Backend.TopUp.Core.Infrastruture.Configuration;
using Backend.TopUp.Core.Infrastruture.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Backend.TopUp.Infrastructure.Repositories
{
    public class TopUpOptionRepository(IDatabaseContext context) : ITopUpOptionRepository
    {
        private readonly IDatabaseContext _context = context;
        private readonly DbSet<TopUpOption> _options = context.Set<TopUpOption>();

        public async Task<Result<TopUpOption>> GetTopUpOptionById(Guid id, bool isActive = true)
        {
            try
            {
                var result = await _options
                     .AsNoTracking()
                     .SingleOrDefaultAsync(x => x.Id == id && x.IsActive == isActive);

                if (result is null)
                    return Result<TopUpOption>.Error("Top-up option doesn't exist");

                return Result<TopUpOption>.Ok(result);

            }
            catch (Exception ex)
            {
                // todo: put some log here
                return Result<TopUpOption>.Error("An error ocurred");
            }
        }

        public async Task<Result<List<TopUpOption>>> ListTopUpOptionsAsync(Expression<Func<TopUpOption, bool>> predicate)
        {
            try
            {
                return Result<List<TopUpOption>>.Ok(await _options
                    .AsNoTracking()
                    .Where(predicate)
                    .ToListAsync());
            }
            catch (Exception ex)
            {
                // todo: put some log here
                return Result<List<TopUpOption>>.Error("An error ocurred");
            }
        }
    }
}
