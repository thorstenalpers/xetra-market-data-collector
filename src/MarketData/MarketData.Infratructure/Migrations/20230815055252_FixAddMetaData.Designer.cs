﻿// <auto-generated />
using System;
using MarketData.Infratructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace MarketData.Infratructure.Migrations
{
    [DbContext(typeof(MarketDataDbContext))]
    [Migration("20230815055252_FixAddMetaData")]
    partial class FixAddMetaData
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.10")
                .HasAnnotation("Proxies:ChangeTracking", false)
                .HasAnnotation("Proxies:CheckEquality", false)
                .HasAnnotation("Proxies:LazyLoading", true)
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("MarketData.Domain.Entities.Asset", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<long?>("AvgTradingVolume")
                        .HasColumnType("bigint");

                    b.Property<long?>("CntEmployees")
                        .HasColumnType("bigint");

                    b.Property<DateTime?>("CreatedAt")
                        .HasColumnType("date");

                    b.Property<string>("Currency")
                        .HasColumnType("text");

                    b.Property<string>("Exchange")
                        .HasColumnType("text");

                    b.Property<string>("ExchangeCloseTime")
                        .HasColumnType("text");

                    b.Property<string>("ExchangeCountryIso")
                        .HasColumnType("text");

                    b.Property<string>("Industry")
                        .HasColumnType("text");

                    b.Property<string>("Isin")
                        .HasColumnType("text");

                    b.Property<long?>("MarketCapitalization")
                        .HasColumnType("bigint");

                    b.Property<string>("Mnemonic")
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .HasColumnType("text");

                    b.Property<string>("Sector")
                        .HasColumnType("text");

                    b.Property<string>("Symbol")
                        .HasColumnType("text");

                    b.Property<string>("Type")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Url")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("Isin")
                        .IsUnique();

                    b.ToTable("Assets", "TradeX");
                });

            modelBuilder.Entity("MarketData.Domain.Entities.AssetMetric", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<DateTime?>("CreatedAt")
                        .HasColumnType("date");

                    b.Property<string>("Type")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("AssetMetrics", "TradeX");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            CreatedAt = new DateTime(2023, 8, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            Type = "PriceClose"
                        },
                        new
                        {
                            Id = 2,
                            CreatedAt = new DateTime(2023, 8, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            Type = "PriceOpen"
                        },
                        new
                        {
                            Id = 3,
                            CreatedAt = new DateTime(2023, 8, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            Type = "Volumen"
                        });
                });

            modelBuilder.Entity("MarketData.Domain.Entities.AssetRecord", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<int>("AssetId")
                        .HasColumnType("integer");

                    b.Property<int>("AssetMetricId")
                        .HasColumnType("integer");

                    b.Property<DateTime?>("CreatedAt")
                        .HasColumnType("date");

                    b.Property<DateTime>("Date")
                        .HasColumnType("date");

                    b.Property<double>("Value")
                        .HasColumnType("double precision");

                    b.HasKey("Id");

                    b.HasIndex("AssetId");

                    b.HasIndex("AssetMetricId");

                    b.HasIndex("Date", "AssetId", "AssetMetricId")
                        .IsUnique();

                    b.ToTable("AssetRecords", "TradeX");
                });

            modelBuilder.Entity("MarketData.Domain.Entities.AssetRecord", b =>
                {
                    b.HasOne("MarketData.Domain.Entities.Asset", "Asset")
                        .WithMany()
                        .HasForeignKey("AssetId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("MarketData.Domain.Entities.AssetMetric", "AssetMetric")
                        .WithMany()
                        .HasForeignKey("AssetMetricId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Asset");

                    b.Navigation("AssetMetric");
                });
#pragma warning restore 612, 618
        }
    }
}
