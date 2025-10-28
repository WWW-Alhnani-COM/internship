using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using InternshipManagement.API.Helpers;

namespace InternshipManagement.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class FileUploadController : ControllerBase
{
    private readonly IWebHostEnvironment _environment;

    public FileUploadController(IWebHostEnvironment environment)
    {
        _environment = environment;
    }

    [HttpPost("internship-agreement")]
    [Authorize(Policy = "StudentOnly")]
    public async Task<ActionResult<string>> UploadInternshipAgreement(IFormFile file)
    {
        var uploadPath = Path.Combine(_environment.WebRootPath, "uploads", "internships");
        var fileName = await FileUploadHelper.UploadFileAsync(file, uploadPath);

        if (fileName == null)
            return BadRequest(new { message = "Invalid file type or no file provided." });

        // يمكنك حفظ اسم الملف في جدول Internships لاحقًا
        return Ok(new { fileName, url = $"/uploads/internships/{fileName}" });
    }

    [HttpPost("weekly-report")]
    [Authorize(Policy = "StudentOnly")]
    public async Task<ActionResult<string>> UploadWeeklyReportFile(IFormFile file)
    {
        var uploadPath = Path.Combine(_environment.WebRootPath, "uploads", "reports");
        var fileName = await FileUploadHelper.UploadFileAsync(file, uploadPath);

        if (fileName == null)
            return BadRequest(new { message = "Invalid file type or no file provided." });

        // يمكنك حفظ اسم الملف في جدول WeeklyReports لاحقًا
        return Ok(new { fileName, url = $"/uploads/reports/{fileName}" });
    }

    [HttpPost("evaluation-document")]
    [Authorize(Policy = "SupervisorOnly")] // Site or Academic
    public async Task<ActionResult<string>> UploadEvaluationDocument(IFormFile file)
    {
        var uploadPath = Path.Combine(_environment.WebRootPath, "uploads", "evaluations");
        var fileName = await FileUploadHelper.UploadFileAsync(file, uploadPath);

        if (fileName == null)
            return BadRequest(new { message = "Invalid file type or no file provided." });

        // يمكنك حفظ اسم الملف في جدول Evaluations لاحقًا
        return Ok(new { fileName, url = $"/uploads/evaluations/{fileName}" });
    }

    // Endpoint لعرض/تنزيل الملف (محمي)
    [HttpGet("download/{category}/{fileName}")]
    [Authorize]
    public IActionResult DownloadFile(string category, string fileName)
    {
        var validCategories = new[] { "internships", "reports", "evaluations" };
        if (!validCategories.Contains(category))
            return BadRequest(new { message = "Invalid category." });

        var filePath = Path.Combine(_environment.WebRootPath, "uploads", category, fileName);
        if (!System.IO.File.Exists(filePath))
            return NotFound();

        var contentType = FileUploadHelper.GetContentType(fileName);
        var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
        return File(fileStream, contentType, fileName);
    }
}