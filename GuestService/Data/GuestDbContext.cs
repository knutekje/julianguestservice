using Microsoft.EntityFrameworkCore;
using GuestService.Models;

namespace GuestService.Data;

public class GuestDbContext : DbContext
{
    public GuestDbContext(DbContextOptions<GuestDbContext> options) : base(options)
    {
    }

    public DbSet<Guest> Guests { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Guest>()
            .HasIndex(g => g.Email)
            .IsUnique();

        modelBuilder.Entity<Guest>()
            .HasIndex(g => g.PhoneNumber)
            .IsUnique();
    }
}