﻿// <auto-generated />
using System;
using LifeSync.API.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace LifeSync.API.Migrations;

[DbContext(typeof(ApplicationDbContext))]
partial class ApplicationDbContextModelSnapshot : ModelSnapshot
{
    protected override void BuildModel(ModelBuilder modelBuilder)
    {
#pragma warning disable 612, 618
        modelBuilder
            .HasAnnotation("ProductVersion", "9.0.0")
            .HasAnnotation("Relational:MaxIdentifierLength", 128);

        SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

        modelBuilder.Entity("LifeSync.API.Models.ApplicationUser.User", b =>
            {
                b.Property<string>("Id")
                    .HasColumnType("nvarchar(450)");

                b.Property<int>("AccessFailedCount")
                    .HasColumnType("int");

                b.Property<string>("ConcurrencyStamp")
                    .HasColumnType("nvarchar(max)");

                b.Property<string>("Email")
                    .HasMaxLength(320)
                    .HasColumnType("nvarchar(320)");

                b.Property<bool>("EmailConfirmed")
                    .HasColumnType("bit");

                b.Property<string>("FirstName")
                    .IsRequired()
                    .HasMaxLength(500)
                    .HasColumnType("nvarchar(500)");

                b.Property<Guid>("LanguageId")
                    .HasColumnType("uniqueidentifier");

                b.Property<string>("LastName")
                    .IsRequired()
                    .HasMaxLength(500)
                    .HasColumnType("nvarchar(500)");

                b.Property<bool>("LockoutEnabled")
                    .HasColumnType("bit");

                b.Property<DateTimeOffset?>("LockoutEnd")
                    .HasColumnType("datetimeoffset");

                b.Property<string>("NormalizedEmail")
                    .HasColumnType("nvarchar(max)");

                b.Property<string>("NormalizedUserName")
                    .HasColumnType("nvarchar(max)");

                b.Property<string>("PasswordHash")
                    .HasColumnType("nvarchar(max)");

                b.Property<string>("PhoneNumber")
                    .HasColumnType("nvarchar(max)");

                b.Property<bool>("PhoneNumberConfirmed")
                    .HasColumnType("bit");

                b.Property<string>("SecurityStamp")
                    .HasColumnType("nvarchar(max)");

                b.Property<bool>("TwoFactorEnabled")
                    .HasColumnType("bit");

                b.Property<string>("UserName")
                    .HasColumnType("nvarchar(max)");

                b.HasKey("Id");

                b.HasIndex("Email")
                    .IsUnique()
                    .HasFilter("[Email] IS NOT NULL");

                b.HasIndex("LanguageId");

                b.ToTable("Users", (string)null);
            });

        modelBuilder.Entity("LifeSync.API.Models.Currencies.Currency", b =>
            {
                b.Property<Guid>("Id")
                    .ValueGeneratedOnAdd()
                    .HasColumnType("uniqueidentifier");

                b.Property<string>("Code")
                    .IsRequired()
                    .HasMaxLength(3)
                    .IsUnicode(false)
                    .HasColumnType("varchar(3)");

                b.Property<string>("Name")
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasColumnType("nvarchar(50)");

                b.Property<string>("NativeName")
                    .IsRequired()
                    .HasColumnType("nvarchar(max)");

                b.Property<int>("NumericCode")
                    .HasColumnType("int");

                b.Property<string>("Symbol")
                    .IsRequired()
                    .HasMaxLength(5)
                    .HasColumnType("nvarchar(5)");

                b.HasKey("Id");

                b.HasIndex("Code")
                    .IsUnique();

                b.ToTable("Currencies", (string)null);
            });

        modelBuilder.Entity("LifeSync.API.Models.Expenses.ExpenseTransaction", b =>
            {
                b.Property<Guid>("Id")
                    .ValueGeneratedOnAdd()
                    .HasColumnType("uniqueidentifier");

                b.Property<DateTime>("Date")
                    .HasColumnType("datetime2");

                b.Property<string>("Description")
                    .IsRequired()
                    .HasColumnType("nvarchar(max)");

                b.Property<int>("ExpenseType")
                    .HasColumnType("int");

                b.Property<Guid>("UserId")
                    .HasColumnType("uniqueidentifier");

                b.Property<string>("UserId1")
                    .HasColumnType("nvarchar(450)");

                b.HasKey("Id");

                b.HasIndex("UserId1");

                b.ToTable("ExpenseTransactions", (string)null);
            });

        modelBuilder.Entity("LifeSync.API.Models.Incomes.IncomeTransaction", b =>
            {
                b.Property<Guid>("Id")
                    .ValueGeneratedOnAdd()
                    .HasColumnType("uniqueidentifier");

                b.Property<DateTime>("Date")
                    .HasColumnType("datetime2");

                b.Property<string>("Description")
                    .IsRequired()
                    .HasColumnType("nvarchar(max)");

                b.Property<Guid>("UserId")
                    .HasColumnType("uniqueidentifier");

                b.Property<string>("UserId1")
                    .HasColumnType("nvarchar(450)");

                b.HasKey("Id");

                b.HasIndex("UserId1");

                b.ToTable("IncomeTransactions", (string)null);
            });

        modelBuilder.Entity("LifeSync.API.Models.Languages.Language", b =>
            {
                b.Property<Guid>("Id")
                    .ValueGeneratedOnAdd()
                    .HasColumnType("uniqueidentifier");

                b.Property<string>("Code")
                    .IsRequired()
                    .HasMaxLength(20)
                    .HasColumnType("nvarchar(20)");

                b.Property<string>("Name")
                    .IsRequired()
                    .HasMaxLength(100)
                    .HasColumnType("nvarchar(100)");

                b.HasKey("Id");

                b.ToTable("Languages", (string)null);
            });

        modelBuilder.Entity("LifeSync.API.Models.ApplicationUser.User", b =>
            {
                b.HasOne("LifeSync.API.Models.Languages.Language", "Language")
                    .WithMany()
                    .HasForeignKey("LanguageId")
                    .OnDelete(DeleteBehavior.Cascade)
                    .IsRequired();

                b.OwnsOne("LifeSync.API.Shared.Money", "Balance", b1 =>
                    {
                        b1.Property<string>("UserId")
                            .HasColumnType("nvarchar(450)");

                        b1.Property<decimal>("Amount")
                            .HasColumnType("decimal(18,2)");

                        b1.Property<string>("Currency")
                            .IsRequired()
                            .HasColumnType("nvarchar(max)");

                        b1.HasKey("UserId");

                        b1.ToTable("Users");

                        b1.WithOwner()
                            .HasForeignKey("UserId");
                    });

                b.OwnsOne("LifeSync.API.Shared.Currency", "CurrencyPreference", b1 =>
                    {
                        b1.Property<string>("UserId")
                            .HasColumnType("nvarchar(450)");

                        b1.Property<string>("Code")
                            .IsRequired()
                            .HasColumnType("nvarchar(max)")
                            .HasColumnName("CurrencyPreference");

                        b1.HasKey("UserId");

                        b1.ToTable("Users");

                        b1.WithOwner()
                            .HasForeignKey("UserId");
                    });

                b.Navigation("Balance")
                    .IsRequired();

                b.Navigation("CurrencyPreference")
                    .IsRequired();

                b.Navigation("Language");
            });

        modelBuilder.Entity("LifeSync.API.Models.Expenses.ExpenseTransaction", b =>
            {
                b.HasOne("LifeSync.API.Models.ApplicationUser.User", "User")
                    .WithMany("ExpenseTransactions")
                    .HasForeignKey("UserId1");

                b.OwnsOne("LifeSync.API.Shared.Money", "Amount", b1 =>
                    {
                        b1.Property<Guid>("ExpenseTransactionId")
                            .HasColumnType("uniqueidentifier");

                        b1.Property<decimal>("Amount")
                            .HasColumnType("decimal(18,2)");

                        b1.Property<string>("Currency")
                            .IsRequired()
                            .HasColumnType("nvarchar(max)");

                        b1.HasKey("ExpenseTransactionId");

                        b1.ToTable("ExpenseTransactions");

                        b1.WithOwner()
                            .HasForeignKey("ExpenseTransactionId");
                    });

                b.Navigation("Amount")
                    .IsRequired();

                b.Navigation("User");
            });

        modelBuilder.Entity("LifeSync.API.Models.Incomes.IncomeTransaction", b =>
            {
                b.HasOne("LifeSync.API.Models.ApplicationUser.User", "User")
                    .WithMany("IncomeTransactions")
                    .HasForeignKey("UserId1");

                b.OwnsOne("LifeSync.API.Shared.Money", "Amount", b1 =>
                    {
                        b1.Property<Guid>("IncomeTransactionId")
                            .HasColumnType("uniqueidentifier");

                        b1.Property<decimal>("Amount")
                            .HasColumnType("decimal(18,2)");

                        b1.Property<string>("Currency")
                            .IsRequired()
                            .HasColumnType("nvarchar(max)");

                        b1.HasKey("IncomeTransactionId");

                        b1.ToTable("IncomeTransactions");

                        b1.WithOwner()
                            .HasForeignKey("IncomeTransactionId");
                    });

                b.Navigation("Amount")
                    .IsRequired();

                b.Navigation("User");
            });

        modelBuilder.Entity("LifeSync.API.Models.ApplicationUser.User", b =>
            {
                b.Navigation("ExpenseTransactions");

                b.Navigation("IncomeTransactions");
            });
#pragma warning restore 612, 618
    }
}
