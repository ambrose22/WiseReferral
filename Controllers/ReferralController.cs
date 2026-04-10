using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ReferralTracker.Models;
using ReferralTracker.Models.ViewModels;
using ReferralTracker.Services.Interfaces;

namespace ReferralTracker.Controllers;

[Authorize]
public class ReferralController : Controller
{
    private readonly IReferralService _service;
    private readonly IWebHostEnvironment _env;

    public ReferralController(IReferralService service, IWebHostEnvironment env)
    {
        _service = service;
        _env = env;
    }

    [HttpGet]
    public async Task<IActionResult> Index(string? search, ReferralStatus? status, string? location)
    {
        var userId = GetCurrentUserId();
        if (userId == null)
            return Forbid();

        var referrals = await _service.GetByUserAsync(userId, search, status, location);
        var model = new ReferralFilterViewModel
        {
            Search = search,
            Status = status,
            Location = location,
            Results = referrals
        };
        return View(model);
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View(new ReferralViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(ReferralViewModel model, IFormFile? resume)
    {
        if (!ModelState.IsValid)
            return View(model);

        var userId = GetCurrentUserId();
        if (userId == null)
            return Forbid();

        string? resumeFileName = null;
        string? resumeFilePath = null;

        if (resume != null && resume.Length > 0)
        {
            var ext = Path.GetExtension(resume.FileName).ToLowerInvariant();
            if (ext != ".pdf" && ext != ".docx")
            {
                ModelState.AddModelError("resume", "Only PDF and DOCX files are accepted.");
                return View(model);
            }

            var uploadsDir = Path.Combine(_env.ContentRootPath, "App_Data", "Resumes");
            Directory.CreateDirectory(uploadsDir);

            var uniqueName = Guid.NewGuid() + ext;
            var filePath = Path.Combine(uploadsDir, uniqueName);

            using var stream = new FileStream(filePath, FileMode.Create);
            await resume.CopyToAsync(stream);

            resumeFileName = resume.FileName;
            resumeFilePath = filePath;
        }

        var referral = await _service.CreateAsync(userId, model);

        if (resumeFileName != null)
        {
            await _service.UpdateResumeAsync(referral.Id, resumeFileName, resumeFilePath!);
        }

        return RedirectToAction(nameof(Details), new { id = referral.Id });
    }

    [HttpGet]
    public async Task<IActionResult> Details(int id)
    {
        var userId = GetCurrentUserId();
        if (userId == null)
            return Forbid();

        var role = GetCurrentUserRole();
        var referral = await _service.GetByIdAsync(id, userId, role);
        if (referral == null)
        {
            TempData["ErrorMessage"] = "Referral not found.";
            return RedirectToAction(nameof(Index));
        }

        ViewData["CurrentUserRole"] = role;
        ViewData["CurrentUserId"] = userId;

        if (role == "Admin")
        {
            ViewData["TalentTeamUsers"] = await _service.GetTalentTeamUsersAsync();
        }

        return View(referral);
    }

    [HttpGet]
    [Authorize(Roles = "Admin,TalentTeam")]
    public async Task<IActionResult> AllReferrals(string? search, ReferralStatus? status, string? location, string? assignedTo)
    {
        var referrals = await _service.GetAllAsync(search, status, location, assignedTo);
        var model = new ReferralFilterViewModel
        {
            Search = search,
            Status = status,
            Location = location,
            AssignedTo = assignedTo,
            Results = referrals
        };
        ViewData["TalentTeamUsers"] = await _service.GetTalentTeamUsersAsync();
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> AssignTo(int referralId, string assignToUserId)
    {
        await _service.AssignToUserAsync(referralId, assignToUserId);
        return RedirectToAction(nameof(Details), new { id = referralId });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin,TalentTeam")]
    public async Task<IActionResult> ChangeStatus(int referralId, ReferralStatus newStatus, string? notes)
    {
        var userId = GetCurrentUserId();
        if (userId == null)
            return Forbid();

        // TalentTeam can only change status on referrals assigned to them
        if (User.IsInRole("TalentTeam") && !User.IsInRole("Admin"))
        {
            var referral = await _service.GetByIdAsync(referralId, userId, "TalentTeam");
            if (referral == null || referral.AssignedToUserId != userId)
            {
                TempData["ErrorMessage"] = "You can only change the status of referrals assigned to you.";
                return RedirectToAction(nameof(Details), new { id = referralId });
            }
        }

        var result = await _service.ChangeStatusAsync(referralId, userId, newStatus, notes);
        if (!result)
            TempData["ErrorMessage"] = "Invalid status transition.";

        return RedirectToAction(nameof(Details), new { id = referralId });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> RespondToMoreInfo(int referralId, string responseText, IFormFile? resume)
    {
        var userId = GetCurrentUserId();
        if (userId == null)
            return Forbid();

        var referral = await _service.GetByIdAsync(referralId, userId, "Employee");
        if (referral == null || referral.ReferredByUserId != userId || referral.Status != ReferralStatus.MoreInfoRequested)
        {
            TempData["ErrorMessage"] = "Unable to respond to this referral.";
            return RedirectToAction(nameof(Details), new { id = referralId });
        }

        if (resume != null && resume.Length > 0)
        {
            var ext = Path.GetExtension(resume.FileName).ToLowerInvariant();
            if (ext != ".pdf" && ext != ".docx")
            {
                TempData["ErrorMessage"] = "Only PDF and DOCX files are accepted.";
                return RedirectToAction(nameof(Details), new { id = referralId });
            }

            var uploadsDir = Path.Combine(_env.ContentRootPath, "App_Data", "Resumes");
            Directory.CreateDirectory(uploadsDir);

            var uniqueName = Guid.NewGuid() + ext;
            var filePath = Path.Combine(uploadsDir, uniqueName);

            using var stream = new FileStream(filePath, FileMode.Create);
            await resume.CopyToAsync(stream);

            await _service.UpdateResumeAsync(referralId, resume.FileName, filePath);
        }

        await _service.RespondToMoreInfoAsync(referralId, userId, responseText);
        return RedirectToAction(nameof(Details), new { id = referralId });
    }

    [HttpGet]
    public async Task<IActionResult> DownloadResume(int id)
    {
        var userId = GetCurrentUserId();
        if (userId == null)
            return Forbid();

        var referral = await _service.GetByIdAsync(id, userId, GetCurrentUserRole());
        if (referral == null || string.IsNullOrEmpty(referral.ResumeFilePath))
            return NotFound();

        if (!System.IO.File.Exists(referral.ResumeFilePath))
            return NotFound();

        var contentType = Path.GetExtension(referral.ResumeFilePath).ToLowerInvariant() == ".pdf"
            ? "application/pdf"
            : "application/vnd.openxmlformats-officedocument.wordprocessingml.document";

        return PhysicalFile(referral.ResumeFilePath, contentType, referral.ResumeFileName);
    }

    private string? GetCurrentUserId() =>
        User?.FindFirstValue(ClaimTypes.NameIdentifier);

    private string GetCurrentUserRole()
    {
        if (User.IsInRole("Admin")) return "Admin";
        if (User.IsInRole("TalentTeam")) return "TalentTeam";
        return "Employee";
    }
}
