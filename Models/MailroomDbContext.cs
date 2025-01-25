using Mailroom.Models;
using Microsoft.EntityFrameworkCore;

public class MailroomDbContext : DbContext
{
    public DbSet<User> Users { get; set; }
    public DbSet<Packages> Packages { get; set; }
    public DbSet<UnknownPackage> UnknownPackages { get; set; }

    public MailroomDbContext(DbContextOptions<MailroomDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>(entity => { entity.HasKey(e => e.UserId); });

        modelBuilder.Entity<Packages>(entity => { entity.HasKey(e => e.PackageId); });

        modelBuilder.Entity<UnknownPackage>(entity => { entity.HasKey(e => e.UnknownPackageId); });
    }
}