using System.ComponentModel.DataAnnotations;

namespace ReferralTracker.Models.ViewModels;

public class ReferralViewModel
{
    [Required]
    [Display(Name = "First Name")]
    public string CandidateFirstName { get; set; } = string.Empty;

    [Required]
    [Display(Name = "Last Name")]
    public string CandidateLastName { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    [Display(Name = "Email")]
    public string CandidateEmail { get; set; } = string.Empty;

    [Display(Name = "Phone")]
    public string? CandidatePhone { get; set; }

    [Display(Name = "LinkedIn")]
    public string? CandidateLinkedIn { get; set; }

    [Required]
    public string Location { get; set; } = string.Empty;

    [Required]
    public string Relationship { get; set; } = string.Empty;

    [Required]
    public string Justification { get; set; } = string.Empty;

    public static readonly List<string> Locations = new()
    {
        // Priority locations first
        "Sydney, Australia",
        "Bengaluru, India",
        "Chicago, US",
        "London, United Kingdom",
        "Nanjing, China",
        "Madrid, Spain",
        // Rest alphabetically
        "Adelaide, Australia",
        "Basel, Switzerland",
        "Berchem, Belgium",
        "Buenos Aires, Argentina",
        "Dallas, US",
        "Dubai, United Arab Emirates",
        "Dublin, Ireland",
        "Fuzhou, China",
        "Hamburg, Germany",
        "Istanbul, Turkey",
        "Johannesburg, South Africa",
        "Kobe, Japan",
        "Lima, Peru",
        "Los Angeles, US",
        "Manchester, United Kingdom",
        "Massachusetts, US",
        "Melbourne, Australia",
        "Milan, Italy",
        "Montevideo, Uruguay",
        "North Carolina, US",
        "Oslo, Norway",
        "San Francisco, US",
        "Seoul, Republic of Korea",
        "Shanghai, China",
        "Shenzhen, China",
        "Singapore, Singapore",
        "Stockholm, Sweden",
        "São Paulo, Brazil",
        "Taipei, Taiwan",
        "Utrecht, The Netherlands",
        "York, United Kingdom"
    };
}
