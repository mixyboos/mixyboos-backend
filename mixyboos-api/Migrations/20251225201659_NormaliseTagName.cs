using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MixyBoos.Api.Migrations
{
    /// <inheritdoc />
    public partial class NormaliseTagName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "tag_name",
                schema: "mixyboos",
                table: "tags",
                newName: "name");

            migrationBuilder.RenameIndex(
                name: "ix_tags_tag_name",
                schema: "mixyboos",
                table: "tags",
                newName: "ix_tags_name");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "name",
                schema: "mixyboos",
                table: "tags",
                newName: "tag_name");

            migrationBuilder.RenameIndex(
                name: "ix_tags_name",
                schema: "mixyboos",
                table: "tags",
                newName: "ix_tags_tag_name");
        }
    }
}
