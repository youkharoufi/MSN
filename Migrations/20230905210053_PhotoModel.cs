using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MSN.Migrations
{
    public partial class PhotoModel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_Photo_ProfilePicId",
                table: "AspNetUsers");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_ProfilePicId",
                table: "AspNetUsers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Photo",
                table: "Photo");

            migrationBuilder.DropColumn(
                name: "ProfilePicId",
                table: "AspNetUsers");

            migrationBuilder.RenameTable(
                name: "Photo",
                newName: "Photos");

            migrationBuilder.AddColumn<string>(
                name: "PhotoUrl",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Url",
                table: "Photos",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "Photos",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Photos",
                table: "Photos",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_Photos_UserId",
                table: "Photos",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Photos_AspNetUsers_UserId",
                table: "Photos",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Photos_AspNetUsers_UserId",
                table: "Photos");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Photos",
                table: "Photos");

            migrationBuilder.DropIndex(
                name: "IX_Photos_UserId",
                table: "Photos");

            migrationBuilder.DropColumn(
                name: "PhotoUrl",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "Url",
                table: "Photos");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Photos");

            migrationBuilder.RenameTable(
                name: "Photos",
                newName: "Photo");

            migrationBuilder.AddColumn<int>(
                name: "ProfilePicId",
                table: "AspNetUsers",
                type: "int",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Photo",
                table: "Photo",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_ProfilePicId",
                table: "AspNetUsers",
                column: "ProfilePicId");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_Photo_ProfilePicId",
                table: "AspNetUsers",
                column: "ProfilePicId",
                principalTable: "Photo",
                principalColumn: "Id");
        }
    }
}
