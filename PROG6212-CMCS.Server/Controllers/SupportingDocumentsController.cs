using Microsoft.AspNetCore.Mvc;
using PROG6212_CMCS.Server.Data;
using PROG6212_CMCS.Server.Models;

namespace PROG6212_CMCS.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SupportingDocumentsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _env;

        public SupportingDocumentsController(ApplicationDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        [HttpPost("{claimId}")]
        public async Task<IActionResult> Upload(int claimId, IFormFile file)
        {
            var claim = _context.Claims.Find(claimId);
            if (claim == null) return NotFound("Claim not found");
            if (file == null || file.Length == 0) return BadRequest("File is empty");

            var uploadPath = Path.Combine(_env.WebRootPath ?? "wwwroot", "uploads", "claims", claimId.ToString());
            Directory.CreateDirectory(uploadPath);

            var fileName = Path.GetFileName(file.FileName);
            var fullPath = Path.Combine(uploadPath, fileName);

            using (var stream = new FileStream(fullPath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            var doc = new SupportingDocument
            {
                ClaimId = claimId,
                FileName = fileName,
                FilePath = $"/uploads/claims/{claimId}/{fileName}",
                UploadDate = DateTime.UtcNow,
                ContentType = file.ContentType
            };

            _context.SupportingDocuments.Add(doc);
            _context.SaveChanges();

            return Ok(doc);
        }

        [HttpGet("{claimId}")]
        public IActionResult GetDocuments(int claimId)
        {
            var docs = _context.SupportingDocuments.Where(d => d.ClaimId == claimId).ToList();
            return Ok(docs);
        }

        [HttpGet("download/{id}")]
        public IActionResult DownloadDocument(int id)
        {
            var doc = _context.SupportingDocuments.FirstOrDefault(d => d.DocumentId == id);
            if (doc == null) return NotFound("Document not found");

            var filePath = Path.Combine(_env.WebRootPath ?? "wwwroot", doc.FilePath.TrimStart('/'));
            if (!System.IO.File.Exists(filePath)) return NotFound("File not found on disk");

            var stream = System.IO.File.OpenRead(filePath);
            return File(stream, doc.ContentType ?? "application/octet-stream", doc.FileName);
        }


    }
}
