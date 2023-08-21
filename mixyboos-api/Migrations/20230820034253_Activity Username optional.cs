using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MixyBoos.Api.Migrations
{
    /// <inheritdoc />
    public partial class ActivityUsernameoptional : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_mix_download_user_user_id",
                schema: "mixyboos",
                table: "mix_download");

            migrationBuilder.DropForeignKey(
                name: "fk_mix_likes_users_user_id",
                schema: "mixyboos",
                table: "mix_likes");

            migrationBuilder.DropForeignKey(
                name: "fk_mix_shares_users_user_id",
                schema: "mixyboos",
                table: "mix_shares");

            migrationBuilder.AlterColumn<Guid>(
                name: "user_id",
                schema: "mixyboos",
                table: "mix_shares",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AlterColumn<Guid>(
                name: "user_id",
                schema: "mixyboos",
                table: "mix_likes",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AlterColumn<Guid>(
                name: "user_id",
                schema: "mixyboos",
                table: "mix_download",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AddForeignKey(
                name: "fk_mix_download_user_user_id",
                schema: "mixyboos",
                table: "mix_download",
                column: "user_id",
                principalSchema: "oid",
                principalTable: "user",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_mix_likes_users_user_id",
                schema: "mixyboos",
                table: "mix_likes",
                column: "user_id",
                principalSchema: "oid",
                principalTable: "user",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_mix_shares_users_user_id",
                schema: "mixyboos",
                table: "mix_shares",
                column: "user_id",
                principalSchema: "oid",
                principalTable: "user",
                principalColumn: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_mix_download_user_user_id",
                schema: "mixyboos",
                table: "mix_download");

            migrationBuilder.DropForeignKey(
                name: "fk_mix_likes_users_user_id",
                schema: "mixyboos",
                table: "mix_likes");

            migrationBuilder.DropForeignKey(
                name: "fk_mix_shares_users_user_id",
                schema: "mixyboos",
                table: "mix_shares");

            migrationBuilder.AlterColumn<Guid>(
                name: "user_id",
                schema: "mixyboos",
                table: "mix_shares",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "user_id",
                schema: "mixyboos",
                table: "mix_likes",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "user_id",
                schema: "mixyboos",
                table: "mix_download",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "fk_mix_download_user_user_id",
                schema: "mixyboos",
                table: "mix_download",
                column: "user_id",
                principalSchema: "oid",
                principalTable: "user",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_mix_likes_users_user_id",
                schema: "mixyboos",
                table: "mix_likes",
                column: "user_id",
                principalSchema: "oid",
                principalTable: "user",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_mix_shares_users_user_id",
                schema: "mixyboos",
                table: "mix_shares",
                column: "user_id",
                principalSchema: "oid",
                principalTable: "user",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
