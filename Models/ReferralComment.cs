using System.ComponentModel.DataAnnotations;

namespace ReferralTracker.Models;

public class ReferralComment
{
    public int Id { get; set; }

    public int ReferralId { get; set; }

    [Required]
    public string UserId { get; set; } = string.Empty;

    [Required]
    public string Comment { get; set; } = string.Empty;

    [Required]
    public string Action { get; set; } = string.Empty;

    public DateTime CreatedUtc { get; set; }

    // Navigation properties
    public Referral Referral { get; set; } = null!;
    public ApplicationUser User { get; set; } = null!;
}
