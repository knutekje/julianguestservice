using GuestService.Models;

namespace GuestService.Services;

public interface IGuestService
{
    Task<IEnumerable<Guest>> GetAllGuestsAsync();
    Task<Guest?> GetGuestByIdAsync(int id);
    Task<Guest> AddGuestAsync(Guest guest);
    Task<bool> UpdateGuestAsync(int id, Guest updatedGuest);
    Task<bool> DeleteGuestAsync(int id);
    Task<Guest?> GetGuestByEmailAsync(string email);
    Task<Guest?> GetGuestByPhoneAsync(string phoneNumber);
}