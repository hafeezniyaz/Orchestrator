using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Orchestrator.Application.Features.Packages.Interfaces;
using Orchestrator.Application.Features.Packages.Models;

namespace Orchestrator.API.Controllers;

[ApiController]
[Route("api/v1/apps/{appId}/packages")]
[Authorize]
public class PackagesController : ControllerBase
{
    private readonly IPackageService _packageService;
    private readonly IConfiguration _configuration;
    public PackagesController(IPackageService packageService, IConfiguration configuration)
    {
        _packageService = packageService;
        _configuration = configuration;
    }

    [HttpGet]
    public async Task<IActionResult> ListPackagesForApp(
        Guid appId,
            [FromQuery] string? version,
            [FromQuery] bool? isActive,
            [FromQuery] int skip = 0,
            [FromQuery] int top = 20
        )
    {
        var packages = await _packageService.GetPackagesForAppAsync(appId, version, isActive, skip, top);
        return Ok(packages);
    }

    [HttpGet("{version}")]
    public async Task<IActionResult> GetPackageByVersion(Guid appId, string version)
    {
        // Call the new service method
        var package = await _packageService.GetPackageByVersionAsync(appId, version);

        // If the package is not found, return a 404 Not Found response
        if (package == null)
        {
            return NotFound($"Package with version '{version}' not found for this app.");
        }

        // If the package is found, return a 200 OK response with the package details
        return Ok(package);
    }


    [HttpGet("{version}/download")]
    public async Task<IActionResult> DownloadPackage(Guid appId, string version)
    {
        // 1. Get the file path from our service
        var filePath = await _packageService.GetPackageFilePathAsync(appId, version);

        // 2. Validate that the package record and physical file exist
        if (filePath == null || !System.IO.File.Exists(filePath))
        {
            return NotFound($"Package with version '{version}' not found for this app.");
        }

        // 3. Return the file for download
        var fileName = Path.GetFileName(filePath);
        return PhysicalFile(filePath, "application/zip", fileName);
    }

    [HttpPatch("{version}")]
    [Authorize(Roles = "Administrator")] // Managing packages is an admin task
    public async Task<IActionResult> UpdatePackage(Guid appId, string version, [FromBody] UpdatePackageRequest request)
    {
        var updatedPackage = await _packageService.UpdatePackageAsync(appId, version, request);

        if (updatedPackage == null)
        {
            return NotFound();
        }

        return Ok(updatedPackage);
    }

    [HttpDelete("{version}")]
    [Authorize(Roles = "Administrator")]
    public async Task<IActionResult> DeletePackage(Guid appId, string version)
    {
        var success = await _packageService.DeletePackageAsync(appId, version);

        if (!success)
        {
            return NotFound();
        }

        return NoContent();
    }

    [HttpPost]
    [Authorize(Roles = "Administrator,Publisher")]
    [RequestFormLimits(MultipartBodyLengthLimit = 524288000)]
    [DisableRequestSizeLimit]
    public async Task<IActionResult> UploadPackage(Guid appId, IFormFile file)
    {
        // Basic validation
        if (file == null || file.Length == 0)
        {
            return BadRequest("No file uploaded.");
        }

        // 1. Version Parsing from filename
        var fileName = file.FileName;
        if (!fileName.EndsWith(".zip", StringComparison.OrdinalIgnoreCase))
        {
            return BadRequest("Invalid file type. Only .zip files are allowed.");
        }
        var version = fileName[..^4]; // Removes the last 4 characters (".zip")

        // 2. Validation logic as per PRD
        if (!await _packageService.IsAppActiveAsync(appId))
        {
            return BadRequest($"App with ID '{appId}' is not active or does not exist."); //
        }

        if (await _packageService.DoesVersionExistAsync(appId, version))
        {
            return BadRequest($"Package version '{version}' already exists for this app."); //
        }

        try
        {
            // 1. Get the root path from configuration
            var rootPath = _configuration["StorageSettings:RootPath"];
            if (string.IsNullOrEmpty(rootPath))
            {
                // In a real app, you would log this error with Serilog
                return StatusCode(500, "Storage path is not configured on the server.");
            }

            // 2. Create a subdirectory for the app to keep packages organized
            var appStoragePath = Path.Combine(rootPath, appId.ToString());
            Directory.CreateDirectory(appStoragePath);

            // 3. Define the full path for the new file
            var filePath = Path.Combine(appStoragePath, fileName);

            // 4. Copy the uploaded file stream to the destination file stream
            await using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            var newPackage = await _packageService.CreatePackageRecordAsync(appId, version, filePath);

            return CreatedAtAction(nameof(UploadPackage), new { appId = newPackage.AppId, version = newPackage.Version }, newPackage);
        }
        catch (Exception ex)
        {
            // Log the exception with Serilog
            // For now, just return a generic server error
            return StatusCode(500, $"An internal error occurred: {ex.Message}");
        }
    }

}