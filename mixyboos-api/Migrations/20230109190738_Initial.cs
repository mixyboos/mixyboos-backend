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
                name: "auth");

            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:PostgresExtension:uuid-ossp", ",,");

            migrationBuilder.CreateTable(
                name: "identity_user",
                schema: "auth",
                columns: table => new
                {
                    id = table.Column<string>(type: "text", nullable: false),
                    username = table.Column<string>(name: "user_name", type: "text", nullable: true),
                    normalizedusername = table.Column<string>(name: "normalized_user_name", type: "text", nullable: true),
                    email = table.Column<string>(type: "text", nullable: true),
                    normalizedemail = table.Column<string>(name: "normalized_email", type: "text", nullable: true),
                    emailconfirmed = table.Column<bool>(name: "email_confirmed", type: "boolean", nullable: false),
                    passwordhash = table.Column<string>(name: "password_hash", type: "text", nullable: true),
                    securitystamp = table.Column<string>(name: "security_stamp", type: "text", nullable: true),
                    concurrencystamp = table.Column<string>(name: "concurrency_stamp", type: "text", nullable: true),
                    phonenumber = table.Column<string>(name: "phone_number", type: "text", nullable: true),
                    phonenumberconfirmed = table.Column<bool>(name: "phone_number_confirmed", type: "boolean", nullable: false),
                    twofactorenabled = table.Column<bool>(name: "two_factor_enabled", type: "boolean", nullable: false),
                    lockoutend = table.Column<DateTimeOffset>(name: "lockout_end", type: "timestamp with time zone", nullable: true),
                    lockoutenabled = table.Column<bool>(name: "lockout_enabled", type: "boolean", nullable: false),
                    accessfailedcount = table.Column<int>(name: "access_failed_count", type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_identity_user", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "openiddict_application",
                schema: "auth",
                columns: table => new
                {
                    id = table.Column<string>(type: "text", nullable: false),
                    clientid = table.Column<string>(name: "client_id", type: "character varying(100)", maxLength: 100, nullable: true),
                    clientsecret = table.Column<string>(name: "client_secret", type: "text", nullable: true),
                    concurrencytoken = table.Column<string>(name: "concurrency_token", type: "character varying(50)", maxLength: 50, nullable: true),
                    consenttype = table.Column<string>(name: "consent_type", type: "character varying(50)", maxLength: 50, nullable: true),
                    displayname = table.Column<string>(name: "display_name", type: "text", nullable: true),
                    displaynames = table.Column<string>(name: "display_names", type: "text", nullable: true),
                    permissions = table.Column<string>(type: "text", nullable: true),
                    postlogoutredirecturis = table.Column<string>(name: "post_logout_redirect_uris", type: "text", nullable: true),
                    properties = table.Column<string>(type: "text", nullable: true),
                    redirecturis = table.Column<string>(name: "redirect_uris", type: "text", nullable: true),
                    requirements = table.Column<string>(type: "text", nullable: true),
                    type = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_openiddict_application", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "openiddict_scope",
                schema: "auth",
                columns: table => new
                {
                    id = table.Column<string>(type: "text", nullable: false),
                    concurrencytoken = table.Column<string>(name: "concurrency_token", type: "character varying(50)", maxLength: 50, nullable: true),
                    description = table.Column<string>(type: "text", nullable: true),
                    descriptions = table.Column<string>(type: "text", nullable: true),
                    displayname = table.Column<string>(name: "display_name", type: "text", nullable: true),
                    displaynames = table.Column<string>(name: "display_names", type: "text", nullable: true),
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
                    tagname = table.Column<string>(name: "tag_name", type: "text", nullable: true),
                    datecreated = table.Column<DateTime>(name: "date_created", type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    dateupdated = table.Column<DateTime>(name: "date_updated", type: "timestamp with time zone", nullable: false, defaultValueSql: "now()")
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
                    id = table.Column<string>(type: "text", nullable: false),
                    title = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    displayname = table.Column<string>(name: "display_name", type: "character varying(30)", maxLength: 30, nullable: true),
                    profileimage = table.Column<string>(name: "profile_image", type: "text", nullable: true),
                    headerimage = table.Column<string>(name: "header_image", type: "text", nullable: true),
                    slug = table.Column<string>(type: "text", nullable: true),
                    biography = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    streamkey = table.Column<string>(name: "stream_key", type: "text", nullable: true),
                    username = table.Column<string>(name: "user_name", type: "character varying(256)", maxLength: 256, nullable: true),
                    normalizedusername = table.Column<string>(name: "normalized_user_name", type: "character varying(256)", maxLength: 256, nullable: true),
                    email = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    normalizedemail = table.Column<string>(name: "normalized_email", type: "character varying(256)", maxLength: 256, nullable: true),
                    emailconfirmed = table.Column<bool>(name: "email_confirmed", type: "boolean", nullable: false),
                    passwordhash = table.Column<string>(name: "password_hash", type: "text", nullable: true),
                    securitystamp = table.Column<string>(name: "security_stamp", type: "text", nullable: true),
                    concurrencystamp = table.Column<string>(name: "concurrency_stamp", type: "text", nullable: true),
                    phonenumber = table.Column<string>(name: "phone_number", type: "text", nullable: true),
                    phonenumberconfirmed = table.Column<bool>(name: "phone_number_confirmed", type: "boolean", nullable: false),
                    twofactorenabled = table.Column<bool>(name: "two_factor_enabled", type: "boolean", nullable: false),
                    lockoutend = table.Column<DateTimeOffset>(name: "lockout_end", type: "timestamp with time zone", nullable: true),
                    lockoutenabled = table.Column<bool>(name: "lockout_enabled", type: "boolean", nullable: false),
                    accessfailedcount = table.Column<int>(name: "access_failed_count", type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_user", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "user_role",
                schema: "auth",
                columns: table => new
                {
                    id = table.Column<string>(type: "text", nullable: false),
                    name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    normalizedname = table.Column<string>(name: "normalized_name", type: "character varying(256)", maxLength: 256, nullable: true),
                    concurrencystamp = table.Column<string>(name: "concurrency_stamp", type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_user_role", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "openiddict_authorization",
                schema: "auth",
                columns: table => new
                {
                    id = table.Column<string>(type: "text", nullable: false),
                    applicationid = table.Column<string>(name: "application_id", type: "text", nullable: true),
                    concurrencytoken = table.Column<string>(name: "concurrency_token", type: "character varying(50)", maxLength: 50, nullable: true),
                    creationdate = table.Column<DateTime>(name: "creation_date", type: "timestamp with time zone", nullable: true),
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
                        column: x => x.applicationid,
                        principalSchema: "auth",
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
                    startdate = table.Column<DateTime>(name: "start_date", type: "timestamp with time zone", nullable: false),
                    isfinished = table.Column<bool>(name: "is_finished", type: "boolean", nullable: false),
                    userid = table.Column<string>(name: "user_id", type: "text", nullable: true),
                    datecreated = table.Column<DateTime>(name: "date_created", type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    dateupdated = table.Column<DateTime>(name: "date_updated", type: "timestamp with time zone", nullable: false, defaultValueSql: "now()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_live_shows", x => x.id);
                    table.ForeignKey(
                        name: "fk_live_shows_users_user_id",
                        column: x => x.userid,
                        principalSchema: "auth",
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
                    audiourl = table.Column<string>(name: "audio_url", type: "text", nullable: true),
                    userid = table.Column<string>(name: "user_id", type: "text", nullable: false),
                    datecreated = table.Column<DateTime>(name: "date_created", type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    dateupdated = table.Column<DateTime>(name: "date_updated", type: "timestamp with time zone", nullable: false, defaultValueSql: "now()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_mixes", x => x.id);
                    table.ForeignKey(
                        name: "fk_mixes_users_user_id",
                        column: x => x.userid,
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
                    userid = table.Column<string>(name: "user_id", type: "text", nullable: false),
                    claimtype = table.Column<string>(name: "claim_type", type: "text", nullable: true),
                    claimvalue = table.Column<string>(name: "claim_value", type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_user_claim", x => x.id);
                    table.ForeignKey(
                        name: "fk_user_claim_user_user_id",
                        column: x => x.userid,
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
                    followersid = table.Column<string>(name: "followers_id", type: "text", nullable: false),
                    followingid = table.Column<string>(name: "following_id", type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_user_followers", x => new { x.followersid, x.followingid });
                    table.ForeignKey(
                        name: "fk_user_followers_user_followers_id",
                        column: x => x.followersid,
                        principalSchema: "auth",
                        principalTable: "user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_user_followers_user_following_id",
                        column: x => x.followingid,
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
                    loginprovider = table.Column<string>(name: "login_provider", type: "text", nullable: false),
                    providerkey = table.Column<string>(name: "provider_key", type: "text", nullable: false),
                    providerdisplayname = table.Column<string>(name: "provider_display_name", type: "text", nullable: true),
                    userid = table.Column<string>(name: "user_id", type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_user_login", x => new { x.loginprovider, x.providerkey });
                    table.ForeignKey(
                        name: "fk_user_login_user_user_id",
                        column: x => x.userid,
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
                    userid = table.Column<string>(name: "user_id", type: "text", nullable: false),
                    loginprovider = table.Column<string>(name: "login_provider", type: "text", nullable: false),
                    name = table.Column<string>(type: "text", nullable: false),
                    value = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_user_token", x => new { x.userid, x.loginprovider, x.name });
                    table.ForeignKey(
                        name: "fk_user_token_user_user_id",
                        column: x => x.userid,
                        principalSchema: "auth",
                        principalTable: "user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "role_claim",
                schema: "auth",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    roleid = table.Column<string>(name: "role_id", type: "text", nullable: false),
                    claimtype = table.Column<string>(name: "claim_type", type: "text", nullable: true),
                    claimvalue = table.Column<string>(name: "claim_value", type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_role_claim", x => x.id);
                    table.ForeignKey(
                        name: "fk_role_claim_user_role_role_id",
                        column: x => x.roleid,
                        principalSchema: "auth",
                        principalTable: "user_role",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "user_identity_role",
                schema: "auth",
                columns: table => new
                {
                    userid = table.Column<string>(name: "user_id", type: "text", nullable: false),
                    roleid = table.Column<string>(name: "role_id", type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_user_identity_role", x => new { x.userid, x.roleid });
                    table.ForeignKey(
                        name: "fk_user_identity_role_user_role_role_id",
                        column: x => x.roleid,
                        principalSchema: "auth",
                        principalTable: "user_role",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_user_identity_role_user_user_id",
                        column: x => x.userid,
                        principalSchema: "auth",
                        principalTable: "user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "openiddict_token",
                schema: "auth",
                columns: table => new
                {
                    id = table.Column<string>(type: "text", nullable: false),
                    applicationid = table.Column<string>(name: "application_id", type: "text", nullable: true),
                    authorizationid = table.Column<string>(name: "authorization_id", type: "text", nullable: true),
                    concurrencytoken = table.Column<string>(name: "concurrency_token", type: "character varying(50)", maxLength: 50, nullable: true),
                    creationdate = table.Column<DateTime>(name: "creation_date", type: "timestamp with time zone", nullable: true),
                    expirationdate = table.Column<DateTime>(name: "expiration_date", type: "timestamp with time zone", nullable: true),
                    payload = table.Column<string>(type: "text", nullable: true),
                    properties = table.Column<string>(type: "text", nullable: true),
                    redemptiondate = table.Column<DateTime>(name: "redemption_date", type: "timestamp with time zone", nullable: true),
                    referenceid = table.Column<string>(name: "reference_id", type: "character varying(100)", maxLength: 100, nullable: true),
                    status = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    subject = table.Column<string>(type: "character varying(400)", maxLength: 400, nullable: true),
                    type = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_openiddict_token", x => x.id);
                    table.ForeignKey(
                        name: "fk_openiddict_token_openiddict_application_application_id",
                        column: x => x.applicationid,
                        principalSchema: "auth",
                        principalTable: "openiddict_application",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_openiddict_token_openiddict_authorization_authorization_id",
                        column: x => x.authorizationid,
                        principalSchema: "auth",
                        principalTable: "openiddict_authorization",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "live_show_tag",
                columns: table => new
                {
                    liveshowsid = table.Column<Guid>(name: "live_shows_id", type: "uuid", nullable: false),
                    tagsid = table.Column<Guid>(name: "tags_id", type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_live_show_tag", x => new { x.liveshowsid, x.tagsid });
                    table.ForeignKey(
                        name: "fk_live_show_tag_live_shows_live_shows_id",
                        column: x => x.liveshowsid,
                        principalTable: "live_shows",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_live_show_tag_tags_tags_id",
                        column: x => x.tagsid,
                        principalTable: "tags",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "show_chat",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "uuid_generate_v4()"),
                    fromuserid = table.Column<string>(name: "from_user_id", type: "text", nullable: true),
                    touserid = table.Column<string>(name: "to_user_id", type: "text", nullable: true),
                    datesent = table.Column<DateTime>(name: "date_sent", type: "timestamp with time zone", nullable: false),
                    showid = table.Column<Guid>(name: "show_id", type: "uuid", nullable: false),
                    datecreated = table.Column<DateTime>(name: "date_created", type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    dateupdated = table.Column<DateTime>(name: "date_updated", type: "timestamp with time zone", nullable: false, defaultValueSql: "now()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_show_chat", x => x.id);
                    table.ForeignKey(
                        name: "fk_show_chat_live_shows_show_id",
                        column: x => x.showid,
                        principalTable: "live_shows",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_show_chat_users_from_user_id",
                        column: x => x.fromuserid,
                        principalSchema: "auth",
                        principalTable: "user",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_show_chat_users_to_user_id",
                        column: x => x.touserid,
                        principalSchema: "auth",
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
                schema: "auth",
                table: "openiddict_application",
                column: "client_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_openiddict_authorization_application_id_status_subject_type",
                schema: "auth",
                table: "openiddict_authorization",
                columns: new[] { "application_id", "status", "subject", "type" });

            migrationBuilder.CreateIndex(
                name: "ix_openiddict_scope_name",
                schema: "auth",
                table: "openiddict_scope",
                column: "name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_openiddict_token_application_id_status_subject_type",
                schema: "auth",
                table: "openiddict_token",
                columns: new[] { "application_id", "status", "subject", "type" });

            migrationBuilder.CreateIndex(
                name: "ix_openiddict_token_authorization_id",
                schema: "auth",
                table: "openiddict_token",
                column: "authorization_id");

            migrationBuilder.CreateIndex(
                name: "ix_openiddict_token_reference_id",
                schema: "auth",
                table: "openiddict_token",
                column: "reference_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_role_claim_role_id",
                schema: "auth",
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

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                schema: "auth",
                table: "user_role",
                column: "normalized_name",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "identity_user",
                schema: "auth");

            migrationBuilder.DropTable(
                name: "live_show_tag");

            migrationBuilder.DropTable(
                name: "mixes");

            migrationBuilder.DropTable(
                name: "openiddict_scope",
                schema: "auth");

            migrationBuilder.DropTable(
                name: "openiddict_token",
                schema: "auth");

            migrationBuilder.DropTable(
                name: "role_claim",
                schema: "auth");

            migrationBuilder.DropTable(
                name: "show_chat");

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
                name: "tags");

            migrationBuilder.DropTable(
                name: "openiddict_authorization",
                schema: "auth");

            migrationBuilder.DropTable(
                name: "live_shows");

            migrationBuilder.DropTable(
                name: "user_role",
                schema: "auth");

            migrationBuilder.DropTable(
                name: "openiddict_application",
                schema: "auth");

            migrationBuilder.DropTable(
                name: "user",
                schema: "auth");
        }
    }
}
