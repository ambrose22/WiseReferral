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
    public async Task<IActionResult> Index()
    {
        var userId = GetCurrentUserId();
        if (userId == null)
            return Forbid();

        var referrals = await _service.GetByUserAsync(userId);
        return View(referrals);
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

        ViewData["IsAdminOrTalent"] = role == "Admin" || role == "TalentTeam";
        ViewData["CurrentUserId"] = userId;

        return View(referral);
    }

    [HttpGet]
    [Authorize(Roles = "Admin,TalentTeam")]
    public async Task<IActionResult> AllReferrals()
    {
        var referrals = await _service.GetAllAsync();
        return View(referrals);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin,TalentTeam")]
    public async Task<IActionResult> AssignToMe(int referralId)
    {
        var userId = GetCurrentUserId();
        if (userId == null)
            return Forbid();

        await _service.AssignToUserAsync(referralId, userId);
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

        var result = await _service.ChangeStatusAsync(referralId, userId, newStatus, notes);
        if (!result)
            TempData["ErrorMessage"] = "Invalid status transition.";

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
