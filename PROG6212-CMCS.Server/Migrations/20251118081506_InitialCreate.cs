using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PROG6212_CMCS.Server.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Roles",
                columns: table => new
                {
                    RoleId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    RoleName = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    AccessLevel = table.Column<int>(type: "INTEGER", nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roles", x => x.RoleId);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 120, nullable: false),
                    Email = table.Column<string>(type: "TEXT", maxLength: 150, nullable: false),
                    PasswordHash = table.Column<string>(type: "TEXT", nullable: true),
                    RoleId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.UserId);
                    table.ForeignKey(
                        name: "FK_Users_Roles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Roles",
                        principalColumn: "RoleId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Lecturers",
                columns: table => new
                {
                    LecturerId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    UserId = table.Column<int>(type: "INTEGER", nullable: false),
                    BankDetails = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Lecturers", x => x.LecturerId);
                    table.ForeignKey(
                        name: "FK_Lecturers_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Claims",
                columns: table => new
                {
                    ClaimId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    LecturerId = table.Column<int>(type: "INTEGER", nullable: false),
                    ClaimDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    HoursWorked = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    HourlyRate = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TotalAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Status = table.Column<int>(type: "INTEGER", nullable: false),
                    Notes = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Claims", x => x.ClaimId);
                    table.ForeignKey(
                        name: "FK_Claims_Lecturers_LecturerId",
                        column: x => x.LecturerId,
                        principalTable: "Lecturers",
                        principalColumn: "LecturerId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ClaimApprovals",
                columns: table => new
                {
                    ApprovalId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ClaimId = table.Column<int>(type: "INTEGER", nullable: false),
                    ApproverId = table.Column<int>(type: "INTEGER", nullable: false),
                    ApprovalDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Decision = table.Column<int>(type: "INTEGER", nullable: false),
                    Comments = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClaimApprovals", x => x.ApprovalId);
                    table.ForeignKey(
                        name: "FK_ClaimApprovals_Claims_ClaimId",
                        column: x => x.ClaimId,
                        principalTable: "Claims",
                        principalColumn: "ClaimId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ClaimApprovals_Users_ApproverId",
                        column: x => x.ApproverId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SupportingDocuments",
                columns: table => new
                {
                    DocumentId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ClaimId = table.Column<int>(type: "INTEGER", nullable: false),
                    FileName = table.Column<string>(type: "TEXT", maxLength: 260, nullable: false),
                    FilePath = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false),
                    UploadDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    ContentType = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SupportingDocuments", x => x.DocumentId);
                    table.ForeignKey(
                        name: "FK_SupportingDocuments_Claims_ClaimId",
                        column: x => x.ClaimId,
                        principalTable: "Claims",
                        principalColumn: "ClaimId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "RoleId", "AccessLevel", "Description", "RoleName" },
                values: new object[] { 1, 1, null, "Admin" });

            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "RoleId", "AccessLevel", "Description", "RoleName" },
                values: new object[] { 2, 2, null, "AcademicManager" });

            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "RoleId", "AccessLevel", "Description", "RoleName" },
                values: new object[] { 3, 3, null, "ProgrammeCoordinator" });

            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "RoleId", "AccessLevel", "Description", "RoleName" },
                values: new object[] { 4, 4, null, "Lecturer" });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "UserId", "Email", "Name", "PasswordHash", "RoleId" },
                values: new object[] { 1, "admin@gmail.com", "Admin", "$2a$11$LC70hkVjkSDMOyEAfWdEf.j2zF4spJcQ4ltFybJi74JyW4uB0WmlO", 1 });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "UserId", "Email", "Name", "PasswordHash", "RoleId" },
                values: new object[] { 2, "manager@gmail.com", "Academic Manager", "$2a$11$86OZcMZPNBlUIxK543zZleXkYhRtHnogmvjTgE5nY1YS41bimIggW", 2 });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "UserId", "Email", "Name", "PasswordHash", "RoleId" },
                values: new object[] { 3, "coordinator@gmail.com", "Programme Coordinator", "$2a$11$S81MIziaHpTPJIVdxtSGdeteCc1OKtHU1lE6AxumWS1ypO3sfkEQS", 3 });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "UserId", "Email", "Name", "PasswordHash", "RoleId" },
                values: new object[] { 4, "lecturer@gmail.com", "Lecturer", "$2a$11$u65twpa4gMxvzzCy2IsgZOaSAKd2C95xovcxq63b47dmrGsqirdTC", 4 });

            migrationBuilder.InsertData(
                table: "Lecturers",
                columns: new[] { "LecturerId", "BankDetails", "UserId" },
                values: new object[] { 1, "ABSA - Acc: 0626673286", 4 });

            migrationBuilder.CreateIndex(
                name: "IX_ClaimApprovals_ApproverId",
                table: "ClaimApprovals",
                column: "ApproverId");

            migrationBuilder.CreateIndex(
                name: "IX_ClaimApprovals_ClaimId",
                table: "ClaimApprovals",
                column: "ClaimId");

            migrationBuilder.CreateIndex(
                name: "IX_Claims_LecturerId",
                table: "Claims",
                column: "LecturerId");

            migrationBuilder.CreateIndex(
                name: "IX_Lecturers_UserId",
                table: "Lecturers",
                column: "UserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SupportingDocuments_ClaimId",
                table: "SupportingDocuments",
                column: "ClaimId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_RoleId",
                table: "Users",
                column: "RoleId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ClaimApprovals");

            migrationBuilder.DropTable(
                name: "SupportingDocuments");

            migrationBuilder.DropTable(
                name: "Claims");

            migrationBuilder.DropTable(
                name: "Lecturers");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Roles");
        }
    }
}
