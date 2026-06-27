namespace ECIP.Infrastructure.Persistence;

using ECIP.Core.Entities;
using Microsoft.EntityFrameworkCore;

/// <summary>
/// Entity Framework Core database context for ECIP.
/// </summary>
public class EcipDbContext : DbContext
{
    public EcipDbContext(DbContextOptions<EcipDbContext> options) : base(options)
    {
    }

    public DbSet<RepositoryEntity> Repositories => Set<RepositoryEntity>();
    public DbSet<RepositoryFile> RepositoryFiles => Set<RepositoryFile>();
    public DbSet<RepositoryFolder> RepositoryFolders => Set<RepositoryFolder>();
    public DbSet<RepositoryLanguage> RepositoryLanguages => Set<RepositoryLanguage>();
    public DbSet<RepositoryProject> RepositoryProjects => Set<RepositoryProject>();
    public DbSet<RepositoryStatistics> RepositoryStatistics => Set<RepositoryStatistics>();
    public DbSet<RepositoryFolderStatistics> RepositoryFolderStatistics => Set<RepositoryFolderStatistics>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<RepositoryEntity>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
            entity.Property(e => e.RepositoryUrl).HasMaxLength(500);
            entity.Property(e => e.Branch).HasMaxLength(100);
            entity.Property(e => e.LocalPath).HasMaxLength(500);
            entity.Property(e => e.DefaultBranch).HasMaxLength(100);
            entity.HasIndex(e => e.Name).IsUnique();
        });

        modelBuilder.Entity<RepositoryFile>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.FileName).IsRequired().HasMaxLength(250);
            entity.Property(e => e.RelativePath).IsRequired().HasMaxLength(1000);
            entity.Property(e => e.FullPath).IsRequired().HasMaxLength(2000);
            entity.Property(e => e.Directory).HasMaxLength(2000);
            entity.Property(e => e.Language).HasMaxLength(100);
        });

        modelBuilder.Entity<RepositoryFolder>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(250);
            entity.Property(e => e.RelativePath).HasMaxLength(1000);
            entity.Property(e => e.FullPath).HasMaxLength(2000);
            entity.Property(e => e.ParentPath).HasMaxLength(2000);
        });

        modelBuilder.Entity<RepositoryLanguage>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Extension).HasMaxLength(50);
        });

        modelBuilder.Entity<RepositoryProject>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(250);
            entity.Property(e => e.ProjectType).IsRequired().HasMaxLength(100);
            entity.Property(e => e.FileName).IsRequired().HasMaxLength(250);
            entity.Property(e => e.RelativePath).HasMaxLength(1000);
            entity.Property(e => e.FullPath).HasMaxLength(2000);
            entity.Property(e => e.Directory).HasMaxLength(2000);
        });

        modelBuilder.Entity<RepositoryStatistics>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.RepositoryName).IsRequired().HasMaxLength(200);
            entity.Property(e => e.RepositoryType).HasMaxLength(100);
            entity.Property(e => e.RepositoryRoot).HasMaxLength(2000);
            entity.Property(e => e.LargestFileName).HasMaxLength(500);
            entity.Property(e => e.ScanDuration).HasMaxLength(50);
        });

        modelBuilder.Entity<RepositoryFolderStatistics>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.FolderName).IsRequired().HasMaxLength(250);
            entity.Property(e => e.ParentFolder).HasMaxLength(1000);
            entity.Property(e => e.RelativePath).HasMaxLength(1000);
        });
    }
}
