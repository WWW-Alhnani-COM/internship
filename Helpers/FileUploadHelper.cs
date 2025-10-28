using Microsoft.AspNetCore.StaticFiles;

namespace InternshipManagement.API.Helpers;

public static class FileUploadHelper
{
    // أنواع الملفات المسموحة
    private static readonly string[] AllowedExtensions = { ".pdf", ".doc", ".docx", ".txt", ".jpg", ".jpeg", ".png" };

    public static async Task<string?> UploadFileAsync(IFormFile file, string uploadPath)
    {
        if (file == null || file.Length == 0)
            return null;

        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
        if (!AllowedExtensions.Contains(extension))
            return null;

        var fileName = $"{Guid.NewGuid()}{extension}";
        var filePath = Path.Combine(uploadPath, fileName);

        Directory.CreateDirectory(uploadPath); // إنشاء المجلد إذا لم يكن موجودًا

        using var stream = new FileStream(filePath, FileMode.Create);
        await file.CopyToAsync(stream);

        return fileName; // إرجاع اسم الملف فقط (ليس المسار الكامل)
    }

    public static void DeleteFile(string fileName, string uploadPath)
    {
        var filePath = Path.Combine(uploadPath, fileName);
        if (File.Exists(filePath))
            File.Delete(filePath);
    }

    public static string GetContentType(string fileName)
    {
        var provider = new FileExtensionContentTypeProvider();
        if (provider.TryGetContentType(fileName, out var contentType))
            return contentType;
        return "application/octet-stream";
    }
}