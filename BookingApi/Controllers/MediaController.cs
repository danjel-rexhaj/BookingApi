using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookingPlatform.API.Controllers;

[ApiController]
[Route("api/media")]
[Authorize]
public class MediaController : ControllerBase
{
    private readonly IWebHostEnvironment _environment;

    public MediaController(IWebHostEnvironment environment)
    {
        _environment = environment;
    }

    [HttpPost("upload")]
    public async Task<IActionResult> Upload(IFormFile file)
    {
        if (file == null || file.Length == 0)
            return BadRequest("File is required.");

        //  Validate file size (max 5MB)
        if (file.Length > 5 * 1024 * 1024)
            return BadRequest("File too large.");

        //  Validate file type
        var allowedTypes = new[] { ".jpg", ".jpeg", ".png" };
        var extension = Path.GetExtension(file.FileName).ToLower();

        if (!allowedTypes.Contains(extension))
            return BadRequest("Invalid file type.");

        var uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads");

        if (!Directory.Exists(uploadsFolder))
            Directory.CreateDirectory(uploadsFolder);

        var fileName = Guid.NewGuid() + extension;
        var filePath = Path.Combine(uploadsFolder, fileName);

        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        var fileUrl = $"{Request.Scheme}://{Request.Host}/uploads/{fileName}";

        return Ok(new { Url = fileUrl });
    }

    [HttpDelete]
    public IActionResult Delete(string fileName)
    {
        var filePath = Path.Combine(_environment.WebRootPath, "uploads", fileName);

        if (!System.IO.File.Exists(filePath))
            return NotFound();

        System.IO.File.Delete(filePath);

        return NoContent();
    }
}