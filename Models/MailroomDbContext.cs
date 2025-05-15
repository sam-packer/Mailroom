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

        // User
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId);

            // Set the password to VARCHAR(100). It is 32 in the model for input validation
            entity.Property(e => e.Password)
                .HasMaxLength(100);
            
            entity.HasIndex(e => e.Email)
                .IsUnique()
                .HasDatabaseName("Users_email_uindex")
                .HasAnnotation("Relational:Comment", "Requires emails to be unique");

            entity.HasIndex(e => e.Role)
                .HasDatabaseName("Users_Role_index")
                .HasAnnotation("Relational:Comment", "Keep track of roles...");
        });

        // Packages
        modelBuilder.Entity<Packages>(entity =>
        {
            entity.HasKey(e => e.PackageId);

            entity.HasOne(p => p.User)
                .WithMany(u => u.Packages)
                .HasForeignKey(p => p.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(p => new {p.Delivered, p.DeliveredDate})
                .HasDatabaseName("Packages_Delivered_DeliveredDate_index")
                .HasAnnotation("Relational:Comment", "Used on the admin side...");

            entity.HasIndex(p => new {p.Delivered, p.UserId})
                .HasDatabaseName("Packages_Delivered_UserId_index")
                .HasAnnotation("Relational:Comment", "Used for the resident side");

            entity.HasIndex(p => new {p.UserId, p.DeliveredDate})
                .HasDatabaseName("Packages_UserId_DeliveredDate_index")
                .HasAnnotation("Relational:Comment", "Used for the user detail page");

            entity.HasIndex(p => p.UserId)
                .HasDatabaseName("Packages_UserId_index")
                .HasAnnotation("Relational:Comment", "Foreign key index for UserId");
        });

        // UnknownPackage
        modelBuilder.Entity<UnknownPackage>(entity => { entity.HasKey(e => e.UnknownPackageId); });
    }
}