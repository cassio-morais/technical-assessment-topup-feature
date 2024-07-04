using Backend.TopUp.Core.Entities;
using Backend.TopUp.Core.Infrastruture.Configuration;
using Microsoft.EntityFrameworkCore;

namespace Backend.TopUp.Infrastructure.Configuration.Db
{
    public class DatabaseContext(DbContextOptions<DatabaseContext> dbContextOptions) : DbContext(dbContextOptions), IDatabaseContext
    {
        public DbSet<TopUpBeneficiary> TopUpBeneficiaries { get; set; }
        public DbSet<TopUpTransaction> TopUpTransactions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TopUpBeneficiary>(builder =>
            {
                builder.ToTable("top_up_beneficiaries");
                builder.HasKey(x => x.Id);
                builder.Property(x => x.Id).HasColumnName("id");
                builder.HasIndex(x => x.UserId);
                builder.Property(x => x.UserId).HasColumnName("user_id").IsRequired();
                builder.Property(x => x.IsActive).HasColumnName("is_active").HasDefaultValue(false).IsRequired();
                builder.Property(x => x.Nickname).HasColumnName("nickname").HasColumnType("VARCHAR(20)").IsRequired();
                builder.Property(x => x.PhoneNumber).HasColumnName("phone_number").HasColumnType("VARCHAR(16)").IsRequired();
                builder.Property(x => x.CreatedAt).HasColumnName("created_at").ValueGeneratedOnUpdate().IsRequired();
                builder.Property(x => x.UpdatedAt).HasColumnName("updated_at").ValueGeneratedOnUpdate();
            });

            modelBuilder.Entity<TopUpTransaction>(builder =>
            {
                builder.ToTable("top_up_transactions");
                builder.HasKey(x => x.Id);
                builder.Property(x => x.Id).HasColumnName("id");
                builder.Property(x => x.UserId).HasColumnName("user_id").IsRequired();
                builder.HasIndex(x => x.UserId);
                builder.HasOne(x => x.ToUpBeneficiary);
                builder.Property(x => x.Amount).HasColumnName("amount").HasDefaultValue((decimal)0);
                builder.Property(x => x.TransactionDate).HasColumnName("transaction_date").ValueGeneratedOnAdd().IsRequired();
                builder.Property(x => x.TopUpBeneficiaryId).HasColumnName("top_up_beneficiary_id").IsRequired();
                builder.Property(x => x.Status).HasColumnName("status").HasDefaultValue(TopUpTransactionStatus.Pending).IsRequired();
                builder.Property(x => x.Reason).HasColumnName("reason").HasColumnType("VARCHAR(100)");
                builder.Property(x => x.CreatedAt).HasColumnName("created_at").ValueGeneratedOnUpdate().IsRequired();
                builder.Property(x => x.UpdatedAt).HasColumnName("updated_at").ValueGeneratedOnUpdate();
            });

            modelBuilder.Entity<TopUpOption>(builder =>
            {
                builder.ToTable("top_up_options");
                builder.HasKey(x => x.Id);
                builder.Property(x => x.Id).HasColumnName("id");
                builder.Property(x => x.CurrencyAbbreviation).HasColumnName("currency_abbreviation").HasColumnType("CHAR(3)").IsRequired();
                builder.HasIndex(x => x.CurrencyAbbreviation);
                builder.Property(x => x.Value).HasColumnName("value").IsRequired();
                builder.Property(x => x.CreatedAt).HasColumnName("created_at").ValueGeneratedOnUpdate().IsRequired();
                builder.Property(x => x.UpdatedAt).HasColumnName("updated_at").ValueGeneratedOnUpdate();
                builder.Property(x => x.IsActive).HasColumnName("is_active").HasDefaultValue(false).IsRequired();
            });

            base.OnModelCreating(modelBuilder);
        }

        public Task<int> SaveChangesAsync()
        {
            return base.SaveChangesAsync();
        }
    }
}
