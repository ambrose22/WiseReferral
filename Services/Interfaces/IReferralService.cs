using ReferralTracker.Models;
using ReferralTracker.Models.ViewModels;

namespace ReferralTracker.Services.Interfaces;

public interface IReferralService
{
    Task<IEnumerable<Referral>> GetByUserAsync(string userId, string? search = null, ReferralStatus? status = null, string? location = null);
    Task<Referral?> GetByIdAsync(int id, string userId, string userRole);
    Task<Referral> CreateAsync(string userId, ReferralViewModel vm);
    Task UpdateResumeAsync(int referralId, string fileName, string filePath);
    Task<IEnumerable<Referral>> GetAllAsync(string? search = null, ReferralStatus? status = null, string? location = null, string? assignedTo = null);
    Task AssignToUserAsync(int referralId, string assignedUserId);
    Task<bool> ChangeStatusAsync(int referralId, string userId, ReferralStatus newStatus, string? notes);
    Task RespondToMoreInfoAsync(int referralId, string userId, string responseText);
    Task<IEnumerable<ApplicationUser>> GetTalentTeamUsersAsync();
}
