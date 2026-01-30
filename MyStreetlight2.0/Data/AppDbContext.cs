using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Streetlight2._0.Models.GatewayModels;
using Streetlight2._0.Models.LightModels;
using Streetlight2._0.Models.MaintenanceModels;
using Streetlight2._0.Models.Misc;
using Streetlight2._0.Models.UserModels;

namespace Streetlight2._0.Data;

public partial class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public virtual DbSet<AreaHierarchyMaster> AreaHierarchyMasters { get; set; }

    public virtual DbSet<LightActionLog> LightActionLogs { get; set; }

    public virtual DbSet<LightLiveDataLog> LightLiveDataLogs { get; set; }

    public virtual DbSet<LightLiveData> LightLiveData { get; set; }

    public virtual DbSet<LightStsMaster> LightStsMasters { get; set; }

    public virtual DbSet<LightsMaster> LightsMasters { get; set; }

    public virtual DbSet<Permission> Permissions { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<UserPermission> UserPermissions { get; set; }

    public virtual DbSet<Gateway> Gateways { get; set; }

    public virtual DbSet<FaultyLightLog> FaultyLightLogs { get; set; }

    public virtual DbSet<LightWithLiveData> LightWithLiveData { get; set; }

    public virtual DbSet<MiscValue> MiscValues { get; set; }

    public virtual DbSet<FaultyLightMaintenanceLog> FaultyLightMaintenanceLogs { get; set; }

    public virtual DbSet<FaultyLightWithMaintenanceLog> FaultyLightWithMaintenanceLogs { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AreaHierarchyMaster>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("AreaHierarchyMaster");

            entity.Property(e => e.Ward).HasMaxLength(50);
            entity.Property(e => e.Zone).HasMaxLength(50);
        });

        modelBuilder.Entity<LightActionLog>(entity =>
        {
            entity.HasKey(e => e.RecordId);

            entity.Property(e => e.Action).HasMaxLength(50);
            entity.Property(e => e.ActionRemark).HasMaxLength(500);
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.LightId).HasMaxLength(50);
            entity.Property(e => e.CommandFor).HasMaxLength(100);
        });

        modelBuilder.Entity<LightLiveDataLog>(entity =>
        {
            entity.HasKey(e => e.RecordId).HasName("PK_LightLiveDataLog");

            entity.Property(e => e.Comin)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("COMin");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.LightId).HasMaxLength(50);
            entity.Property(e => e.MacId).HasMaxLength(50);
        });

        modelBuilder.Entity<LightLiveData>(entity =>
        {
            entity.HasKey(e => e.RecordId);

            entity.Property(e => e.Ampere).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.LightId).HasMaxLength(50);
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
        });

        modelBuilder.Entity<LightStsMaster>(entity =>
        {
            entity.HasKey(e => e.StatusId);

            entity.ToTable("LightStsMaster");

            entity.Property(e => e.StatusId).ValueGeneratedNever();
            entity.Property(e => e.StatusDescription).HasMaxLength(500);
            entity.Property(e => e.StatusName).HasMaxLength(50);
        });

        modelBuilder.Entity<LightsMaster>(entity =>
        {
            entity.HasKey(e => e.RecordId);

            entity.ToTable("LightsMaster");

            entity.Property(e => e.Address).HasMaxLength(100);
            entity.Property(e => e.GatewayId).HasMaxLength(50);
            entity.Property(e => e.Latitude).HasMaxLength(50);
            entity.Property(e => e.LightId).HasMaxLength(50);
            entity.Property(e => e.Longitude).HasMaxLength(50);
            entity.Property(e => e.MacId).HasMaxLength(50);
            entity.Property(e => e.PollId).HasMaxLength(50);
            entity.Property(e => e.Ward).HasMaxLength(50);
            entity.Property(e => e.Zone).HasMaxLength(50);
        });

        modelBuilder.Entity<Permission>(entity =>
        {
            entity.Property(e => e.Description).HasMaxLength(100);
            entity.Property(e => e.Name).HasMaxLength(50);
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasIndex(e => e.UserName, "UQ_Users_UserName").IsUnique();

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Password).HasMaxLength(50);
            entity.Property(e => e.UserName).HasMaxLength(50);
        });

        modelBuilder.Entity<UserPermission>(entity =>
        {
            entity.HasKey(e => e.RecordId);

            entity.HasOne(d => d.Permission).WithMany(p => p.UserPermissions)
                .HasForeignKey(d => d.PermissionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_UserPermissions_Permissions");

            entity.HasOne(d => d.User).WithMany(p => p.UserPermissions)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_UserPermissions_User");
        });

        modelBuilder.Entity<FaultyLightLog>(entity =>
        {
            entity.HasKey(e => e.RecordId);
            entity.Property(e => e.LightId).HasMaxLength(50);
            entity.Property(e => e.GatewayId).HasMaxLength(50);
            entity.Property(e => e.MacId).HasMaxLength(50);
            entity.Property(e => e.LightStatus).HasMaxLength(50);
            entity.Property(e => e.CreatedAt).IsRequired();
        });

        modelBuilder.Entity<FaultyLightMaintenanceLog>(entity =>
        {
            entity.ToTable("FaultyLightMaintenanceLogs");
            entity.HasKey(e => e.LogId);
            entity.Property(e => e.Action).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Remark).HasMaxLength(500);
            entity.Property(e => e.LoggedAt).HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
