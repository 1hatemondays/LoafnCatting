using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;

namespace DataAccessLayer.Models;

public partial class LoafNcattingDbContext : DbContext
{
    public LoafNcattingDbContext()
    {
    }

    public LoafNcattingDbContext(DbContextOptions<LoafNcattingDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Cat> Cats { get; set; }

    public virtual DbSet<CatStatus> CatStatuses { get; set; }

    public virtual DbSet<Category> Categories { get; set; }

    public virtual DbSet<Gender> Genders { get; set; }

    public virtual DbSet<Order> Orders { get; set; }

    public virtual DbSet<OrderDetail> OrderDetails { get; set; }

    public virtual DbSet<Payment> Payments { get; set; }

    public virtual DbSet<PaymentMethod> PaymentMethods { get; set; }

    public virtual DbSet<Product> Products { get; set; }

    public virtual DbSet<Reservation> Reservations { get; set; }

    public virtual DbSet<ReservationStatus> ReservationStatuses { get; set; }

    public virtual DbSet<RestaurantTable> RestaurantTables { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json")
            .Build();

        var connectionString = configuration.GetConnectionString("DBDefault");
        optionsBuilder.UseSqlServer(connectionString);
    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Cat>(entity =>
        {
            entity.HasKey(e => e.CatId).HasName("PK__Cat__6A1C8AFA0AB9A1BD");

            entity.ToTable("Cat");

            entity.Property(e => e.Name).HasMaxLength(255);

            entity.HasOne(d => d.Gender).WithMany(p => p.Cats)
                .HasForeignKey(d => d.GenderId)
                .HasConstraintName("FK__Cat__GenderId__46E78A0C");

            entity.HasOne(d => d.Status).WithMany(p => p.Cats)
                .HasForeignKey(d => d.StatusId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Cat__StatusId__47DBAE45");
        });

        modelBuilder.Entity<CatStatus>(entity =>
        {
            entity.HasKey(e => e.StatusId).HasName("PK__CatStatu__C8EE20631FBFBED4");

            entity.ToTable("CatStatus");

            entity.HasIndex(e => e.StatusName, "UQ__CatStatu__05E7698A4480EBA8").IsUnique();

            entity.Property(e => e.StatusName).HasMaxLength(50);
        });

        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.CategoryId).HasName("PK__Category__19093A0B3E639CB3");

            entity.ToTable("Category");

            entity.Property(e => e.Name).HasMaxLength(255);
        });

