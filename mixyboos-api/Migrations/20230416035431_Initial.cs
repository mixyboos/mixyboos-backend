using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace MixyBoos.Api.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "oid");

            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:PostgresExtension:uuid-ossp", ",,");

            migrationBuilder.CreateTable(
                name: "identity_user",
                schema: "oid",
                columns: table => new
                {
                    id = table.Column<string>(type: "text", nullable: false),
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
                name: "openiddict_application",
                schema: "oid",
                columns: table => new
                {
                    id = table.Column<string>(type: "text", nullable: false),
                    client_id = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    client_secret = table.Column<string>(type: "text", nullable: true),
                    concurrency_token = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    consent_type = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    display_name = table.Column<string>(type: "text", nullable: true),
                    display_names = table.Column<string>(type: "text", nullable: true),
                    permissions = table.Column<string>(type: "text", nullable: true),
                    post_logout_redirect_uris = table.Column<string>(type: "text", nullable: true),
                    properties = table.Column<string>(type: "text", nullable: true),
                    redirect_uris = table.Column<string>(type: "text", nullable: true),
                    requirements = table.Column<string>(type: "text", nullable: true),
                    type = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_openiddict_application", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "openiddict_scope",
                schema: "oid",
                columns: table => new
                {
                    id = table.Column<string>(type: "text", nullable: false),
                    concurrency_token = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    description = table.Column<string>(type: "text", nullable: true),
                    descriptions = table.Column<string>(type: "text", nullable: true),
                    display_name = table.Column<string>(type: "text", nullable: true),
                    display_names = table.Column<string>(type: "text", nullable: true),
                    name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    properties = table.Column<string>(type: "text", nullable: true),
                    resources = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_openiddict_scope", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "tags",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "uuid_generate_v4()"),
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
                schema: "oid",
                columns: table => new
                {
                    id = table.Column<string>(type: "text", nullable: false),
                    title = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    display_name = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    profile_image = table.Column<string>(type: "text", nullable: true),
                    header_image = table.Column<string>(type: "text", nullable: true),
                    slug = table.Column<string>(type: "text", nullable: true),
                    country = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    biography = table.Column<string>(type: "character varying(2048)", maxLength: 2048, nullable: true),
                    stream_key = table.Column<string>(type: "text", nullable: true),
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
                name: "user_role",
                schema: "oid",
                columns: table => new
                {
                    id = table.Column<string>(type: "text", nullable: false),
                    name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    normalized_name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    concurrency_stamp = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_user_role", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "openiddict_authorization",
                schema: "oid",
                columns: table => new
                {
                    id = table.Column<string>(type: "text", nullable: false),
                    application_id = table.Column<string>(type: "text", nullable: true),
                    concurrency_token = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    creation_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    properties = table.Column<string>(type: "text", nullable: true),
                    scopes = table.Column<string>(type: "text", nullable: true),
                    status = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    subject = table.Column<string>(type: "character varying(400)", maxLength: 400, nullable: true),
                    type = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_openiddict_authorization", x => x.id);
                    table.ForeignKey(
                        name: "fk_openiddict_authorization_openiddict_application_application",
                        column: x => x.application_id,
                        principalSchema: "oid",
                        principalTable: "openiddict_application",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "live_shows",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "uuid_generate_v4()"),
                    title = table.Column<string>(type: "text", nullable: true),
                    description = table.Column<string>(type: "text", nullable: true),
                    start_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    is_finished = table.Column<bool>(type: "boolean", nullable: false),
                    user_id = table.Column<string>(type: "text", nullable: true),
                    date_created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    date_updated = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_live_shows", x => x.id);
                    table.ForeignKey(
                        name: "fk_live_shows_users_user_id",
                        column: x => x.user_id,
                        principalSchema: "oid",
                        principalTable: "user",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "mixes",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "uuid_generate_v4()"),
                    slug = table.Column<string>(type: "text", nullable: true),
                    title = table.Column<string>(type: "text", nullable: false),
                    description = table.Column<string>(type: "text", nullable: false),
                    image = table.Column<string>(type: "text", nullable: true),
                    audio_url = table.Column<string>(type: "text", nullable: true),
                    is_processed = table.Column<bool>(type: "boolean", nullable: false),
                    user_id = table.Column<string>(type: "text", nullable: false),
                    date_created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    date_updated = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_mixes", x => x.id);
                    table.ForeignKey(
                        name: "fk_mixes_users_user_id",
                        column: x => x.user_id,
                        principalSchema: "oid",
                        principalTable: "user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "user_claim",
                schema: "oid",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    user_id = table.Column<string>(type: "text", nullable: false),
                    claim_type = table.Column<string>(type: "text", nullable: true),
                    claim_value = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_user_claim", x => x.id);
                    table.ForeignKey(
                        name: "fk_user_claim_user_user_id",
                        column: x => x.user_id,
                        principalSchema: "oid",
                        principalTable: "user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "user_followers",
                schema: "oid",
                columns: table => new
                {
                    followers_id = table.Column<string>(type: "text", nullable: false),
                    following_id = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_user_followers", x => new { x.followers_id, x.following_id });
                    table.ForeignKey(
                        name: "fk_user_followers_user_followers_id",
                        column: x => x.followers_id,
                        principalSchema: "oid",
                        principalTable: "user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_user_followers_user_following_id",
                        column: x => x.following_id,
                        principalSchema: "oid",
                        principalTable: "user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "user_login",
                schema: "oid",
                columns: table => new
                {
                    login_provider = table.Column<string>(type: "text", nullable: false),
                    provider_key = table.Column<string>(type: "text", nullable: false),
                    provider_display_name = table.Column<string>(type: "text", nullable: true),
                    user_id = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_user_login", x => new { x.login_provider, x.provider_key });
                    table.ForeignKey(
                        name: "fk_user_login_user_user_id",
                        column: x => x.user_id,
                        principalSchema: "oid",
                        principalTable: "user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "user_token",
                schema: "oid",
                columns: table => new
                {
                    user_id = table.Column<string>(type: "text", nullable: false),
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
                        principalSchema: "oid",
                        principalTable: "user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "role_claim",
                schema: "oid",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    role_id = table.Column<string>(type: "text", nullable: false),
                    claim_type = table.Column<string>(type: "text", nullable: true),
                    claim_value = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_role_claim", x => x.id);
                    table.ForeignKey(
                        name: "fk_role_claim_user_role_role_id",
                        column: x => x.role_id,
                        principalSchema: "oid",
                        principalTable: "user_role",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "user_identity_role",
                schema: "oid",
                columns: table => new
                {
                    user_id = table.Column<string>(type: "text", nullable: false),
                    role_id = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_user_identity_role", x => new { x.user_id, x.role_id });
                    table.ForeignKey(
                        name: "fk_user_identity_role_user_role_role_id",
                        column: x => x.role_id,
                        principalSchema: "oid",
                        principalTable: "user_role",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_user_identity_role_user_user_id",
                        column: x => x.user_id,
                        principalSchema: "oid",
                        principalTable: "user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "openiddict_token",
                schema: "oid",
                columns: table => new
                {
                    id = table.Column<string>(type: "text", nullable: false),
                    application_id = table.Column<string>(type: "text", nullable: true),
                    authorization_id = table.Column<string>(type: "text", nullable: true),
                    concurrency_token = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    creation_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    expiration_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    payload = table.Column<string>(type: "text", nullable: true),
                    properties = table.Column<string>(type: "text", nullable: true),
                    redemption_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    reference_id = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    status = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    subject = table.Column<string>(type: "character varying(400)", maxLength: 400, nullable: true),
                    type = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_openiddict_token", x => x.id);
                    table.ForeignKey(
                        name: "fk_openiddict_token_openiddict_application_application_id",
                        column: x => x.application_id,
                        principalSchema: "oid",
                        principalTable: "openiddict_application",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_openiddict_token_openiddict_authorization_authorization_id",
                        column: x => x.authorization_id,
                        principalSchema: "oid",
                        principalTable: "openiddict_authorization",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "live_show_tag",
                columns: table => new
                {
                    live_shows_id = table.Column<Guid>(type: "uuid", nullable: false),
                    tags_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_live_show_tag", x => new { x.live_shows_id, x.tags_id });
                    table.ForeignKey(
                        name: "fk_live_show_tag_live_shows_live_shows_id",
                        column: x => x.live_shows_id,
                        principalTable: "live_shows",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_live_show_tag_tags_tags_id",
                        column: x => x.tags_id,
                        principalTable: "tags",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "show_chat",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "uuid_generate_v4()"),
                    from_user_id = table.Column<string>(type: "text", nullable: true),
                    to_user_id = table.Column<string>(type: "text", nullable: true),
                    date_sent = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    show_id = table.Column<Guid>(type: "uuid", nullable: false),
                    date_created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    date_updated = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_show_chat", x => x.id);
                    table.ForeignKey(
                        name: "fk_show_chat_live_shows_show_id",
                        column: x => x.show_id,
                        principalTable: "live_shows",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_show_chat_users_from_user_id",
                        column: x => x.from_user_id,
                        principalSchema: "oid",
                        principalTable: "user",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_show_chat_users_to_user_id",
                        column: x => x.to_user_id,
                        principalSchema: "oid",
                        principalTable: "user",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "mix_plays",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "uuid_generate_v4()"),
                    mix_id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<string>(type: "text", nullable: true),
                    date_created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    date_updated = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_mix_plays", x => x.id);
                    table.ForeignKey(
                        name: "fk_mix_plays_mixes_mix_id",
                        column: x => x.mix_id,
                        principalTable: "mixes",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_mix_plays_users_user_id",
                        column: x => x.user_id,
                        principalSchema: "oid",
                        principalTable: "user",
                        principalColumn: "id");
                });

            migrationBuilder.CreateIndex(
                name: "ix_live_show_tag_tags_id",
                table: "live_show_tag",
                column: "tags_id");

            migrationBuilder.CreateIndex(
                name: "ix_live_shows_user_id",
                table: "live_shows",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "ix_mix_plays_mix_id",
                table: "mix_plays",
                column: "mix_id");

            migrationBuilder.CreateIndex(
                name: "ix_mix_plays_user_id",
                table: "mix_plays",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "ix_mixes_slug",
                table: "mixes",
                column: "slug",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_mixes_user_id",
                table: "mixes",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "ix_openiddict_application_client_id",
                schema: "oid",
                table: "openiddict_application",
                column: "client_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_openiddict_authorization_application_id_status_subject_type",
                schema: "oid",
                table: "openiddict_authorization",
                columns: new[] { "application_id", "status", "subject", "type" });

            migrationBuilder.CreateIndex(
                name: "ix_openiddict_scope_name",
                schema: "oid",
                table: "openiddict_scope",
                column: "name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_openiddict_token_application_id_status_subject_type",
                schema: "oid",
                table: "openiddict_token",
                columns: new[] { "application_id", "status", "subject", "type" });

            migrationBuilder.CreateIndex(
                name: "ix_openiddict_token_authorization_id",
                schema: "oid",
                table: "openiddict_token",
                column: "authorization_id");

            migrationBuilder.CreateIndex(
                name: "ix_openiddict_token_reference_id",
                schema: "oid",
                table: "openiddict_token",
                column: "reference_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_role_claim_role_id",
                schema: "oid",
                table: "role_claim",
                column: "role_id");

            migrationBuilder.CreateIndex(
                name: "ix_show_chat_from_user_id",
                table: "show_chat",
                column: "from_user_id");

            migrationBuilder.CreateIndex(
                name: "ix_show_chat_show_id",
                table: "show_chat",
                column: "show_id");

            migrationBuilder.CreateIndex(
                name: "ix_show_chat_to_user_id",
                table: "show_chat",
                column: "to_user_id");

            migrationBuilder.CreateIndex(
                name: "ix_tags_tag_name",
                table: "tags",
                column: "tag_name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                schema: "oid",
                table: "user",
                column: "normalized_email");

            migrationBuilder.CreateIndex(
                name: "ix_user_slug",
                schema: "oid",
                table: "user",
                column: "slug",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                schema: "oid",
                table: "user",
                column: "normalized_user_name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_user_claim_user_id",
                schema: "oid",
                table: "user_claim",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "ix_user_followers_following_id",
                schema: "oid",
                table: "user_followers",
                column: "following_id");

            migrationBuilder.CreateIndex(
                name: "ix_user_identity_role_role_id",
                schema: "oid",
                table: "user_identity_role",
                column: "role_id");

            migrationBuilder.CreateIndex(
                name: "ix_user_login_user_id",
                schema: "oid",
                table: "user_login",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                schema: "oid",
                table: "user_role",
                column: "normalized_name",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "identity_user",
                schema: "oid");

            migrationBuilder.DropTable(
                name: "live_show_tag");

            migrationBuilder.DropTable(
                name: "mix_plays");

            migrationBuilder.DropTable(
                name: "openiddict_scope",
                schema: "oid");

            migrationBuilder.DropTable(
                name: "openiddict_token",
                schema: "oid");

            migrationBuilder.DropTable(
                name: "role_claim",
                schema: "oid");

            migrationBuilder.DropTable(
                name: "show_chat");

            migrationBuilder.DropTable(
                name: "user_claim",
                schema: "oid");

            migrationBuilder.DropTable(
                name: "user_followers",
                schema: "oid");

            migrationBuilder.DropTable(
                name: "user_identity_role",
                schema: "oid");

            migrationBuilder.DropTable(
                name: "user_login",
                schema: "oid");

            migrationBuilder.DropTable(
                name: "user_token",
                schema: "oid");

            migrationBuilder.DropTable(
                name: "tags");

            migrationBuilder.DropTable(
                name: "mixes");

            migrationBuilder.DropTable(
                name: "openiddict_authorization",
                schema: "oid");

            migrationBuilder.DropTable(
                name: "live_shows");

            migrationBuilder.DropTable(
                name: "user_role",
                schema: "oid");

            migrationBuilder.DropTable(
                name: "openiddict_application",
                schema: "oid");

            migrationBuilder.DropTable(
                name: "user",
                schema: "oid");
        }
    }
}
