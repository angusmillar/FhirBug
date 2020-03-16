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
                name: "ResourceType",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false),
                    created = table.Column<DateTime>(nullable: false),
                    updated = table.Column<DateTime>(nullable: false),
                    code = table.Column<string>(maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ResourceType", x => x.id);
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
                    fk_resourcetype_id = table.Column<int>(nullable: false),
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
                        name: "FK_ResourceStore_ResourceType_fk_resourcetype_id",
                        column: x => x.fk_resourcetype_id,
                        principalTable: "ResourceType",
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
                    description = table.Column<string>(nullable: true),
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
                name: "SearchParameterComponent",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    created = table.Column<DateTime>(nullable: false),
                    updated = table.Column<DateTime>(nullable: false),
                    FkSearchParameterId = table.Column<int>(nullable: false),
                    definition = table.Column<string>(maxLength: 450, nullable: false),
                    expression = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SearchParameterComponent", x => x.id);
                    table.ForeignKey(
                        name: "FK_SearchParameterComponent_SearchParameter_FkSearchParameterId",
                        column: x => x.FkSearchParameterId,
                        principalTable: "SearchParameter",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SearchParameterResourceType",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    created = table.Column<DateTime>(nullable: false),
                    updated = table.Column<DateTime>(nullable: false),
                    FkSearchParameterId = table.Column<int>(nullable: false),
                    fk_resourcetype_id = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SearchParameterResourceType", x => x.id);
                    table.ForeignKey(
                        name: "FK_SearchParameterResourceType_ResourceType_fk_resourcetype_id",
                        column: x => x.fk_resourcetype_id,
                        principalTable: "ResourceType",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SearchParameterResourceType_SearchParameter_FkSearchParamet~",
                        column: x => x.FkSearchParameterId,
                        principalTable: "SearchParameter",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SearchParameterTargetResourceType",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    created = table.Column<DateTime>(nullable: false),
                    updated = table.Column<DateTime>(nullable: false),
                    FkSearchParameterId = table.Column<int>(nullable: false),
                    fk_resourcetype_id = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SearchParameterTargetResourceType", x => x.id);
                    table.ForeignKey(
                        name: "FK_SearchParameterTargetResourceType_ResourceType_fk_resourcet~",
                        column: x => x.fk_resourcetype_id,
                        principalTable: "ResourceType",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SearchParameterTargetResourceType_SearchParameter_FkSearchP~",
                        column: x => x.FkSearchParameterId,
                        principalTable: "SearchParameter",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
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
                name: "IX_ResourceStore_fk_resourcetype_id",
                table: "ResourceStore",
                column: "fk_resourcetype_id");

            migrationBuilder.CreateIndex(
                name: "Ix_LastUpdated",
                table: "ResourceStore",
                column: "last_updated");

            migrationBuilder.CreateIndex(
                name: "UniqueIx_FhirVer_ResType_ResId_ResVer",
                table: "ResourceStore",
                columns: new[] { "fk_fhirversion_id", "fk_resourcetype_id", "resource_id", "version_id" },
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
                name: "IX_SearchParameterComponent_FkSearchParameterId",
                table: "SearchParameterComponent",
                column: "FkSearchParameterId");

            migrationBuilder.CreateIndex(
                name: "IX_SearchParameterResourceType_fk_resourcetype_id",
                table: "SearchParameterResourceType",
                column: "fk_resourcetype_id");

            migrationBuilder.CreateIndex(
                name: "IX_SearchParameterResourceType_FkSearchParameterId",
                table: "SearchParameterResourceType",
                column: "FkSearchParameterId");

            migrationBuilder.CreateIndex(
                name: "IX_SearchParameterTargetResourceType_fk_resourcetype_id",
                table: "SearchParameterTargetResourceType",
                column: "fk_resourcetype_id");

            migrationBuilder.CreateIndex(
                name: "IX_SearchParameterTargetResourceType_FkSearchParameterId",
                table: "SearchParameterTargetResourceType",
                column: "FkSearchParameterId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ResourceStore");

            migrationBuilder.DropTable(
                name: "SearchParameterComponent");

            migrationBuilder.DropTable(
                name: "SearchParameterResourceType");

            migrationBuilder.DropTable(
                name: "SearchParameterTargetResourceType");

            migrationBuilder.DropTable(
                name: "HttpStatusCode");

            migrationBuilder.DropTable(
                name: "Method");

            migrationBuilder.DropTable(
                name: "ResourceType");

            migrationBuilder.DropTable(
                name: "SearchParameter");

            migrationBuilder.DropTable(
                name: "FhirVersion");

            migrationBuilder.DropTable(
                name: "SearchParamType");
        }
    }
}
