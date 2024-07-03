using Backend.TopUp.Core.Contracts;
using Backend.TopUp.Core.Entities;
using Backend.TopUp.Core.Infrastruture.Configuration;
using Backend.TopUp.Core.Infrastruture.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Backend.TopUp.Infrastructure.Repository
{
    public class TopUpBeneficiaryRepository(IDatabaseContext context) : ITopUpBeneficiaryRepository
    {
        private readonly IDatabaseContext _context = context;
        private readonly DbSet<TopUpBeneficiary> _beneficaries = context.Set<TopUpBeneficiary>();

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

        public async Task<Result<bool>> TopUpBeneficiaryExists(Expression<Func<TopUpBeneficiary, bool>> predicate)
        {
            try
            {
                var result = await _beneficaries
                   .AsNoTracking()
                   .AnyAsync(predicate);

                return Result<bool>.Ok(result);
            }
            catch (Exception ex)
            {
                // todo: LOG
                return Result<bool>.Error("An error ocurred");
            }
        }

        public async Task<Result<List<TopUpBeneficiary>>> ListTopUpBeneficiariesAsync(Expression<Func<TopUpBeneficiary, bool>> predicate)
        {
            try
            {
                var result = await _beneficaries
                    .AsNoTracking()
                    .Where(predicate)
                    .ToListAsync();

                return Result<List<TopUpBeneficiary>>.Ok(result);

            }
            catch (Exception ex)
            {

                // todo: LOG
                return Result<List<TopUpBeneficiary>>.Error("An error ocurred");
            }
        }

        public async Task<Result<TopUpBeneficiary>> GetTopUpBeneficiaryById(Guid id)
        {
            try
            {
                var result = await _beneficaries
                     .AsNoTracking()
                     .SingleOrDefaultAsync(x => x.Id == id);

                if (result is null)
                    return Result<TopUpBeneficiary>.Error("Beneficiary doesn't exist");

               return Result<TopUpBeneficiary>.Ok(result);

            }
            catch (Exception ex)
            {
                // todo: LOG
                return Result<TopUpBeneficiary>.Error("An error ocurred");
            }
        }
    }
}
