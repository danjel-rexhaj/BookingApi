using BookingPlatform.Domain.Entities;
using Microsoft.EntityFrameworkCore;




namespace BookingPlatform.Infrastructure.Persistence;

public class BookingDbContext : DbContext
{
    public BookingDbContext(DbContextOptions<BookingDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();
    public DbSet<Role> Roles => Set<Role>();
    public DbSet<UserRole> UserRoles => Set<UserRole>();


    public DbSet<OwnerProfile> OwnerProfiles => Set<OwnerProfile>();
    public DbSet<Address> Addresses => Set<Address>();
    public DbSet<Property> Properties => Set<Property>();


    public DbSet<Booking> Bookings => Set<Booking>();
    public DbSet<Review> Reviews => Set<Review>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // --------------------
        // USER
        // --------------------
        modelBuilder.Entity<User>()
            .HasIndex(u => u.Email)
            .IsUnique();

        // --------------------
        // USERROLE (Many-to-Many)
        // --------------------
        modelBuilder.Entity<UserRole>()
            .HasKey(ur => new { ur.UserId, ur.RoleId });

        modelBuilder.Entity<UserRole>()
            .HasOne(ur => ur.User)
            .WithMany(u => u.UserRoles)
            .HasForeignKey(ur => ur.UserId);

        modelBuilder.Entity<UserRole>()
            .HasOne(ur => ur.Role)
            .WithMany(r => r.UserRoles)
            .HasForeignKey(ur => ur.RoleId);

        // --------------------
        // OWNERPROFILE (1-to-1 with User)
        // --------------------
        modelBuilder.Entity<OwnerProfile>()
            .HasKey(op => op.UserId);

        modelBuilder.Entity<OwnerProfile>()
            .HasOne(op => op.User)
            .WithOne()
            .HasForeignKey<OwnerProfile>(op => op.UserId);

        // --------------------
        // PROPERTY (Many-to-1 with User as Owner)
        // --------------------
        modelBuilder.Entity<Property>()
            .HasOne(p => p.Owner)
            .WithMany()
            .HasForeignKey(p => p.OwnerId)
            .OnDelete(DeleteBehavior.Restrict);

        // --------------------
        // ADDRESS (1-to-many with Property)
        // --------------------
        modelBuilder.Entity<Property>()
            .HasOne(p => p.Address)
            .WithMany(a => a.Properties)
            .HasForeignKey(p => p.AddressId);


        // --------------------
        // BOOKING
        // --------------------
        modelBuilder.Entity<Booking>()
            .HasOne(b => b.Property)
            .WithMany()
            .HasForeignKey(b => b.PropertyId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Booking>()
            .HasOne(b => b.Guest)
            .WithMany()
            .HasForeignKey(b => b.GuestId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Booking>()
        .Property(b => b.BookingStatus)
        .HasConversion<string>();
        // --------------------
        // REVIEW
        // --------------------
        modelBuilder.Entity<Review>()
            .HasOne(r => r.Booking)
            .WithMany()
            .HasForeignKey(r => r.BookingId);

        modelBuilder.Entity<Review>()
            .HasOne(r => r.Guest)
            .WithMany()
            .HasForeignKey(r => r.GuestId)
            .OnDelete(DeleteBehavior.Restrict);


        // --------------------
        // SEED ROLES
        // --------------------
        var adminRoleId = Guid.Parse("11111111-1111-1111-1111-111111111111");
        var ownerRoleId = Guid.Parse("22222222-2222-2222-2222-222222222222");
        var guestRoleId = Guid.Parse("33333333-3333-3333-3333-333333333333");

        modelBuilder.Entity<Role>().HasData(
            new Role(
                Guid.Parse("11111111-1111-1111-1111-111111111111"),
                "Admin",
                "System Administrator"
            ),
            new Role(
                Guid.Parse("22222222-2222-2222-2222-222222222222"),
                "Owner",
                "Property Owner"
            ),
            new Role(
                Guid.Parse("33333333-3333-3333-3333-333333333333"),
                "Guest",
                "Customer"
            )
        );

    }


}
