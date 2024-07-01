using Backend.TopUp.Core.Contracts;
using Backend.TopUp.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace Backend.TopUp.Infrastructure.Configuration.Db
{
    public class DatabaseContext(DbContextOptions<DatabaseContext> dbContextOptions) : DbContext(dbContextOptions), IDatabaseContext
    {
        //public DbSet<User> Users { get; set; }
        public DbSet<TopUpBeneficiary> Beneficiaries { get; set; }
        public DbSet<TopUpTransaction> TopUpTransactions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //modelBuilder.Entity<User>(builder =>
            //{
            //    builder.ToTable("users");
            //    builder.HasKey(x => x.Id);
            //    builder.Property(x => x.Id).HasColumnName("id");
            //    builder.Property(x => x.IsVerified).HasColumnName("is_verified").HasDefaultValue(false);
            //    builder.HasMany(x => x.TopUpBeneficiaries);
            //    builder.Property(x => x.Email).HasColumnType("VARCHAR(100)");
            //    builder.Property(x => x.CreatedAt).HasColumnName("created_at").ValueGeneratedOnAdd().IsRequired();
            //    builder.Property(x => x.UpdatedAt).HasColumnName("updated_at").ValueGeneratedOnUpdate();
            //});
           

            modelBuilder.Entity<TopUpBeneficiary>(builder =>
            {
                builder.ToTable("top_up_beneficiaries");
                builder.HasKey(x => x.Id);
                builder.Property(x => x.Id).HasColumnName("id");
                builder.HasIndex(x => x.UserId);
                builder.Property(x => x.UserId).HasColumnName("user_id").IsRequired();
                builder.Property(x => x.IsActive).HasColumnName("is_active").HasDefaultValue(false).IsRequired();
                builder.Property(x => x.Nickname).HasColumnName("nickname").HasColumnType("VARCHAR(20)").IsRequired();
                builder.Property(x => x.PhoneNumber).HasColumnName("phone_number").HasColumnType("VARCHAR(15)").IsRequired();
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

            base.OnModelCreating(modelBuilder);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
                => optionsBuilder.UseNpgsql(@"Host=localhost;Username=some_user;Password=Example@Bad@Password!123;Database=topup_db");

        public Task<int> SaveChangesAsync()
        {
            return base.SaveChangesAsync();
        }
    }
}
