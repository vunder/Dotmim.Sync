﻿using System;
using Dotmim.Sync.Tests.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Dotmim.Sync.Tests.Models
{


    public partial class AdventureWorksContext : DbContext
    {
        internal bool useSchema = false;
        public ProviderType ProviderType { get; }
        public string ConnectionString { get; }

        public AdventureWorksContext(ProviderType providerType, string connectionString, bool fallbackUseSchema = true) : this()
        {
            ProviderType = providerType;
            ConnectionString = connectionString;

            useSchema = ProviderType == ProviderType.Sql && fallbackUseSchema;
        }
        public AdventureWorksContext(ProviderRun providerRun, bool fallbackUseSchema = true) : this()
        {

            ProviderType = providerRun.ProviderType;
            ConnectionString = providerRun.ConnectionString;

            useSchema = ProviderType == ProviderType.Sql && fallbackUseSchema;
        }

        public AdventureWorksContext(DbContextOptions<AdventureWorksContext> options)
            : base(options)
        {
        }

        public AdventureWorksContext()
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                switch (ProviderType)
                {
                    case ProviderType.Sql:
                        optionsBuilder.UseSqlServer(ConnectionString);
                        break;
                    case ProviderType.MySql:
                        optionsBuilder.UseMySql(ConnectionString);
                        break;
                    case ProviderType.Sqlite:
                        optionsBuilder.UseSqlite(ConnectionString);
                        break;
                }
            }

            // Invalid cache based on ProviderType and UseSchema and not Only Provider
            // So we can have two models shared on SQL : One with Schema, the other without Schema
            optionsBuilder.ReplaceService<IModelCacheKeyFactory, MyModelCacheKeyFactory>();

        }

        public virtual DbSet<Address> Address { get; set; }
        public virtual DbSet<Customer> Customer { get; set; }
        public virtual DbSet<CustomerAddress> CustomerAddress { get; set; }
        public virtual DbSet<Log> Log { get; set; }
        public virtual DbSet<Product> Product { get; set; }
        public virtual DbSet<ProductCategory> ProductCategory { get; set; }
        public virtual DbSet<ProductModel> ProductModel { get; set; }
        public virtual DbSet<SalesOrderDetail> SalesOrderDetail { get; set; }
        public virtual DbSet<SalesOrderHeader> SalesOrderHeader { get; set; }
        public virtual DbSet<Sql> Sql { get; set; }
        public virtual DbSet<Posts> Posts { get; set; }
        public virtual DbSet<PostTag> PostTag { get; set; }
        public virtual DbSet<Tags> Tags { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Address>(entity =>
            {
                entity.HasIndex(e => e.StateProvince);

                entity.HasIndex(e => new { e.City, e.StateProvince, e.PostalCode, e.CountryRegion });

                entity.Property(e => e.AddressId).HasColumnName("AddressID");

                entity.Property(e => e.AddressLine1)
                    .IsRequired();

                entity.Property(e => e.City)
                    .IsRequired()
                    .HasMaxLength(30);

                entity.Property(e => e.CountryRegion)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.ModifiedDate)
                    .HasColumnType("datetime")
                    .ValueGeneratedOnAdd()
                    .HasDefaultValue(DateTime.Now);

                if (this.ProviderType == ProviderType.Sql)
                    entity.Property(e => e.ModifiedDate).HasDefaultValueSql("(getdate())");
                else if (this.ProviderType == ProviderType.MySql)
                    entity.Property(e => e.ModifiedDate).HasDefaultValueSql("CURRENT_TIMESTAMP()");

                entity.Property(e => e.PostalCode)
                    .IsRequired()
                    .IsUnicode()
                    .HasMaxLength(15);

                entity.Property(e => e.Rowguid)
                    .HasColumnName("rowguid")
                    .ValueGeneratedOnAdd();

                if (this.ProviderType == ProviderType.Sql)
                    entity.Property(e => e.Rowguid).HasDefaultValueSql("(newid())");

                entity.Property(e => e.StateProvince)
                    .IsRequired()
                    .HasMaxLength(50);
            });

            modelBuilder.Entity<Customer>(entity =>
            {
                entity.HasIndex(e => e.EmailAddress);

                entity.Property(e => e.CustomerId)
                    .HasColumnName("CustomerID");

                if (this.ProviderType == ProviderType.Sql)
                    entity.Property(e => e.CustomerId).HasDefaultValueSql("(newid())");

                entity.Property(e => e.CompanyName).HasMaxLength(128);

                entity.Property(e => e.EmailAddress).HasMaxLength(50);

                entity.Property(e => e.FirstName)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.LastName)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.MiddleName).HasMaxLength(50);

                entity.Property(e => e.ModifiedDate)
                    .HasColumnType("datetime");

                if (this.ProviderType == ProviderType.Sql)
                    entity.Property(e => e.ModifiedDate).HasDefaultValueSql("(getdate())");
                else if (this.ProviderType == ProviderType.MySql)
                    entity.Property(e => e.ModifiedDate).HasDefaultValueSql("CURRENT_TIMESTAMP()");


                entity.Property(e => e.PasswordHash)
        .IsRequired()
        .HasMaxLength(128)
        .IsUnicode(false);

                entity.Property(e => e.PasswordSalt)
        .IsRequired()
        .HasMaxLength(10)
        .IsUnicode(false);

                entity.Property(e => e.Phone).HasMaxLength(25);

                entity.Property(e => e.Rowguid)
        .HasColumnName("rowguid")
        .ValueGeneratedOnAdd();

                if (this.ProviderType == ProviderType.Sql)
                    entity.Property(e => e.Rowguid).HasDefaultValueSql("(newid())");

                entity.Property(e => e.SalesPerson).HasMaxLength(256);

                entity.Property(e => e.Suffix).HasMaxLength(10);

                entity.Property(e => e.Title).HasMaxLength(8);
            });

            modelBuilder.Entity<CustomerAddress>(entity =>
            {
                entity.HasKey(e => new { e.CustomerId, e.AddressId });

                entity.Property(e => e.CustomerId)
                    .HasColumnName("CustomerID");

                entity.Property(e => e.AddressId).HasColumnName("AddressID");

                entity.Property(e => e.AddressType)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.ModifiedDate)
                    .HasColumnType("datetime");

                if (this.ProviderType == ProviderType.Sql)
                    entity.Property(e => e.ModifiedDate).HasDefaultValueSql("(getdate())");
                else if (this.ProviderType == ProviderType.MySql)
                    entity.Property(e => e.ModifiedDate).HasDefaultValueSql("CURRENT_TIMESTAMP()");

                entity.Property(e => e.Rowguid)
                    .HasColumnName("rowguid");

                if (this.ProviderType == ProviderType.Sql)
                    entity.Property(e => e.Rowguid).HasDefaultValueSql("(newid())");

                entity.HasOne(d => d.Address)
                    .WithMany(p => p.CustomerAddress)
                    .HasForeignKey(d => d.AddressId)
                    .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.Customer)
                    .WithMany(p => p.CustomerAddress)
                    .HasForeignKey(d => d.CustomerId)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<Log>(entity =>
            {
                entity.HasKey(e => e.Oid);

                entity.Property(e => e.Oid).ValueGeneratedNever();

                entity.Property(e => e.ErrorDescription).HasMaxLength(50);

                entity.Property(e => e.Gcrecord).HasColumnName("GCRecord");

                entity.Property(e => e.Operation).HasMaxLength(50);

                entity.Property(e => e.TimeStamp).HasColumnType("datetime");
            });

            modelBuilder.Entity<Product>(entity =>
            {
                if (useSchema)
                    entity.ToTable("Product", "SalesLT");

                entity.HasKey(e => e.ProductId);

                entity.HasIndex(e => e.Name)
                    .HasName("AK_Product_Name")
                    .IsUnique();

                entity.HasIndex(e => e.ProductNumber)
                    .HasName("AK_Product_ProductNumber")
                    .IsUnique();

                entity.Property(e => e.ProductId)
                    .HasColumnName("ProductID");

                entity.Property(e => e.Color).HasMaxLength(15);

                entity.Property(e => e.DiscontinuedDate).HasColumnType("datetime");

                if (this.ProviderType == ProviderType.Sql)
                    entity.Property(e => e.ListPrice).HasColumnType("money");

                entity.Property(e => e.ModifiedDate)
                    .HasColumnType("datetime");

                if (this.ProviderType == ProviderType.Sql)
                    entity.Property(e => e.ModifiedDate).HasDefaultValueSql("(getdate())");
                else if (this.ProviderType == ProviderType.MySql)
                    entity.Property(e => e.ModifiedDate).HasDefaultValueSql("CURRENT_TIMESTAMP()");


                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.ProductCategoryId)
                    .HasColumnName("ProductCategoryID")
                    .HasMaxLength(6);

                entity.Property(e => e.ProductModelId).HasColumnName("ProductModelID");

                entity.Property(e => e.ProductNumber)
                    .IsRequired()
                    .HasMaxLength(25);

                entity.Property(e => e.Rowguid)
                    .HasColumnName("rowguid");

                if (this.ProviderType == ProviderType.Sql)
                    entity.Property(e => e.Rowguid).HasDefaultValueSql("(newid())");

                entity.Property(e => e.SellEndDate).HasColumnType("datetime");

                entity.Property(e => e.SellStartDate).HasColumnType("datetime");

                entity.Property(e => e.Size).HasMaxLength(5);

                if (this.ProviderType == ProviderType.Sql)
                    entity.Property(e => e.StandardCost).HasColumnType("money");

                entity.Property(e => e.ThumbnailPhotoFileName).HasMaxLength(50);

                entity.Property(e => e.Weight).HasColumnType("decimal(8, 2)");

                entity.HasOne(d => d.ProductCategory)
                    .WithMany(p => p.Product)
                    .HasForeignKey(d => d.ProductCategoryId);

                entity.HasOne(d => d.ProductModel)
                    .WithMany(p => p.Product)
                    .HasForeignKey(d => d.ProductModelId);
            });

            modelBuilder.Entity<ProductCategory>(entity =>
            {
                if (useSchema)
                    entity.ToTable("ProductCategory", "SalesLT");

                entity.HasIndex(e => e.Name)
                    .HasName("AK_ProductCategory_Name")
                    .IsUnique();

                entity.Property(e => e.ProductCategoryId)
                    .HasColumnName("ProductCategoryID")
                    .HasMaxLength(6)
                    .ValueGeneratedNever();

                entity.Property(e => e.ModifiedDate)
                    .HasColumnType("datetime");

                if (this.ProviderType == ProviderType.Sql)
                    entity.Property(e => e.ModifiedDate).HasDefaultValueSql("(getdate())");
                else if (this.ProviderType == ProviderType.MySql)
                    entity.Property(e => e.ModifiedDate).HasDefaultValueSql("CURRENT_TIMESTAMP()");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnName("Name")
                    .HasMaxLength(50);

                entity.Property(e => e.ParentProductCategoryId)
                    .HasColumnName("ParentProductCategoryID")
                    .HasMaxLength(6);

                entity.Property(e => e.Rowguid)
                    .HasColumnName("rowguid");

                if (this.ProviderType == ProviderType.Sql)
                    entity.Property(e => e.Rowguid).HasDefaultValueSql("(newid())");

                entity.HasOne(d => d.ParentProductCategory)
                    .WithMany(p => p.InverseParentProductCategory)
                    .HasForeignKey(d => d.ParentProductCategoryId)
                    .HasConstraintName("FK_ProductCategory_ProductCategory_ParentProductCategoryID_ProductCategoryID");
            });

            modelBuilder.Entity<ProductModel>(entity =>
            {
                if (useSchema)
                    entity.ToTable("ProductModel", "SalesLT");

                entity.HasIndex(e => e.Name)
                    .HasName("AK_ProductModel_Name")
                    .IsUnique();

                entity.Property(e => e.ProductModelId).HasColumnName("ProductModelID");

                if (this.ProviderType == ProviderType.Sql)
                    entity.Property(e => e.CatalogDescription).HasColumnType("xml");

                entity.Property(e => e.ModifiedDate)
                    .HasColumnType("datetime");

                if (this.ProviderType == ProviderType.Sql)
                    entity.Property(e => e.ModifiedDate).HasDefaultValueSql("(getdate())");
                else if (this.ProviderType == ProviderType.MySql)
                    entity.Property(e => e.ModifiedDate).HasDefaultValueSql("CURRENT_TIMESTAMP()");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.Rowguid)
                    .HasColumnName("rowguid");

                if (this.ProviderType == ProviderType.Sql)
                    entity.Property(e => e.Rowguid).HasDefaultValueSql("(newid())");
            });

            modelBuilder.Entity<SalesOrderDetail>(entity =>
            {
                if (useSchema)
                    entity.ToTable("SalesOrderDetail", "SalesLT");

                entity.HasKey(e => new { e.SalesOrderDetailId });

                entity.HasIndex(e => e.ProductId);

                entity.Property(e => e.SalesOrderId)
                    .HasColumnName("SalesOrderID")
                    .ValueGeneratedNever();

                entity.Property(e => e.SalesOrderDetailId)
                    .HasColumnName("SalesOrderDetailID")
                    .ValueGeneratedOnAdd();

                if (this.ProviderType == ProviderType.Sql)
                    entity.Property(e => e.LineTotal).HasColumnType("numeric(38, 6)");

                entity.Property(e => e.ModifiedDate)
                    .HasColumnType("datetime");

                if (this.ProviderType == ProviderType.Sql)
                    entity.Property(e => e.ModifiedDate).HasDefaultValueSql("(getdate())");
                else if (this.ProviderType == ProviderType.MySql)
                    entity.Property(e => e.ModifiedDate).HasDefaultValueSql("CURRENT_TIMESTAMP()");

                entity.Property(e => e.ProductId)
                    .HasColumnName("ProductID")
                    .ValueGeneratedNever();

                entity.Property(e => e.Rowguid)
                    .HasColumnName("rowguid");

                if (this.ProviderType == ProviderType.Sql)
                    entity.Property(e => e.Rowguid).HasDefaultValueSql("(newid())");

                if (this.ProviderType == ProviderType.Sql)
                    entity.Property(e => e.UnitPrice).HasColumnType("money");

                if (this.ProviderType == ProviderType.Sql)
                    entity.Property(e => e.UnitPriceDiscount).HasColumnType("money");

                entity.HasOne(d => d.Product)
                    .WithMany(p => p.SalesOrderDetail)
                    .HasForeignKey(d => d.ProductId)
                    .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.SalesOrder)
                    .WithMany(p => p.SalesOrderDetail)
                    .HasForeignKey(d => d.SalesOrderId);
            });

            modelBuilder.Entity<SalesOrderHeader>(entity =>
            {
                if (useSchema)
                    entity.ToTable("SalesOrderHeader", "SalesLT");

                entity.HasKey(e => e.SalesOrderId);

                entity.HasIndex(e => e.CustomerId);

                entity.Property(e => e.SalesOrderId).HasColumnName("SalesOrderID");

                entity.Property(e => e.AccountNumber)
                    .HasColumnName("AccountNumber")
                    .HasMaxLength(15);

                entity.Property(e => e.BillToAddressId).HasColumnName("BillToAddressID");

                entity.Property(e => e.CreditCardApprovalCode)
                    .HasMaxLength(15)
                    .IsUnicode(false);

                entity.Property(e => e.CustomerId).HasColumnName("CustomerID");

                entity.Property(e => e.DueDate).HasColumnType("datetime");

                if (this.ProviderType == ProviderType.Sql)
                    entity.Property(e => e.Freight).HasColumnType("money");

                if (this.ProviderType == ProviderType.Sql)
                    entity.Property(e => e.Freight).HasDefaultValueSql("((0.00))");
                else if (this.ProviderType == ProviderType.MySql)
                    entity.Property(e => e.Freight).HasDefaultValueSql("0");

                entity.Property(e => e.ModifiedDate)
                    .HasColumnType("datetime");

                if (this.ProviderType == ProviderType.Sql)
                    entity.Property(e => e.ModifiedDate).HasDefaultValueSql("(getdate())");
                else if (this.ProviderType == ProviderType.MySql)
                    entity.Property(e => e.ModifiedDate).HasDefaultValueSql("CURRENT_TIMESTAMP()");

                entity.Property(e => e.OnlineOrderFlag)
                    .IsRequired()
                    .HasColumnType("bit");

                if (this.ProviderType == ProviderType.Sql)
                    entity.Property(e => e.OnlineOrderFlag).HasDefaultValueSql("((1))");
                else if (this.ProviderType == ProviderType.MySql)
                    entity.Property(e => e.OnlineOrderFlag).HasDefaultValueSql("1");

                entity.Property(e => e.OrderDate)
                    .HasColumnType("datetime");

                if (this.ProviderType == ProviderType.Sql)
                    entity.Property(e => e.OrderDate).HasDefaultValueSql("(getdate())");
                else if (this.ProviderType == ProviderType.MySql)
                    entity.Property(e => e.OrderDate).HasDefaultValueSql("CURRENT_TIMESTAMP()");

                entity.Property(e => e.PurchaseOrderNumber)
                    .HasMaxLength(25);

                entity.Property(e => e.Rowguid)
                    .HasColumnName("rowguid");

                if (this.ProviderType == ProviderType.Sql)
                    entity.Property(e => e.Rowguid).HasDefaultValueSql("(newid())");

                entity.Property(e => e.SalesOrderNumber)
                    .IsRequired()
                    .HasMaxLength(25);

                if (this.ProviderType == ProviderType.Sql)
                    entity.Property(e => e.SalesOrderNumber).HasDefaultValueSql("(('SO-XXXX'))");
                else if (this.ProviderType == ProviderType.MySql)
                    entity.Property(e => e.SalesOrderNumber).HasDefaultValueSql("'SO-XXXX'");


                entity.Property(e => e.ShipDate).HasColumnType("datetime");

                entity.Property(e => e.ShipMethod)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.ShipToAddressId).HasColumnName("ShipToAddressID");

                if (this.ProviderType == ProviderType.Sql)
                    entity.Property(e => e.Status).HasDefaultValueSql("((1))");
                else if (this.ProviderType == ProviderType.MySql)
                    entity.Property(e => e.Status).HasDefaultValueSql("1");

                if (this.ProviderType == ProviderType.Sql)
                    entity.Property(e => e.SubTotal).HasColumnType("money");

                if (this.ProviderType == ProviderType.Sql)
                    entity.Property(e => e.SubTotal).HasDefaultValueSql("((0.00))");
                else if (this.ProviderType == ProviderType.MySql)
                    entity.Property(e => e.SubTotal).HasDefaultValueSql("0");

                if (this.ProviderType == ProviderType.Sql)
                    entity.Property(e => e.TaxAmt).HasColumnType("money");

                if (this.ProviderType == ProviderType.Sql)
                    entity.Property(e => e.TaxAmt).HasDefaultValueSql("((0.00))");
                else if (this.ProviderType == ProviderType.MySql)
                    entity.Property(e => e.TaxAmt).HasDefaultValueSql("0");

                if (this.ProviderType == ProviderType.Sql)
                    entity.Property(e => e.TotalDue).HasColumnType("money");

                if (this.ProviderType == ProviderType.Sql)
                    entity.Property(e => e.TotalDue).HasDefaultValueSql("((0.00))");
                else if (this.ProviderType == ProviderType.MySql)
                    entity.Property(e => e.TotalDue).HasDefaultValueSql("0");

                entity.HasOne(d => d.BillToAddress)
                    .WithMany(p => p.SalesOrderHeaderBillToAddress)
                    .HasForeignKey(d => d.BillToAddressId)
                    .HasConstraintName("FK_SalesOrderHeader_Address_BillTo_AddressID");

                entity.HasOne(d => d.Customer)
                    .WithMany(p => p.SalesOrderHeader)
                    .HasForeignKey(d => d.CustomerId)
                    .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.ShipToAddress)
                    .WithMany(p => p.SalesOrderHeaderShipToAddress)
                    .HasForeignKey(d => d.ShipToAddressId)
                    .HasConstraintName("FK_SalesOrderHeader_Address_ShipTo_AddressID");
            });

            modelBuilder.Entity<Sql>(entity =>
            {
                entity.Property(e => e.SqlId).ValueGeneratedNever();

                entity.Property(e => e.File).IsRequired();

                // since mysql ef provider does not support Object as type in a property
                // just ignore it for this provider
                if (this.ProviderType == ProviderType.MySql)
                    entity.Ignore(e => e.Value);
                else
                    entity.Property(e => e.Value).HasColumnType("sql_variant");
            });

            modelBuilder.Entity<Posts>(entity =>
            {
                entity.HasKey(e => e.PostId);
            });

            modelBuilder.Entity<PostTag>(entity =>
            {
                entity.HasKey(e => new { e.PostId, e.TagId });

                entity.HasOne(d => d.Post)
                    .WithMany(p => p.PostTag)
                    .HasForeignKey(d => d.PostId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_PostTag_Posts");

                entity.HasOne(d => d.Tag)
                    .WithMany(p => p.PostTag)
                    .HasForeignKey(d => d.TagId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_PostTag_Tags");
            });

            modelBuilder.Entity<Tags>(entity =>
            {
                entity.HasKey(e => e.TagId);
            });

            this.OnSeeding(modelBuilder);
        }

        /// <summary>
        /// Need to specify all default values
        /// See https://github.com/aspnet/EntityFrameworkCore/issues/13206 for current issue
        /// </summary>
protected void OnSeeding(ModelBuilder modelBuilder)
{
    modelBuilder.Entity<Address>().HasData(
        new Address { AddressId = 1, AddressLine1 = "8713 Yosemite Ct.", City = "Bothell", StateProvince = "Washington", CountryRegion = "United States", PostalCode = "98011" },
        new Address { AddressId = 2, AddressLine1 = "1318 Lasalle Street", City = "Bothell", StateProvince = "Washington", CountryRegion = "United States", PostalCode = "98011" },
        new Address { AddressId = 3, AddressLine1 = "9178 Jumping St.", City = "Dallas", StateProvince = "Texas", CountryRegion = "United States", PostalCode = "75201" },
        new Address { AddressId = 4, AddressLine1 = "9228 Via Del Sol", City = "Phoenix", StateProvince = "Arizona", CountryRegion = "United States", PostalCode = "85004" },
        new Address { AddressId = 5, AddressLine1 = "26910 Indela Road", City = "Montreal", StateProvince = "Quebec", CountryRegion = "Canada", PostalCode = "H1Y 2H5" },
        new Address { AddressId = 6, AddressLine1 = "2681 Eagle Peak", City = "Bellevue", StateProvince = "Washington", CountryRegion = "United States", PostalCode = "98004" },
        new Address { AddressId = 7, AddressLine1 = "7943 Walnut Ave", City = "Renton", StateProvince = "Washington", CountryRegion = "United States", PostalCode = "98055" },
        new Address { AddressId = 8, AddressLine1 = "6388 Lake City Way", City = "Burnaby", StateProvince = "British Columbia", CountryRegion = "Canada", PostalCode = "V5A 3A6" },
        new Address { AddressId = 9, AddressLine1 = "52560 Free Street", City = "Toronto", StateProvince = "Ontario", CountryRegion = "Canada", PostalCode = "M4B 1V7" },
        new Address { AddressId = 10, AddressLine1 = "22580 Free Street", City = "Toronto", StateProvince = "Ontario", CountryRegion = "Canada", PostalCode = "M4B 1V7" },
        new Address { AddressId = 11, AddressLine1 = "2575 Bloor Street East", City = "Toronto", StateProvince = "Ontario", CountryRegion = "Canada", PostalCode = "M4B 1V6" },
        new Address { AddressId = 12, AddressLine1 = "Station E", City = "Chalk Riber", StateProvince = "Ontario", CountryRegion = "Canada", PostalCode = "K0J 1J0" },
        new Address { AddressId = 13, AddressLine1 = "575 Rue St Amable", City = "Quebec", StateProvince = "Quebec", CountryRegion = "Canada", PostalCode = "G1R" },
        new Address { AddressId = 14, AddressLine1 = "2512-4th Ave Sw", City = "Calgary", StateProvince = "Alberta", CountryRegion = "Canada", PostalCode = "T2P 2G8" },
        new Address { AddressId = 15, AddressLine1 = "55 Lakeshore Blvd East", City = "Toronto", StateProvince = "Ontario", CountryRegion = "Canada", PostalCode = "M4B 1V6" },
        new Address { AddressId = 16, AddressLine1 = "6333 Cote Vertu", City = "Montreal", StateProvince = "Quebec", CountryRegion = "Canada", PostalCode = "H1Y 2H5" },
        new Address { AddressId = 17, AddressLine1 = "3255 Front Street West", City = "Toronto", StateProvince = "Ontario", CountryRegion = "Canada", PostalCode = "H1Y 2H5" },
        new Address { AddressId = 18, AddressLine1 = "2550 Signet Drive", City = "Weston", StateProvince = "Ontario", CountryRegion = "Canada", PostalCode = "H1Y 2H7" },
        new Address { AddressId = 19, AddressLine1 = "6777 Kingsway", City = "Burnaby", StateProvince = "British Columbia", CountryRegion = "Canada", PostalCode = "H1Y 2H8" },
        new Address { AddressId = 20, AddressLine1 = "5250-505 Burning St", City = "Vancouver", StateProvince = "British Columbia", CountryRegion = "Canada", PostalCode = "H1Y 2H9" },
        new Address { AddressId = 21, AddressLine1 = "600 Slater Street", City = "Ottawa", StateProvince = "Ontario", CountryRegion = "Canada", PostalCode = "M9V 4W3" }
    );

    Guid customerId1 = Guid.NewGuid();
    Guid customerId2 = Guid.NewGuid();
    Guid customerId3 = Guid.NewGuid();
    Guid customerId4 = Guid.NewGuid();

    modelBuilder.Entity<Customer>().HasData(
        new Customer { CustomerId = customerId1, NameStyle = false, Title = "Mr.", FirstName = "Orlando", MiddleName = "N.", LastName = "Gee", CompanyName = "A Bike Store", SalesPerson = @"adventure-works\pamela0", EmailAddress = "orlando0@adventure-works.com", Phone = "245-555-0173", PasswordHash = "L/Rlwxzp4w7RWmEgXX+/A7cXaePEPcp+KwQhl2fJL7w=", PasswordSalt = "1KjXYs4=" },
        new Customer { CustomerId = customerId2, NameStyle = false, Title = "Mr.", FirstName = "Keith", MiddleName = "N.", LastName = "Harris", CompanyName = "Progressive Sports", SalesPerson = @"adventure-works\david8", EmailAddress = "keith0@adventure-works.com", Phone = "170-555-0127", PasswordHash = "YPdtRdvqeAhj6wyxEsFdshBDNXxkCXn+CRgbvJItknw=", PasswordSalt = "fs1ZGhY=" },
        new Customer { CustomerId = customerId3, NameStyle = false, Title = "Ms.", FirstName = "Donna", MiddleName = "F.", LastName = "Carreras", CompanyName = "Advanced Bike Components", SalesPerson = @"adventure-works\jillian0", EmailAddress = "donna0@adventure-works.com", Phone = "279-555-0130", PasswordHash = "LNoK27abGQo48gGue3EBV/UrlYSToV0/s87dCRV7uJk=", PasswordSalt = "YTNH5Rw=" },
        new Customer { CustomerId = customerId4, NameStyle = false, Title = "Ms.", FirstName = "Janet", MiddleName = "M.", LastName = "Gates", CompanyName = "Modular Cycle Systems", SalesPerson = @"adventure-works\jillian0", EmailAddress = "janet1@adventure-works.com", Phone = "710-555-0173", PasswordHash = "ElzTpSNbUW1Ut+L5cWlfR7MF6nBZia8WpmGaQPjLOJA=", PasswordSalt = "nm7D5e4=" }
    );

    modelBuilder.Entity<CustomerAddress>().HasData(
        new CustomerAddress { CustomerId = customerId1, AddressId = 4, AddressType = "Main Office" },
        new CustomerAddress { CustomerId = customerId1, AddressId = 5, AddressType = "Office Depot" },
        new CustomerAddress { CustomerId = customerId2, AddressId = 3, AddressType = "Main Office" },
        new CustomerAddress { CustomerId = customerId3, AddressId = 2, AddressType = "Main Office" },
        new CustomerAddress { CustomerId = customerId4, AddressId = 1, AddressType = "Main Office" }
    );

    modelBuilder.Entity<ProductCategory>().HasData(
        new ProductCategory { ProductCategoryId = "BIKES", Name = "Bikes" },
        new ProductCategory { ProductCategoryId = "COMPT", Name = "Components" },
        new ProductCategory { ProductCategoryId = "CLOTHE", Name = "Clothing" },
        new ProductCategory { ProductCategoryId = "ACCESS", Name = "Accessories" },
        new ProductCategory { ProductCategoryId = "MOUNTB", Name = "Mountain Bikes", ParentProductCategoryId = "BIKES" },
        new ProductCategory { ProductCategoryId = "ROADB", Name = "Road Bikes", ParentProductCategoryId = "BIKES" },
        new ProductCategory { ProductCategoryId = "ROADFR", Name = "Road Frames", ParentProductCategoryId = "COMPT" },
        new ProductCategory { ProductCategoryId = "TOURB", Name = "Touring Bikes", ParentProductCategoryId = "BIKES" },
        new ProductCategory { ProductCategoryId = "HANDLB", Name = "Handlebars", ParentProductCategoryId = "COMPT" },
        new ProductCategory { ProductCategoryId = "BRACK", Name = "Bottom Brackets", ParentProductCategoryId = "COMPT" },
        new ProductCategory { ProductCategoryId = "BRAKES", Name = "Brakes", ParentProductCategoryId = "COMPT" }

    );

            modelBuilder.Entity<ProductModel>().HasData(
                new ProductModel { ProductModelId = 6, Name = "HL Road Frame" },
                new ProductModel { ProductModelId = 19, Name = "Mountain-100", CatalogDescription = @"
                        <?xml-stylesheet href=""ProductDescription.xsl"" type=""text/xsl""?><p1:ProductDescription xmlns:p1=""http://schemas.microsoft.com/sqlserver/2004/07/adventure-works/ProductModelDescription"" xmlns:wm=""http://schemas.microsoft.com/sqlserver/2004/07/adventure-works/ProductModelWarrAndMain"" xmlns:wf=""http://www.adventure-works.com/schemas/OtherFeatures"" xmlns:html=""http://www.w3.org/1999/xhtml"" ProductModelID=""19"" ProductModelName=""Mountain 100""><p1:Summary><html:p>Our top-of-the-line competition mountain bike. 
                        Performance-enhancing options include the innovative HL Frame,
                        super-smooth front suspension, and traction for all terrain.
                        </html:p></p1:Summary><p1:Manufacturer><p1:Name>AdventureWorks</p1:Name><p1:Copyright>2002</p1:Copyright><p1:ProductURL>HTTP://www.Adventure-works.com</p1:ProductURL></p1:Manufacturer><p1:Features>These are the product highlights. 
                        <wm:Warranty><wm:WarrantyPeriod>3 years</wm:WarrantyPeriod><wm:Description>parts and labor</wm:Description></wm:Warranty><wm:Maintenance><wm:NoOfYears>10 years</wm:NoOfYears><wm:Description>maintenance contract available through your dealer or any AdventureWorks retail store.</wm:Description></wm:Maintenance><wf:wheel>High performance wheels.</wf:wheel><wf:saddle><html:i>Anatomic design</html:i> and made from durable leather for a full-day of riding in comfort.</wf:saddle><wf:pedal><html:b>Top-of-the-line</html:b> clipless pedals with adjustable tension.</wf:pedal><wf:BikeFrame>Each frame is hand-crafted in our Bothell facility to the optimum diameter
                        and wall-thickness required of a premium mountain frame.
                        The heat-treated welded aluminum frame has a larger diameter tube that absorbs the bumps.</wf:BikeFrame><wf:crankset> Triple crankset; alumunim crank arm; flawless shifting. </wf:crankset></p1:Features><!-- add one or more of these elements...one for each specific product in this product model --><p1:Picture><p1:Angle>front</p1:Angle><p1:Size>small</p1:Size><p1:ProductPhotoID>118</p1:ProductPhotoID></p1:Picture><!-- add any tags in <specifications> --><p1:Specifications> These are the product specifications.
                        <Material>Almuminum Alloy</Material><Color>Available in most colors</Color><ProductLine>Mountain bike</ProductLine><Style>Unisex</Style><RiderExperience>Advanced to Professional riders</RiderExperience></p1:Specifications></p1:ProductDescription>
                " },
                new ProductModel { ProductModelId = 20, Name = "Mountain-200" },
                new ProductModel { ProductModelId = 21, Name = "Mountain-300" },
                new ProductModel { ProductModelId = 25, Name = "Road-150", CatalogDescription = @"
                        <?xml-stylesheet href=""ProductDescription.xsl"" type=""text/xsl""?><p1:ProductDescription xmlns:p1=""http://schemas.microsoft.com/sqlserver/2004/07/adventure-works/ProductModelDescription"" xmlns:wm=""http://schemas.microsoft.com/sqlserver/2004/07/adventure-works/ProductModelWarrAndMain"" xmlns:wf=""http://www.adventure-works.com/schemas/OtherFeatures"" xmlns:html=""http://www.w3.org/1999/xhtml"" ProductModelID=""25"" ProductModelName=""Road-150""><p1:Summary><html:p>This bike is ridden by race winners. Developed with the 
                        Adventure Works Cycles professional race team, it has a extremely light
                        heat-treated aluminum frame, and steering that allows precision control.
                        </html:p></p1:Summary><p1:Manufacturer><p1:Name>AdventureWorks</p1:Name><p1:Copyright>2002</p1:Copyright><p1:ProductURL>HTTP://www.Adventure-works.com</p1:ProductURL></p1:Manufacturer><p1:Features>These are the product highlights. 
                        <wm:Warranty><wm:WarrantyPeriod>4 years</wm:WarrantyPeriod><wm:Description>parts and labor</wm:Description></wm:Warranty><wm:Maintenance><wm:NoOfYears>7 years</wm:NoOfYears><wm:Description>maintenance contact available through dealer or any Adventure Works Cycles retail store.</wm:Description></wm:Maintenance><wf:handlebar>Designed for racers; high-end anatomically shaped bar from aluminum alloy.</wf:handlebar><wf:wheel>Strong wheels with double-walled rims.</wf:wheel><wf:saddle><html:i>Lightweight</html:i> kevlar racing saddle.</wf:saddle><wf:pedal><html:b>Top-of-the-line</html:b> clipless pedals with adjustable tension.</wf:pedal><wf:BikeFrame><html:i>Our lightest and best quality</html:i> aluminum frame made from the newest alloy;
                        it is welded and heat-treated for strength.
                        Our innovative design results in maximum comfort and performance.</wf:BikeFrame></p1:Features><!-- add one or more of these elements...one for each specific product in this product model --><p1:Picture><p1:Angle>front</p1:Angle><p1:Size>small</p1:Size><p1:ProductPhotoID>126</p1:ProductPhotoID></p1:Picture><!-- add any tags in <specifications> --><p1:Specifications> These are the product specifications.
                        <Material>Aluminum</Material><Color>Available in all colors.</Color><ProductLine>Road bike</ProductLine><Style>Unisex</Style><RiderExperience>Intermediate to Professional riders</RiderExperience></p1:Specifications></p1:ProductDescription>
                " },
                new ProductModel { ProductModelId = 30, Name = "Road-650" },
                new ProductModel { ProductModelId = 52, Name = "LL Mountain Handlebars" },
                new ProductModel { ProductModelId = 54, Name = "ML Mountain Handlebars" },
                new ProductModel { ProductModelId = 55, Name = "HL Mountain Handlebars" }
            );


            var p1 = Guid.NewGuid();
            var p2 = Guid.NewGuid();
            var p3 = Guid.NewGuid();


            modelBuilder.Entity<Product>().HasData(
                new Product { ProductId = Guid.NewGuid(), Name = "HL Road Frame - Black, 58", ProductNumber = "FR-R92B-58", Color = "Black", StandardCost = 1059.3100M, ListPrice = 1431.5000M, Size = "58", Weight = 1016.04M, ProductCategoryId = "ROADFR", ProductModelId = 6 },
                new Product { ProductId = p1, Name = "HL Road Frame - Red, 58", ProductNumber = "FR-R92R-58", Color = "Red", StandardCost = 1059.3100M, ListPrice = 1431.5000M, Size = "58", Weight = 1016.04M, ProductCategoryId = "ROADFR", ProductModelId = 6 },
                new Product { ProductId = p2, Name = "Road-150 Red, 62", ProductNumber = "BK-R93R-62", Color = "Red", StandardCost = 2171.2942M, ListPrice = 3578.2700M, Size = "62", Weight = 6803.85M, ProductCategoryId = "ROADB", ProductModelId = 25 },
                new Product { ProductId = Guid.NewGuid(), Name = "Road-650 Black, 58", ProductNumber = "BK-R50B-58", Color = "Black", StandardCost = 486.7066M, ListPrice = 782.9900M, Size = "58", Weight = 8976.55M, ProductCategoryId = "ROADB", ProductModelId = 30 },
                new Product { ProductId = Guid.NewGuid(), Name = "Mountain-100 Silver, 38", ProductNumber = "BK-M82S-38", Color = "Silver", StandardCost = 1912.1544M, ListPrice = 3399.9900M, Size = "38", Weight = 9230.56M, ProductCategoryId = "MOUNTB", ProductModelId = 19 },
                new Product { ProductId = Guid.NewGuid(), Name = "Mountain-100 Black, 38", ProductNumber = "BK-M82B-38", Color = "Black", StandardCost = 1898.0944M, ListPrice = 3374.9900M, Size = "38", Weight = 9230.56M, ProductCategoryId = "MOUNTB", ProductModelId = 19 },
                new Product { ProductId = Guid.NewGuid(), Name = "Mountain-200 Silver, 38", ProductNumber = "BK-M68S-38", Color = "Silver", StandardCost = 1265.6195M, ListPrice = 2319.9900M, Size = "38", Weight = 10591.33M, ProductCategoryId = "MOUNTB", ProductModelId = 20 },
                new Product { ProductId = Guid.NewGuid(), Name = "Mountain-200 Black, 38", ProductNumber = "BK-M68B-38", Color = "Black", StandardCost = 1251.9813M, ListPrice = 2294.9900M, Size = "38", Weight = 10591.33M, ProductCategoryId = "MOUNTB", ProductModelId = 20 },
                new Product { ProductId = Guid.NewGuid(), Name = "Mountain-200 Black, 42", ProductNumber = "BK-M68B-42", Color = "Black", StandardCost = 1251.9813M, ListPrice = 2294.9900M, Size = "42", Weight = 10781.83M, ProductCategoryId = "MOUNTB", ProductModelId = 20 },
                new Product { ProductId = Guid.NewGuid(), Name = "Mountain-200 Black, 46", ProductNumber = "BK-M68B-46", Color = "Black", StandardCost = 1251.9813M, ListPrice = 2294.9900M, Size = "46", Weight = 10945.13M, ProductCategoryId = "MOUNTB", ProductModelId = 20 },
                new Product { ProductId = Guid.NewGuid(), Name = "Mountain-300 Black, 38", ProductNumber = "BK-M47B-38", Color = "Black", StandardCost = 598.4354M, ListPrice = 1079.9900M, Size = "38", Weight = 11498.51M, ProductCategoryId = "MOUNTB", ProductModelId = 21 },
                new Product { ProductId = p3, Name = "LL Mountain Handlebars", ProductNumber = "HB-M243", StandardCost = 19.7758M, ListPrice = 44.5400M, ProductCategoryId = "HANDLB", ProductModelId = 52 },
                new Product { ProductId = Guid.NewGuid(), Name = "ML Mountain Handlebars", ProductNumber = "HB-M763", StandardCost = 27.4925M, ListPrice = 61.9200M, ProductCategoryId = "HANDLB", ProductModelId = 54 },
                new Product { ProductId = Guid.NewGuid(), Name = "HL Mountain Handlebars", ProductNumber = "HB-M918", StandardCost = 53.3999M, ListPrice = 120.2700M, ProductCategoryId = "HANDLB", ProductModelId = 55 }
              );

            modelBuilder.Entity<SalesOrderHeader>().HasData(
                new SalesOrderHeader
                {
                    SalesOrderId = 1000,
                    SalesOrderNumber = "SO-1000",
                    RevisionNumber = 1,
                    Status = 5,
                    OnlineOrderFlag = true,
                    PurchaseOrderNumber = "PO348186287",
                    AccountNumber = "10-4020-000609",
                    CustomerId = customerId1,
                    ShipToAddressId = 4,
                    BillToAddressId = 5,
                    ShipMethod = "CAR TRANSPORTATION",
                    SubTotal = 6530.35M,
                    TaxAmt = 70.4279M,
                    Freight = 22.0087M,
                    TotalDue = (6530.35M + 70.4279M + 22.0087M)
                }
            );

            modelBuilder.Entity<SalesOrderDetail>().HasData(
                new SalesOrderDetail { SalesOrderId = 1000, SalesOrderDetailId = 110562, OrderQty = 1, ProductId = p2, UnitPrice = 3578.2700M },
                new SalesOrderDetail { SalesOrderId = 1000, SalesOrderDetailId = 110563, OrderQty = 2, ProductId = p3, UnitPrice = 44.5400M },
                new SalesOrderDetail { SalesOrderId = 1000, SalesOrderDetailId = 110564, OrderQty = 2, ProductId = p1, UnitPrice = 1431.5000M }
            );

            modelBuilder.Entity<Posts>().HasData(
                new Posts { PostId = 1, Title = "Best Boutiques on the Eastside" },
                new Posts { PostId = 2, Title = "Avoiding over-priced helmets" },
                new Posts { PostId = 3, Title = "Where to buy Mars Bars" }
            );

            modelBuilder.Entity<Tags>().HasData(
                new Tags { TagId = 1, Text = "Golden" },
                new Tags { TagId = 2, Text = "Pineapple" },
                new Tags { TagId = 3, Text = "Girlscout" },
                new Tags { TagId = 4, Text = "Cookies" }
            );

            modelBuilder.Entity<PostTag>().HasData(
                new PostTag { PostId = 1, TagId = 1 },
                new PostTag { PostId = 1, TagId = 2 },
                new PostTag { PostId = 1, TagId = 3 },
                new PostTag { PostId = 2, TagId = 1 },
                new PostTag { PostId = 2, TagId = 4 },
                new PostTag { PostId = 3, TagId = 3 },
                new PostTag { PostId = 3, TagId = 4 }
            );
        }

    }

    class MyModelCacheKeyFactory : IModelCacheKeyFactory
    {
        public object Create(DbContext context)
            => new MyModelCacheKey(context);
    }

    class MyModelCacheKey : ModelCacheKey
    {
        ProviderType providerType;
        bool useSchema;

        public MyModelCacheKey(DbContext context)
            : base(context)
        {

            AdventureWorksContext adventureWorksContext = (AdventureWorksContext)context;

            providerType = adventureWorksContext.ProviderType;
            useSchema = adventureWorksContext.useSchema;
        }

        protected override bool Equals(ModelCacheKey other)
            => base.Equals(other)
                && (other as MyModelCacheKey)?.providerType == providerType
                && (other as MyModelCacheKey)?.useSchema == useSchema;

        public override int GetHashCode()
        {
            var hashCode = base.GetHashCode() * 397;
            hashCode ^= useSchema.GetHashCode();
            hashCode ^= providerType.GetHashCode();

            return hashCode;
        }
    }
}
