﻿using Backend.TopUp.Core.Contracts;
using Backend.TopUp.Core.Entities;
using Backend.TopUp.Core.Infrastruture.Configuration;
using Backend.TopUp.Core.Infrastruture.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Backend.TopUp.Infrastructure.Repositories
{
    public class TopUpTransactionRepository(IDatabaseContext context) : ITopUpTransactionRepository
    {
        private readonly IDatabaseContext _context = context;
        private readonly DbSet<TopUpTransaction> _transactions = context.Set<TopUpTransaction>();

        public async Task<Result<Guid>> CreateTopUpTransactionAsync(TopUpTransaction topUpTransaction)
        {
            try
            {
                var result = await _transactions.AddAsync(topUpTransaction);
                await _context.SaveChangesAsync();

                return Result<Guid>.Ok(result.Entity.Id);
            }
            catch (Exception)
            {
                // todo: put some log here
                return Result<Guid>.Error("An error ocurred");
            }
        }

        public async Task<Result<List<TopUpTransaction>>> ListTopUpTransactionsByUserIdWithinAPeriodAsync(Guid id, DateTimeOffset startDate, DateTimeOffset endDate)
        {
            try
            {
                return Result<List<TopUpTransaction>>.Ok(await _transactions
                   .AsNoTracking()
                   .Where(x => x.TransactionDate >= startDate && x.TransactionDate <= endDate)
                   .ToListAsync());
            }
            catch (Exception ex)
            {
                // todo: put some log here
                return Result<List<TopUpTransaction>>.Error("An error ocurred");
            }
        }

        public async Task<Result<Guid>> UpdateTopUpTransactionStatusAsync(Guid topUpTransactionId, TopUpTransactionStatus topUpTransactionStatus, string? reason = null)
        {
            try
            {
                var transaction = await _transactions.FirstOrDefaultAsync(x => x.Id == topUpTransactionId);

                if (transaction == null)
                    return Result<Guid>.Error("An error ocurred");

                transaction.UpdateStatus(topUpTransactionStatus, reason);

                _transactions.Update(transaction);
                await _context.SaveChangesAsync();

                return Result<Guid>.Ok(transaction.Id);
            }
            catch (Exception)
            {
                // todo: put some log here
                return Result<Guid>.Error("An error ocurred");
            }
        }
    }
}
