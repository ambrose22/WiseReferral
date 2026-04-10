using ReferralTracker.Models;

namespace ReferralTracker.Models.ViewModels;

public class ReferralFilterViewModel
{
    public string? Search { get; set; }
    public ReferralStatus? Status { get; set; }
    public string? Location { get; set; }
    public string? AssignedTo { get; set; }

    public IEnumerable<Referral> Results { get; set; } = Enumerable.Empty<Referral>();

    public bool HasActiveFilters =>
        !string.IsNullOrWhiteSpace(Search) || Status.HasValue || !string.IsNullOrWhiteSpace(Location) || !string.IsNullOrWhiteSpace(AssignedTo);
}
