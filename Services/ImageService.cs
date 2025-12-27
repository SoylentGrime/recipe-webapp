namespace Recipe_Webpage.Services;

public class ImageService : IImageService
{
    private readonly IWebHostEnvironment _environment;
    private readonly ILogger<ImageService> _logger;
    private const string ImageFolder = "images/recipes";
    
    public long MaxFileSize => 5 * 1024 * 1024; // 5MB
    public string[] AllowedExtensions => new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
    
    public ImageService(IWebHostEnvironment environment, ILogger<ImageService> logger)
    {
        _environment = environment;
        _logger = logger;
    }
    
    public bool IsValidImage(IFormFile file)
    {
        if (file == null || file.Length == 0)
            return false;
            
        if (file.Length > MaxFileSize)
            return false;
            
        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
        if (!AllowedExtensions.Contains(extension))
            return false;
            
        // Check if it's actually an image by examining the content type
        var allowedContentTypes = new[] { "image/jpeg", "image/png", "image/gif", "image/webp" };
        if (!allowedContentTypes.Contains(file.ContentType.ToLowerInvariant()))
            return false;
            
        return true;
    }
    
    public async Task<string?> UploadImageAsync(IFormFile file, int? recipeId = null)
    {
        if (!IsValidImage(file))
        {
            _logger.LogWarning("Invalid image file attempted to upload: {FileName}", file?.FileName);
            return null;
        }
        
        try
        {
            // Create the uploads folder if it doesn't exist
            var uploadsPath = Path.Combine(_environment.WebRootPath, ImageFolder);
            if (!Directory.Exists(uploadsPath))
            {
                Directory.CreateDirectory(uploadsPath);
            }
            
            // Generate a unique filename
            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            var timestamp = DateTime.UtcNow.ToString("yyyyMMddHHmmss");
            var uniqueId = Guid.NewGuid().ToString("N")[..8];
            
            string fileName;
            if (recipeId.HasValue)
            {
                fileName = $"recipe-{recipeId}-{timestamp}-{uniqueId}{extension}";
            }
            else
            {
                // Clean the original filename for use
                var originalName = Path.GetFileNameWithoutExtension(file.FileName);
                originalName = SanitizeFileName(originalName);
                fileName = $"{originalName}-{timestamp}-{uniqueId}{extension}";
            }
            
            var filePath = Path.Combine(uploadsPath, fileName);
            
            // Save the file
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }
            
            _logger.LogInformation("Image uploaded successfully: {FileName}", fileName);
            
            // Return the URL path (URL-encoded for spaces and special characters)
            return $"/{ImageFolder}/{Uri.EscapeDataString(fileName)}";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error uploading image: {FileName}", file.FileName);
            return null;
        }
    }
    
    public Task<bool> DeleteImageAsync(string imageUrl)
    {
        if (string.IsNullOrWhiteSpace(imageUrl))
            return Task.FromResult(false);
            
        try
        {
            // Convert URL to file path
            var relativePath = Uri.UnescapeDataString(imageUrl.TrimStart('/'));
            var filePath = Path.Combine(_environment.WebRootPath, relativePath);
            
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
                _logger.LogInformation("Image deleted: {FilePath}", filePath);
                return Task.FromResult(true);
            }
            
            _logger.LogWarning("Image not found for deletion: {FilePath}", filePath);
            return Task.FromResult(false);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting image: {ImageUrl}", imageUrl);
            return Task.FromResult(false);
        }
    }
    
    private static string SanitizeFileName(string fileName)
    {
        // Remove invalid characters and limit length
        var invalidChars = Path.GetInvalidFileNameChars();
        var sanitized = new string(fileName
            .Where(c => !invalidChars.Contains(c))
            .Take(50)
            .ToArray());
            
        // Replace spaces with hyphens
        sanitized = sanitized.Replace(' ', '-');
        
        // Remove multiple consecutive hyphens
        while (sanitized.Contains("--"))
        {
            sanitized = sanitized.Replace("--", "-");
        }
        
        return sanitized.Trim('-');
    }
}
