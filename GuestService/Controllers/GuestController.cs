using GuestService.Models;
using GuestService.Services;
using Microsoft.AspNetCore.Mvc;

namespace GuestService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class GuestsController : ControllerBase
{
    private readonly IGuestService _guestService;

    public GuestsController(IGuestService guestService)
    {
        _guestService = guestService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllGuests()
    {
        var guests = await _guestService.GetAllGuestsAsync();
        return Ok(guests);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetGuestById(int id)
    {
        var guest = await _guestService.GetGuestByIdAsync(id);
        if (guest == null) return NotFound();
        return Ok(guest);
    }

    [HttpPost]
    public async Task<IActionResult> AddGuest([FromBody] Guest guest)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var addedGuest = await _guestService.AddGuestAsync(guest);
        return CreatedAtAction(nameof(GetGuestById), new { id = addedGuest.Id }, addedGuest);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateGuest(int id, [FromBody] Guest updatedGuest)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var success = await _guestService.UpdateGuestAsync(id, updatedGuest);
        if (!success) return NotFound();

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteGuest(int id)
    {
        var success = await _guestService.DeleteGuestAsync(id);
        if (!success) return NotFound();

        return NoContent();
    }
}