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

            migrationBuilder.EnsureSchema(
                name: "mixyboos");

            migrationBuilder.CreateTable(
                name: "identity_user",
                schema: "oid",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserName = table.Column<string>(type: "text", nullable: true),
                    NormalizedUserName = table.Column<string>(type: "text", nullable: true),
                    Email = table.Column<string>(type: "text", nullable: true),
                    NormalizedEmail = table.Column<string>(type: "text", nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "boolean", nullable: false),
                    PasswordHash = table.Column<string>(type: "text", nullable: true),
                    SecurityStamp = table.Column<string>(type: "text", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "text", nullable: true),
                    PhoneNumber = table.Column<string>(type: "text", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "boolean", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "boolean", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "boolean", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_identity_user", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "identity_user_base",
                schema: "oid",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    UserName = table.Column<string>(type: "text", nullable: true),
                    NormalizedUserName = table.Column<string>(type: "text", nullable: true),
                    Email = table.Column<string>(type: "text", nullable: true),
                    NormalizedEmail = table.Column<string>(type: "text", nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "boolean", nullable: false),
                    PasswordHash = table.Column<string>(type: "text", nullable: true),
                    SecurityStamp = table.Column<string>(type: "text", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "text", nullable: true),
                    PhoneNumber = table.Column<string>(type: "text", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "boolean", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "boolean", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "boolean", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_identity_user_base", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "openiddict_application",
                schema: "oid",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    ClientId = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    ClientSecret = table.Column<string>(type: "text", nullable: true),
                    ConcurrencyToken = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    ConsentType = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    DisplayName = table.Column<string>(type: "text", nullable: true),
                    DisplayNames = table.Column<string>(type: "text", nullable: true),
                    Permissions = table.Column<string>(type: "text", nullable: true),
                    PostLogoutRedirectUris = table.Column<string>(type: "text", nullable: true),
                    Properties = table.Column<string>(type: "text", nullable: true),
                    RedirectUris = table.Column<string>(type: "text", nullable: true),
                    Requirements = table.Column<string>(type: "text", nullable: true),
                    Type = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_openiddict_application", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "openiddict_scope",
                schema: "oid",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    ConcurrencyToken = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    Description = table.Column<string>(type: "text", nullable: true),
                    Descriptions = table.Column<string>(type: "text", nullable: true),
                    DisplayName = table.Column<string>(type: "text", nullable: true),
                    DisplayNames = table.Column<string>(type: "text", nullable: true),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    Properties = table.Column<string>(type: "text", nullable: true),
                    Resources = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_openiddict_scope", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "tags",
                schema: "mixyboos",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(36)", nullable: false),
                    TagName = table.Column<string>(type: "text", nullable: true),
                    DateCreated = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    DateUpdated = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tags", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "user",
                schema: "oid",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Title = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    DisplayName = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    ProfileImage = table.Column<string>(type: "text", nullable: true),
                    HeaderImage = table.Column<string>(type: "text", nullable: true),
                    Slug = table.Column<string>(type: "text", nullable: true),
                    City = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Country = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Biography = table.Column<string>(type: "character varying(2048)", maxLength: 2048, nullable: true),
                    StreamKey = table.Column<string>(type: "text", nullable: true),
                    UserName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    Email = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "boolean", nullable: false),
                    PasswordHash = table.Column<string>(type: "text", nullable: true),
                    SecurityStamp = table.Column<string>(type: "text", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "text", nullable: true),
                    PhoneNumber = table.Column<string>(type: "text", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "boolean", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "boolean", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "boolean", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "user_user_role",
                schema: "oid",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user_user_role", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "openiddict_authorization",
                schema: "oid",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    ApplicationId = table.Column<string>(type: "text", nullable: true),
                    ConcurrencyToken = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    CreationDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Properties = table.Column<string>(type: "text", nullable: true),
                    Scopes = table.Column<string>(type: "text", nullable: true),
                    Status = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    Subject = table.Column<string>(type: "character varying(400)", maxLength: 400, nullable: true),
                    Type = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_openiddict_authorization", x => x.Id);
                    table.ForeignKey(
                        name: "FK_openiddict_authorization_openiddict_application_Application~",
                        column: x => x.ApplicationId,
                        principalSchema: "oid",
                        principalTable: "openiddict_application",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "live_shows",
                schema: "mixyboos",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(36)", nullable: false),
                    Title = table.Column<string>(type: "text", nullable: true),
                    Description = table.Column<string>(type: "text", nullable: true),
                    StartDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: true),
                    DateCreated = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    DateUpdated = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_live_shows", x => x.Id);
                    table.ForeignKey(
                        name: "FK_live_shows_user_UserId",
                        column: x => x.UserId,
                        principalSchema: "oid",
                        principalTable: "user",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "mixes",
                schema: "mixyboos",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(36)", nullable: false),
                    Slug = table.Column<string>(type: "text", nullable: true),
                    Title = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    Image = table.Column<string>(type: "text", nullable: true),
                    AudioUrl = table.Column<string>(type: "text", nullable: true),
                    IsProcessed = table.Column<bool>(type: "boolean", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    DateCreated = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    DateUpdated = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_mixes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_mixes_user_UserId",
                        column: x => x.UserId,
                        principalSchema: "oid",
                        principalTable: "user",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "user_claim",
                schema: "oid",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    ClaimType = table.Column<string>(type: "text", nullable: true),
                    ClaimValue = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user_claim", x => x.Id);
                    table.ForeignKey(
                        name: "FK_user_claim_user_UserId",
                        column: x => x.UserId,
                        principalSchema: "oid",
                        principalTable: "user",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "user_followers",
                schema: "oid",
                columns: table => new
                {
                    FollowersId = table.Column<Guid>(type: "uuid", nullable: false),
                    FollowingId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user_followers", x => new { x.FollowersId, x.FollowingId });
                    table.ForeignKey(
                        name: "FK_user_followers_user_FollowersId",
                        column: x => x.FollowersId,
                        principalSchema: "oid",
                        principalTable: "user",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_user_followers_user_FollowingId",
                        column: x => x.FollowingId,
                        principalSchema: "oid",
                        principalTable: "user",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "user_login",
                schema: "oid",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "text", nullable: false),
                    ProviderKey = table.Column<string>(type: "text", nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "text", nullable: true),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user_login", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_user_login_user_UserId",
                        column: x => x.UserId,
                        principalSchema: "oid",
                        principalTable: "user",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "user_token",
                schema: "oid",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    LoginProvider = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Value = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user_token", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_user_token_user_UserId",
                        column: x => x.UserId,
                        principalSchema: "oid",
                        principalTable: "user",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "role_claim",
                schema: "oid",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    RoleId = table.Column<Guid>(type: "uuid", nullable: false),
                    ClaimType = table.Column<string>(type: "text", nullable: true),
                    ClaimValue = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_role_claim", x => x.Id);
                    table.ForeignKey(
                        name: "FK_role_claim_user_user_role_RoleId",
                        column: x => x.RoleId,
                        principalSchema: "oid",
                        principalTable: "user_user_role",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "user_identity_role",
                schema: "oid",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    RoleId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user_identity_role", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_user_identity_role_user_UserId",
                        column: x => x.UserId,
                        principalSchema: "oid",
                        principalTable: "user",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_user_identity_role_user_user_role_RoleId",
                        column: x => x.RoleId,
                        principalSchema: "oid",
                        principalTable: "user_user_role",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "openiddict_token",
                schema: "oid",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    ApplicationId = table.Column<string>(type: "text", nullable: true),
                    AuthorizationId = table.Column<string>(type: "text", nullable: true),
                    ConcurrencyToken = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    CreationDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ExpirationDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Payload = table.Column<string>(type: "text", nullable: true),
                    Properties = table.Column<string>(type: "text", nullable: true),
                    RedemptionDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ReferenceId = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Status = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    Subject = table.Column<string>(type: "character varying(400)", maxLength: 400, nullable: true),
                    Type = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_openiddict_token", x => x.Id);
                    table.ForeignKey(
                        name: "FK_openiddict_token_openiddict_application_ApplicationId",
                        column: x => x.ApplicationId,
                        principalSchema: "oid",
                        principalTable: "openiddict_application",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_openiddict_token_openiddict_authorization_AuthorizationId",
                        column: x => x.AuthorizationId,
                        principalSchema: "oid",
                        principalTable: "openiddict_authorization",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "show_chats",
                schema: "mixyboos",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(36)", nullable: false),
                    FromUserId = table.Column<Guid>(type: "uuid", nullable: false),
                    ToUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    DateSent = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Message = table.Column<string>(type: "text", nullable: false),
                    ShowId = table.Column<string>(type: "character varying(36)", nullable: false),
                    DateCreated = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    DateUpdated = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_show_chats", x => x.Id);
                    table.ForeignKey(
                        name: "FK_show_chats_live_shows_ShowId",
                        column: x => x.ShowId,
                        principalSchema: "mixyboos",
                        principalTable: "live_shows",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_show_chats_user_FromUserId",
                        column: x => x.FromUserId,
                        principalSchema: "oid",
                        principalTable: "user",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_show_chats_user_ToUserId",
                        column: x => x.ToUserId,
                        principalSchema: "oid",
                        principalTable: "user",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "show_tags",
                schema: "mixyboos",
                columns: table => new
                {
                    LiveShowId = table.Column<string>(type: "character varying(36)", nullable: false),
                    TagsId = table.Column<string>(type: "character varying(36)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_show_tags", x => new { x.LiveShowId, x.TagsId });
                    table.ForeignKey(
                        name: "FK_show_tags_live_shows_LiveShowId",
                        column: x => x.LiveShowId,
                        principalSchema: "mixyboos",
                        principalTable: "live_shows",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_show_tags_tags_TagsId",
                        column: x => x.TagsId,
                        principalSchema: "mixyboos",
                        principalTable: "tags",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "mix_download",
                schema: "mixyboos",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(36)", nullable: false),
                    DateCreated = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    DateUpdated = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    MixId = table.Column<string>(type: "character varying(36)", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_mix_download", x => x.Id);
                    table.ForeignKey(
                        name: "FK_mix_download_mixes_MixId",
                        column: x => x.MixId,
                        principalSchema: "mixyboos",
                        principalTable: "mixes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_mix_download_user_UserId",
                        column: x => x.UserId,
                        principalSchema: "oid",
                        principalTable: "user",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "mix_likes",
                schema: "mixyboos",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(36)", nullable: false),
                    DateCreated = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    DateUpdated = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    MixId = table.Column<string>(type: "character varying(36)", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_mix_likes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_mix_likes_mixes_MixId",
                        column: x => x.MixId,
                        principalSchema: "mixyboos",
                        principalTable: "mixes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_mix_likes_user_UserId",
                        column: x => x.UserId,
                        principalSchema: "oid",
                        principalTable: "user",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "mix_plays",
                schema: "mixyboos",
                columns: table => new
                {
                    MixId = table.Column<string>(type: "character varying(36)", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    Id = table.Column<string>(type: "character varying(36)", nullable: false),
                    DateCreated = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    DateUpdated = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_mix_plays", x => new { x.MixId, x.UserId });
                    table.ForeignKey(
                        name: "FK_mix_plays_mixes_MixId",
                        column: x => x.MixId,
                        principalSchema: "mixyboos",
                        principalTable: "mixes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_mix_plays_user_UserId",
                        column: x => x.UserId,
                        principalSchema: "oid",
                        principalTable: "user",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "mix_shares",
                schema: "mixyboos",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(36)", nullable: false),
                    DateCreated = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    DateUpdated = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    MixId = table.Column<string>(type: "character varying(36)", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_mix_shares", x => x.Id);
                    table.ForeignKey(
                        name: "FK_mix_shares_mixes_MixId",
                        column: x => x.MixId,
                        principalSchema: "mixyboos",
                        principalTable: "mixes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_mix_shares_user_UserId",
                        column: x => x.UserId,
                        principalSchema: "oid",
                        principalTable: "user",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "mix_tags",
                schema: "mixyboos",
                columns: table => new
                {
                    MixId = table.Column<string>(type: "character varying(36)", nullable: false),
                    TagsId = table.Column<string>(type: "character varying(36)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_mix_tags", x => new { x.MixId, x.TagsId });
                    table.ForeignKey(
                        name: "FK_mix_tags_mixes_MixId",
                        column: x => x.MixId,
                        principalSchema: "mixyboos",
                        principalTable: "mixes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_mix_tags_tags_TagsId",
                        column: x => x.TagsId,
                        principalSchema: "mixyboos",
                        principalTable: "tags",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_live_shows_UserId",
                schema: "mixyboos",
                table: "live_shows",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_mix_download_MixId",
                schema: "mixyboos",
                table: "mix_download",
                column: "MixId");

            migrationBuilder.CreateIndex(
                name: "IX_mix_download_UserId",
                schema: "mixyboos",
                table: "mix_download",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_mix_likes_MixId",
                schema: "mixyboos",
                table: "mix_likes",
                column: "MixId");

            migrationBuilder.CreateIndex(
                name: "IX_mix_likes_UserId",
                schema: "mixyboos",
                table: "mix_likes",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_mix_plays_UserId",
                schema: "mixyboos",
                table: "mix_plays",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_mix_shares_MixId",
                schema: "mixyboos",
                table: "mix_shares",
                column: "MixId");

            migrationBuilder.CreateIndex(
                name: "IX_mix_shares_UserId",
                schema: "mixyboos",
                table: "mix_shares",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_mix_tags_TagsId",
                schema: "mixyboos",
                table: "mix_tags",
                column: "TagsId");

            migrationBuilder.CreateIndex(
                name: "IX_mixes_Slug",
                schema: "mixyboos",
                table: "mixes",
                column: "Slug",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_mixes_UserId",
                schema: "mixyboos",
                table: "mixes",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_openiddict_application_ClientId",
                schema: "oid",
                table: "openiddict_application",
                column: "ClientId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_openiddict_authorization_ApplicationId_Status_Subject_Type",
                schema: "oid",
                table: "openiddict_authorization",
                columns: new[] { "ApplicationId", "Status", "Subject", "Type" });

            migrationBuilder.CreateIndex(
                name: "IX_openiddict_scope_Name",
                schema: "oid",
                table: "openiddict_scope",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_openiddict_token_ApplicationId_Status_Subject_Type",
                schema: "oid",
                table: "openiddict_token",
                columns: new[] { "ApplicationId", "Status", "Subject", "Type" });

            migrationBuilder.CreateIndex(
                name: "IX_openiddict_token_AuthorizationId",
                schema: "oid",
                table: "openiddict_token",
                column: "AuthorizationId");

            migrationBuilder.CreateIndex(
                name: "IX_openiddict_token_ReferenceId",
                schema: "oid",
                table: "openiddict_token",
                column: "ReferenceId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_role_claim_RoleId",
                schema: "oid",
                table: "role_claim",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_show_chats_FromUserId",
                schema: "mixyboos",
                table: "show_chats",
                column: "FromUserId");

            migrationBuilder.CreateIndex(
                name: "IX_show_chats_ShowId",
                schema: "mixyboos",
                table: "show_chats",
                column: "ShowId");

            migrationBuilder.CreateIndex(
                name: "IX_show_chats_ToUserId",
                schema: "mixyboos",
                table: "show_chats",
                column: "ToUserId");

            migrationBuilder.CreateIndex(
                name: "IX_show_tags_TagsId",
                schema: "mixyboos",
                table: "show_tags",
                column: "TagsId");

            migrationBuilder.CreateIndex(
                name: "IX_tags_TagName",
                schema: "mixyboos",
                table: "tags",
                column: "TagName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                schema: "oid",
                table: "user",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "IX_user_Slug",
                schema: "oid",
                table: "user",
                column: "Slug",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                schema: "oid",
                table: "user",
                column: "NormalizedUserName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_user_claim_UserId",
                schema: "oid",
                table: "user_claim",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_user_followers_FollowingId",
                schema: "oid",
                table: "user_followers",
                column: "FollowingId");

            migrationBuilder.CreateIndex(
                name: "IX_user_identity_role_RoleId",
                schema: "oid",
                table: "user_identity_role",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_user_login_UserId",
                schema: "oid",
                table: "user_login",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                schema: "oid",
                table: "user_user_role",
                column: "NormalizedName",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "identity_user",
                schema: "oid");

            migrationBuilder.DropTable(
                name: "identity_user_base",
                schema: "oid");

            migrationBuilder.DropTable(
                name: "mix_download",
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
                name: "openiddict_scope",
                schema: "oid");

            migrationBuilder.DropTable(
                name: "openiddict_token",
                schema: "oid");

            migrationBuilder.DropTable(
                name: "role_claim",
                schema: "oid");

            migrationBuilder.DropTable(
                name: "show_chats",
                schema: "mixyboos");

            migrationBuilder.DropTable(
                name: "show_tags",
                schema: "mixyboos");

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
                name: "mixes",
                schema: "mixyboos");

            migrationBuilder.DropTable(
                name: "openiddict_authorization",
                schema: "oid");

            migrationBuilder.DropTable(
                name: "live_shows",
                schema: "mixyboos");

            migrationBuilder.DropTable(
                name: "tags",
                schema: "mixyboos");

            migrationBuilder.DropTable(
                name: "user_user_role",
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
