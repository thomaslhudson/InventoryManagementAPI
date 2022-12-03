﻿// <auto-generated />
using System;
using InventoryManagement.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace InventoryManagement.Data.Migrations
{
    [DbContext(typeof(InventoryContext))]
    [Migration("20220925043736_CreateDatabase")]
    partial class CreateDatabase
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.8")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder, 1L, 1);

            modelBuilder.Entity("InventoryManagement.Data.Models.Group", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier")
                        .HasDefaultValueSql("newsequentialid()");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.HasKey("Id");

                    b.HasIndex("Name")
                        .IsUnique();

                    b.ToTable("Group", (string)null);
                });

            modelBuilder.Entity("InventoryManagement.Data.Models.Product", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier")
                        .HasDefaultValueSql("newsequentialid()");

                    b.Property<Guid>("GroupId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<bool>("IsActive")
                        .HasColumnType("bit")
                        .HasDefaultValue(false);

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<decimal>("UnitPrice")
                        .HasColumnType("decimal(9,3)");

                    b.Property<string>("Upc")
                        .HasMaxLength(12)
                        .HasColumnType("nchar(12)")
                        .IsFixedLength();

                    b.HasKey("Id");

                    b.HasIndex("GroupId");

                    b.HasIndex("Name")
                        .IsUnique()
                        .HasDatabaseName("UX_Prouct_Name");

                    b.HasIndex("Upc")
                        .IsUnique()
                        .HasDatabaseName("UX_Product_Upc_Exclude_Nulls")
                        .HasFilter("Upc IS NOT NULL");

                    b.ToTable("Product", (string)null);
                });

            modelBuilder.Entity("InventoryManagement.Data.Models.Record", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier")
                        .HasDefaultValueSql("newsequentialid()");

                    b.Property<DateTime>("Created")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime2")
                        .HasDefaultValueSql("GETUTCDATE()");

                    b.Property<DateTime?>("Ended")
                        .HasColumnType("datetime2");

                    b.Property<byte>("Month")
                        .HasColumnType("tinyint");

                    b.Property<DateTime?>("Started")
                        .HasColumnType("datetime2");

                    b.Property<short>("Year")
                        .HasColumnType("smallint");

                    b.HasKey("Id");

                    b.HasIndex("Month", "Year")
                        .IsUnique();

                    b.ToTable("Record", (string)null);
                });

            modelBuilder.Entity("InventoryManagement.Data.Models.RecordItem", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier")
                        .HasDefaultValueSql("newsequentialid()");

                    b.Property<DateTime>("DateCreated")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("DateModified")
                        .HasColumnType("datetime2");

                    b.Property<Guid>("ProductId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<decimal>("Quantity")
                        .HasColumnType("decimal(9,3)");

                    b.Property<Guid>("RecordId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("ProductId");

                    b.HasIndex("RecordId", "ProductId")
                        .IsUnique();

                    b.ToTable("RecordItem", (string)null);
                });

            modelBuilder.Entity("InventoryManagement.Data.Models.Product", b =>
                {
                    b.HasOne("InventoryManagement.Data.Models.Group", "Group")
                        .WithMany("Products")
                        .HasForeignKey("GroupId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Group");
                });

            modelBuilder.Entity("InventoryManagement.Data.Models.RecordItem", b =>
                {
                    b.HasOne("InventoryManagement.Data.Models.Product", "Product")
                        .WithMany("RecordItems")
                        .HasForeignKey("ProductId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("InventoryManagement.Data.Models.Record", "Record")
                        .WithMany("RecordItems")
                        .HasForeignKey("RecordId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Product");

                    b.Navigation("Record");
                });

            modelBuilder.Entity("InventoryManagement.Data.Models.Group", b =>
                {
                    b.Navigation("Products");
                });

            modelBuilder.Entity("InventoryManagement.Data.Models.Product", b =>
                {
                    b.Navigation("RecordItems");
                });

            modelBuilder.Entity("InventoryManagement.Data.Models.Record", b =>
                {
                    b.Navigation("RecordItems");
                });
#pragma warning restore 612, 618
        }
    }
}
