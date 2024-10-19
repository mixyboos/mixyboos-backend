using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace MixyBoos.Api.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "auth");

            migrationBuilder.EnsureSchema(
                name: "mixyboos");

            migrationBuilder.CreateTable(
                name: "identity_role",
                schema: "auth",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    normalized_name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    concurrency_stamp = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_identity_role", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "identity_user",
                schema: "auth",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_name = table.Column<string>(type: "text", nullable: true),
                    normalized_user_name = table.Column<string>(type: "text", nullable: true),
                    email = table.Column<string>(type: "text", nullable: true),
                    normalized_email = table.Column<string>(type: "text", nullable: true),
                    email_confirmed = table.Column<bool>(type: "boolean", nullable: false),
                    password_hash = table.Column<string>(type: "text", nullable: true),
                    security_stamp = table.Column<string>(type: "text", nullable: true),
                    concurrency_stamp = table.Column<string>(type: "text", nullable: true),
                    phone_number = table.Column<string>(type: "text", nullable: true),
                    phone_number_confirmed = table.Column<bool>(type: "boolean", nullable: false),
                    two_factor_enabled = table.Column<bool>(type: "boolean", nullable: false),
                    lockout_end = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    lockout_enabled = table.Column<bool>(type: "boolean", nullable: false),
                    access_failed_count = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_identity_user", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "tags",
                schema: "mixyboos",
                columns: table => new
                {
                    id = table.Column<string>(type: "character varying(36)", nullable: false),
                    tag_name = table.Column<string>(type: "text", nullable: true),
                    date_created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    date_updated = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_tags", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "user",
                schema: "auth",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    title = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    display_name = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    profile_image = table.Column<string>(type: "text", nullable: true),
                    header_image = table.Column<string>(type: "text", nullable: true),
                    city = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    country = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    biography = table.Column<string>(type: "character varying(2048)", maxLength: 2048, nullable: true),
                    stream_key = table.Column<string>(type: "text", nullable: true),
                    slug = table.Column<string>(type: "text", nullable: true),
                    user_name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    normalized_user_name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    email = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    normalized_email = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    email_confirmed = table.Column<bool>(type: "boolean", nullable: false),
                    password_hash = table.Column<string>(type: "text", nullable: true),
                    security_stamp = table.Column<string>(type: "text", nullable: true),
                    concurrency_stamp = table.Column<string>(type: "text", nullable: true),
                    phone_number = table.Column<string>(type: "text", nullable: true),
                    phone_number_confirmed = table.Column<bool>(type: "boolean", nullable: false),
                    two_factor_enabled = table.Column<bool>(type: "boolean", nullable: false),
                    lockout_end = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    lockout_enabled = table.Column<bool>(type: "boolean", nullable: false),
                    access_failed_count = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_user", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "role_claim",
                schema: "auth",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    role_id = table.Column<Guid>(type: "uuid", nullable: false),
                    claim_type = table.Column<string>(type: "text", nullable: true),
                    claim_value = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_role_claim", x => x.id);
                    table.ForeignKey(
                        name: "fk_role_claim_identity_role_role_id",
                        column: x => x.role_id,
                        principalSchema: "auth",
                        principalTable: "identity_role",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "live_shows",
                schema: "mixyboos",
                columns: table => new
                {
                    id = table.Column<string>(type: "character varying(36)", nullable: false),
                    title = table.Column<string>(type: "text", nullable: true),
                    description = table.Column<string>(type: "text", nullable: true),
                    start_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    status = table.Column<int>(type: "integer", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: true),
                    date_created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    date_updated = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_live_shows", x => x.id);
                    table.ForeignKey(
                        name: "fk_live_shows_users_user_id",
                        column: x => x.user_id,
                        principalSchema: "auth",
                        principalTable: "user",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "mixes",
                schema: "mixyboos",
                columns: table => new
                {
                    id = table.Column<string>(type: "character varying(36)", nullable: false),
                    title = table.Column<string>(type: "text", nullable: false),
                    description = table.Column<string>(type: "text", nullable: false),
                    image = table.Column<string>(type: "text", nullable: true),
                    audio_url = table.Column<string>(type: "text", nullable: true),
                    is_processed = table.Column<bool>(type: "boolean", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    slug = table.Column<string>(type: "text", nullable: true),
                    date_created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    date_updated = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_mixes", x => x.id);
                    table.ForeignKey(
                        name: "fk_mixes_users_user_id",
                        column: x => x.user_id,
                        principalSchema: "auth",
                        principalTable: "user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "user_claim",
                schema: "auth",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    claim_type = table.Column<string>(type: "text", nullable: true),
                    claim_value = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_user_claim", x => x.id);
                    table.ForeignKey(
                        name: "fk_user_claim_user_user_id",
                        column: x => x.user_id,
                        principalSchema: "auth",
                        principalTable: "user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "user_followers",
                schema: "auth",
                columns: table => new
                {
                    followers_id = table.Column<Guid>(type: "uuid", nullable: false),
                    following_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_user_followers", x => new { x.followers_id, x.following_id });
                    table.ForeignKey(
                        name: "fk_user_followers_user_followers_id",
                        column: x => x.followers_id,
                        principalSchema: "auth",
                        principalTable: "user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_user_followers_user_following_id",
                        column: x => x.following_id,
                        principalSchema: "auth",
                        principalTable: "user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "user_identity_role",
                schema: "auth",
                columns: table => new
                {
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    role_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_user_identity_role", x => new { x.user_id, x.role_id });
                    table.ForeignKey(
                        name: "fk_user_identity_role_identity_role_role_id",
                        column: x => x.role_id,
                        principalSchema: "auth",
                        principalTable: "identity_role",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_user_identity_role_user_user_id",
                        column: x => x.user_id,
                        principalSchema: "auth",
                        principalTable: "user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "user_login",
                schema: "auth",
                columns: table => new
                {
                    login_provider = table.Column<string>(type: "text", nullable: false),
                    provider_key = table.Column<string>(type: "text", nullable: false),
                    provider_display_name = table.Column<string>(type: "text", nullable: true),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_user_login", x => new { x.login_provider, x.provider_key });
                    table.ForeignKey(
                        name: "fk_user_login_user_user_id",
                        column: x => x.user_id,
                        principalSchema: "auth",
                        principalTable: "user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "user_token",
                schema: "auth",
                columns: table => new
                {
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    login_provider = table.Column<string>(type: "text", nullable: false),
                    name = table.Column<string>(type: "text", nullable: false),
                    value = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_user_token", x => new { x.user_id, x.login_provider, x.name });
                    table.ForeignKey(
                        name: "fk_user_token_user_user_id",
                        column: x => x.user_id,
                        principalSchema: "auth",
                        principalTable: "user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "show_chat",
                schema: "mixyboos",
                columns: table => new
                {
                    id = table.Column<string>(type: "character varying(36)", nullable: false),
                    from_user_id = table.Column<Guid>(type: "uuid", nullable: true),
                    to_user_id = table.Column<Guid>(type: "uuid", nullable: true),
                    date_sent = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    message = table.Column<string>(type: "text", nullable: true),
                    show_id = table.Column<string>(type: "character varying(36)", nullable: true),
                    date_created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    date_updated = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_show_chat", x => x.id);
                    table.ForeignKey(
                        name: "fk_show_chat_live_shows_show_id",
                        column: x => x.show_id,
                        principalSchema: "mixyboos",
                        principalTable: "live_shows",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_show_chat_users_from_user_id",
                        column: x => x.from_user_id,
                        principalSchema: "auth",
                        principalTable: "user",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_show_chat_users_to_user_id",
                        column: x => x.to_user_id,
                        principalSchema: "auth",
                        principalTable: "user",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "show_tags",
                schema: "mixyboos",
                columns: table => new
                {
                    live_show_id = table.Column<string>(type: "character varying(36)", nullable: false),
                    tags_id = table.Column<string>(type: "character varying(36)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_show_tags", x => new { x.live_show_id, x.tags_id });
                    table.ForeignKey(
                        name: "fk_show_tags_live_shows_live_show_id",
                        column: x => x.live_show_id,
                        principalSchema: "mixyboos",
                        principalTable: "live_shows",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_show_tags_tags_tags_id",
                        column: x => x.tags_id,
                        principalSchema: "mixyboos",
                        principalTable: "tags",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "mix_downloads",
                schema: "mixyboos",
                columns: table => new
                {
                    id = table.Column<string>(type: "character varying(36)", nullable: false),
                    date_created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    date_updated = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    mix_id = table.Column<string>(type: "character varying(36)", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_mix_downloads", x => x.id);
                    table.ForeignKey(
                        name: "fk_mix_downloads_mixes_mix_id",
                        column: x => x.mix_id,
                        principalSchema: "mixyboos",
                        principalTable: "mixes",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_mix_downloads_users_user_id",
                        column: x => x.user_id,
                        principalSchema: "auth",
                        principalTable: "user",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "mix_likes",
                schema: "mixyboos",
                columns: table => new
                {
                    id = table.Column<string>(type: "character varying(36)", nullable: false),
                    date_created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    date_updated = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    mix_id = table.Column<string>(type: "character varying(36)", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_mix_likes", x => x.id);
                    table.ForeignKey(
                        name: "fk_mix_likes_mixes_mix_id",
                        column: x => x.mix_id,
                        principalSchema: "mixyboos",
                        principalTable: "mixes",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_mix_likes_users_user_id",
                        column: x => x.user_id,
                        principalSchema: "auth",
                        principalTable: "user",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "mix_plays",
                schema: "mixyboos",
                columns: table => new
                {
                    mix_id = table.Column<string>(type: "character varying(36)", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    id = table.Column<string>(type: "character varying(36)", nullable: false),
                    date_created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    date_updated = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_mix_plays", x => new { x.mix_id, x.user_id });
                    table.ForeignKey(
                        name: "fk_mix_plays_mixes_mix_id",
                        column: x => x.mix_id,
                        principalSchema: "mixyboos",
                        principalTable: "mixes",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_mix_plays_users_user_id",
                        column: x => x.user_id,
                        principalSchema: "auth",
                        principalTable: "user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "mix_shares",
                schema: "mixyboos",
                columns: table => new
                {
                    id = table.Column<string>(type: "character varying(36)", nullable: false),
                    date_created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    date_updated = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    mix_id = table.Column<string>(type: "character varying(36)", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_mix_shares", x => x.id);
                    table.ForeignKey(
                        name: "fk_mix_shares_mixes_mix_id",
                        column: x => x.mix_id,
                        principalSchema: "mixyboos",
                        principalTable: "mixes",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_mix_shares_users_user_id",
                        column: x => x.user_id,
                        principalSchema: "auth",
                        principalTable: "user",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "mix_tags",
                schema: "mixyboos",
                columns: table => new
                {
                    mix_id = table.Column<string>(type: "character varying(36)", nullable: false),
                    tags_id = table.Column<string>(type: "character varying(36)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_mix_tags", x => new { x.mix_id, x.tags_id });
                    table.ForeignKey(
                        name: "fk_mix_tags_mixes_mix_id",
                        column: x => x.mix_id,
                        principalSchema: "mixyboos",
                        principalTable: "mixes",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_mix_tags_tags_tags_id",
                        column: x => x.tags_id,
                        principalSchema: "mixyboos",
                        principalTable: "tags",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                schema: "auth",
                table: "identity_role",
                columns: new[] { "id", "concurrency_stamp", "name", "normalized_name" },
                values: new object[,]
                {
                    { new Guid("34a72006-b670-4818-9b30-b4c82f04fefa"), null, "Member", "MEMBER" },
                    { new Guid("40f63451-76d9-4b54-82d6-cb4e0278e9e9"), null, "Admin", "ADMIN" },
                    { new Guid("797a8b5d-adc4-4083-9ad3-7d1afc83b105"), null, "SuperAdmin", "SUPERADMIN" },
                    { new Guid("ee39d193-0ada-43dc-b168-e84a6a9f912e"), null, "Artist", "ARTIST" }
                });

            migrationBuilder.InsertData(
                schema: "auth",
                table: "user",
                columns: new[] { "id", "access_failed_count", "biography", "city", "concurrency_stamp", "country", "display_name", "email", "email_confirmed", "header_image", "lockout_enabled", "lockout_end", "normalized_email", "normalized_user_name", "password_hash", "phone_number", "phone_number_confirmed", "profile_image", "security_stamp", "slug", "stream_key", "title", "two_factor_enabled", "user_name" },
                values: new object[] { new Guid("28c796f3-bb77-467f-a53e-2eb7f8c48bab"), 0, null, null, "b3df0bf6-3c68-48be-a25f-50fd6f0263ae", null, "Fergal Moran", "fergal.moran+mixyboos@gmail.com", true, null, false, null, "FERGAL.MORAN+MIXYBOOS@GMAIL.COM", "FERGAL.MORAN", "AQAAAAIAAYagAAAAEFNLexXClVZcDHtchHeo9ssBtPn66cuaVFSh6VK8awRFN0RlHuinjjYZXVNGWxn05w==", null, false, null, null, null, "YfbUdfzcgjgIXvUaNZ3X9lQoyhdEc6nc", null, false, "fergal.moran" });

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                schema: "auth",
                table: "identity_role",
                column: "normalized_name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_live_shows_user_id",
                schema: "mixyboos",
                table: "live_shows",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "ix_mix_downloads_mix_id",
                schema: "mixyboos",
                table: "mix_downloads",
                column: "mix_id");

            migrationBuilder.CreateIndex(
                name: "ix_mix_downloads_user_id",
                schema: "mixyboos",
                table: "mix_downloads",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "ix_mix_likes_mix_id",
                schema: "mixyboos",
                table: "mix_likes",
                column: "mix_id");

            migrationBuilder.CreateIndex(
                name: "ix_mix_likes_user_id",
                schema: "mixyboos",
                table: "mix_likes",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "ix_mix_plays_user_id",
                schema: "mixyboos",
                table: "mix_plays",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "ix_mix_shares_mix_id",
                schema: "mixyboos",
                table: "mix_shares",
                column: "mix_id");

            migrationBuilder.CreateIndex(
                name: "ix_mix_shares_user_id",
                schema: "mixyboos",
                table: "mix_shares",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "ix_mix_tags_tags_id",
                schema: "mixyboos",
                table: "mix_tags",
                column: "tags_id");

            migrationBuilder.CreateIndex(
                name: "ix_mixes_slug",
                schema: "mixyboos",
                table: "mixes",
                column: "slug",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_mixes_user_id",
                schema: "mixyboos",
                table: "mixes",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "ix_role_claim_role_id",
                schema: "auth",
                table: "role_claim",
                column: "role_id");

            migrationBuilder.CreateIndex(
                name: "ix_show_chat_from_user_id",
                schema: "mixyboos",
                table: "show_chat",
                column: "from_user_id");

            migrationBuilder.CreateIndex(
                name: "ix_show_chat_show_id",
                schema: "mixyboos",
                table: "show_chat",
                column: "show_id");

            migrationBuilder.CreateIndex(
                name: "ix_show_chat_to_user_id",
                schema: "mixyboos",
                table: "show_chat",
                column: "to_user_id");

            migrationBuilder.CreateIndex(
                name: "ix_show_tags_tags_id",
                schema: "mixyboos",
                table: "show_tags",
                column: "tags_id");

            migrationBuilder.CreateIndex(
                name: "ix_tags_tag_name",
                schema: "mixyboos",
                table: "tags",
                column: "tag_name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                schema: "auth",
                table: "user",
                column: "normalized_email");

            migrationBuilder.CreateIndex(
                name: "ix_user_slug",
                schema: "auth",
                table: "user",
                column: "slug",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                schema: "auth",
                table: "user",
                column: "normalized_user_name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_user_claim_user_id",
                schema: "auth",
                table: "user_claim",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "ix_user_followers_following_id",
                schema: "auth",
                table: "user_followers",
                column: "following_id");

            migrationBuilder.CreateIndex(
                name: "ix_user_identity_role_role_id",
                schema: "auth",
                table: "user_identity_role",
                column: "role_id");

            migrationBuilder.CreateIndex(
                name: "ix_user_login_user_id",
                schema: "auth",
                table: "user_login",
                column: "user_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "identity_user",
                schema: "auth");

            migrationBuilder.DropTable(
                name: "mix_downloads",
                schema: "mixyboos");

            migrationBuilder.DropTable(
                name: "mix_likes",
                schema: "mixyboos");

            migrationBuilder.DropTable(
                name: "mix_plays",
                schema: "mixyboos");

            migrationBuilder.DropTable(
                name: "mix_shares",
                schema: "mixyboos");

            migrationBuilder.DropTable(
                name: "mix_tags",
                schema: "mixyboos");

            migrationBuilder.DropTable(
                name: "role_claim",
                schema: "auth");

            migrationBuilder.DropTable(
                name: "show_chat",
                schema: "mixyboos");

            migrationBuilder.DropTable(
                name: "show_tags",
                schema: "mixyboos");

            migrationBuilder.DropTable(
                name: "user_claim",
                schema: "auth");

            migrationBuilder.DropTable(
                name: "user_followers",
                schema: "auth");

            migrationBuilder.DropTable(
                name: "user_identity_role",
                schema: "auth");

            migrationBuilder.DropTable(
                name: "user_login",
                schema: "auth");

            migrationBuilder.DropTable(
                name: "user_token",
                schema: "auth");

            migrationBuilder.DropTable(
                name: "mixes",
                schema: "mixyboos");

            migrationBuilder.DropTable(
                name: "live_shows",
                schema: "mixyboos");

            migrationBuilder.DropTable(
                name: "tags",
                schema: "mixyboos");

            migrationBuilder.DropTable(
                name: "identity_role",
                schema: "auth");

            migrationBuilder.DropTable(
                name: "user",
                schema: "auth");
        }
    }
}
