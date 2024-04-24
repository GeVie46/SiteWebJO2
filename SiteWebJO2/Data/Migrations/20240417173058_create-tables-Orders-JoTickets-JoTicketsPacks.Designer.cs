﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using SiteWebJO2.Data;

#nullable disable

namespace SiteWebJO2.Data.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20240417173058_create-tables-Orders-JoTickets-JoTicketsPacks")]
    partial class createtablesOrdersJoTicketsJoTicketsPacks
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.4")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            MySqlModelBuilderExtensions.AutoIncrementColumns(modelBuilder);

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRole", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("varchar(255)");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken()
                        .HasColumnType("longtext");

                    b.Property<string>("Name")
                        .HasMaxLength(256)
                        .HasColumnType("varchar(256)");

                    b.Property<string>("NormalizedName")
                        .HasMaxLength(256)
                        .HasColumnType("varchar(256)");

                    b.HasKey("Id");

                    b.HasIndex("NormalizedName")
                        .IsUnique()
                        .HasDatabaseName("RoleNameIndex");

                    b.ToTable("AspNetRoles", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    MySqlPropertyBuilderExtensions.UseMySqlIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("ClaimType")
                        .HasColumnType("longtext");

                    b.Property<string>("ClaimValue")
                        .HasColumnType("longtext");

                    b.Property<string>("RoleId")
                        .IsRequired()
                        .HasColumnType("varchar(255)");

                    b.HasKey("Id");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetRoleClaims", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    MySqlPropertyBuilderExtensions.UseMySqlIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("ClaimType")
                        .HasColumnType("longtext");

                    b.Property<string>("ClaimValue")
                        .HasColumnType("longtext");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("varchar(255)");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserClaims", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                {
                    b.Property<string>("LoginProvider")
                        .HasMaxLength(128)
                        .HasColumnType("varchar(128)");

                    b.Property<string>("ProviderKey")
                        .HasMaxLength(128)
                        .HasColumnType("varchar(128)");

                    b.Property<string>("ProviderDisplayName")
                        .HasColumnType("longtext");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("varchar(255)");

                    b.HasKey("LoginProvider", "ProviderKey");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserLogins", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
                {
                    b.Property<string>("UserId")
                        .HasColumnType("varchar(255)");

                    b.Property<string>("RoleId")
                        .HasColumnType("varchar(255)");

                    b.HasKey("UserId", "RoleId");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetUserRoles", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                {
                    b.Property<string>("UserId")
                        .HasColumnType("varchar(255)");

                    b.Property<string>("LoginProvider")
                        .HasMaxLength(128)
                        .HasColumnType("varchar(128)");

                    b.Property<string>("Name")
                        .HasMaxLength(128)
                        .HasColumnType("varchar(128)");

                    b.Property<string>("Value")
                        .HasColumnType("longtext");

                    b.HasKey("UserId", "LoginProvider", "Name");

                    b.ToTable("AspNetUserTokens", (string)null);
                });

            modelBuilder.Entity("SiteWebJO2.Models.ApplicationUser", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("varchar(255)");

                    b.Property<int>("AccessFailedCount")
                        .HasColumnType("int");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken()
                        .HasColumnType("longtext");

                    b.Property<string>("Email")
                        .HasMaxLength(256)
                        .HasColumnType("varchar(256)");

                    b.Property<bool>("EmailConfirmed")
                        .HasColumnType("tinyint(1)");

                    b.Property<string>("Lastname")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<bool>("LockoutEnabled")
                        .HasColumnType("tinyint(1)");

                    b.Property<DateTimeOffset?>("LockoutEnd")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("NormalizedEmail")
                        .HasMaxLength(256)
                        .HasColumnType("varchar(256)");

                    b.Property<string>("NormalizedUserName")
                        .HasMaxLength(256)
                        .HasColumnType("varchar(256)");

                    b.Property<string>("PasswordHash")
                        .HasColumnType("longtext");

                    b.Property<string>("PhoneNumber")
                        .HasColumnType("longtext");

                    b.Property<bool>("PhoneNumberConfirmed")
                        .HasColumnType("tinyint(1)");

                    b.Property<string>("SecurityStamp")
                        .HasColumnType("longtext");

                    b.Property<bool>("TwoFactorEnabled")
                        .HasColumnType("tinyint(1)");

                    b.Property<string>("UserName")
                        .HasMaxLength(256)
                        .HasColumnType("varchar(256)");

                    b.Property<byte[]>("Userkey")
                        .IsRequired()
                        .HasColumnType("longblob");

                    b.HasKey("Id");

                    b.HasIndex("NormalizedEmail")
                        .HasDatabaseName("EmailIndex");

                    b.HasIndex("NormalizedUserName")
                        .IsUnique()
                        .HasDatabaseName("UserNameIndex");

                    b.ToTable("AspNetUsers", (string)null);
                });

            modelBuilder.Entity("SiteWebJO2.Models.JoSession", b =>
                {
                    b.Property<int>("JoSessionId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    MySqlPropertyBuilderExtensions.UseMySqlIdentityColumn(b.Property<int>("JoSessionId"));

                    b.Property<DateTime>("JoSessionDate")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("JoSessionDescription")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("JoSessionImage")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("JoSessionName")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<int>("JoSessionNbTotalAttendees")
                        .HasColumnType("int");

                    b.Property<int>("JoSessionNbTotalBooked")
                        .HasColumnType("int");

                    b.Property<string>("JoSessionPlace")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<decimal>("JoSessionPrice")
                        .HasColumnType("decimal(65,30)");

                    b.HasKey("JoSessionId");

                    b.ToTable("JoSessions");
                });

            modelBuilder.Entity("SiteWebJO2.Models.JoTicket", b =>
                {
                    b.Property<int>("JoTicketId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    MySqlPropertyBuilderExtensions.UseMySqlIdentityColumn(b.Property<int>("JoTicketId"));

                    b.Property<string>("ApplicationUserId")
                        .IsRequired()
                        .HasColumnType("varchar(255)");

                    b.Property<int>("JoSessionId")
                        .HasColumnType("int");

                    b.Property<byte[]>("JoTicketKey")
                        .IsRequired()
                        .HasColumnType("longblob");

                    b.Property<int>("JoTicketPackId")
                        .HasColumnType("int");

                    b.Property<decimal>("JoTicketPrice")
                        .HasColumnType("decimal(65,30)");

                    b.Property<bool>("JoTicketStatus")
                        .HasColumnType("tinyint(1)");

                    b.Property<int>("OrderId")
                        .HasColumnType("int");

                    b.HasKey("JoTicketId");

                    b.HasIndex("ApplicationUserId");

                    b.HasIndex("JoSessionId");

                    b.HasIndex("JoTicketPackId");

                    b.HasIndex("OrderId");

                    b.ToTable("JoTickets");
                });

            modelBuilder.Entity("SiteWebJO2.Models.JoTicketPack", b =>
                {
                    b.Property<int>("JoTicketPackId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    MySqlPropertyBuilderExtensions.UseMySqlIdentityColumn(b.Property<int>("JoTicketPackId"));

                    b.Property<string>("JoTicketPackName")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("varchar(255)");

                    b.Property<bool>("JoTicketPackStatus")
                        .HasColumnType("tinyint(1)");

                    b.Property<int>("NbAttendees")
                        .HasColumnType("int");

                    b.Property<decimal>("ReductionRate")
                        .HasColumnType("decimal(65,30)");

                    b.HasKey("JoTicketPackId");

                    b.ToTable("JoTicketPacks");
                });

            modelBuilder.Entity("SiteWebJO2.Models.Order", b =>
                {
                    b.Property<int>("OrderId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    MySqlPropertyBuilderExtensions.UseMySqlIdentityColumn(b.Property<int>("OrderId"));

                    b.Property<string>("ApplicationUserId")
                        .IsRequired()
                        .HasColumnType("varchar(255)");

                    b.Property<decimal>("OrderAmount")
                        .HasColumnType("decimal(65,30)");

                    b.Property<DateTime>("OrderDate")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("OrderStatus")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("varchar(255)");

                    b.Property<string>("TransactionId")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.HasKey("OrderId");

                    b.HasIndex("ApplicationUserId");

                    b.ToTable("Orders");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityRole", null)
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                {
                    b.HasOne("SiteWebJO2.Models.ApplicationUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                {
                    b.HasOne("SiteWebJO2.Models.ApplicationUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityRole", null)
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("SiteWebJO2.Models.ApplicationUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                {
                    b.HasOne("SiteWebJO2.Models.ApplicationUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("SiteWebJO2.Models.JoTicket", b =>
                {
                    b.HasOne("SiteWebJO2.Models.ApplicationUser", "ApplicationUser")
                        .WithMany("JoTickets")
                        .HasForeignKey("ApplicationUserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("SiteWebJO2.Models.JoSession", "JoSession")
                        .WithMany("JoTickets")
                        .HasForeignKey("JoSessionId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("SiteWebJO2.Models.JoTicketPack", "JoTicketPack")
                        .WithMany("JoTickets")
                        .HasForeignKey("JoTicketPackId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("SiteWebJO2.Models.Order", "Order")
                        .WithMany("JoTickets")
                        .HasForeignKey("OrderId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("ApplicationUser");

                    b.Navigation("JoSession");

                    b.Navigation("JoTicketPack");

                    b.Navigation("Order");
                });

            modelBuilder.Entity("SiteWebJO2.Models.Order", b =>
                {
                    b.HasOne("SiteWebJO2.Models.ApplicationUser", "ApplicationUser")
                        .WithMany("Orders")
                        .HasForeignKey("ApplicationUserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("ApplicationUser");
                });

            modelBuilder.Entity("SiteWebJO2.Models.ApplicationUser", b =>
                {
                    b.Navigation("JoTickets");

                    b.Navigation("Orders");
                });

            modelBuilder.Entity("SiteWebJO2.Models.JoSession", b =>
                {
                    b.Navigation("JoTickets");
                });

            modelBuilder.Entity("SiteWebJO2.Models.JoTicketPack", b =>
                {
                    b.Navigation("JoTickets");
                });

            modelBuilder.Entity("SiteWebJO2.Models.Order", b =>
                {
                    b.Navigation("JoTickets");
                });
#pragma warning restore 612, 618
        }
    }
}