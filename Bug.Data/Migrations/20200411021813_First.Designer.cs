﻿// <auto-generated />
using System;
using Bug.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace Bug.Data.Migrations
{
    [DbContext(typeof(AppDbContext))]
    [Migration("20200411021813_First")]
    partial class First
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn)
                .HasAnnotation("ProductVersion", "3.1.1")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            modelBuilder.Entity("Bug.Logic.DomainModel.FhirVersion", b =>
                {
                    b.Property<int>("Id")
                        .HasColumnName("id")
                        .HasColumnType("integer");

                    b.Property<string>("Code")
                        .IsRequired()
                        .HasColumnName("code")
                        .HasColumnType("character varying(128)")
                        .HasMaxLength(128);

                    b.Property<DateTime>("Created")
                        .HasColumnName("created")
                        .HasColumnType("timestamp without time zone");

                    b.Property<DateTime>("Updated")
                        .HasColumnName("updated")
                        .HasColumnType("timestamp without time zone");

                    b.HasKey("Id");

                    b.ToTable("FhirVersion");
                });

            modelBuilder.Entity("Bug.Logic.DomainModel.HttpStatusCode", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("id")
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<string>("Code")
                        .IsRequired()
                        .HasColumnName("code")
                        .HasColumnType("character varying(128)")
                        .HasMaxLength(128);

                    b.Property<DateTime>("Created")
                        .HasColumnName("created")
                        .HasColumnType("timestamp without time zone");

                    b.Property<int>("Number")
                        .HasColumnName("number")
                        .HasColumnType("integer");

                    b.Property<DateTime>("Updated")
                        .HasColumnName("updated")
                        .HasColumnType("timestamp without time zone");

                    b.HasKey("Id");

                    b.HasIndex("Number")
                        .IsUnique()
                        .HasName("UniqueIx_HttpStatusCode_number");

                    b.ToTable("HttpStatusCode");
                });

            modelBuilder.Entity("Bug.Logic.DomainModel.IndexDateTime", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("id")
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<DateTime?>("High")
                        .HasColumnName("high")
                        .HasColumnType("timestamp without time zone");

                    b.Property<DateTime?>("Low")
                        .HasColumnName("low")
                        .HasColumnType("timestamp without time zone");

                    b.Property<int>("ResourceStoreId")
                        .HasColumnName("resourcestore_id")
                        .HasColumnType("integer");

                    b.Property<int>("SearchParameterId")
                        .HasColumnName("searchparameter_id")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("High")
                        .HasName("Ix_IndexDateTime_High");

                    b.HasIndex("Low")
                        .HasName("Ix_IndexDateTime_Low");

                    b.HasIndex("ResourceStoreId");

                    b.HasIndex("SearchParameterId");

                    b.ToTable("IndexDateTime");
                });

            modelBuilder.Entity("Bug.Logic.DomainModel.IndexQuantity", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("id")
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<string>("Code")
                        .HasColumnName("code")
                        .HasColumnType("character varying(128)")
                        .HasMaxLength(128);

                    b.Property<string>("CodeHigh")
                        .HasColumnName("code_high")
                        .HasColumnType("character varying(128)")
                        .HasMaxLength(128);

                    b.Property<int?>("Comparator")
                        .HasColumnName("comparator")
                        .HasColumnType("integer");

                    b.Property<int?>("ComparatorHigh")
                        .HasColumnName("comparator_high")
                        .HasColumnType("integer");

                    b.Property<decimal?>("Quantity")
                        .HasColumnName("quantity")
                        .HasColumnType("numeric");

                    b.Property<decimal?>("QuantityHigh")
                        .HasColumnName("quantity_high")
                        .HasColumnType("numeric");

                    b.Property<int>("ResourceStoreId")
                        .HasColumnName("resourcestore_id")
                        .HasColumnType("integer");

                    b.Property<int>("SearchParameterId")
                        .HasColumnName("searchparameter_id")
                        .HasColumnType("integer");

                    b.Property<string>("System")
                        .HasColumnName("system")
                        .HasColumnType("character varying(450)")
                        .HasMaxLength(450);

                    b.Property<string>("SystemHigh")
                        .HasColumnName("system_high")
                        .HasColumnType("character varying(450)")
                        .HasMaxLength(450);

                    b.Property<string>("Unit")
                        .HasColumnName("unit")
                        .HasColumnType("character varying(450)")
                        .HasMaxLength(450);

                    b.Property<string>("UnitHigh")
                        .HasColumnName("unit_high")
                        .HasColumnType("character varying(450)")
                        .HasMaxLength(450);

                    b.HasKey("Id");

                    b.HasIndex("Code")
                        .HasName("Ix_IndexQuantity_Low");

                    b.HasIndex("CodeHigh")
                        .HasName("Ix_IndexQuantity_CodeHigh");

                    b.HasIndex("Quantity")
                        .HasName("Ix_IndexQuantity_Quantity");

                    b.HasIndex("QuantityHigh")
                        .HasName("Ix_IndexQuantity_QuantityHigh");

                    b.HasIndex("ResourceStoreId");

                    b.HasIndex("SearchParameterId");

                    b.HasIndex("System")
                        .HasName("Ix_IndexQuantity_System");

                    b.HasIndex("SystemHigh")
                        .HasName("Ix_IndexQuantity_SystemHigh");

                    b.ToTable("IndexQuantity");
                });

            modelBuilder.Entity("Bug.Logic.DomainModel.IndexReference", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("id")
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<string>("CanonicalVersionId")
                        .HasColumnName("canonical_version_id")
                        .HasColumnType("text");

                    b.Property<string>("ResourceId")
                        .IsRequired()
                        .HasColumnName("resource_id")
                        .HasColumnType("text");

                    b.Property<int>("ResourceStoreId")
                        .HasColumnName("resourcestore_id")
                        .HasColumnType("integer");

                    b.Property<int>("ResourceTypeId")
                        .HasColumnName("resourcetype_id")
                        .HasColumnType("integer");

                    b.Property<int>("SearchParameterId")
                        .HasColumnName("searchparameter_id")
                        .HasColumnType("integer");

                    b.Property<int?>("ServiceBaseUrlId")
                        .IsRequired()
                        .HasColumnName("servicebaseurl_id")
                        .HasColumnType("integer");

                    b.Property<string>("VersionId")
                        .HasColumnName("version_id")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("CanonicalVersionId")
                        .HasName("Ix_IndexReference_CanonicalVersionId");

                    b.HasIndex("ResourceId")
                        .HasName("Ix_IndexReference_ResourceId");

                    b.HasIndex("ResourceStoreId");

                    b.HasIndex("ResourceTypeId");

                    b.HasIndex("SearchParameterId");

                    b.HasIndex("ServiceBaseUrlId");

                    b.HasIndex("VersionId")
                        .HasName("Ix_IndexReference_VersionId");

                    b.ToTable("IndexReference");
                });

            modelBuilder.Entity("Bug.Logic.DomainModel.IndexString", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("id")
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<int>("ResourceStoreId")
                        .HasColumnName("resourcestore_id")
                        .HasColumnType("integer");

                    b.Property<int>("SearchParameterId")
                        .HasColumnName("searchparameter_id")
                        .HasColumnType("integer");

                    b.Property<string>("String")
                        .HasColumnName("string")
                        .HasColumnType("character varying(450)")
                        .HasMaxLength(450);

                    b.HasKey("Id");

                    b.HasIndex("ResourceStoreId");

                    b.HasIndex("SearchParameterId");

                    b.HasIndex("String")
                        .HasName("Ix_IndexString_String");

                    b.ToTable("IndexString");
                });

            modelBuilder.Entity("Bug.Logic.DomainModel.IndexToken", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("id")
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<string>("Code")
                        .HasColumnName("code")
                        .HasColumnType("character varying(128)")
                        .HasMaxLength(128);

                    b.Property<int>("ResourceStoreId")
                        .HasColumnName("resourcestore_id")
                        .HasColumnType("integer");

                    b.Property<int>("SearchParameterId")
                        .HasColumnName("searchparameter_id")
                        .HasColumnType("integer");

                    b.Property<string>("System")
                        .HasColumnName("system")
                        .HasColumnType("character varying(450)")
                        .HasMaxLength(450);

                    b.HasKey("Id");

                    b.HasIndex("Code")
                        .HasName("Ix_IndexToken_Code");

                    b.HasIndex("ResourceStoreId");

                    b.HasIndex("SearchParameterId");

                    b.HasIndex("System")
                        .HasName("Ix_IndexToken_System");

                    b.ToTable("IndexToken");
                });

            modelBuilder.Entity("Bug.Logic.DomainModel.IndexUri", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("id")
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<int>("ResourceStoreId")
                        .HasColumnName("resourcestore_id")
                        .HasColumnType("integer");

                    b.Property<int>("SearchParameterId")
                        .HasColumnName("searchparameter_id")
                        .HasColumnType("integer");

                    b.Property<string>("Uri")
                        .IsRequired()
                        .HasColumnName("uri")
                        .HasColumnType("character varying(450)")
                        .HasMaxLength(450);

                    b.HasKey("Id");

                    b.HasIndex("ResourceStoreId");

                    b.HasIndex("SearchParameterId");

                    b.HasIndex("Uri")
                        .HasName("Ix_IndexUri_Uri");

                    b.ToTable("IndexUri");
                });

            modelBuilder.Entity("Bug.Logic.DomainModel.Method", b =>
                {
                    b.Property<int>("Id")
                        .HasColumnName("id")
                        .HasColumnType("integer");

                    b.Property<string>("Code")
                        .IsRequired()
                        .HasColumnName("code")
                        .HasColumnType("character varying(128)")
                        .HasMaxLength(128);

                    b.Property<DateTime>("Created")
                        .HasColumnName("created")
                        .HasColumnType("timestamp without time zone");

                    b.Property<DateTime>("Updated")
                        .HasColumnName("updated")
                        .HasColumnType("timestamp without time zone");

                    b.HasKey("Id");

                    b.ToTable("Method");
                });

            modelBuilder.Entity("Bug.Logic.DomainModel.ResourceStore", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("id")
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<string>("ContainedId")
                        .HasColumnName("contained_id")
                        .HasColumnType("character varying(128)")
                        .HasMaxLength(128);

                    b.Property<DateTime>("Created")
                        .HasColumnName("created")
                        .HasColumnType("timestamp without time zone");

                    b.Property<int>("FhirVersionId")
                        .HasColumnName("fhirversion_id")
                        .HasColumnType("integer");

                    b.Property<int>("HttpStatusCodeId")
                        .HasColumnName("httpstatuscode_id")
                        .HasColumnType("integer");

                    b.Property<bool>("IsCurrent")
                        .HasColumnName("is_current")
                        .HasColumnType("boolean");

                    b.Property<bool>("IsDeleted")
                        .HasColumnName("is_deleted")
                        .HasColumnType("boolean");

                    b.Property<DateTime>("LastUpdated")
                        .HasColumnName("last_updated")
                        .HasColumnType("timestamp without time zone");

                    b.Property<int>("MethodId")
                        .HasColumnName("method_id")
                        .HasColumnType("integer");

                    b.Property<byte[]>("ResourceBlob")
                        .HasColumnName("resource_blob")
                        .HasColumnType("bytea");

                    b.Property<string>("ResourceId")
                        .IsRequired()
                        .HasColumnName("resource_id")
                        .HasColumnType("character varying(128)")
                        .HasMaxLength(128);

                    b.Property<int>("ResourceTypeId")
                        .HasColumnName("resourcetype_id")
                        .HasColumnType("integer");

                    b.Property<DateTime>("Updated")
                        .HasColumnName("updated")
                        .HasColumnType("timestamp without time zone");

                    b.Property<int>("VersionId")
                        .HasColumnName("version_id")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("HttpStatusCodeId");

                    b.HasIndex("LastUpdated")
                        .HasName("Ix_ResourceStore_LastUpdated");

                    b.HasIndex("MethodId");

                    b.HasIndex("ResourceTypeId");

                    b.HasIndex("FhirVersionId", "ResourceTypeId", "ResourceId", "ContainedId", "VersionId")
                        .IsUnique()
                        .HasName("UniqueIx_ResourceStore_FhirVer_ResType_ResId_ContId_ResVer");

                    b.ToTable("ResourceStore");
                });

            modelBuilder.Entity("Bug.Logic.DomainModel.ResourceType", b =>
                {
                    b.Property<int>("Id")
                        .HasColumnName("id")
                        .HasColumnType("integer");

                    b.Property<string>("Code")
                        .IsRequired()
                        .HasColumnName("code")
                        .HasColumnType("character varying(50)")
                        .HasMaxLength(50);

                    b.Property<DateTime>("Created")
                        .HasColumnName("created")
                        .HasColumnType("timestamp without time zone");

                    b.Property<DateTime>("Updated")
                        .HasColumnName("updated")
                        .HasColumnType("timestamp without time zone");

                    b.HasKey("Id");

                    b.ToTable("ResourceType");
                });

            modelBuilder.Entity("Bug.Logic.DomainModel.SearchParamType", b =>
                {
                    b.Property<int>("Id")
                        .HasColumnName("id")
                        .HasColumnType("integer");

                    b.Property<string>("Code")
                        .IsRequired()
                        .HasColumnName("code")
                        .HasColumnType("character varying(128)")
                        .HasMaxLength(128);

                    b.Property<DateTime>("Created")
                        .HasColumnName("created")
                        .HasColumnType("timestamp without time zone");

                    b.Property<DateTime>("Updated")
                        .HasColumnName("updated")
                        .HasColumnType("timestamp without time zone");

                    b.HasKey("Id");

                    b.ToTable("SearchParamType");
                });

            modelBuilder.Entity("Bug.Logic.DomainModel.SearchParameter", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("id")
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<DateTime>("Created")
                        .HasColumnName("created")
                        .HasColumnType("timestamp without time zone");

                    b.Property<string>("Description")
                        .HasColumnName("description")
                        .HasColumnType("text");

                    b.Property<string>("FhirPath")
                        .HasColumnName("fhir_path")
                        .HasColumnType("text");

                    b.Property<int>("FhirVersionId")
                        .HasColumnName("fhirversion_id")
                        .HasColumnType("integer");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnName("name")
                        .HasColumnType("character varying(128)")
                        .HasMaxLength(128);

                    b.Property<int>("SearchParamTypeId")
                        .HasColumnName("searchparamtype_id")
                        .HasColumnType("integer");

                    b.Property<DateTime>("Updated")
                        .HasColumnName("updated")
                        .HasColumnType("timestamp without time zone");

                    b.Property<string>("Url")
                        .HasColumnName("url")
                        .HasColumnType("character varying(450)")
                        .HasMaxLength(450);

                    b.HasKey("Id");

                    b.HasIndex("FhirVersionId");

                    b.HasIndex("Name")
                        .HasName("Ix_SearchParameter_Url");

                    b.HasIndex("SearchParamTypeId");

                    b.ToTable("SearchParameter");
                });

            modelBuilder.Entity("Bug.Logic.DomainModel.SearchParameterComponent", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("id")
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<DateTime>("Created")
                        .HasColumnName("created")
                        .HasColumnType("timestamp without time zone");

                    b.Property<string>("Definition")
                        .IsRequired()
                        .HasColumnName("definition")
                        .HasColumnType("character varying(450)")
                        .HasMaxLength(450);

                    b.Property<string>("Expression")
                        .IsRequired()
                        .HasColumnName("expression")
                        .HasColumnType("text");

                    b.Property<int>("SearchParameterId")
                        .HasColumnName("searchparameter_id")
                        .HasColumnType("integer");

                    b.Property<DateTime>("Updated")
                        .HasColumnName("updated")
                        .HasColumnType("timestamp without time zone");

                    b.HasKey("Id");

                    b.HasIndex("SearchParameterId");

                    b.ToTable("SearchParameterComponent");
                });

            modelBuilder.Entity("Bug.Logic.DomainModel.SearchParameterResourceType", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("id")
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<DateTime>("Created")
                        .HasColumnName("created")
                        .HasColumnType("timestamp without time zone");

                    b.Property<int>("ResourceTypeId")
                        .HasColumnName("resourcetype_id")
                        .HasColumnType("integer");

                    b.Property<int>("SearchParameterId")
                        .HasColumnName("searchparameter_id")
                        .HasColumnType("integer");

                    b.Property<DateTime>("Updated")
                        .HasColumnName("updated")
                        .HasColumnType("timestamp without time zone");

                    b.HasKey("Id");

                    b.HasIndex("ResourceTypeId");

                    b.HasIndex("SearchParameterId");

                    b.ToTable("SearchParameterResourceType");
                });

            modelBuilder.Entity("Bug.Logic.DomainModel.SearchParameterTargetResourceType", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("id")
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<DateTime>("Created")
                        .HasColumnName("created")
                        .HasColumnType("timestamp without time zone");

                    b.Property<int>("ResourceTypeId")
                        .HasColumnName("resourcetype_id")
                        .HasColumnType("integer");

                    b.Property<int>("SearchParameterId")
                        .HasColumnName("searchparameter_id")
                        .HasColumnType("integer");

                    b.Property<DateTime>("Updated")
                        .HasColumnName("updated")
                        .HasColumnType("timestamp without time zone");

                    b.HasKey("Id");

                    b.HasIndex("ResourceTypeId");

                    b.HasIndex("SearchParameterId");

                    b.ToTable("SearchParameterTargetResourceType");
                });

            modelBuilder.Entity("Bug.Logic.DomainModel.ServiceBaseUrl", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("id")
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<DateTime>("Created")
                        .HasColumnName("created")
                        .HasColumnType("timestamp without time zone");

                    b.Property<int>("FhirVersionId")
                        .HasColumnName("fhirversion_id")
                        .HasColumnType("integer");

                    b.Property<bool>("IsPrimary")
                        .HasColumnName("is_primary")
                        .HasColumnType("boolean");

                    b.Property<DateTime>("Updated")
                        .HasColumnName("updated")
                        .HasColumnType("timestamp without time zone");

                    b.Property<string>("Url")
                        .IsRequired()
                        .HasColumnName("url")
                        .HasColumnType("character varying(450)")
                        .HasMaxLength(450);

                    b.HasKey("Id");

                    b.HasIndex("Url", "FhirVersionId")
                        .IsUnique()
                        .HasName("Ix_ServiceBaseUrl_Url_FhirVersionId");

                    b.ToTable("ServiceBaseUrl");
                });

            modelBuilder.Entity("Bug.Logic.DomainModel.IndexDateTime", b =>
                {
                    b.HasOne("Bug.Logic.DomainModel.ResourceStore", "ResourceStore")
                        .WithMany("DateTimeIndexList")
                        .HasForeignKey("ResourceStoreId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Bug.Logic.DomainModel.SearchParameter", "SearchParameter")
                        .WithMany()
                        .HasForeignKey("SearchParameterId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Bug.Logic.DomainModel.IndexQuantity", b =>
                {
                    b.HasOne("Bug.Logic.DomainModel.ResourceStore", "ResourceStore")
                        .WithMany("QuantityIndexList")
                        .HasForeignKey("ResourceStoreId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Bug.Logic.DomainModel.SearchParameter", "SearchParameter")
                        .WithMany()
                        .HasForeignKey("SearchParameterId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Bug.Logic.DomainModel.IndexReference", b =>
                {
                    b.HasOne("Bug.Logic.DomainModel.ResourceStore", "ResourceStore")
                        .WithMany("ReferenceIndexList")
                        .HasForeignKey("ResourceStoreId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Bug.Logic.DomainModel.ResourceType", "ResourceType")
                        .WithMany()
                        .HasForeignKey("ResourceTypeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Bug.Logic.DomainModel.SearchParameter", "SearchParameter")
                        .WithMany()
                        .HasForeignKey("SearchParameterId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Bug.Logic.DomainModel.ServiceBaseUrl", "ServiceBaseUrl")
                        .WithMany()
                        .HasForeignKey("ServiceBaseUrlId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Bug.Logic.DomainModel.IndexString", b =>
                {
                    b.HasOne("Bug.Logic.DomainModel.ResourceStore", "ResourceStore")
                        .WithMany("StringIndexList")
                        .HasForeignKey("ResourceStoreId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Bug.Logic.DomainModel.SearchParameter", "SearchParameter")
                        .WithMany()
                        .HasForeignKey("SearchParameterId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Bug.Logic.DomainModel.IndexToken", b =>
                {
                    b.HasOne("Bug.Logic.DomainModel.ResourceStore", "ResourceStore")
                        .WithMany("TokenIndexList")
                        .HasForeignKey("ResourceStoreId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Bug.Logic.DomainModel.SearchParameter", "SearchParameter")
                        .WithMany()
                        .HasForeignKey("SearchParameterId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Bug.Logic.DomainModel.IndexUri", b =>
                {
                    b.HasOne("Bug.Logic.DomainModel.ResourceStore", "ResourceStore")
                        .WithMany("UriIndexList")
                        .HasForeignKey("ResourceStoreId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Bug.Logic.DomainModel.SearchParameter", "SearchParameter")
                        .WithMany()
                        .HasForeignKey("SearchParameterId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Bug.Logic.DomainModel.ResourceStore", b =>
                {
                    b.HasOne("Bug.Logic.DomainModel.FhirVersion", "FhirVersion")
                        .WithMany()
                        .HasForeignKey("FhirVersionId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Bug.Logic.DomainModel.HttpStatusCode", "HttpStatusCode")
                        .WithMany()
                        .HasForeignKey("HttpStatusCodeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Bug.Logic.DomainModel.Method", "Method")
                        .WithMany()
                        .HasForeignKey("MethodId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Bug.Logic.DomainModel.ResourceType", "ResourceType")
                        .WithMany()
                        .HasForeignKey("ResourceTypeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Bug.Logic.DomainModel.SearchParameter", b =>
                {
                    b.HasOne("Bug.Logic.DomainModel.FhirVersion", "FhirVersion")
                        .WithMany()
                        .HasForeignKey("FhirVersionId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Bug.Logic.DomainModel.SearchParamType", "SearchParamType")
                        .WithMany()
                        .HasForeignKey("SearchParamTypeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Bug.Logic.DomainModel.SearchParameterComponent", b =>
                {
                    b.HasOne("Bug.Logic.DomainModel.SearchParameter", "SearchParameter")
                        .WithMany("ComponentList")
                        .HasForeignKey("SearchParameterId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Bug.Logic.DomainModel.SearchParameterResourceType", b =>
                {
                    b.HasOne("Bug.Logic.DomainModel.ResourceType", "ResourceType")
                        .WithMany()
                        .HasForeignKey("ResourceTypeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Bug.Logic.DomainModel.SearchParameter", "SearchParameter")
                        .WithMany("ResourceTypeList")
                        .HasForeignKey("SearchParameterId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Bug.Logic.DomainModel.SearchParameterTargetResourceType", b =>
                {
                    b.HasOne("Bug.Logic.DomainModel.ResourceType", "ResourceType")
                        .WithMany()
                        .HasForeignKey("ResourceTypeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Bug.Logic.DomainModel.SearchParameter", "SearchParameter")
                        .WithMany("TargetResourceTypeList")
                        .HasForeignKey("SearchParameterId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });
#pragma warning restore 612, 618
        }
    }
}