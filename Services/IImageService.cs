namespace Recipe_Webpage.Services;

public interface IImageService
{
    /// <summary>
    /// Uploads an image file and returns the URL path to the saved image.
    /// </summary>
    /// <param name="file">The uploaded file</param>
    /// <param name="recipeId">Optional recipe ID to include in filename</param>
    /// <returns>The URL path to the saved image, or null if upload failed</returns>
    Task<string?> UploadImageAsync(IFormFile file, int? recipeId = null);
    
    /// <summary>
    /// Deletes an image from the server.
    /// </summary>
    /// <param name="imageUrl">The URL path to the image</param>
    /// <returns>True if deletion was successful</returns>
    Task<bool> DeleteImageAsync(string imageUrl);
    
    /// <summary>
    /// Validates if the uploaded file is an acceptable image.
    /// </summary>
    /// <param name="file">The uploaded file</param>
    /// <returns>True if the file is valid</returns>
    bool IsValidImage(IFormFile file);
    
    /// <summary>
    /// Gets the maximum allowed file size in bytes.
    /// </summary>
    long MaxFileSize { get; }
    
    /// <summary>
    /// Gets the allowed file extensions.
    /// </summary>
    string[] AllowedExtensions { get; }
}
