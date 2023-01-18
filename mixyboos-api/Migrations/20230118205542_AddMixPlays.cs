using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MixyBoos.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddMixPlays : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_mix_play_mixes_mix_id",
                table: "mix_play");

            migrationBuilder.DropForeignKey(
                name: "fk_mix_play_users_user_id",
                table: "mix_play");

            migrationBuilder.DropPrimaryKey(
                name: "pk_mix_play",
                table: "mix_play");

            migrationBuilder.RenameTable(
                name: "mix_play",
                newName: "mix_plays");

            migrationBuilder.RenameIndex(
                name: "ix_mix_play_user_id",
                table: "mix_plays",
                newName: "ix_mix_plays_user_id");

            migrationBuilder.RenameIndex(
                name: "ix_mix_play_mix_id",
                table: "mix_plays",
                newName: "ix_mix_plays_mix_id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_mix_plays",
                table: "mix_plays",
                column: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_mix_plays_mixes_mix_id",
                table: "mix_plays",
                column: "mix_id",
                principalTable: "mixes",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_mix_plays_users_user_id",
                table: "mix_plays",
                column: "user_id",
                principalSchema: "auth",
                principalTable: "user",
                principalColumn: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_mix_plays_mixes_mix_id",
                table: "mix_plays");

            migrationBuilder.DropForeignKey(
                name: "fk_mix_plays_users_user_id",
                table: "mix_plays");

            migrationBuilder.DropPrimaryKey(
                name: "pk_mix_plays",
                table: "mix_plays");

            migrationBuilder.RenameTable(
                name: "mix_plays",
                newName: "mix_play");

            migrationBuilder.RenameIndex(
                name: "ix_mix_plays_user_id",
                table: "mix_play",
                newName: "ix_mix_play_user_id");

            migrationBuilder.RenameIndex(
                name: "ix_mix_plays_mix_id",
                table: "mix_play",
                newName: "ix_mix_play_mix_id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_mix_play",
                table: "mix_play",
                column: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_mix_play_mixes_mix_id",
                table: "mix_play",
                column: "mix_id",
                principalTable: "mixes",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_mix_play_users_user_id",
                table: "mix_play",
                column: "user_id",
                principalSchema: "auth",
                principalTable: "user",
                principalColumn: "id");
        }
    }
}
