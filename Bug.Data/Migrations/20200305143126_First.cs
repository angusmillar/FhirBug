using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace Bug.Data.Migrations
{
    public partial class First : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FhirVersion",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false),
                    created = table.Column<DateTime>(nullable: false),
                    updated = table.Column<DateTime>(nullable: false),
                    code = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FhirVersion", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "HttpStatusCode",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    created = table.Column<DateTime>(nullable: false),
                    updated = table.Column<DateTime>(nullable: false),
                    number = table.Column<int>(nullable: false),
                    code = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HttpStatusCode", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "Method",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false),
                    created = table.Column<DateTime>(nullable: false),
                    updated = table.Column<DateTime>(nullable: false),
                    code = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Method", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "ResourceName",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    created = table.Column<DateTime>(nullable: false),
                    updated = table.Column<DateTime>(nullable: false),
                    name = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ResourceName", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "ResourceStore",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    created = table.Column<DateTime>(nullable: false),
                    updated = table.Column<DateTime>(nullable: false),
                    resource_id = table.Column<string>(nullable: false),
                    version_id = table.Column<int>(nullable: false),
                    is_deleted = table.Column<bool>(nullable: false),
                    is_current = table.Column<bool>(nullable: false),
                    last_updated = table.Column<DateTime>(nullable: false),
                    resource_blob = table.Column<byte[]>(nullable: true),
                    fk_resourcename_id = table.Column<int>(nullable: false),
                    fk_fhirversion_id = table.Column<int>(nullable: false),
                    fk_method_id = table.Column<int>(nullable: false),
                    fk_httpstatuscode_id = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ResourceStore", x => x.id);
                    table.ForeignKey(
                        name: "FK_ResourceStore_FhirVersion_fk_fhirversion_id",
                        column: x => x.fk_fhirversion_id,
                        principalTable: "FhirVersion",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ResourceStore_HttpStatusCode_fk_httpstatuscode_id",
                        column: x => x.fk_httpstatuscode_id,
                        principalTable: "HttpStatusCode",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ResourceStore_Method_fk_method_id",
                        column: x => x.fk_method_id,
                        principalTable: "Method",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ResourceStore_ResourceName_fk_resourcename_id",
                        column: x => x.fk_resourcename_id,
                        principalTable: "ResourceName",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "FhirVersion",
                columns: new[] { "id", "code", "created", "updated" },
                values: new object[,]
                {
                    { 0, "Stu3", new DateTime(2020, 3, 5, 14, 31, 21, 450, DateTimeKind.Utc).AddTicks(5506), new DateTime(2020, 3, 5, 14, 31, 21, 450, DateTimeKind.Utc).AddTicks(5506) },
                    { 1, "R4", new DateTime(2020, 3, 5, 14, 31, 21, 450, DateTimeKind.Utc).AddTicks(5506), new DateTime(2020, 3, 5, 14, 31, 21, 450, DateTimeKind.Utc).AddTicks(5506) }
                });

            migrationBuilder.InsertData(
                table: "HttpStatusCode",
                columns: new[] { "id", "code", "created", "number", "updated" },
                values: new object[,]
                {
                    { 34, "Gone", new DateTime(2020, 3, 5, 14, 31, 21, 450, DateTimeKind.Utc).AddTicks(5506), 410, new DateTime(2020, 3, 5, 14, 31, 21, 450, DateTimeKind.Utc).AddTicks(5506) },
                    { 35, "LengthRequired", new DateTime(2020, 3, 5, 14, 31, 21, 450, DateTimeKind.Utc).AddTicks(5506), 411, new DateTime(2020, 3, 5, 14, 31, 21, 450, DateTimeKind.Utc).AddTicks(5506) },
                    { 36, "PreconditionFailed", new DateTime(2020, 3, 5, 14, 31, 21, 450, DateTimeKind.Utc).AddTicks(5506), 412, new DateTime(2020, 3, 5, 14, 31, 21, 450, DateTimeKind.Utc).AddTicks(5506) },
                    { 37, "RequestEntityTooLarge", new DateTime(2020, 3, 5, 14, 31, 21, 450, DateTimeKind.Utc).AddTicks(5506), 413, new DateTime(2020, 3, 5, 14, 31, 21, 450, DateTimeKind.Utc).AddTicks(5506) },
                    { 38, "RequestUriTooLong", new DateTime(2020, 3, 5, 14, 31, 21, 450, DateTimeKind.Utc).AddTicks(5506), 414, new DateTime(2020, 3, 5, 14, 31, 21, 450, DateTimeKind.Utc).AddTicks(5506) },
                    { 39, "UnsupportedMediaType", new DateTime(2020, 3, 5, 14, 31, 21, 450, DateTimeKind.Utc).AddTicks(5506), 415, new DateTime(2020, 3, 5, 14, 31, 21, 450, DateTimeKind.Utc).AddTicks(5506) },
                    { 40, "RequestedRangeNotSatisfiable", new DateTime(2020, 3, 5, 14, 31, 21, 450, DateTimeKind.Utc).AddTicks(5506), 416, new DateTime(2020, 3, 5, 14, 31, 21, 450, DateTimeKind.Utc).AddTicks(5506) },
                    { 41, "ExpectationFailed", new DateTime(2020, 3, 5, 14, 31, 21, 450, DateTimeKind.Utc).AddTicks(5506), 417, new DateTime(2020, 3, 5, 14, 31, 21, 450, DateTimeKind.Utc).AddTicks(5506) },
                    { 42, "MisdirectedRequest", new DateTime(2020, 3, 5, 14, 31, 21, 450, DateTimeKind.Utc).AddTicks(5506), 421, new DateTime(2020, 3, 5, 14, 31, 21, 450, DateTimeKind.Utc).AddTicks(5506) },
                    { 43, "UnprocessableEntity", new DateTime(2020, 3, 5, 14, 31, 21, 450, DateTimeKind.Utc).AddTicks(5506), 422, new DateTime(2020, 3, 5, 14, 31, 21, 450, DateTimeKind.Utc).AddTicks(5506) },
                    { 44, "Locked", new DateTime(2020, 3, 5, 14, 31, 21, 450, DateTimeKind.Utc).AddTicks(5506), 423, new DateTime(2020, 3, 5, 14, 31, 21, 450, DateTimeKind.Utc).AddTicks(5506) },
                    { 45, "FailedDependency", new DateTime(2020, 3, 5, 14, 31, 21, 450, DateTimeKind.Utc).AddTicks(5506), 424, new DateTime(2020, 3, 5, 14, 31, 21, 450, DateTimeKind.Utc).AddTicks(5506) },
                    { 46, "UpgradeRequired", new DateTime(2020, 3, 5, 14, 31, 21, 450, DateTimeKind.Utc).AddTicks(5506), 426, new DateTime(2020, 3, 5, 14, 31, 21, 450, DateTimeKind.Utc).AddTicks(5506) },
                    { 47, "PreconditionRequired", new DateTime(2020, 3, 5, 14, 31, 21, 450, DateTimeKind.Utc).AddTicks(5506), 428, new DateTime(2020, 3, 5, 14, 31, 21, 450, DateTimeKind.Utc).AddTicks(5506) },
                    { 48, "TooManyRequests", new DateTime(2020, 3, 5, 14, 31, 21, 450, DateTimeKind.Utc).AddTicks(5506), 429, new DateTime(2020, 3, 5, 14, 31, 21, 450, DateTimeKind.Utc).AddTicks(5506) },
                    { 49, "RequestHeaderFieldsTooLarge", new DateTime(2020, 3, 5, 14, 31, 21, 450, DateTimeKind.Utc).AddTicks(5506), 431, new DateTime(2020, 3, 5, 14, 31, 21, 450, DateTimeKind.Utc).AddTicks(5506) },
                    { 50, "UnavailableForLegalReasons", new DateTime(2020, 3, 5, 14, 31, 21, 450, DateTimeKind.Utc).AddTicks(5506), 451, new DateTime(2020, 3, 5, 14, 31, 21, 450, DateTimeKind.Utc).AddTicks(5506) },
                    { 51, "InternalServerError", new DateTime(2020, 3, 5, 14, 31, 21, 450, DateTimeKind.Utc).AddTicks(5506), 500, new DateTime(2020, 3, 5, 14, 31, 21, 450, DateTimeKind.Utc).AddTicks(5506) },
                    { 52, "NotImplemented", new DateTime(2020, 3, 5, 14, 31, 21, 450, DateTimeKind.Utc).AddTicks(5506), 501, new DateTime(2020, 3, 5, 14, 31, 21, 450, DateTimeKind.Utc).AddTicks(5506) },
                    { 53, "BadGateway", new DateTime(2020, 3, 5, 14, 31, 21, 450, DateTimeKind.Utc).AddTicks(5506), 502, new DateTime(2020, 3, 5, 14, 31, 21, 450, DateTimeKind.Utc).AddTicks(5506) },
                    { 54, "ServiceUnavailable", new DateTime(2020, 3, 5, 14, 31, 21, 450, DateTimeKind.Utc).AddTicks(5506), 503, new DateTime(2020, 3, 5, 14, 31, 21, 450, DateTimeKind.Utc).AddTicks(5506) },
                    { 55, "GatewayTimeout", new DateTime(2020, 3, 5, 14, 31, 21, 450, DateTimeKind.Utc).AddTicks(5506), 504, new DateTime(2020, 3, 5, 14, 31, 21, 450, DateTimeKind.Utc).AddTicks(5506) },
                    { 56, "HttpVersionNotSupported", new DateTime(2020, 3, 5, 14, 31, 21, 450, DateTimeKind.Utc).AddTicks(5506), 505, new DateTime(2020, 3, 5, 14, 31, 21, 450, DateTimeKind.Utc).AddTicks(5506) },
                    { 57, "VariantAlsoNegotiates", new DateTime(2020, 3, 5, 14, 31, 21, 450, DateTimeKind.Utc).AddTicks(5506), 506, new DateTime(2020, 3, 5, 14, 31, 21, 450, DateTimeKind.Utc).AddTicks(5506) },
                    { 58, "InsufficientStorage", new DateTime(2020, 3, 5, 14, 31, 21, 450, DateTimeKind.Utc).AddTicks(5506), 507, new DateTime(2020, 3, 5, 14, 31, 21, 450, DateTimeKind.Utc).AddTicks(5506) },
                    { 59, "LoopDetected", new DateTime(2020, 3, 5, 14, 31, 21, 450, DateTimeKind.Utc).AddTicks(5506), 508, new DateTime(2020, 3, 5, 14, 31, 21, 450, DateTimeKind.Utc).AddTicks(5506) },
                    { 60, "NotExtended", new DateTime(2020, 3, 5, 14, 31, 21, 450, DateTimeKind.Utc).AddTicks(5506), 510, new DateTime(2020, 3, 5, 14, 31, 21, 450, DateTimeKind.Utc).AddTicks(5506) },
                    { 61, "NetworkAuthenticationRequired", new DateTime(2020, 3, 5, 14, 31, 21, 450, DateTimeKind.Utc).AddTicks(5506), 511, new DateTime(2020, 3, 5, 14, 31, 21, 450, DateTimeKind.Utc).AddTicks(5506) },
                    { 32, "RequestTimeout", new DateTime(2020, 3, 5, 14, 31, 21, 450, DateTimeKind.Utc).AddTicks(5506), 408, new DateTime(2020, 3, 5, 14, 31, 21, 450, DateTimeKind.Utc).AddTicks(5506) },
                    { 31, "ProxyAuthenticationRequired", new DateTime(2020, 3, 5, 14, 31, 21, 450, DateTimeKind.Utc).AddTicks(5506), 407, new DateTime(2020, 3, 5, 14, 31, 21, 450, DateTimeKind.Utc).AddTicks(5506) },
                    { 33, "Conflict", new DateTime(2020, 3, 5, 14, 31, 21, 450, DateTimeKind.Utc).AddTicks(5506), 409, new DateTime(2020, 3, 5, 14, 31, 21, 450, DateTimeKind.Utc).AddTicks(5506) },
                    { 29, "MethodNotAllowed", new DateTime(2020, 3, 5, 14, 31, 21, 450, DateTimeKind.Utc).AddTicks(5506), 405, new DateTime(2020, 3, 5, 14, 31, 21, 450, DateTimeKind.Utc).AddTicks(5506) },
                    { 1, "Continue", new DateTime(2020, 3, 5, 14, 31, 21, 450, DateTimeKind.Utc).AddTicks(5506), 100, new DateTime(2020, 3, 5, 14, 31, 21, 450, DateTimeKind.Utc).AddTicks(5506) },
                    { 2, "SwitchingProtocols", new DateTime(2020, 3, 5, 14, 31, 21, 450, DateTimeKind.Utc).AddTicks(5506), 101, new DateTime(2020, 3, 5, 14, 31, 21, 450, DateTimeKind.Utc).AddTicks(5506) },
                    { 3, "Processing", new DateTime(2020, 3, 5, 14, 31, 21, 450, DateTimeKind.Utc).AddTicks(5506), 102, new DateTime(2020, 3, 5, 14, 31, 21, 450, DateTimeKind.Utc).AddTicks(5506) },
                    { 4, "EarlyHints", new DateTime(2020, 3, 5, 14, 31, 21, 450, DateTimeKind.Utc).AddTicks(5506), 103, new DateTime(2020, 3, 5, 14, 31, 21, 450, DateTimeKind.Utc).AddTicks(5506) },
                    { 5, "OK", new DateTime(2020, 3, 5, 14, 31, 21, 450, DateTimeKind.Utc).AddTicks(5506), 200, new DateTime(2020, 3, 5, 14, 31, 21, 450, DateTimeKind.Utc).AddTicks(5506) },
                    { 6, "Created", new DateTime(2020, 3, 5, 14, 31, 21, 450, DateTimeKind.Utc).AddTicks(5506), 201, new DateTime(2020, 3, 5, 14, 31, 21, 450, DateTimeKind.Utc).AddTicks(5506) },
                    { 7, "Accepted", new DateTime(2020, 3, 5, 14, 31, 21, 450, DateTimeKind.Utc).AddTicks(5506), 202, new DateTime(2020, 3, 5, 14, 31, 21, 450, DateTimeKind.Utc).AddTicks(5506) },
                    { 8, "NonAuthoritativeInformation", new DateTime(2020, 3, 5, 14, 31, 21, 450, DateTimeKind.Utc).AddTicks(5506), 203, new DateTime(2020, 3, 5, 14, 31, 21, 450, DateTimeKind.Utc).AddTicks(5506) },
                    { 9, "NoContent", new DateTime(2020, 3, 5, 14, 31, 21, 450, DateTimeKind.Utc).AddTicks(5506), 204, new DateTime(2020, 3, 5, 14, 31, 21, 450, DateTimeKind.Utc).AddTicks(5506) },
                    { 10, "ResetContent", new DateTime(2020, 3, 5, 14, 31, 21, 450, DateTimeKind.Utc).AddTicks(5506), 205, new DateTime(2020, 3, 5, 14, 31, 21, 450, DateTimeKind.Utc).AddTicks(5506) },
                    { 11, "PartialContent", new DateTime(2020, 3, 5, 14, 31, 21, 450, DateTimeKind.Utc).AddTicks(5506), 206, new DateTime(2020, 3, 5, 14, 31, 21, 450, DateTimeKind.Utc).AddTicks(5506) },
                    { 12, "MultiStatus", new DateTime(2020, 3, 5, 14, 31, 21, 450, DateTimeKind.Utc).AddTicks(5506), 207, new DateTime(2020, 3, 5, 14, 31, 21, 450, DateTimeKind.Utc).AddTicks(5506) },
                    { 30, "NotAcceptable", new DateTime(2020, 3, 5, 14, 31, 21, 450, DateTimeKind.Utc).AddTicks(5506), 406, new DateTime(2020, 3, 5, 14, 31, 21, 450, DateTimeKind.Utc).AddTicks(5506) },
                    { 14, "IMUsed", new DateTime(2020, 3, 5, 14, 31, 21, 450, DateTimeKind.Utc).AddTicks(5506), 226, new DateTime(2020, 3, 5, 14, 31, 21, 450, DateTimeKind.Utc).AddTicks(5506) },
                    { 13, "AlreadyReported", new DateTime(2020, 3, 5, 14, 31, 21, 450, DateTimeKind.Utc).AddTicks(5506), 208, new DateTime(2020, 3, 5, 14, 31, 21, 450, DateTimeKind.Utc).AddTicks(5506) },
                    { 16, "Moved", new DateTime(2020, 3, 5, 14, 31, 21, 450, DateTimeKind.Utc).AddTicks(5506), 301, new DateTime(2020, 3, 5, 14, 31, 21, 450, DateTimeKind.Utc).AddTicks(5506) },
                    { 28, "NotFound", new DateTime(2020, 3, 5, 14, 31, 21, 450, DateTimeKind.Utc).AddTicks(5506), 404, new DateTime(2020, 3, 5, 14, 31, 21, 450, DateTimeKind.Utc).AddTicks(5506) },
                    { 27, "Forbidden", new DateTime(2020, 3, 5, 14, 31, 21, 450, DateTimeKind.Utc).AddTicks(5506), 403, new DateTime(2020, 3, 5, 14, 31, 21, 450, DateTimeKind.Utc).AddTicks(5506) },
                    { 26, "PaymentRequired", new DateTime(2020, 3, 5, 14, 31, 21, 450, DateTimeKind.Utc).AddTicks(5506), 402, new DateTime(2020, 3, 5, 14, 31, 21, 450, DateTimeKind.Utc).AddTicks(5506) },
                    { 15, "Ambiguous", new DateTime(2020, 3, 5, 14, 31, 21, 450, DateTimeKind.Utc).AddTicks(5506), 300, new DateTime(2020, 3, 5, 14, 31, 21, 450, DateTimeKind.Utc).AddTicks(5506) },
                    { 24, "BadRequest", new DateTime(2020, 3, 5, 14, 31, 21, 450, DateTimeKind.Utc).AddTicks(5506), 400, new DateTime(2020, 3, 5, 14, 31, 21, 450, DateTimeKind.Utc).AddTicks(5506) },
                    { 23, "PermanentRedirect", new DateTime(2020, 3, 5, 14, 31, 21, 450, DateTimeKind.Utc).AddTicks(5506), 308, new DateTime(2020, 3, 5, 14, 31, 21, 450, DateTimeKind.Utc).AddTicks(5506) },
                    { 25, "Unauthorized", new DateTime(2020, 3, 5, 14, 31, 21, 450, DateTimeKind.Utc).AddTicks(5506), 401, new DateTime(2020, 3, 5, 14, 31, 21, 450, DateTimeKind.Utc).AddTicks(5506) },
                    { 21, "Unused", new DateTime(2020, 3, 5, 14, 31, 21, 450, DateTimeKind.Utc).AddTicks(5506), 306, new DateTime(2020, 3, 5, 14, 31, 21, 450, DateTimeKind.Utc).AddTicks(5506) },
                    { 20, "UseProxy", new DateTime(2020, 3, 5, 14, 31, 21, 450, DateTimeKind.Utc).AddTicks(5506), 305, new DateTime(2020, 3, 5, 14, 31, 21, 450, DateTimeKind.Utc).AddTicks(5506) },
                    { 19, "NotModified", new DateTime(2020, 3, 5, 14, 31, 21, 450, DateTimeKind.Utc).AddTicks(5506), 304, new DateTime(2020, 3, 5, 14, 31, 21, 450, DateTimeKind.Utc).AddTicks(5506) },
                    { 18, "RedirectMethod", new DateTime(2020, 3, 5, 14, 31, 21, 450, DateTimeKind.Utc).AddTicks(5506), 303, new DateTime(2020, 3, 5, 14, 31, 21, 450, DateTimeKind.Utc).AddTicks(5506) },
                    { 17, "Redirect", new DateTime(2020, 3, 5, 14, 31, 21, 450, DateTimeKind.Utc).AddTicks(5506), 302, new DateTime(2020, 3, 5, 14, 31, 21, 450, DateTimeKind.Utc).AddTicks(5506) },
                    { 22, "TemporaryRedirect", new DateTime(2020, 3, 5, 14, 31, 21, 450, DateTimeKind.Utc).AddTicks(5506), 307, new DateTime(2020, 3, 5, 14, 31, 21, 450, DateTimeKind.Utc).AddTicks(5506) }
                });

            migrationBuilder.InsertData(
                table: "Method",
                columns: new[] { "id", "code", "created", "updated" },
                values: new object[,]
                {
                    { 1, "POST", new DateTime(2020, 3, 5, 14, 31, 21, 450, DateTimeKind.Utc).AddTicks(5506), new DateTime(2020, 3, 5, 14, 31, 21, 450, DateTimeKind.Utc).AddTicks(5506) },
                    { 4, "DELETE", new DateTime(2020, 3, 5, 14, 31, 21, 450, DateTimeKind.Utc).AddTicks(5506), new DateTime(2020, 3, 5, 14, 31, 21, 450, DateTimeKind.Utc).AddTicks(5506) },
                    { 3, "GET", new DateTime(2020, 3, 5, 14, 31, 21, 450, DateTimeKind.Utc).AddTicks(5506), new DateTime(2020, 3, 5, 14, 31, 21, 450, DateTimeKind.Utc).AddTicks(5506) },
                    { 5, "HEAD", new DateTime(2020, 3, 5, 14, 31, 21, 450, DateTimeKind.Utc).AddTicks(5506), new DateTime(2020, 3, 5, 14, 31, 21, 450, DateTimeKind.Utc).AddTicks(5506) },
                    { 6, "PATCH", new DateTime(2020, 3, 5, 14, 31, 21, 450, DateTimeKind.Utc).AddTicks(5506), new DateTime(2020, 3, 5, 14, 31, 21, 450, DateTimeKind.Utc).AddTicks(5506) },
                    { 2, "PUT", new DateTime(2020, 3, 5, 14, 31, 21, 450, DateTimeKind.Utc).AddTicks(5506), new DateTime(2020, 3, 5, 14, 31, 21, 450, DateTimeKind.Utc).AddTicks(5506) }
                });

            migrationBuilder.CreateIndex(
                name: "Unique_Number",
                table: "HttpStatusCode",
                column: "number",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ResourceStore_fk_httpstatuscode_id",
                table: "ResourceStore",
                column: "fk_httpstatuscode_id");

            migrationBuilder.CreateIndex(
                name: "IX_ResourceStore_fk_method_id",
                table: "ResourceStore",
                column: "fk_method_id");

            migrationBuilder.CreateIndex(
                name: "IX_ResourceStore_fk_resourcename_id",
                table: "ResourceStore",
                column: "fk_resourcename_id");

            migrationBuilder.CreateIndex(
                name: "Unique_FhirVer_ResName_ResId_ResVer",
                table: "ResourceStore",
                columns: new[] { "fk_fhirversion_id", "fk_resourcename_id", "resource_id", "version_id" },
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ResourceStore");

            migrationBuilder.DropTable(
                name: "FhirVersion");

            migrationBuilder.DropTable(
                name: "HttpStatusCode");

            migrationBuilder.DropTable(
                name: "Method");

            migrationBuilder.DropTable(
                name: "ResourceName");
        }
    }
}
