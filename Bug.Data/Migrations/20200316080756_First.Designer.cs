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
    [Migration("20200316080756_First")]
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
                        .HasName("Unique_Number");

                    b.ToTable("HttpStatusCode");
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

                    b.Property<DateTime>("Created")
                        .HasColumnName("created")
                        .HasColumnType("timestamp without time zone");

                    b.Property<int>("FkFhirVersionId")
                        .HasColumnName("fk_fhirversion_id")
                        .HasColumnType("integer");

                    b.Property<int>("FkHttpStatusCodeId")
                        .HasColumnName("fk_httpstatuscode_id")
                        .HasColumnType("integer");

                    b.Property<int>("FkMethodId")
                        .HasColumnName("fk_method_id")
                        .HasColumnType("integer");

                    b.Property<int>("FkResourceTypeId")
                        .HasColumnName("fk_resourcetype_id")
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

                    b.Property<byte[]>("ResourceBlob")
                        .HasColumnName("resource_blob")
                        .HasColumnType("bytea");

                    b.Property<string>("ResourceId")
                        .IsRequired()
                        .HasColumnName("resource_id")
                        .HasColumnType("character varying(128)")
                        .HasMaxLength(128);

                    b.Property<DateTime>("Updated")
                        .HasColumnName("updated")
                        .HasColumnType("timestamp without time zone");

                    b.Property<int>("VersionId")
                        .HasColumnName("version_id")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("FkHttpStatusCodeId");

                    b.HasIndex("FkMethodId");

                    b.HasIndex("FkResourceTypeId");

                    b.HasIndex("LastUpdated")
                        .HasName("Ix_LastUpdated");

                    b.HasIndex("FkFhirVersionId", "FkResourceTypeId", "ResourceId", "VersionId")
                        .IsUnique()
                        .HasName("UniqueIx_FhirVer_ResType_ResId_ResVer");

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

                    b.Property<int>("FkFhirVersionId")
                        .HasColumnName("fk_fhirversion_id")
                        .HasColumnType("integer");

                    b.Property<int>("FkSearchParamTypeId")
                        .HasColumnName("fk_searchparamtype_id")
                        .HasColumnType("integer");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnName("name")
                        .HasColumnType("character varying(128)")
                        .HasMaxLength(128);

                    b.Property<DateTime>("Updated")
                        .HasColumnName("updated")
                        .HasColumnType("timestamp without time zone");

                    b.Property<string>("Url")
                        .HasColumnName("url")
                        .HasColumnType("character varying(450)")
                        .HasMaxLength(450);

                    b.HasKey("Id");

                    b.HasIndex("FkFhirVersionId");

                    b.HasIndex("FkSearchParamTypeId");

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

                    b.Property<int>("FkSearchParameterId")
                        .HasColumnType("integer");

                    b.Property<DateTime>("Updated")
                        .HasColumnName("updated")
                        .HasColumnType("timestamp without time zone");

                    b.HasKey("Id");

                    b.HasIndex("FkSearchParameterId");

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

                    b.Property<int>("FkResourceTypeId")
                        .HasColumnName("fk_resourcetype_id")
                        .HasColumnType("integer");

                    b.Property<int>("FkSearchParameterId")
                        .HasColumnType("integer");

                    b.Property<DateTime>("Updated")
                        .HasColumnName("updated")
                        .HasColumnType("timestamp without time zone");

                    b.HasKey("Id");

                    b.HasIndex("FkResourceTypeId");

                    b.HasIndex("FkSearchParameterId");

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

                    b.Property<int>("FkResourceTypeId")
                        .HasColumnName("fk_resourcetype_id")
                        .HasColumnType("integer");

                    b.Property<int>("FkSearchParameterId")
                        .HasColumnType("integer");

                    b.Property<DateTime>("Updated")
                        .HasColumnName("updated")
                        .HasColumnType("timestamp without time zone");

                    b.HasKey("Id");

                    b.HasIndex("FkResourceTypeId");

                    b.HasIndex("FkSearchParameterId");

                    b.ToTable("SearchParameterTargetResourceType");
                });

            modelBuilder.Entity("Bug.Logic.DomainModel.ResourceStore", b =>
                {
                    b.HasOne("Bug.Logic.DomainModel.FhirVersion", "FhirVersion")
                        .WithMany()
                        .HasForeignKey("FkFhirVersionId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Bug.Logic.DomainModel.HttpStatusCode", "HttpStatusCode")
                        .WithMany()
                        .HasForeignKey("FkHttpStatusCodeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Bug.Logic.DomainModel.Method", "Method")
                        .WithMany()
                        .HasForeignKey("FkMethodId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Bug.Logic.DomainModel.ResourceType", "ResourceType")
                        .WithMany()
                        .HasForeignKey("FkResourceTypeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Bug.Logic.DomainModel.SearchParameter", b =>
                {
                    b.HasOne("Bug.Logic.DomainModel.FhirVersion", "FhirVersion")
                        .WithMany()
                        .HasForeignKey("FkFhirVersionId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Bug.Logic.DomainModel.SearchParamType", "SearchParamType")
                        .WithMany()
                        .HasForeignKey("FkSearchParamTypeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Bug.Logic.DomainModel.SearchParameterComponent", b =>
                {
                    b.HasOne("Bug.Logic.DomainModel.SearchParameter", "SearchParameter")
                        .WithMany("ComponentList")
                        .HasForeignKey("FkSearchParameterId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Bug.Logic.DomainModel.SearchParameterResourceType", b =>
                {
                    b.HasOne("Bug.Logic.DomainModel.ResourceType", "ResourceType")
                        .WithMany()
                        .HasForeignKey("FkResourceTypeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Bug.Logic.DomainModel.SearchParameter", "SearchParameter")
                        .WithMany("ResourceTypeList")
                        .HasForeignKey("FkSearchParameterId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Bug.Logic.DomainModel.SearchParameterTargetResourceType", b =>
                {
                    b.HasOne("Bug.Logic.DomainModel.ResourceType", "ResourceType")
                        .WithMany()
                        .HasForeignKey("FkResourceTypeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Bug.Logic.DomainModel.SearchParameter", "SearchParameter")
                        .WithMany("TargetResourceTypeList")
                        .HasForeignKey("FkSearchParameterId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });
#pragma warning restore 612, 618
        }
    }
}