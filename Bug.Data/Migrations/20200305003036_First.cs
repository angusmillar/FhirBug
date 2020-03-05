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
                    id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Created = table.Column<DateTime>(nullable: false),
                    Updated = table.Column<DateTime>(nullable: false),
                    code = table.Column<string>(nullable: false),
                    fhir_major_version = table.Column<int>(nullable: false)
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
                columns: new[] { "id", "code", "Created", "fhir_major_version", "Updated" },
                values: new object[,]
                {
                    { 1, "Stu3", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 0, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 2, "R4", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) }
                });

            migrationBuilder.InsertData(
                table: "HttpStatusCode",
                columns: new[] { "id", "code", "created", "number", "updated" },
                values: new object[,]
                {
                    { 34, "Gone", new DateTime(2020, 3, 5, 0, 30, 35, 396, DateTimeKind.Utc).AddTicks(8776), 410, new DateTime(2020, 3, 5, 0, 30, 35, 396, DateTimeKind.Utc).AddTicks(8776) },
                    { 35, "LengthRequired", new DateTime(2020, 3, 5, 0, 30, 35, 396, DateTimeKind.Utc).AddTicks(8776), 411, new DateTime(2020, 3, 5, 0, 30, 35, 396, DateTimeKind.Utc).AddTicks(8776) },
                    { 36, "PreconditionFailed", new DateTime(2020, 3, 5, 0, 30, 35, 396, DateTimeKind.Utc).AddTicks(8776), 412, new DateTime(2020, 3, 5, 0, 30, 35, 396, DateTimeKind.Utc).AddTicks(8776) },
                    { 37, "RequestEntityTooLarge", new DateTime(2020, 3, 5, 0, 30, 35, 396, DateTimeKind.Utc).AddTicks(8776), 413, new DateTime(2020, 3, 5, 0, 30, 35, 396, DateTimeKind.Utc).AddTicks(8776) },
                    { 38, "RequestUriTooLong", new DateTime(2020, 3, 5, 0, 30, 35, 396, DateTimeKind.Utc).AddTicks(8776), 414, new DateTime(2020, 3, 5, 0, 30, 35, 396, DateTimeKind.Utc).AddTicks(8776) },
                    { 39, "UnsupportedMediaType", new DateTime(2020, 3, 5, 0, 30, 35, 396, DateTimeKind.Utc).AddTicks(8776), 415, new DateTime(2020, 3, 5, 0, 30, 35, 396, DateTimeKind.Utc).AddTicks(8776) },
                    { 40, "RequestedRangeNotSatisfiable", new DateTime(2020, 3, 5, 0, 30, 35, 396, DateTimeKind.Utc).AddTicks(8776), 416, new DateTime(2020, 3, 5, 0, 30, 35, 396, DateTimeKind.Utc).AddTicks(8776) },
                    { 41, "ExpectationFailed", new DateTime(2020, 3, 5, 0, 30, 35, 396, DateTimeKind.Utc).AddTicks(8776), 417, new DateTime(2020, 3, 5, 0, 30, 35, 396, DateTimeKind.Utc).AddTicks(8776) },
                    { 42, "MisdirectedRequest", new DateTime(2020, 3, 5, 0, 30, 35, 396, DateTimeKind.Utc).AddTicks(8776), 421, new DateTime(2020, 3, 5, 0, 30, 35, 396, DateTimeKind.Utc).AddTicks(8776) },
                    { 43, "UnprocessableEntity", new DateTime(2020, 3, 5, 0, 30, 35, 396, DateTimeKind.Utc).AddTicks(8776), 422, new DateTime(2020, 3, 5, 0, 30, 35, 396, DateTimeKind.Utc).AddTicks(8776) },
                    { 44, "Locked", new DateTime(2020, 3, 5, 0, 30, 35, 396, DateTimeKind.Utc).AddTicks(8776), 423, new DateTime(2020, 3, 5, 0, 30, 35, 396, DateTimeKind.Utc).AddTicks(8776) },
                    { 45, "FailedDependency", new DateTime(2020, 3, 5, 0, 30, 35, 396, DateTimeKind.Utc).AddTicks(8776), 424, new DateTime(2020, 3, 5, 0, 30, 35, 396, DateTimeKind.Utc).AddTicks(8776) },
                    { 46, "UpgradeRequired", new DateTime(2020, 3, 5, 0, 30, 35, 396, DateTimeKind.Utc).AddTicks(8776), 426, new DateTime(2020, 3, 5, 0, 30, 35, 396, DateTimeKind.Utc).AddTicks(8776) },
                    { 47, "PreconditionRequired", new DateTime(2020, 3, 5, 0, 30, 35, 396, DateTimeKind.Utc).AddTicks(8776), 428, new DateTime(2020, 3, 5, 0, 30, 35, 396, DateTimeKind.Utc).AddTicks(8776) },
                    { 48, "TooManyRequests", new DateTime(2020, 3, 5, 0, 30, 35, 396, DateTimeKind.Utc).AddTicks(8776), 429, new DateTime(2020, 3, 5, 0, 30, 35, 396, DateTimeKind.Utc).AddTicks(8776) },
                    { 49, "RequestHeaderFieldsTooLarge", new DateTime(2020, 3, 5, 0, 30, 35, 396, DateTimeKind.Utc).AddTicks(8776), 431, new DateTime(2020, 3, 5, 0, 30, 35, 396, DateTimeKind.Utc).AddTicks(8776) },
                    { 50, "UnavailableForLegalReasons", new DateTime(2020, 3, 5, 0, 30, 35, 396, DateTimeKind.Utc).AddTicks(8776), 451, new DateTime(2020, 3, 5, 0, 30, 35, 396, DateTimeKind.Utc).AddTicks(8776) },
                    { 51, "InternalServerError", new DateTime(2020, 3, 5, 0, 30, 35, 396, DateTimeKind.Utc).AddTicks(8776), 500, new DateTime(2020, 3, 5, 0, 30, 35, 396, DateTimeKind.Utc).AddTicks(8776) },
                    { 52, "NotImplemented", new DateTime(2020, 3, 5, 0, 30, 35, 396, DateTimeKind.Utc).AddTicks(8776), 501, new DateTime(2020, 3, 5, 0, 30, 35, 396, DateTimeKind.Utc).AddTicks(8776) },
                    { 53, "BadGateway", new DateTime(2020, 3, 5, 0, 30, 35, 396, DateTimeKind.Utc).AddTicks(8776), 502, new DateTime(2020, 3, 5, 0, 30, 35, 396, DateTimeKind.Utc).AddTicks(8776) },
                    { 54, "ServiceUnavailable", new DateTime(2020, 3, 5, 0, 30, 35, 396, DateTimeKind.Utc).AddTicks(8776), 503, new DateTime(2020, 3, 5, 0, 30, 35, 396, DateTimeKind.Utc).AddTicks(8776) },
                    { 55, "GatewayTimeout", new DateTime(2020, 3, 5, 0, 30, 35, 396, DateTimeKind.Utc).AddTicks(8776), 504, new DateTime(2020, 3, 5, 0, 30, 35, 396, DateTimeKind.Utc).AddTicks(8776) },
                    { 56, "HttpVersionNotSupported", new DateTime(2020, 3, 5, 0, 30, 35, 396, DateTimeKind.Utc).AddTicks(8776), 505, new DateTime(2020, 3, 5, 0, 30, 35, 396, DateTimeKind.Utc).AddTicks(8776) },
                    { 57, "VariantAlsoNegotiates", new DateTime(2020, 3, 5, 0, 30, 35, 396, DateTimeKind.Utc).AddTicks(8776), 506, new DateTime(2020, 3, 5, 0, 30, 35, 396, DateTimeKind.Utc).AddTicks(8776) },
                    { 58, "InsufficientStorage", new DateTime(2020, 3, 5, 0, 30, 35, 396, DateTimeKind.Utc).AddTicks(8776), 507, new DateTime(2020, 3, 5, 0, 30, 35, 396, DateTimeKind.Utc).AddTicks(8776) },
                    { 59, "LoopDetected", new DateTime(2020, 3, 5, 0, 30, 35, 396, DateTimeKind.Utc).AddTicks(8776), 508, new DateTime(2020, 3, 5, 0, 30, 35, 396, DateTimeKind.Utc).AddTicks(8776) },
                    { 60, "NotExtended", new DateTime(2020, 3, 5, 0, 30, 35, 396, DateTimeKind.Utc).AddTicks(8776), 510, new DateTime(2020, 3, 5, 0, 30, 35, 396, DateTimeKind.Utc).AddTicks(8776) },
                    { 61, "NetworkAuthenticationRequired", new DateTime(2020, 3, 5, 0, 30, 35, 396, DateTimeKind.Utc).AddTicks(8776), 511, new DateTime(2020, 3, 5, 0, 30, 35, 396, DateTimeKind.Utc).AddTicks(8776) },
                    { 32, "RequestTimeout", new DateTime(2020, 3, 5, 0, 30, 35, 396, DateTimeKind.Utc).AddTicks(8776), 408, new DateTime(2020, 3, 5, 0, 30, 35, 396, DateTimeKind.Utc).AddTicks(8776) },
                    { 31, "ProxyAuthenticationRequired", new DateTime(2020, 3, 5, 0, 30, 35, 396, DateTimeKind.Utc).AddTicks(8776), 407, new DateTime(2020, 3, 5, 0, 30, 35, 396, DateTimeKind.Utc).AddTicks(8776) },
                    { 33, "Conflict", new DateTime(2020, 3, 5, 0, 30, 35, 396, DateTimeKind.Utc).AddTicks(8776), 409, new DateTime(2020, 3, 5, 0, 30, 35, 396, DateTimeKind.Utc).AddTicks(8776) },
                    { 29, "MethodNotAllowed", new DateTime(2020, 3, 5, 0, 30, 35, 396, DateTimeKind.Utc).AddTicks(8776), 405, new DateTime(2020, 3, 5, 0, 30, 35, 396, DateTimeKind.Utc).AddTicks(8776) },
                    { 1, "Continue", new DateTime(2020, 3, 5, 0, 30, 35, 396, DateTimeKind.Utc).AddTicks(8776), 100, new DateTime(2020, 3, 5, 0, 30, 35, 396, DateTimeKind.Utc).AddTicks(8776) },
                    { 2, "SwitchingProtocols", new DateTime(2020, 3, 5, 0, 30, 35, 396, DateTimeKind.Utc).AddTicks(8776), 101, new DateTime(2020, 3, 5, 0, 30, 35, 396, DateTimeKind.Utc).AddTicks(8776) },
                    { 3, "Processing", new DateTime(2020, 3, 5, 0, 30, 35, 396, DateTimeKind.Utc).AddTicks(8776), 102, new DateTime(2020, 3, 5, 0, 30, 35, 396, DateTimeKind.Utc).AddTicks(8776) },
                    { 4, "EarlyHints", new DateTime(2020, 3, 5, 0, 30, 35, 396, DateTimeKind.Utc).AddTicks(8776), 103, new DateTime(2020, 3, 5, 0, 30, 35, 396, DateTimeKind.Utc).AddTicks(8776) },
                    { 5, "OK", new DateTime(2020, 3, 5, 0, 30, 35, 396, DateTimeKind.Utc).AddTicks(8776), 200, new DateTime(2020, 3, 5, 0, 30, 35, 396, DateTimeKind.Utc).AddTicks(8776) },
                    { 6, "Created", new DateTime(2020, 3, 5, 0, 30, 35, 396, DateTimeKind.Utc).AddTicks(8776), 201, new DateTime(2020, 3, 5, 0, 30, 35, 396, DateTimeKind.Utc).AddTicks(8776) },
                    { 7, "Accepted", new DateTime(2020, 3, 5, 0, 30, 35, 396, DateTimeKind.Utc).AddTicks(8776), 202, new DateTime(2020, 3, 5, 0, 30, 35, 396, DateTimeKind.Utc).AddTicks(8776) },
                    { 8, "NonAuthoritativeInformation", new DateTime(2020, 3, 5, 0, 30, 35, 396, DateTimeKind.Utc).AddTicks(8776), 203, new DateTime(2020, 3, 5, 0, 30, 35, 396, DateTimeKind.Utc).AddTicks(8776) },
                    { 9, "NoContent", new DateTime(2020, 3, 5, 0, 30, 35, 396, DateTimeKind.Utc).AddTicks(8776), 204, new DateTime(2020, 3, 5, 0, 30, 35, 396, DateTimeKind.Utc).AddTicks(8776) },
                    { 10, "ResetContent", new DateTime(2020, 3, 5, 0, 30, 35, 396, DateTimeKind.Utc).AddTicks(8776), 205, new DateTime(2020, 3, 5, 0, 30, 35, 396, DateTimeKind.Utc).AddTicks(8776) },
                    { 11, "PartialContent", new DateTime(2020, 3, 5, 0, 30, 35, 396, DateTimeKind.Utc).AddTicks(8776), 206, new DateTime(2020, 3, 5, 0, 30, 35, 396, DateTimeKind.Utc).AddTicks(8776) },
                    { 12, "MultiStatus", new DateTime(2020, 3, 5, 0, 30, 35, 396, DateTimeKind.Utc).AddTicks(8776), 207, new DateTime(2020, 3, 5, 0, 30, 35, 396, DateTimeKind.Utc).AddTicks(8776) },
                    { 30, "NotAcceptable", new DateTime(2020, 3, 5, 0, 30, 35, 396, DateTimeKind.Utc).AddTicks(8776), 406, new DateTime(2020, 3, 5, 0, 30, 35, 396, DateTimeKind.Utc).AddTicks(8776) },
                    { 14, "IMUsed", new DateTime(2020, 3, 5, 0, 30, 35, 396, DateTimeKind.Utc).AddTicks(8776), 226, new DateTime(2020, 3, 5, 0, 30, 35, 396, DateTimeKind.Utc).AddTicks(8776) },
                    { 13, "AlreadyReported", new DateTime(2020, 3, 5, 0, 30, 35, 396, DateTimeKind.Utc).AddTicks(8776), 208, new DateTime(2020, 3, 5, 0, 30, 35, 396, DateTimeKind.Utc).AddTicks(8776) },
                    { 16, "Moved", new DateTime(2020, 3, 5, 0, 30, 35, 396, DateTimeKind.Utc).AddTicks(8776), 301, new DateTime(2020, 3, 5, 0, 30, 35, 396, DateTimeKind.Utc).AddTicks(8776) },
                    { 28, "NotFound", new DateTime(2020, 3, 5, 0, 30, 35, 396, DateTimeKind.Utc).AddTicks(8776), 404, new DateTime(2020, 3, 5, 0, 30, 35, 396, DateTimeKind.Utc).AddTicks(8776) },
                    { 27, "Forbidden", new DateTime(2020, 3, 5, 0, 30, 35, 396, DateTimeKind.Utc).AddTicks(8776), 403, new DateTime(2020, 3, 5, 0, 30, 35, 396, DateTimeKind.Utc).AddTicks(8776) },
                    { 26, "PaymentRequired", new DateTime(2020, 3, 5, 0, 30, 35, 396, DateTimeKind.Utc).AddTicks(8776), 402, new DateTime(2020, 3, 5, 0, 30, 35, 396, DateTimeKind.Utc).AddTicks(8776) },
                    { 15, "Ambiguous", new DateTime(2020, 3, 5, 0, 30, 35, 396, DateTimeKind.Utc).AddTicks(8776), 300, new DateTime(2020, 3, 5, 0, 30, 35, 396, DateTimeKind.Utc).AddTicks(8776) },
                    { 24, "BadRequest", new DateTime(2020, 3, 5, 0, 30, 35, 396, DateTimeKind.Utc).AddTicks(8776), 400, new DateTime(2020, 3, 5, 0, 30, 35, 396, DateTimeKind.Utc).AddTicks(8776) },
                    { 23, "PermanentRedirect", new DateTime(2020, 3, 5, 0, 30, 35, 396, DateTimeKind.Utc).AddTicks(8776), 308, new DateTime(2020, 3, 5, 0, 30, 35, 396, DateTimeKind.Utc).AddTicks(8776) },
                    { 25, "Unauthorized", new DateTime(2020, 3, 5, 0, 30, 35, 396, DateTimeKind.Utc).AddTicks(8776), 401, new DateTime(2020, 3, 5, 0, 30, 35, 396, DateTimeKind.Utc).AddTicks(8776) },
                    { 21, "Unused", new DateTime(2020, 3, 5, 0, 30, 35, 396, DateTimeKind.Utc).AddTicks(8776), 306, new DateTime(2020, 3, 5, 0, 30, 35, 396, DateTimeKind.Utc).AddTicks(8776) },
                    { 20, "UseProxy", new DateTime(2020, 3, 5, 0, 30, 35, 396, DateTimeKind.Utc).AddTicks(8776), 305, new DateTime(2020, 3, 5, 0, 30, 35, 396, DateTimeKind.Utc).AddTicks(8776) },
                    { 19, "NotModified", new DateTime(2020, 3, 5, 0, 30, 35, 396, DateTimeKind.Utc).AddTicks(8776), 304, new DateTime(2020, 3, 5, 0, 30, 35, 396, DateTimeKind.Utc).AddTicks(8776) },
                    { 18, "RedirectMethod", new DateTime(2020, 3, 5, 0, 30, 35, 396, DateTimeKind.Utc).AddTicks(8776), 303, new DateTime(2020, 3, 5, 0, 30, 35, 396, DateTimeKind.Utc).AddTicks(8776) },
                    { 17, "Redirect", new DateTime(2020, 3, 5, 0, 30, 35, 396, DateTimeKind.Utc).AddTicks(8776), 302, new DateTime(2020, 3, 5, 0, 30, 35, 396, DateTimeKind.Utc).AddTicks(8776) },
                    { 22, "TemporaryRedirect", new DateTime(2020, 3, 5, 0, 30, 35, 396, DateTimeKind.Utc).AddTicks(8776), 307, new DateTime(2020, 3, 5, 0, 30, 35, 396, DateTimeKind.Utc).AddTicks(8776) }
                });

            migrationBuilder.InsertData(
                table: "Method",
                columns: new[] { "id", "code", "created", "updated" },
                values: new object[,]
                {
                    { 1, "POST", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 4, "DELETE", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 3, "GET", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 5, "HEAD", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 6, "PATCH", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 2, "PUT", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) }
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
