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
                    code = table.Column<string>(maxLength: 128, nullable: false)
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
                    code = table.Column<string>(maxLength: 128, nullable: false)
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
                    code = table.Column<string>(maxLength: 128, nullable: false)
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
                    name = table.Column<string>(maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ResourceName", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "SearchParamType",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false),
                    created = table.Column<DateTime>(nullable: false),
                    updated = table.Column<DateTime>(nullable: false),
                    code = table.Column<string>(maxLength: 128, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SearchParamType", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "ResourceStore",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    created = table.Column<DateTime>(nullable: false),
                    updated = table.Column<DateTime>(nullable: false),
                    resource_id = table.Column<string>(maxLength: 128, nullable: false),
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

            migrationBuilder.CreateTable(
                name: "SearchParameter",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    created = table.Column<DateTime>(nullable: false),
                    updated = table.Column<DateTime>(nullable: false),
                    name = table.Column<string>(maxLength: 128, nullable: false),
                    description = table.Column<string>(maxLength: 256, nullable: true),
                    fk_searchparamtype_id = table.Column<int>(nullable: false),
                    url = table.Column<string>(maxLength: 450, nullable: true),
                    fhir_path = table.Column<string>(nullable: true),
                    fk_fhirversion_id = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SearchParameter", x => x.id);
                    table.ForeignKey(
                        name: "FK_SearchParameter_FhirVersion_fk_fhirversion_id",
                        column: x => x.fk_fhirversion_id,
                        principalTable: "FhirVersion",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SearchParameter_SearchParamType_fk_searchparamtype_id",
                        column: x => x.fk_searchparamtype_id,
                        principalTable: "SearchParamType",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SearchParameterResourceName",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    created = table.Column<DateTime>(nullable: false),
                    updated = table.Column<DateTime>(nullable: false),
                    FkSearchParameterId = table.Column<int>(nullable: false),
                    FkResourceNameId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SearchParameterResourceName", x => x.id);
                    table.ForeignKey(
                        name: "FK_SearchParameterResourceName_ResourceName_FkResourceNameId",
                        column: x => x.FkResourceNameId,
                        principalTable: "ResourceName",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SearchParameterResourceName_SearchParameter_FkSearchParamet~",
                        column: x => x.FkSearchParameterId,
                        principalTable: "SearchParameter",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SearchParameterTargetResourceName",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    created = table.Column<DateTime>(nullable: false),
                    updated = table.Column<DateTime>(nullable: false),
                    FkSearchParameterId = table.Column<int>(nullable: false),
                    FkResourceNameId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SearchParameterTargetResourceName", x => x.id);
                    table.ForeignKey(
                        name: "FK_SearchParameterTargetResourceName_ResourceName_FkResourceNa~",
                        column: x => x.FkResourceNameId,
                        principalTable: "ResourceName",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SearchParameterTargetResourceName_SearchParameter_FkSearchP~",
                        column: x => x.FkSearchParameterId,
                        principalTable: "SearchParameter",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "FhirVersion",
                columns: new[] { "id", "code", "created", "updated" },
                values: new object[,]
                {
                    { 0, "Stu3", new DateTime(2020, 3, 9, 8, 46, 48, 128, DateTimeKind.Utc).AddTicks(9551), new DateTime(2020, 3, 9, 8, 46, 48, 128, DateTimeKind.Utc).AddTicks(9551) },
                    { 1, "R4", new DateTime(2020, 3, 9, 8, 46, 48, 128, DateTimeKind.Utc).AddTicks(9551), new DateTime(2020, 3, 9, 8, 46, 48, 128, DateTimeKind.Utc).AddTicks(9551) }
                });

            migrationBuilder.InsertData(
                table: "HttpStatusCode",
                columns: new[] { "id", "code", "created", "number", "updated" },
                values: new object[,]
                {
                    { 33, "Conflict", new DateTime(2020, 3, 9, 8, 46, 48, 128, DateTimeKind.Utc).AddTicks(9551), 409, new DateTime(2020, 3, 9, 8, 46, 48, 128, DateTimeKind.Utc).AddTicks(9551) },
                    { 34, "Gone", new DateTime(2020, 3, 9, 8, 46, 48, 128, DateTimeKind.Utc).AddTicks(9551), 410, new DateTime(2020, 3, 9, 8, 46, 48, 128, DateTimeKind.Utc).AddTicks(9551) },
                    { 35, "LengthRequired", new DateTime(2020, 3, 9, 8, 46, 48, 128, DateTimeKind.Utc).AddTicks(9551), 411, new DateTime(2020, 3, 9, 8, 46, 48, 128, DateTimeKind.Utc).AddTicks(9551) },
                    { 36, "PreconditionFailed", new DateTime(2020, 3, 9, 8, 46, 48, 128, DateTimeKind.Utc).AddTicks(9551), 412, new DateTime(2020, 3, 9, 8, 46, 48, 128, DateTimeKind.Utc).AddTicks(9551) },
                    { 38, "RequestUriTooLong", new DateTime(2020, 3, 9, 8, 46, 48, 128, DateTimeKind.Utc).AddTicks(9551), 414, new DateTime(2020, 3, 9, 8, 46, 48, 128, DateTimeKind.Utc).AddTicks(9551) },
                    { 39, "UnsupportedMediaType", new DateTime(2020, 3, 9, 8, 46, 48, 128, DateTimeKind.Utc).AddTicks(9551), 415, new DateTime(2020, 3, 9, 8, 46, 48, 128, DateTimeKind.Utc).AddTicks(9551) },
                    { 40, "RequestedRangeNotSatisfiable", new DateTime(2020, 3, 9, 8, 46, 48, 128, DateTimeKind.Utc).AddTicks(9551), 416, new DateTime(2020, 3, 9, 8, 46, 48, 128, DateTimeKind.Utc).AddTicks(9551) },
                    { 41, "ExpectationFailed", new DateTime(2020, 3, 9, 8, 46, 48, 128, DateTimeKind.Utc).AddTicks(9551), 417, new DateTime(2020, 3, 9, 8, 46, 48, 128, DateTimeKind.Utc).AddTicks(9551) },
                    { 42, "MisdirectedRequest", new DateTime(2020, 3, 9, 8, 46, 48, 128, DateTimeKind.Utc).AddTicks(9551), 421, new DateTime(2020, 3, 9, 8, 46, 48, 128, DateTimeKind.Utc).AddTicks(9551) },
                    { 43, "UnprocessableEntity", new DateTime(2020, 3, 9, 8, 46, 48, 128, DateTimeKind.Utc).AddTicks(9551), 422, new DateTime(2020, 3, 9, 8, 46, 48, 128, DateTimeKind.Utc).AddTicks(9551) },
                    { 44, "Locked", new DateTime(2020, 3, 9, 8, 46, 48, 128, DateTimeKind.Utc).AddTicks(9551), 423, new DateTime(2020, 3, 9, 8, 46, 48, 128, DateTimeKind.Utc).AddTicks(9551) },
                    { 45, "FailedDependency", new DateTime(2020, 3, 9, 8, 46, 48, 128, DateTimeKind.Utc).AddTicks(9551), 424, new DateTime(2020, 3, 9, 8, 46, 48, 128, DateTimeKind.Utc).AddTicks(9551) },
                    { 46, "UpgradeRequired", new DateTime(2020, 3, 9, 8, 46, 48, 128, DateTimeKind.Utc).AddTicks(9551), 426, new DateTime(2020, 3, 9, 8, 46, 48, 128, DateTimeKind.Utc).AddTicks(9551) },
                    { 47, "PreconditionRequired", new DateTime(2020, 3, 9, 8, 46, 48, 128, DateTimeKind.Utc).AddTicks(9551), 428, new DateTime(2020, 3, 9, 8, 46, 48, 128, DateTimeKind.Utc).AddTicks(9551) },
                    { 48, "TooManyRequests", new DateTime(2020, 3, 9, 8, 46, 48, 128, DateTimeKind.Utc).AddTicks(9551), 429, new DateTime(2020, 3, 9, 8, 46, 48, 128, DateTimeKind.Utc).AddTicks(9551) },
                    { 49, "RequestHeaderFieldsTooLarge", new DateTime(2020, 3, 9, 8, 46, 48, 128, DateTimeKind.Utc).AddTicks(9551), 431, new DateTime(2020, 3, 9, 8, 46, 48, 128, DateTimeKind.Utc).AddTicks(9551) },
                    { 50, "UnavailableForLegalReasons", new DateTime(2020, 3, 9, 8, 46, 48, 128, DateTimeKind.Utc).AddTicks(9551), 451, new DateTime(2020, 3, 9, 8, 46, 48, 128, DateTimeKind.Utc).AddTicks(9551) },
                    { 51, "InternalServerError", new DateTime(2020, 3, 9, 8, 46, 48, 128, DateTimeKind.Utc).AddTicks(9551), 500, new DateTime(2020, 3, 9, 8, 46, 48, 128, DateTimeKind.Utc).AddTicks(9551) },
                    { 52, "NotImplemented", new DateTime(2020, 3, 9, 8, 46, 48, 128, DateTimeKind.Utc).AddTicks(9551), 501, new DateTime(2020, 3, 9, 8, 46, 48, 128, DateTimeKind.Utc).AddTicks(9551) },
                    { 53, "BadGateway", new DateTime(2020, 3, 9, 8, 46, 48, 128, DateTimeKind.Utc).AddTicks(9551), 502, new DateTime(2020, 3, 9, 8, 46, 48, 128, DateTimeKind.Utc).AddTicks(9551) },
                    { 54, "ServiceUnavailable", new DateTime(2020, 3, 9, 8, 46, 48, 128, DateTimeKind.Utc).AddTicks(9551), 503, new DateTime(2020, 3, 9, 8, 46, 48, 128, DateTimeKind.Utc).AddTicks(9551) },
                    { 55, "GatewayTimeout", new DateTime(2020, 3, 9, 8, 46, 48, 128, DateTimeKind.Utc).AddTicks(9551), 504, new DateTime(2020, 3, 9, 8, 46, 48, 128, DateTimeKind.Utc).AddTicks(9551) },
                    { 56, "HttpVersionNotSupported", new DateTime(2020, 3, 9, 8, 46, 48, 128, DateTimeKind.Utc).AddTicks(9551), 505, new DateTime(2020, 3, 9, 8, 46, 48, 128, DateTimeKind.Utc).AddTicks(9551) },
                    { 57, "VariantAlsoNegotiates", new DateTime(2020, 3, 9, 8, 46, 48, 128, DateTimeKind.Utc).AddTicks(9551), 506, new DateTime(2020, 3, 9, 8, 46, 48, 128, DateTimeKind.Utc).AddTicks(9551) },
                    { 58, "InsufficientStorage", new DateTime(2020, 3, 9, 8, 46, 48, 128, DateTimeKind.Utc).AddTicks(9551), 507, new DateTime(2020, 3, 9, 8, 46, 48, 128, DateTimeKind.Utc).AddTicks(9551) },
                    { 59, "LoopDetected", new DateTime(2020, 3, 9, 8, 46, 48, 128, DateTimeKind.Utc).AddTicks(9551), 508, new DateTime(2020, 3, 9, 8, 46, 48, 128, DateTimeKind.Utc).AddTicks(9551) },
                    { 60, "NotExtended", new DateTime(2020, 3, 9, 8, 46, 48, 128, DateTimeKind.Utc).AddTicks(9551), 510, new DateTime(2020, 3, 9, 8, 46, 48, 128, DateTimeKind.Utc).AddTicks(9551) },
                    { 61, "NetworkAuthenticationRequired", new DateTime(2020, 3, 9, 8, 46, 48, 128, DateTimeKind.Utc).AddTicks(9551), 511, new DateTime(2020, 3, 9, 8, 46, 48, 128, DateTimeKind.Utc).AddTicks(9551) },
                    { 32, "RequestTimeout", new DateTime(2020, 3, 9, 8, 46, 48, 128, DateTimeKind.Utc).AddTicks(9551), 408, new DateTime(2020, 3, 9, 8, 46, 48, 128, DateTimeKind.Utc).AddTicks(9551) },
                    { 31, "ProxyAuthenticationRequired", new DateTime(2020, 3, 9, 8, 46, 48, 128, DateTimeKind.Utc).AddTicks(9551), 407, new DateTime(2020, 3, 9, 8, 46, 48, 128, DateTimeKind.Utc).AddTicks(9551) },
                    { 37, "RequestEntityTooLarge", new DateTime(2020, 3, 9, 8, 46, 48, 128, DateTimeKind.Utc).AddTicks(9551), 413, new DateTime(2020, 3, 9, 8, 46, 48, 128, DateTimeKind.Utc).AddTicks(9551) },
                    { 29, "MethodNotAllowed", new DateTime(2020, 3, 9, 8, 46, 48, 128, DateTimeKind.Utc).AddTicks(9551), 405, new DateTime(2020, 3, 9, 8, 46, 48, 128, DateTimeKind.Utc).AddTicks(9551) },
                    { 1, "Continue", new DateTime(2020, 3, 9, 8, 46, 48, 128, DateTimeKind.Utc).AddTicks(9551), 100, new DateTime(2020, 3, 9, 8, 46, 48, 128, DateTimeKind.Utc).AddTicks(9551) },
                    { 2, "SwitchingProtocols", new DateTime(2020, 3, 9, 8, 46, 48, 128, DateTimeKind.Utc).AddTicks(9551), 101, new DateTime(2020, 3, 9, 8, 46, 48, 128, DateTimeKind.Utc).AddTicks(9551) },
                    { 3, "Processing", new DateTime(2020, 3, 9, 8, 46, 48, 128, DateTimeKind.Utc).AddTicks(9551), 102, new DateTime(2020, 3, 9, 8, 46, 48, 128, DateTimeKind.Utc).AddTicks(9551) },
                    { 4, "EarlyHints", new DateTime(2020, 3, 9, 8, 46, 48, 128, DateTimeKind.Utc).AddTicks(9551), 103, new DateTime(2020, 3, 9, 8, 46, 48, 128, DateTimeKind.Utc).AddTicks(9551) },
                    { 5, "OK", new DateTime(2020, 3, 9, 8, 46, 48, 128, DateTimeKind.Utc).AddTicks(9551), 200, new DateTime(2020, 3, 9, 8, 46, 48, 128, DateTimeKind.Utc).AddTicks(9551) },
                    { 6, "Created", new DateTime(2020, 3, 9, 8, 46, 48, 128, DateTimeKind.Utc).AddTicks(9551), 201, new DateTime(2020, 3, 9, 8, 46, 48, 128, DateTimeKind.Utc).AddTicks(9551) },
                    { 7, "Accepted", new DateTime(2020, 3, 9, 8, 46, 48, 128, DateTimeKind.Utc).AddTicks(9551), 202, new DateTime(2020, 3, 9, 8, 46, 48, 128, DateTimeKind.Utc).AddTicks(9551) },
                    { 30, "NotAcceptable", new DateTime(2020, 3, 9, 8, 46, 48, 128, DateTimeKind.Utc).AddTicks(9551), 406, new DateTime(2020, 3, 9, 8, 46, 48, 128, DateTimeKind.Utc).AddTicks(9551) },
                    { 9, "NoContent", new DateTime(2020, 3, 9, 8, 46, 48, 128, DateTimeKind.Utc).AddTicks(9551), 204, new DateTime(2020, 3, 9, 8, 46, 48, 128, DateTimeKind.Utc).AddTicks(9551) },
                    { 10, "ResetContent", new DateTime(2020, 3, 9, 8, 46, 48, 128, DateTimeKind.Utc).AddTicks(9551), 205, new DateTime(2020, 3, 9, 8, 46, 48, 128, DateTimeKind.Utc).AddTicks(9551) },
                    { 11, "PartialContent", new DateTime(2020, 3, 9, 8, 46, 48, 128, DateTimeKind.Utc).AddTicks(9551), 206, new DateTime(2020, 3, 9, 8, 46, 48, 128, DateTimeKind.Utc).AddTicks(9551) },
                    { 12, "MultiStatus", new DateTime(2020, 3, 9, 8, 46, 48, 128, DateTimeKind.Utc).AddTicks(9551), 207, new DateTime(2020, 3, 9, 8, 46, 48, 128, DateTimeKind.Utc).AddTicks(9551) },
                    { 13, "AlreadyReported", new DateTime(2020, 3, 9, 8, 46, 48, 128, DateTimeKind.Utc).AddTicks(9551), 208, new DateTime(2020, 3, 9, 8, 46, 48, 128, DateTimeKind.Utc).AddTicks(9551) },
                    { 14, "IMUsed", new DateTime(2020, 3, 9, 8, 46, 48, 128, DateTimeKind.Utc).AddTicks(9551), 226, new DateTime(2020, 3, 9, 8, 46, 48, 128, DateTimeKind.Utc).AddTicks(9551) },
                    { 8, "NonAuthoritativeInformation", new DateTime(2020, 3, 9, 8, 46, 48, 128, DateTimeKind.Utc).AddTicks(9551), 203, new DateTime(2020, 3, 9, 8, 46, 48, 128, DateTimeKind.Utc).AddTicks(9551) },
                    { 16, "Moved", new DateTime(2020, 3, 9, 8, 46, 48, 128, DateTimeKind.Utc).AddTicks(9551), 301, new DateTime(2020, 3, 9, 8, 46, 48, 128, DateTimeKind.Utc).AddTicks(9551) },
                    { 15, "Ambiguous", new DateTime(2020, 3, 9, 8, 46, 48, 128, DateTimeKind.Utc).AddTicks(9551), 300, new DateTime(2020, 3, 9, 8, 46, 48, 128, DateTimeKind.Utc).AddTicks(9551) },
                    { 28, "NotFound", new DateTime(2020, 3, 9, 8, 46, 48, 128, DateTimeKind.Utc).AddTicks(9551), 404, new DateTime(2020, 3, 9, 8, 46, 48, 128, DateTimeKind.Utc).AddTicks(9551) },
                    { 27, "Forbidden", new DateTime(2020, 3, 9, 8, 46, 48, 128, DateTimeKind.Utc).AddTicks(9551), 403, new DateTime(2020, 3, 9, 8, 46, 48, 128, DateTimeKind.Utc).AddTicks(9551) },
                    { 26, "PaymentRequired", new DateTime(2020, 3, 9, 8, 46, 48, 128, DateTimeKind.Utc).AddTicks(9551), 402, new DateTime(2020, 3, 9, 8, 46, 48, 128, DateTimeKind.Utc).AddTicks(9551) },
                    { 25, "Unauthorized", new DateTime(2020, 3, 9, 8, 46, 48, 128, DateTimeKind.Utc).AddTicks(9551), 401, new DateTime(2020, 3, 9, 8, 46, 48, 128, DateTimeKind.Utc).AddTicks(9551) },
                    { 23, "PermanentRedirect", new DateTime(2020, 3, 9, 8, 46, 48, 128, DateTimeKind.Utc).AddTicks(9551), 308, new DateTime(2020, 3, 9, 8, 46, 48, 128, DateTimeKind.Utc).AddTicks(9551) },
                    { 24, "BadRequest", new DateTime(2020, 3, 9, 8, 46, 48, 128, DateTimeKind.Utc).AddTicks(9551), 400, new DateTime(2020, 3, 9, 8, 46, 48, 128, DateTimeKind.Utc).AddTicks(9551) },
                    { 21, "Unused", new DateTime(2020, 3, 9, 8, 46, 48, 128, DateTimeKind.Utc).AddTicks(9551), 306, new DateTime(2020, 3, 9, 8, 46, 48, 128, DateTimeKind.Utc).AddTicks(9551) },
                    { 20, "UseProxy", new DateTime(2020, 3, 9, 8, 46, 48, 128, DateTimeKind.Utc).AddTicks(9551), 305, new DateTime(2020, 3, 9, 8, 46, 48, 128, DateTimeKind.Utc).AddTicks(9551) },
                    { 19, "NotModified", new DateTime(2020, 3, 9, 8, 46, 48, 128, DateTimeKind.Utc).AddTicks(9551), 304, new DateTime(2020, 3, 9, 8, 46, 48, 128, DateTimeKind.Utc).AddTicks(9551) },
                    { 18, "RedirectMethod", new DateTime(2020, 3, 9, 8, 46, 48, 128, DateTimeKind.Utc).AddTicks(9551), 303, new DateTime(2020, 3, 9, 8, 46, 48, 128, DateTimeKind.Utc).AddTicks(9551) },
                    { 17, "Redirect", new DateTime(2020, 3, 9, 8, 46, 48, 128, DateTimeKind.Utc).AddTicks(9551), 302, new DateTime(2020, 3, 9, 8, 46, 48, 128, DateTimeKind.Utc).AddTicks(9551) },
                    { 22, "TemporaryRedirect", new DateTime(2020, 3, 9, 8, 46, 48, 128, DateTimeKind.Utc).AddTicks(9551), 307, new DateTime(2020, 3, 9, 8, 46, 48, 128, DateTimeKind.Utc).AddTicks(9551) }
                });

            migrationBuilder.InsertData(
                table: "Method",
                columns: new[] { "id", "code", "created", "updated" },
                values: new object[,]
                {
                    { 1, "POST", new DateTime(2020, 3, 9, 8, 46, 48, 128, DateTimeKind.Utc).AddTicks(9551), new DateTime(2020, 3, 9, 8, 46, 48, 128, DateTimeKind.Utc).AddTicks(9551) },
                    { 2, "PUT", new DateTime(2020, 3, 9, 8, 46, 48, 128, DateTimeKind.Utc).AddTicks(9551), new DateTime(2020, 3, 9, 8, 46, 48, 128, DateTimeKind.Utc).AddTicks(9551) },
                    { 6, "PATCH", new DateTime(2020, 3, 9, 8, 46, 48, 128, DateTimeKind.Utc).AddTicks(9551), new DateTime(2020, 3, 9, 8, 46, 48, 128, DateTimeKind.Utc).AddTicks(9551) },
                    { 4, "DELETE", new DateTime(2020, 3, 9, 8, 46, 48, 128, DateTimeKind.Utc).AddTicks(9551), new DateTime(2020, 3, 9, 8, 46, 48, 128, DateTimeKind.Utc).AddTicks(9551) },
                    { 3, "GET", new DateTime(2020, 3, 9, 8, 46, 48, 128, DateTimeKind.Utc).AddTicks(9551), new DateTime(2020, 3, 9, 8, 46, 48, 128, DateTimeKind.Utc).AddTicks(9551) },
                    { 5, "HEAD", new DateTime(2020, 3, 9, 8, 46, 48, 128, DateTimeKind.Utc).AddTicks(9551), new DateTime(2020, 3, 9, 8, 46, 48, 128, DateTimeKind.Utc).AddTicks(9551) }
                });

            migrationBuilder.InsertData(
                table: "SearchParamType",
                columns: new[] { "id", "code", "created", "updated" },
                values: new object[,]
                {
                    { 3, "Token", new DateTime(2020, 3, 9, 8, 46, 48, 128, DateTimeKind.Utc).AddTicks(9551), new DateTime(2020, 3, 9, 8, 46, 48, 128, DateTimeKind.Utc).AddTicks(9551) },
                    { 5, "Composite", new DateTime(2020, 3, 9, 8, 46, 48, 128, DateTimeKind.Utc).AddTicks(9551), new DateTime(2020, 3, 9, 8, 46, 48, 128, DateTimeKind.Utc).AddTicks(9551) },
                    { 1, "Date", new DateTime(2020, 3, 9, 8, 46, 48, 128, DateTimeKind.Utc).AddTicks(9551), new DateTime(2020, 3, 9, 8, 46, 48, 128, DateTimeKind.Utc).AddTicks(9551) },
                    { 0, "Number", new DateTime(2020, 3, 9, 8, 46, 48, 128, DateTimeKind.Utc).AddTicks(9551), new DateTime(2020, 3, 9, 8, 46, 48, 128, DateTimeKind.Utc).AddTicks(9551) },
                    { 6, "Quantity", new DateTime(2020, 3, 9, 8, 46, 48, 128, DateTimeKind.Utc).AddTicks(9551), new DateTime(2020, 3, 9, 8, 46, 48, 128, DateTimeKind.Utc).AddTicks(9551) },
                    { 4, "Reference", new DateTime(2020, 3, 9, 8, 46, 48, 128, DateTimeKind.Utc).AddTicks(9551), new DateTime(2020, 3, 9, 8, 46, 48, 128, DateTimeKind.Utc).AddTicks(9551) },
                    { 8, "Special", new DateTime(2020, 3, 9, 8, 46, 48, 128, DateTimeKind.Utc).AddTicks(9551), new DateTime(2020, 3, 9, 8, 46, 48, 128, DateTimeKind.Utc).AddTicks(9551) },
                    { 2, "String", new DateTime(2020, 3, 9, 8, 46, 48, 128, DateTimeKind.Utc).AddTicks(9551), new DateTime(2020, 3, 9, 8, 46, 48, 128, DateTimeKind.Utc).AddTicks(9551) },
                    { 7, "Uri", new DateTime(2020, 3, 9, 8, 46, 48, 128, DateTimeKind.Utc).AddTicks(9551), new DateTime(2020, 3, 9, 8, 46, 48, 128, DateTimeKind.Utc).AddTicks(9551) }
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
                name: "Ix_LastUpdated",
                table: "ResourceStore",
                column: "last_updated");

            migrationBuilder.CreateIndex(
                name: "UniqueIx_FhirVer_ResName_ResId_ResVer",
                table: "ResourceStore",
                columns: new[] { "fk_fhirversion_id", "fk_resourcename_id", "resource_id", "version_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SearchParameter_fk_fhirversion_id",
                table: "SearchParameter",
                column: "fk_fhirversion_id");

            migrationBuilder.CreateIndex(
                name: "IX_SearchParameter_fk_searchparamtype_id",
                table: "SearchParameter",
                column: "fk_searchparamtype_id");

            migrationBuilder.CreateIndex(
                name: "IX_SearchParameterResourceName_FkResourceNameId",
                table: "SearchParameterResourceName",
                column: "FkResourceNameId");

            migrationBuilder.CreateIndex(
                name: "IX_SearchParameterResourceName_FkSearchParameterId",
                table: "SearchParameterResourceName",
                column: "FkSearchParameterId");

            migrationBuilder.CreateIndex(
                name: "IX_SearchParameterTargetResourceName_FkResourceNameId",
                table: "SearchParameterTargetResourceName",
                column: "FkResourceNameId");

            migrationBuilder.CreateIndex(
                name: "IX_SearchParameterTargetResourceName_FkSearchParameterId",
                table: "SearchParameterTargetResourceName",
                column: "FkSearchParameterId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ResourceStore");

            migrationBuilder.DropTable(
                name: "SearchParameterResourceName");

            migrationBuilder.DropTable(
                name: "SearchParameterTargetResourceName");

            migrationBuilder.DropTable(
                name: "HttpStatusCode");

            migrationBuilder.DropTable(
                name: "Method");

            migrationBuilder.DropTable(
                name: "ResourceName");

            migrationBuilder.DropTable(
                name: "SearchParameter");

            migrationBuilder.DropTable(
                name: "FhirVersion");

            migrationBuilder.DropTable(
                name: "SearchParamType");
        }
    }
}
