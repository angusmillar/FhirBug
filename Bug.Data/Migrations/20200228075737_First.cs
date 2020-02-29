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
                    code = table.Column<string>(nullable: false),
                    fhir_major_version = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FhirVersion", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "Method",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    code = table.Column<string>(nullable: false),
                    http_verb = table.Column<int>(nullable: false)
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
                    resource_id = table.Column<string>(nullable: false),
                    version_id = table.Column<int>(nullable: false),
                    is_deleted = table.Column<bool>(nullable: false),
                    is_current = table.Column<bool>(nullable: false),
                    last_updated = table.Column<DateTime>(nullable: false),
                    resource_blob = table.Column<byte[]>(nullable: true),
                    fk_resource_name_id = table.Column<int>(nullable: false),
                    fk_fhir_version_id = table.Column<int>(nullable: false),
                    fk_method_id = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ResourceStore", x => x.id);
                    table.ForeignKey(
                        name: "FK_ResourceStore_FhirVersion_fk_fhir_version_id",
                        column: x => x.fk_fhir_version_id,
                        principalTable: "FhirVersion",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ResourceStore_Method_fk_method_id",
                        column: x => x.fk_method_id,
                        principalTable: "Method",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ResourceStore_ResourceName_fk_resource_name_id",
                        column: x => x.fk_resource_name_id,
                        principalTable: "ResourceName",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ResourceStore_fk_method_id",
                table: "ResourceStore",
                column: "fk_method_id");

            migrationBuilder.CreateIndex(
                name: "IX_ResourceStore_fk_resource_name_id",
                table: "ResourceStore",
                column: "fk_resource_name_id");

            migrationBuilder.CreateIndex(
                name: "Unique_FhirVer_ResName_ResId_ResVer",
                table: "ResourceStore",
                columns: new[] { "fk_fhir_version_id", "fk_resource_name_id", "resource_id", "version_id" },
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ResourceStore");

            migrationBuilder.DropTable(
                name: "FhirVersion");

            migrationBuilder.DropTable(
                name: "Method");

            migrationBuilder.DropTable(
                name: "ResourceName");
        }
    }
}
