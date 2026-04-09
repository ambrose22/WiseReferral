using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace ReferralTracker.Models;

public class ApplicationUser : IdentityUser
{
    [Required]
    public string FullName { get; set; } = string.Empty;

    public string? Department { get; set; }
}
