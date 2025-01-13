using FluentAssertions;
using GuestService.Controllers;
using GuestService.Models;
using GuestService.Services;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace GuestService.Tests;

public class GuestsControllerTests
{
    private readonly Mock<IGuestService> _mockGuestService;
    private readonly GuestsController _controller;

    public GuestsControllerTests()
    {
        _mockGuestService = new Mock<IGuestService>();
        _controller = new GuestsController(_mockGuestService.Object);
    }

    [Fact]
    public async Task AddGuest_ShouldReturnCreatedGuest()
    {
        var guest = new Guest
        {
            Id = 1,
            FirstName = "John",
            LastName = "Doe",
            Email = "john.doe@example.com",
            PhoneNumber = "1234567890"
        };
        _mockGuestService.Setup(s => s.AddGuestAsync(It.IsAny<Guest>())).ReturnsAsync(guest);

        var result = await _controller.AddGuest(guest);

        result.Should().BeOfType<CreatedAtActionResult>();
        var createdResult = result as CreatedAtActionResult;
        createdResult?.Value.Should().BeEquivalentTo(guest);
    }
}