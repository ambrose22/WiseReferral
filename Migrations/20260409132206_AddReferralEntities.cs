using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ReferralTracker.Migrations
{
    /// <inheritdoc />
    public partial class AddReferralEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Referrals",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ReferredByUserId = table.Column<string>(type: "TEXT", nullable: false),
                    CandidateFirstName = table.Column<string>(type: "TEXT", nullable: false),
                    CandidateLastName = table.Column<string>(type: "TEXT", nullable: false),
                    CandidateEmail = table.Column<string>(type: "TEXT", nullable: false),
                    CandidatePhone = table.Column<string>(type: "TEXT", nullable: true),
                    CandidateLinkedIn = table.Column<string>(type: "TEXT", nullable: true),
                    ResumeFileName = table.Column<string>(type: "TEXT", nullable: true),
                    ResumeFilePath = table.Column<string>(type: "TEXT", nullable: true),
                    Department = table.Column<string>(type: "TEXT", nullable: false),
                    JobTitle = table.Column<string>(type: "TEXT", nullable: true),
                    Relationship = table.Column<string>(type: "TEXT", nullable: false),
                    Justification = table.Column<string>(type: "TEXT", nullable: false),
                    Status = table.Column<int>(type: "INTEGER", nullable: false),
                    AssignedToUserId = table.Column<string>(type: "TEXT", nullable: true),
                    DeclineReason = table.Column<string>(type: "TEXT", nullable: true),
                    CreatedUtc = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedUtc = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Referrals", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Referrals_AspNetUsers_AssignedToUserId",
                        column: x => x.AssignedToUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Referrals_AspNetUsers_ReferredByUserId",
                        column: x => x.ReferredByUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ReferralComments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ReferralId = table.Column<int>(type: "INTEGER", nullable: false),
                    UserId = table.Column<string>(type: "TEXT", nullable: false),
                    Comment = table.Column<string>(type: "TEXT", nullable: false),
                    Action = table.Column<string>(type: "TEXT", nullable: false),
                    CreatedUtc = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReferralComments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ReferralComments_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ReferralComments_Referrals_ReferralId",
                        column: x => x.ReferralId,
                        principalTable: "Referrals",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ReferralComments_ReferralId",
                table: "ReferralComments",
                column: "ReferralId");

            migrationBuilder.CreateIndex(
                name: "IX_ReferralComments_UserId",
                table: "ReferralComments",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Referrals_AssignedToUserId",
                table: "Referrals",
                column: "AssignedToUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Referrals_ReferredByUserId",
                table: "Referrals",
                column: "ReferredByUserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ReferralComments");

            migrationBuilder.DropTable(
                name: "Referrals");
        }
    }
}
