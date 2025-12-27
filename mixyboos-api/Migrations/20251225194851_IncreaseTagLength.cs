using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MixyBoos.Api.Migrations
{
    /// <inheritdoc />
    public partial class IncreaseTagLength : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "tag_name",
                schema: "mixyboos",
                table: "tags",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(25)",
                oldMaxLength: 25,
                oldNullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "tag_name",
                schema: "mixyboos",
                table: "tags",
                type: "character varying(25)",
                maxLength: 25,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(50)",
                oldMaxLength: 50,
                oldNullable: true);
        }
    }
}
