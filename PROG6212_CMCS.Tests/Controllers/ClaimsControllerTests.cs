using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using PROG6212_CMCS.Server.Controllers;
using PROG6212_CMCS.Server.Data;
using PROG6212_CMCS.Server.Models;
using Xunit;
using System.Security.Claims;

namespace PROG6212_CMCS.Tests.Controllers
{
    public class ClaimsControllerTests
    {
        private ApplicationDbContext GetFreshContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            return new ApplicationDbContext(options);
        }

        private ClaimsController GetControllerWithMockUser(ApplicationDbContext context, int userId = 1)
        {
            var controller = new ClaimsController(context);

            // Mock do User com claims - usa namespace completo para evitar conflito
            var claims = new List<System.Security.Claims.Claim>
            {
                new System.Security.Claims.Claim("id", userId.ToString()),
                new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.Role, "ProgrammeCoordinator")
            };

            var identity = new System.Security.Claims.ClaimsIdentity(claims, "TestAuth");
            var principal = new System.Security.Claims.ClaimsPrincipal(identity);

            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = principal }
            };

            return controller;
        }

        [Fact]
        public void Create_ValidClaim_CalculatesTotalAmount()
        {
            // Arrange
            using var context = GetFreshContext();
            var controller = new ClaimsController(context);

            // Criar lecturer primeiro
            var lecturer = new Lecturer { LecturerId = 1, HourlyRate = 450.00m };
            context.Lecturers.Add(lecturer);
            context.SaveChanges();

            var claim = new PROG6212_CMCS.Server.Models.Claim
            {
                LecturerId = 1,
                HoursWorked = 10
            };

            // Act
            var result = controller.Create(claim);

            // Assert - Testa apenas o cálculo automático
            var createdResult = Assert.IsType<CreatedAtActionResult>(result);
            var returnedClaim = Assert.IsType<PROG6212_CMCS.Server.Models.Claim>(createdResult.Value);
            Assert.Equal(4500.00m, returnedClaim.TotalAmount); // 10 * 450
            Assert.Equal(ClaimStatus.Pending, returnedClaim.Status); // Status deve ser Pending
        }

        [Fact]
        public void Create_InvalidLecturer_ReturnsBadRequest()
        {
            // Arrange
            using var context = GetFreshContext();
            var controller = new ClaimsController(context);

            var claim = new PROG6212_CMCS.Server.Models.Claim { LecturerId = 999, HoursWorked = 10 };

            // Act
            var result = controller.Create(claim);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        //[Fact]
        //public void GetAll_ReturnsClaims_WithoutIncludes()
        //{
        //    // Arrange
        //    using var context = GetFreshContext();
        //    var controller = GetControllerWithMockUser(context); // ← USA O MOCK USER

        //    // Adicionar claims diretamente (sem relações complexas)
        //    context.Claims.Add(new PROG6212_CMCS.Server.Models.Claim { ClaimId = 1, LecturerId = 1, HoursWorked = 10 });
        //    context.Claims.Add(new PROG6212_CMCS.Server.Models.Claim { ClaimId = 2, LecturerId = 1, HoursWorked = 5 });
        //    context.SaveChanges();

        //    // Act
        //    var result = controller.GetAll();

        //    // Assert
        //    var okResult = Assert.IsType<OkObjectResult>(result);
        //    var claims = Assert.IsType<List<PROG6212_CMCS.Server.Models.Claim>>(okResult.Value);
        //    Assert.Equal(2, claims.Count);
        //}

        //[Fact]
        //public void GetPendingClaims_ReturnsOnlyPending()
        //{
        //    // Arrange
        //    using var context = GetFreshContext();
        //    var controller = GetControllerWithMockUser(context); // ← USA O MOCK USER

        //    context.Claims.Add(new PROG6212_CMCS.Server.Models.Claim { ClaimId = 1, LecturerId = 1, HoursWorked = 10, Status = ClaimStatus.Pending });
        //    context.Claims.Add(new PROG6212_CMCS.Server.Models.Claim { ClaimId = 2, LecturerId = 1, HoursWorked = 5, Status = ClaimStatus.Approved });
        //    context.Claims.Add(new PROG6212_CMCS.Server.Models.Claim { ClaimId = 3, LecturerId = 1, HoursWorked = 8, Status = ClaimStatus.Pending });
        //    context.SaveChanges();

        //    // Act
        //    var result = controller.GetPendingClaims();

        //    // Assert
        //    var okResult = Assert.IsType<OkObjectResult>(result);
        //    var claims = Assert.IsType<List<PROG6212_CMCS.Server.Models.Claim>>(okResult.Value);
        //    Assert.Equal(2, claims.Count); // Apenas os 2 com status Pending
        //    Assert.All(claims, c => Assert.Equal(ClaimStatus.Pending, c.Status));
        //}
    }
}