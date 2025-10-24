using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using PROG6212_CMCS.Server.Controllers;
using PROG6212_CMCS.Server.Data;
using PROG6212_CMCS.Server.Models;
using Xunit;

namespace PROG6212_CMCS.Tests.Controllers
{
    public class SupportingDocumentsControllerTests
    {
        private ApplicationDbContext GetFreshContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            return new ApplicationDbContext(options);
        }

        [Fact]
        public void GetDocuments_ExistingClaim_ReturnsDocuments()
        {
            // Arrange
            using var context = GetFreshContext();
            var mockEnv = new TestWebHostEnvironment();
            var controller = new SupportingDocumentsController(context, mockEnv);

            var claim = new PROG6212_CMCS.Server.Models.Claim { ClaimId = 1, LecturerId = 1, HoursWorked = 10 };
            context.Claims.Add(claim);

            var doc = new SupportingDocument
            {
                DocumentId = 1,
                ClaimId = 1,
                FileName = "test.pdf"
            };
            context.SupportingDocuments.Add(doc);
            context.SaveChanges();

            // Act
            var result = controller.GetDocuments(1);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var documents = Assert.IsType<List<SupportingDocument>>(okResult.Value);
            Assert.Single(documents);
        }

        private class TestWebHostEnvironment : IWebHostEnvironment
        {
            public string WebRootPath { get; set; } = "wwwroot";
            public string EnvironmentName { get; set; } = "Test";
            public string ApplicationName { get; set; } = "TestApp";
            public string ContentRootPath { get; set; } = Directory.GetCurrentDirectory();
            public IFileProvider ContentRootFileProvider { get; set; } = null!;
            public IFileProvider WebRootFileProvider { get; set; } = null!;
        }
    }
}