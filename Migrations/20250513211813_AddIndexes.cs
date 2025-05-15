using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Mailroom.Migrations
{
    /// <inheritdoc />
    public partial class AddIndexes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameIndex(
                name: "IX_Packages_UserId",
                table: "Packages",
                newName: "Packages_UserId_index");

            migrationBuilder.CreateIndex(
                name: "Users_email_uindex",
                table: "Users",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "Users_Role_index",
                table: "Users",
                column: "Role");

            migrationBuilder.CreateIndex(
                name: "Packages_Delivered_DeliveredDate_index",
                table: "Packages",
                columns: new[] { "Delivered", "DeliveredDate" });

            migrationBuilder.CreateIndex(
                name: "Packages_Delivered_UserId_index",
                table: "Packages",
                columns: new[] { "Delivered", "UserId" });

            migrationBuilder.CreateIndex(
                name: "Packages_UserId_DeliveredDate_index",
                table: "Packages",
                columns: new[] { "UserId", "DeliveredDate" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "Users_email_uindex",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "Users_Role_index",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "Packages_Delivered_DeliveredDate_index",
                table: "Packages");

            migrationBuilder.DropIndex(
                name: "Packages_Delivered_UserId_index",
                table: "Packages");

            migrationBuilder.DropIndex(
                name: "Packages_UserId_DeliveredDate_index",
                table: "Packages");

            migrationBuilder.RenameIndex(
                name: "Packages_UserId_index",
                table: "Packages",
                newName: "IX_Packages_UserId");
        }
    }
}
