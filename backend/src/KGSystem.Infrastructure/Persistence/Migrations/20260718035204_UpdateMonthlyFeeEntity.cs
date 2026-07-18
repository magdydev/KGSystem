using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KGSystem.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class UpdateMonthlyFeeEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MonthlyFees_KGPhases_KGPhaseId",
                table: "MonthlyFees");

            migrationBuilder.DropIndex(
                name: "IX_MonthlyFees_AcademicYearId",
                table: "MonthlyFees");

            migrationBuilder.DropIndex(
                name: "IX_MonthlyFees_KGPhaseId_AcademicYearId",
                table: "MonthlyFees");

            migrationBuilder.DropColumn(
                name: "KGPhaseId",
                table: "MonthlyFees");

            migrationBuilder.RenameColumn(
                name: "DueDay",
                table: "MonthlyFees",
                newName: "Month");

            migrationBuilder.AddColumn<DateTime>(
                name: "DueDate",
                table: "MonthlyFees",
                type: "datetime2",
                nullable: true);

            migrationBuilder.Sql("DELETE FROM MonthlyFees");

            migrationBuilder.CreateIndex(
                name: "IX_MonthlyFees_AcademicYearId_Month",
                table: "MonthlyFees",
                columns: new[] { "AcademicYearId", "Month" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_MonthlyFees_AcademicYearId_Month",
                table: "MonthlyFees");

            migrationBuilder.DropColumn(
                name: "DueDate",
                table: "MonthlyFees");

            migrationBuilder.RenameColumn(
                name: "Month",
                table: "MonthlyFees",
                newName: "DueDay");

            migrationBuilder.AddColumn<Guid>(
                name: "KGPhaseId",
                table: "MonthlyFees",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_MonthlyFees_AcademicYearId",
                table: "MonthlyFees",
                column: "AcademicYearId");

            migrationBuilder.CreateIndex(
                name: "IX_MonthlyFees_KGPhaseId_AcademicYearId",
                table: "MonthlyFees",
                columns: new[] { "KGPhaseId", "AcademicYearId" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_MonthlyFees_KGPhases_KGPhaseId",
                table: "MonthlyFees",
                column: "KGPhaseId",
                principalTable: "KGPhases",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
