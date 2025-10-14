using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Orchestrator.API.Models;
using Orchestrator.Application.Features.Packages.Interfaces;
using Orchestrator.Application.Features.Packages.Models;
using System.Threading.Tasks;

namespace Orchestrator.API.Controllers
{
    [ApiController]
    [Route("api/v1/apps/{appId}/packages")]
    [Authorize]
    public class PackageController : ControllerBase
    {
        private readonly IPackageServices _packageServices;
        private readonly IConfiguration _configuration;
        private readonly ILogger<PackageController> _logger;
        public PackageController(IPackageServices packageServices, IConfiguration configuration , ILogger<PackageController> logger)
        {
            _packageServices = packageServices;
            _configuration = configuration;
            _logger = logger;
        }

        #region GET
        [HttpGet("{version}")]
        public async Task<IActionResult> GetPackageByVersion(Guid appId, string version)
        {
            var package = await _packageServices.GetPackageByVersionAsync(appId, version);

            if (package == null)
            {
                return NotFound(new ErrorResponse { Error = "not_found", ErrorDesciption = $"Package with version {version} is not found" });
            }

            return Ok(package);
        }

        [HttpGet]
        public async Task<IActionResult> ListPackagesForApp(
            Guid appId,
            [FromQuery] string? version,
            [FromQuery] bool? isActive,
            [FromQuery] int skip = 0,
            [FromQuery] int top = 50
            )
        {
            var packages = await _packageServices.GetPackagesForAppAsync(appId, version, isActive, skip, top);
            return Ok(packages);
        }

        [HttpGet("{version}/download")]
        public async Task<IActionResult> DownloadPackage(Guid appId, string version)
        {
            var filePath = await _packageServices.GetPackageFilePathAsync(appId, version);

            if (filePath == null || !System.IO.File.Exists(filePath))
            {
                return NotFound(new ErrorResponse { Error = "invalid_request", ErrorDesciption = $"Package with version {version} is not found for the app id {appId}" });
            }

            return PhysicalFile(filePath, "application/zip", Path.GetFileName(filePath));
        }
        #endregion

        #region POST

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [RequestFormLimits(MultipartBodyLengthLimit = 524288000)] //500MB
        [DisableRequestSizeLimit]
        public async Task<IActionResult> UploadPackage(Guid appId, IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest(new ErrorResponse { Error = "file_missing", ErrorDesciption = "No file uploaded" });
            }

            var filename = file.FileName;
            if (!filename.EndsWith(".zip", StringComparison.OrdinalIgnoreCase))
            {
                return BadRequest(new ErrorResponse { Error = "invalid_file", ErrorDesciption = "invalid file type. Ony zip file is allowed" });
            }

            var version = filename[..^4]; //remove last 4 characters i.e .zip

            if (!await _packageServices.IsAppActiveAsync(appId))
            {
                return BadRequest(new ErrorResponse { Error = "invalid_app", ErrorDesciption = $"app with ID {appId} is not active or not found" });
            }

            if (await _packageServices.DoesVersionExistAsync(appId, version))
            {
                return BadRequest(new ErrorResponse { Error = "invalid_package", ErrorDesciption = $"package version {version} already exist for the App Id {appId}" });
            }

            try
            {
                var rootStoragePath = _configuration["StorageSettings:RootPath"];
                if (string.IsNullOrEmpty(rootStoragePath))
                {
                    return StatusCode(500, new ErrorResponse { Error = "missing_configuration", ErrorDesciption = "storage path is not configured on server" });
                }

                var appStoragePath = Path.Combine(rootStoragePath, appId.ToString());
                Directory.CreateDirectory(appStoragePath);

                var filePath = Path.Combine(appStoragePath, filename);

                await using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                var newPackage = await _packageServices.CreatePackageRecordAsync(appId, version, filePath);

                return CreatedAtAction(nameof(GetPackageByVersion), new { appId = newPackage.AppId, version = newPackage.Version }, newPackage); // {methodname}/{route}/{returmValueObject}


            }
            catch (Exception ex)
            {
                _logger.LogError($"an erorr occured while uploading the file to storage bucket due to {ex.Message}");
                return StatusCode(500, new ErrorResponse { Error = "internal_server_error", ErrorDesciption = $"an erorr occured while uploading the file to storage bucket" });
            }
        }
        #endregion

        #region PATCH
        [HttpPatch("{version}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdatePackage(Guid appId, string version, [FromBody] UpdatePackageRequest request)
        {
            var updatedPackage = await _packageServices.UpdatePackageAsync(appId, version, request);

            if (updatedPackage == null)
            {
                return NotFound(new ErrorResponse { Error = "invalid_request", ErrorDesciption = $"Package with version {version} is not found for the app id {appId}" });
            }

            return Ok(updatedPackage);
        }
        #endregion

        #region DELETE
        [HttpDelete("{version}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeletePackage(Guid appId, string version)
        {
            var success = await _packageServices.DeletePackageAsync(appId, version);

            if (!success)
            {
                return NotFound(new ErrorResponse { Error = "invalid_request", ErrorDesciption = $"Package with version {version} is not found for the app id {appId}" });
            }

            return NoContent();
        } 
        #endregion

    }
}
