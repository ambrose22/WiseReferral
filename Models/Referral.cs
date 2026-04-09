using System.ComponentModel.DataAnnotations;

namespace ReferralTracker.Models;

public class Referral
{
    public int Id { get; set; }

    [Required]
    public string ReferredByUserId { get; set; } = string.Empty;

    [Required]
    public string CandidateFirstName { get; set; } = string.Empty;

    [Required]
    public string CandidateLastName { get; set; } = string.Empty;

    [Required]
    public string CandidateEmail { get; set; } = string.Empty;

    public string? CandidatePhone { get; set; }

    public string? CandidateLinkedIn { get; set; }

    public string? ResumeFileName { get; set; }

    public string? ResumeFilePath { get; set; }

    [Required]
    public string Location { get; set; } = string.Empty;

    [Required]
    public string Relationship { get; set; } = string.Empty;

    [Required]
    public string Justification { get; set; } = string.Empty;

    public ReferralStatus Status { get; set; } = ReferralStatus.Submitted;

    public string? AssignedToUserId { get; set; }

    public string? DeclineReason { get; set; }

    public DateTime CreatedUtc { get; set; }

    public DateTime UpdatedUtc { get; set; }

    // Navigation properties
    public ApplicationUser ReferredByUser { get; set; } = null!;
    public ApplicationUser? AssignedToUser { get; set; }
    public ICollection<ReferralComment> Comments { get; set; } = new List<ReferralComment>();
}