        modelBuilder.Entity<Gender>(entity =>
        {
            entity.HasKey(e => e.GenderId).HasName("PK__Gender__4E24E9F7B32A9871");

            entity.ToTable("Gender");

            entity.HasIndex(e => e.GenderName, "UQ__Gender__F7C17715CA6066B9").IsUnique();

            entity.Property(e => e.GenderName).HasMaxLength(50);
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(e => e.OrderId).HasName("PK__Order__C3905BCF6CB003BD");

            entity.ToTable("Order");

            entity.Property(e => e.OrderDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.TotalPrice).HasColumnType("decimal(10, 2)");

            entity.HasOne(d => d.CustomerUser).WithMany(p => p.OrderCustomerUsers)
                .HasForeignKey(d => d.CustomerUserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Order__CustomerU__5165187F");

            entity.HasOne(d => d.StaffUser).WithMany(p => p.OrderStaffUsers)
                .HasForeignKey(d => d.StaffUserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Order__StaffUser__52593CB8");

            entity.HasOne(d => d.Table).WithMany(p => p.Orders)
                .HasForeignKey(d => d.TableId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Order__TableId__534D60F1");
        });

        modelBuilder.Entity<OrderDetail>(entity =>
        {
            entity.HasKey(e => new { e.OrderId, e.ProductId }).HasName("PK__OrderDet__08D097A30329BD33");

            entity.ToTable("OrderDetail");

            entity.HasOne(d => d.Order).WithMany(p => p.OrderDetails)
                .HasForeignKey(d => d.OrderId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__OrderDeta__Order__5629CD9C");

            entity.HasOne(d => d.Product).WithMany(p => p.OrderDetails)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__OrderDeta__Produ__571DF1D5");
        });

        modelBuilder.Entity<Payment>(entity =>
        {
            entity.HasKey(e => e.PaymentId).HasName("PK__Payment__9B556A38E4A4B16A");

            entity.ToTable("Payment");

            entity.Property(e => e.PaymentAmount).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.PaymentDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.Method).WithMany(p => p.Payments)
                .HasForeignKey(d => d.MethodId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Payment__MethodI__5DCAEF64");

            entity.HasOne(d => d.Order).WithMany(p => p.Payments)
                .HasForeignKey(d => d.OrderId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Payment__OrderId__5EBF139D");
        });

        modelBuilder.Entity<PaymentMethod>(entity =>
        {
            entity.HasKey(e => e.MethodId).HasName("PK__PaymentM__FC68185174CB10D6");

            entity.ToTable("PaymentMethod");

            entity.HasIndex(e => e.MethodName, "UQ__PaymentM__218CFB17CEA76717").IsUnique();

            entity.Property(e => e.MethodName).HasMaxLength(50);
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.ProductId).HasName("PK__Product__B40CC6CDBA8693F2");

            entity.ToTable("Product");

            entity.Property(e => e.Name).HasMaxLength(255);
            entity.Property(e => e.Price).HasColumnType("decimal(10, 2)");

            entity.HasOne(d => d.Category).WithMany(p => p.Products)
                .HasForeignKey(d => d.CategoryId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Product__Categor__4D94879B");
        });

        modelBuilder.Entity<Reservation>(entity =>
        {
            entity.HasKey(e => e.ReservationId).HasName("PK__Reservat__B7EE5F24D52919B7");

            entity.ToTable("Reservation");

            entity.HasOne(d => d.Status).WithMany(p => p.Reservations)
                .HasForeignKey(d => d.StatusId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Reservati__Statu__6477ECF3");

            entity.HasOne(d => d.Table).WithMany(p => p.Reservations)
                .HasForeignKey(d => d.TableId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Reservati__Table__66603565");

            entity.HasOne(d => d.User).WithMany(p => p.Reservations)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Reservati__UserI__656C112C");
        });

        modelBuilder.Entity<ReservationStatus>(entity =>
        {
            entity.HasKey(e => e.StatusId).HasName("PK__Reservat__C8EE2063E8286576");

            entity.ToTable("ReservationStatus");

            entity.HasIndex(e => e.StatusName, "UQ__Reservat__05E7698A4BB98253").IsUnique();

            entity.Property(e => e.StatusName).HasMaxLength(50);
        });

        modelBuilder.Entity<RestaurantTable>(entity =>
        {
            entity.HasKey(e => e.TableId).HasName("PK__Restaura__7D5F01EEDB0DAD67");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.RoleId).HasName("PK__Role__8AFACE1A0CE4EFB6");

            entity.ToTable("Role");

            entity.HasIndex(e => e.RoleName, "UQ__Role__8A2B6160B863356E").IsUnique();

            entity.Property(e => e.RoleName).HasMaxLength(100);
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__User__1788CC4C52793DDE");

            entity.ToTable("User");

            entity.HasIndex(e => e.PhoneNumber, "UQ__User__85FB4E38E42707BC").IsUnique();

            entity.HasIndex(e => e.Email, "UQ__User__A9D10534D00C5785").IsUnique();

            entity.Property(e => e.Email).HasMaxLength(255);
            entity.Property(e => e.Name).HasMaxLength(255);
            entity.Property(e => e.Password).HasMaxLength(255);
            entity.Property(e => e.PhoneNumber).HasMaxLength(20);

            entity.HasOne(d => d.Role).WithMany(p => p.Users)
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__User__RoleId__3C69FB99");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
