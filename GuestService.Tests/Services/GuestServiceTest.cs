using FluentAssertions;
using GuestService.Data;
using GuestService.Models;
using GuestService.Services;

using Microsoft.EntityFrameworkCore;
using Xunit;

namespace GuestService.Tests;

public class GuestServiceTests
{
    private readonly GuestDbContext _context;
    private readonly IGuestService _guestService;

    public GuestServiceTests()
    {
        var options = new DbContextOptionsBuilder<GuestDbContext>()
            .UseInMemoryDatabase("GuestServiceTestDb")
            .Options;

        _context = new GuestDbContext(options);
        _guestService = new Services.GuestService(_context);
    }

    [Fact]
    public async Task AddGuestAsync_ShouldAddGuestSuccessfully()
    {
        var guest = new Guest
        {
            FirstName = "John",
            LastName = "Doe",
            Email = "john.doe@example.com",
            PhoneNumber = "1234567890",
            Address = "123 Main St"
        };

        var result = await _guestService.AddGuestAsync(guest);

        var dbGuest = await _context.Guests.FindAsync(result.Id);
        dbGuest.Should().NotBeNull();
        dbGuest.FirstName.Should().Be("John");
        dbGuest.LastName.Should().Be("Doe");
    }

    [Fact]
    public async Task UpdateGuestAsync_ShouldUpdateGuestDetails()
    {
        var guest = new Guest
        {
            FirstName = "Jane",
            LastName = "Doe",
            Email = "jane.doe@example.com",
            PhoneNumber = "9876543210",
            Address = "456 Another St"
        };
        _context.Guests.Add(guest);
        await _context.SaveChangesAsync();

        var updatedGuest = new Guest
        {
            FirstName = "Jane",
            LastName = "Smith",
            Email = "jane.smith@example.com",
            PhoneNumber = "1112223333",
            Address = "789 New St"
        };

        var success = await _guestService.UpdateGuestAsync(guest.Id, updatedGuest);

        success.Should().BeTrue();
        var dbGuest = await _context.Guests.FindAsync(guest.Id);
        dbGuest?.LastName.Should().Be("Smith");
        dbGuest?.Email.Should().Be("jane.smith@example.com");
    }
}
