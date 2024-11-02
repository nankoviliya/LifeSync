﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using PersonalFinances.API.Persistence;

#nullable disable

namespace PersonalFinances.API.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    partial class ApplicationDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.10")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("PersonalFinances.API.Models.ExpenseTransaction", b =>
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

                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("ExpenseTransactions", (string)null);
                });

            modelBuilder.Entity("PersonalFinances.API.Models.IncomeTransaction", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("Date")
                        .HasColumnType("datetime2");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("IncomeTransactions", (string)null);
                });

            modelBuilder.Entity("PersonalFinances.API.Models.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasMaxLength(320)
                        .HasColumnType("nvarchar(320)");

                    b.Property<string>("FirstName")
                        .IsRequired()
                        .HasMaxLength(500)
                        .HasColumnType("nvarchar(500)");

                    b.Property<string>("LastName")
                        .IsRequired()
                        .HasMaxLength(500)
                        .HasColumnType("nvarchar(500)");

                    b.Property<string>("PasswordHash")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Username")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("Email")
                        .IsUnique();

                    b.ToTable("Users", (string)null);
                });

            modelBuilder.Entity("PersonalFinances.API.Models.ExpenseTransaction", b =>
                {
                    b.HasOne("PersonalFinances.API.Models.User", "User")
                        .WithMany("ExpenseTransactions")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.OwnsOne("PersonalFinances.API.Shared.Money", "Amount", b1 =>
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

            modelBuilder.Entity("PersonalFinances.API.Models.IncomeTransaction", b =>
                {
                    b.HasOne("PersonalFinances.API.Models.User", "User")
                        .WithMany("IncomeTransactions")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.OwnsOne("PersonalFinances.API.Shared.Money", "Amount", b1 =>
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

            modelBuilder.Entity("PersonalFinances.API.Models.User", b =>
                {
                    b.OwnsOne("PersonalFinances.API.Shared.Money", "Balance", b1 =>
                        {
                            b1.Property<int>("UserId")
                                .HasColumnType("int");

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

                    b.OwnsOne("PersonalFinances.API.Shared.Currency", "CurrencyPreference", b1 =>
                        {
                            b1.Property<int>("UserId")
                                .HasColumnType("int");

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
                });

            modelBuilder.Entity("PersonalFinances.API.Models.User", b =>
                {
                    b.Navigation("ExpenseTransactions");

                    b.Navigation("IncomeTransactions");
                });
#pragma warning restore 612, 618
        }
    }
}