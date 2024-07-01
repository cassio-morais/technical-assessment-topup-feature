using Backend.TopUp.Core.Contracts;
using Backend.TopUp.Core.Entities;
using Backend.TopUp.Core.Infrastruture.Repository;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Backend.TopUp.Infrastructure.Repository
{
    public class TopUpRepository(IDatabaseContext context) : ITopUpRepository
    {
        public readonly IDatabaseContext _context = context;
        public readonly DbSet<TopUpBeneficiary> _beneficaries = context.Set<TopUpBeneficiary>();

        public async Task<Result<Guid>> AddTopUpBeneficiaryAsync(TopUpBeneficiary beneficiary)
        {
            try
            {
                var result = await _beneficaries.AddAsync(beneficiary);
                await _context.SaveChangesAsync();
                return Result<Guid>.Ok(result.Entity.Id);
            }
            catch (Exception ex)
            {
                // todo: LOG
                return Result<Guid>.Error("An error ocurred");
            }
        }

        public async Task<Result<bool>> BeneficiaryExists(Expression<Func<TopUpBeneficiary, bool>> predicate)
        {
            try
            {
                var result = await _beneficaries
                   .AsNoTracking()
                   .AnyAsync(predicate);

                return Result<bool>.Ok(result);
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<Result<List<TopUpBeneficiary>>> ListBeneficiariesAsync(Expression<Func<TopUpBeneficiary, bool>> predicate)
        {
            try
            {
                var result = await _beneficaries
                    .AsNoTracking()
                    .Where(predicate)
                    .ToListAsync();

                return Result<List<TopUpBeneficiary>>.Ok(result);

            }
            catch (Exception)
            {

                // todo: LOG
                return Result<List<TopUpBeneficiary>>.Error("An error ocurred");
            }
        }
    }
}
