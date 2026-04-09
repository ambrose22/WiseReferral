using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ReferralTracker.Migrations
{
    /// <inheritdoc />
    public partial class RemoveDeptAddLocationAndResume : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "JobTitle",
                table: "Referrals");

            migrationBuilder.RenameColumn(
                name: "Department",
                table: "Referrals",
                newName: "Location");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Location",
                table: "Referrals",
                newName: "Department");

            migrationBuilder.AddColumn<string>(
                name: "JobTitle",
                table: "Referrals",
                type: "TEXT",
                nullable: true);
        }
    }
}
