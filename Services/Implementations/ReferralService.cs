using Microsoft.EntityFrameworkCore;
using ReferralTracker.Data;
using ReferralTracker.Models;
using ReferralTracker.Models.ViewModels;
using ReferralTracker.Services.Interfaces;

namespace ReferralTracker.Services.Implementations;

public class ReferralService : IReferralService
{
    private readonly AppDbContext _context;

    public ReferralService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Referral>> GetByUserAsync(string userId)
    {
        return await _context.Referrals
            .Include(r => r.Comments)
            .Where(r => r.ReferredByUserId == userId)
            .OrderByDescending(r => r.CreatedUtc)
            .ToListAsync();
    }

    public async Task<Referral?> GetByIdAsync(int id, string userId, string userRole)
    {
        var query = _context.Referrals
            .Include(r => r.ReferredByUser)
            .Include(r => r.AssignedToUser)
            .Include(r => r.Comments.OrderBy(c => c.CreatedUtc))
                .ThenInclude(c => c.User);

        // Admin and TalentTeam can see any referral; employees see only their own
        if (userRole == "Admin" || userRole == "TalentTeam")
            return await query.FirstOrDefaultAsync(r => r.Id == id);

        return await query.FirstOrDefaultAsync(r => r.Id == id && r.ReferredByUserId == userId);
    }

    public async Task<Referral> CreateAsync(string userId, ReferralViewModel vm)
    {
        var now = DateTime.UtcNow;

        var referral = new Referral
        {
            ReferredByUserId = userId,
            CandidateFirstName = vm.CandidateFirstName,
            CandidateLastName = vm.CandidateLastName,
            CandidateEmail = vm.CandidateEmail,
            CandidatePhone = vm.CandidatePhone,
            CandidateLinkedIn = vm.CandidateLinkedIn,
            Location = vm.Location,
            Relationship = vm.Relationship,
            Justification = vm.Justification,
            Status = ReferralStatus.Submitted,
            CreatedUtc = now,
            UpdatedUtc = now
        };

        _context.Referrals.Add(referral);
        await _context.SaveChangesAsync();

        var comment = new ReferralComment
        {
            ReferralId = referral.Id,
            UserId = userId,
            Comment = "Referral submitted.",
            Action = "Submitted",
            CreatedUtc = now
        };

        _context.ReferralComments.Add(comment);
        await _context.SaveChangesAsync();

        return referral;
    }

    public async Task UpdateResumeAsync(int referralId, string fileName, string filePath)
    {
        var referral = await _context.Referrals.FindAsync(referralId);
        if (referral == null) return;

        referral.ResumeFileName = fileName;
        referral.ResumeFilePath = filePath;
        await _context.SaveChangesAsync();
    }

    public async Task<IEnumerable<Referral>> GetAllAsync()
    {
        return await _context.Referrals
            .Include(r => r.ReferredByUser)
            .Include(r => r.AssignedToUser)
            .Include(r => r.Comments)
            .OrderByDescending(r => r.CreatedUtc)
            .ToListAsync();
    }

    public async Task AssignToUserAsync(int referralId, string assignedUserId)
    {
        var referral = await _context.Referrals.FindAsync(referralId);
        if (referral == null) return;

        referral.AssignedToUserId = assignedUserId;
        referral.UpdatedUtc = DateTime.UtcNow;

        _context.ReferralComments.Add(new ReferralComment
        {
            ReferralId = referralId,
            UserId = assignedUserId,
            Comment = "Referral assigned.",
            Action = "Assigned",
            CreatedUtc = DateTime.UtcNow
        });

        await _context.SaveChangesAsync();
    }

    private static readonly Dictionary<ReferralStatus, ReferralStatus[]> ValidTransitions = new()
    {
        { ReferralStatus.Submitted, new[] { ReferralStatus.UnderReview } },
        { ReferralStatus.UnderReview, new[] { ReferralStatus.Screening, ReferralStatus.Declined, ReferralStatus.MoreInfoRequested } },
        { ReferralStatus.MoreInfoRequested, new[] { ReferralStatus.UnderReview } },
        { ReferralStatus.Screening, new[] { ReferralStatus.Interviewing, ReferralStatus.Declined } },
        { ReferralStatus.Interviewing, new[] { ReferralStatus.Hired, ReferralStatus.Declined } },
    };

    public static ReferralStatus[] GetValidNextStatuses(ReferralStatus current)
    {
        return ValidTransitions.TryGetValue(current, out var next) ? next : [];
    }

    public async Task<bool> ChangeStatusAsync(int referralId, string userId, ReferralStatus newStatus, string? notes)
    {
        var referral = await _context.Referrals.FindAsync(referralId);
        if (referral == null) return false;

        if (!ValidTransitions.TryGetValue(referral.Status, out var allowed) || !allowed.Contains(newStatus))
            return false;

        var now = DateTime.UtcNow;
        referral.Status = newStatus;
        referral.UpdatedUtc = now;

        if (newStatus == ReferralStatus.Declined)
            referral.DeclineReason = notes;

        _context.ReferralComments.Add(new ReferralComment
        {
            ReferralId = referralId,
            UserId = userId,
            Comment = notes ?? $"Status changed to {newStatus}.",
            Action = newStatus.ToString(),
            CreatedUtc = now
        });

        await _context.SaveChangesAsync();
        return true;
    }
}
