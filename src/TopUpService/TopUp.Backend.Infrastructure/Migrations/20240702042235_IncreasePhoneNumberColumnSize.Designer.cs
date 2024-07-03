﻿// <auto-generated />
using System;
using Backend.TopUp.Infrastructure.Configuration.Db;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Backend.TopUp.Infrastructure.Migrations
{
    [DbContext(typeof(DatabaseContext))]
    [Migration("20240702042235_IncreasePhoneNumberColumnSize")]
    partial class IncreasePhoneNumberColumnSize
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.6")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("Backend.TopUp.Core.Entities.TopUpBeneficiary", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasColumnName("id");

                    b.Property<DateTimeOffset>("CreatedAt")
                        .ValueGeneratedOnUpdate()
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("created_at");

                    b.Property<bool>("IsActive")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("boolean")
                        .HasDefaultValue(false)
                        .HasColumnName("is_active");

                    b.Property<string>("Nickname")
                        .IsRequired()
                        .HasColumnType("VARCHAR(20)")
                        .HasColumnName("nickname");

                    b.Property<string>("PhoneNumber")
                        .IsRequired()
                        .HasColumnType("VARCHAR(16)")
                        .HasColumnName("phone_number");

                    b.Property<DateTimeOffset?>("UpdatedAt")
                        .ValueGeneratedOnUpdate()
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("updated_at");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uuid")
                        .HasColumnName("user_id");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("top_up_beneficiaries", (string)null);
                });

            modelBuilder.Entity("Backend.TopUp.Core.Entities.TopUpOption", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasColumnName("id");

                    b.Property<DateTimeOffset>("CreatedAt")
                        .ValueGeneratedOnUpdate()
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("created_at");

                    b.Property<string>("CurrencyAbbreviation")
                        .IsRequired()
                        .HasColumnType("CHAR(3)")
                        .HasColumnName("currency_abbreviation");

                    b.Property<bool>("IsActive")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("boolean")
                        .HasDefaultValue(false)
                        .HasColumnName("is_active");

                    b.Property<DateTimeOffset?>("UpdatedAt")
                        .ValueGeneratedOnUpdate()
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("updated_at");

                    b.Property<decimal>("Value")
                        .HasColumnType("numeric")
                        .HasColumnName("value");

                    b.HasKey("Id");

                    b.HasIndex("CurrencyAbbreviation");

                    b.ToTable("top_up_options", (string)null);
                });

            modelBuilder.Entity("Backend.TopUp.Core.Entities.TopUpTransaction", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasColumnName("id");

                    b.Property<decimal>("Amount")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("numeric")
                        .HasDefaultValue(0m)
                        .HasColumnName("amount");

                    b.Property<DateTimeOffset>("CreatedAt")
                        .ValueGeneratedOnUpdate()
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("created_at");

                    b.Property<string>("Reason")
                        .HasColumnType("VARCHAR(100)")
                        .HasColumnName("reason");

                    b.Property<int>("Status")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasDefaultValue(1)
                        .HasColumnName("status");

                    b.Property<Guid>("TopUpBeneficiaryId")
                        .HasColumnType("uuid")
                        .HasColumnName("top_up_beneficiary_id");

                    b.Property<DateTimeOffset>("TransactionDate")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("transaction_date");

                    b.Property<DateTimeOffset?>("UpdatedAt")
                        .ValueGeneratedOnUpdate()
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("updated_at");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uuid")
                        .HasColumnName("user_id");

                    b.HasKey("Id");

                    b.HasIndex("TopUpBeneficiaryId");

                    b.HasIndex("UserId");

                    b.ToTable("top_up_transactions", (string)null);
                });

            modelBuilder.Entity("Backend.TopUp.Core.Entities.TopUpTransaction", b =>
                {
                    b.HasOne("Backend.TopUp.Core.Entities.TopUpBeneficiary", "ToUpBeneficiary")
                        .WithMany()
                        .HasForeignKey("TopUpBeneficiaryId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("ToUpBeneficiary");
                });
#pragma warning restore 612, 618
        }
    }
}
