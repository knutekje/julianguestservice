using GuestService.Data;
using GuestService.Models;
using Microsoft.EntityFrameworkCore;

namespace GuestService.Services;

public class GuestService : IGuestService
{
    private readonly GuestDbContext _context;

    public GuestService(GuestDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Guest>> GetAllGuestsAsync()
    {
        return await _context.Guests.ToListAsync();
    }

    public async Task<Guest?> GetGuestByIdAsync(int id)
    {
        return await _context.Guests.FindAsync(id);
    }

    public async Task<Guest> AddGuestAsync(Guest guest)
    {
        guest.CreatedAt = DateTime.UtcNow;
        _context.Guests.Add(guest);
        await _context.SaveChangesAsync();
        return guest;
    }

    public async Task<bool> UpdateGuestAsync(int id, Guest updatedGuest)
    {
        var existingGuest = await _context.Guests.FindAsync(id);
        if (existingGuest == null) return false;

        existingGuest.FirstName = updatedGuest.FirstName;
        existingGuest.LastName = updatedGuest.LastName;
        existingGuest.Email = updatedGuest.Email;
        existingGuest.PhoneNumber = updatedGuest.PhoneNumber;
        existingGuest.Address = updatedGuest.Address;
        existingGuest.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteGuestAsync(int id)
    {
        var guest = await _context.Guests.FindAsync(id);
        if (guest == null) return false;

        _context.Guests.Remove(guest);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<Guest?> GetGuestByEmailAsync(string email)
    {
        return await _context.Guests.FirstOrDefaultAsync(g => g.Email == email);
    }

    public async Task<Guest?> GetGuestByPhoneAsync(string phoneNumber)
    {
        return await _context.Guests.FirstOrDefaultAsync(g => g.PhoneNumber == phoneNumber);
    }
}