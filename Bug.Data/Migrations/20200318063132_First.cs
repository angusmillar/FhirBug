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
                name: "ServiceBaseUrl",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    created = table.Column<DateTime>(nullable: false),
                    updated = table.Column<DateTime>(nullable: false),
                    url = table.Column<string>(maxLength: 450, nullable: false),
                    is_primary = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceBaseUrl", x => x.id);
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
                name: "IndexDateTime",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    fk_resourcestore_id = table.Column<int>(nullable: false),
                    fk_searchparameter_id = table.Column<int>(nullable: false),
                    low = table.Column<DateTime>(nullable: true),
                    high = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IndexDateTime", x => x.id);
                    table.ForeignKey(
                        name: "FK_IndexDateTime_ResourceStore_fk_resourcestore_id",
                        column: x => x.fk_resourcestore_id,
                        principalTable: "ResourceStore",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_IndexDateTime_SearchParameter_fk_searchparameter_id",
                        column: x => x.fk_searchparameter_id,
                        principalTable: "SearchParameter",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "IndexQuantity",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    fk_resourcestore_id = table.Column<int>(nullable: false),
                    fk_searchparameter_id = table.Column<int>(nullable: false),
                    comparator = table.Column<int>(nullable: true),
                    quantity = table.Column<decimal>(nullable: true),
                    code = table.Column<string>(maxLength: 128, nullable: true),
                    system = table.Column<string>(maxLength: 450, nullable: true),
                    unit = table.Column<string>(maxLength: 450, nullable: true),
                    comparator_high = table.Column<int>(nullable: true),
                    quantity_high = table.Column<decimal>(nullable: true),
                    code_high = table.Column<string>(maxLength: 128, nullable: true),
                    system_high = table.Column<string>(maxLength: 450, nullable: true),
                    unit_high = table.Column<string>(maxLength: 450, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IndexQuantity", x => x.id);
                    table.ForeignKey(
                        name: "FK_IndexQuantity_ResourceStore_fk_resourcestore_id",
                        column: x => x.fk_resourcestore_id,
                        principalTable: "ResourceStore",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_IndexQuantity_SearchParameter_fk_searchparameter_id",
                        column: x => x.fk_searchparameter_id,
                        principalTable: "SearchParameter",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "IndexReference",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    fk_resourcestore_id = table.Column<int>(nullable: false),
                    fk_searchparameter_id = table.Column<int>(nullable: false),
                    fk_servicebaseurl_id = table.Column<int>(nullable: false),
                    fk_resourcetype_id = table.Column<int>(nullable: false),
                    ResourceTypeId = table.Column<int>(nullable: false),
                    resource_id = table.Column<string>(nullable: false),
                    version_id = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IndexReference", x => x.id);
                    table.ForeignKey(
                        name: "FK_IndexReference_ResourceStore_fk_resourcestore_id",
                        column: x => x.fk_resourcestore_id,
                        principalTable: "ResourceStore",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_IndexReference_SearchParameter_fk_searchparameter_id",
                        column: x => x.fk_searchparameter_id,
                        principalTable: "SearchParameter",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_IndexReference_ServiceBaseUrl_fk_servicebaseurl_id",
                        column: x => x.fk_servicebaseurl_id,
                        principalTable: "ServiceBaseUrl",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_IndexReference_ResourceType_ResourceTypeId",
                        column: x => x.ResourceTypeId,
                        principalTable: "ResourceType",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "IndexString",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    fk_resourcestore_id = table.Column<int>(nullable: false),
                    fk_searchparameter_id = table.Column<int>(nullable: false),
                    @string = table.Column<string>(name: "string", maxLength: 450, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IndexString", x => x.id);
                    table.ForeignKey(
                        name: "FK_IndexString_ResourceStore_fk_resourcestore_id",
                        column: x => x.fk_resourcestore_id,
                        principalTable: "ResourceStore",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_IndexString_SearchParameter_fk_searchparameter_id",
                        column: x => x.fk_searchparameter_id,
                        principalTable: "SearchParameter",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "IndexToken",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    fk_resourcestore_id = table.Column<int>(nullable: false),
                    fk_searchparameter_id = table.Column<int>(nullable: false),
                    code = table.Column<string>(maxLength: 128, nullable: true),
                    system = table.Column<string>(maxLength: 450, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IndexToken", x => x.id);
                    table.ForeignKey(
                        name: "FK_IndexToken_ResourceStore_fk_resourcestore_id",
                        column: x => x.fk_resourcestore_id,
                        principalTable: "ResourceStore",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_IndexToken_SearchParameter_fk_searchparameter_id",
                        column: x => x.fk_searchparameter_id,
                        principalTable: "SearchParameter",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "IndexUri",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    fk_resourcestore_id = table.Column<int>(nullable: false),
                    fk_searchparameter_id = table.Column<int>(nullable: false),
                    uri = table.Column<string>(maxLength: 450, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IndexUri", x => x.id);
                    table.ForeignKey(
                        name: "FK_IndexUri_ResourceStore_fk_resourcestore_id",
                        column: x => x.fk_resourcestore_id,
                        principalTable: "ResourceStore",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_IndexUri_SearchParameter_fk_searchparameter_id",
                        column: x => x.fk_searchparameter_id,
                        principalTable: "SearchParameter",
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
                    fk_searchparameter_id = table.Column<int>(nullable: false),
                    definition = table.Column<string>(maxLength: 450, nullable: false),
                    expression = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SearchParameterComponent", x => x.id);
                    table.ForeignKey(
                        name: "FK_SearchParameterComponent_SearchParameter_fk_searchparameter~",
                        column: x => x.fk_searchparameter_id,
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
                    fk_searchparameter_id = table.Column<int>(nullable: false),
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
                        name: "FK_SearchParameterResourceType_SearchParameter_fk_searchparame~",
                        column: x => x.fk_searchparameter_id,
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
                    fk_searchparameter_id = table.Column<int>(nullable: false),
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
                        name: "FK_SearchParameterTargetResourceType_SearchParameter_fk_search~",
                        column: x => x.fk_searchparameter_id,
                        principalTable: "SearchParameter",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "UniqueIx_HttpStatusCode_number",
                table: "HttpStatusCode",
                column: "number",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_IndexDateTime_fk_resourcestore_id",
                table: "IndexDateTime",
                column: "fk_resourcestore_id");

            migrationBuilder.CreateIndex(
                name: "IX_IndexDateTime_fk_searchparameter_id",
                table: "IndexDateTime",
                column: "fk_searchparameter_id");

            migrationBuilder.CreateIndex(
                name: "Ix_IndexDateTime_High",
                table: "IndexDateTime",
                column: "high");

            migrationBuilder.CreateIndex(
                name: "Ix_IndexDateTime_Low",
                table: "IndexDateTime",
                column: "low");

            migrationBuilder.CreateIndex(
                name: "Ix_IndexQuantity_Low",
                table: "IndexQuantity",
                column: "code");

            migrationBuilder.CreateIndex(
                name: "Ix_IndexQuantity_CodeHigh",
                table: "IndexQuantity",
                column: "code_high");

            migrationBuilder.CreateIndex(
                name: "IX_IndexQuantity_fk_resourcestore_id",
                table: "IndexQuantity",
                column: "fk_resourcestore_id");

            migrationBuilder.CreateIndex(
                name: "IX_IndexQuantity_fk_searchparameter_id",
                table: "IndexQuantity",
                column: "fk_searchparameter_id");

            migrationBuilder.CreateIndex(
                name: "Ix_IndexQuantity_Quantity",
                table: "IndexQuantity",
                column: "quantity");

            migrationBuilder.CreateIndex(
                name: "Ix_IndexQuantity_QuantityHigh",
                table: "IndexQuantity",
                column: "quantity_high");

            migrationBuilder.CreateIndex(
                name: "Ix_IndexQuantity_System",
                table: "IndexQuantity",
                column: "system");

            migrationBuilder.CreateIndex(
                name: "Ix_IndexQuantity_SystemHigh",
                table: "IndexQuantity",
                column: "system_high");

            migrationBuilder.CreateIndex(
                name: "IX_IndexReference_fk_resourcestore_id",
                table: "IndexReference",
                column: "fk_resourcestore_id");

            migrationBuilder.CreateIndex(
                name: "IX_IndexReference_fk_searchparameter_id",
                table: "IndexReference",
                column: "fk_searchparameter_id");

            migrationBuilder.CreateIndex(
                name: "IX_IndexReference_fk_servicebaseurl_id",
                table: "IndexReference",
                column: "fk_servicebaseurl_id");

            migrationBuilder.CreateIndex(
                name: "Ix_IndexReference_ResourceId",
                table: "IndexReference",
                column: "resource_id");

            migrationBuilder.CreateIndex(
                name: "IX_IndexReference_ResourceTypeId",
                table: "IndexReference",
                column: "ResourceTypeId");

            migrationBuilder.CreateIndex(
                name: "Ix_IndexReference_VersionId",
                table: "IndexReference",
                column: "version_id");

            migrationBuilder.CreateIndex(
                name: "IX_IndexString_fk_resourcestore_id",
                table: "IndexString",
                column: "fk_resourcestore_id");

            migrationBuilder.CreateIndex(
                name: "IX_IndexString_fk_searchparameter_id",
                table: "IndexString",
                column: "fk_searchparameter_id");

            migrationBuilder.CreateIndex(
                name: "Ix_IndexString_String",
                table: "IndexString",
                column: "string");

            migrationBuilder.CreateIndex(
                name: "Ix_IndexToken_Code",
                table: "IndexToken",
                column: "code");

            migrationBuilder.CreateIndex(
                name: "IX_IndexToken_fk_resourcestore_id",
                table: "IndexToken",
                column: "fk_resourcestore_id");

            migrationBuilder.CreateIndex(
                name: "IX_IndexToken_fk_searchparameter_id",
                table: "IndexToken",
                column: "fk_searchparameter_id");

            migrationBuilder.CreateIndex(
                name: "Ix_IndexToken_System",
                table: "IndexToken",
                column: "system");

            migrationBuilder.CreateIndex(
                name: "IX_IndexUri_fk_resourcestore_id",
                table: "IndexUri",
                column: "fk_resourcestore_id");

            migrationBuilder.CreateIndex(
                name: "IX_IndexUri_fk_searchparameter_id",
                table: "IndexUri",
                column: "fk_searchparameter_id");

            migrationBuilder.CreateIndex(
                name: "Ix_IndexUri_Uri",
                table: "IndexUri",
                column: "uri");

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
                name: "Ix_ResourceStore_LastUpdated",
                table: "ResourceStore",
                column: "last_updated");

            migrationBuilder.CreateIndex(
                name: "UniqueIx_ResourceStore_FhirVer_ResType_ResId_ResVer",
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
                name: "Ix_SearchParameter_Url",
                table: "SearchParameter",
                column: "name");

            migrationBuilder.CreateIndex(
                name: "IX_SearchParameterComponent_fk_searchparameter_id",
                table: "SearchParameterComponent",
                column: "fk_searchparameter_id");

            migrationBuilder.CreateIndex(
                name: "IX_SearchParameterResourceType_fk_resourcetype_id",
                table: "SearchParameterResourceType",
                column: "fk_resourcetype_id");

            migrationBuilder.CreateIndex(
                name: "IX_SearchParameterResourceType_fk_searchparameter_id",
                table: "SearchParameterResourceType",
                column: "fk_searchparameter_id");

            migrationBuilder.CreateIndex(
                name: "IX_SearchParameterTargetResourceType_fk_resourcetype_id",
                table: "SearchParameterTargetResourceType",
                column: "fk_resourcetype_id");

            migrationBuilder.CreateIndex(
                name: "IX_SearchParameterTargetResourceType_fk_searchparameter_id",
                table: "SearchParameterTargetResourceType",
                column: "fk_searchparameter_id");

            migrationBuilder.CreateIndex(
                name: "Ix_ServiceBaseUrl_Url",
                table: "ServiceBaseUrl",
                column: "url",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "IndexDateTime");

            migrationBuilder.DropTable(
                name: "IndexQuantity");

            migrationBuilder.DropTable(
                name: "IndexReference");

            migrationBuilder.DropTable(
                name: "IndexString");

            migrationBuilder.DropTable(
                name: "IndexToken");

            migrationBuilder.DropTable(
                name: "IndexUri");

            migrationBuilder.DropTable(
                name: "SearchParameterComponent");

            migrationBuilder.DropTable(
                name: "SearchParameterResourceType");

            migrationBuilder.DropTable(
                name: "SearchParameterTargetResourceType");

            migrationBuilder.DropTable(
                name: "ServiceBaseUrl");

            migrationBuilder.DropTable(
                name: "ResourceStore");

            migrationBuilder.DropTable(
                name: "SearchParameter");

            migrationBuilder.DropTable(
                name: "HttpStatusCode");

            migrationBuilder.DropTable(
                name: "Method");

            migrationBuilder.DropTable(
                name: "ResourceType");

            migrationBuilder.DropTable(
                name: "FhirVersion");

            migrationBuilder.DropTable(
                name: "SearchParamType");
        }
    }
}
