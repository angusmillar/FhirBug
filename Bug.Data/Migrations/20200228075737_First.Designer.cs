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
    [Migration("20200228075737_First")]
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
                        .ValueGeneratedOnAdd()
                        .HasColumnName("id")
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<int>("FhirMajorVersion")
                        .HasColumnName("fhir_major_version")
                        .HasColumnType("integer");

                    b.Property<string>("VersionCode")
                        .IsRequired()
                        .HasColumnName("code")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("FhirVersion");
                });

            modelBuilder.Entity("Bug.Logic.DomainModel.Method", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("id")
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<string>("Code")
                        .IsRequired()
                        .HasColumnName("code")
                        .HasColumnType("text");

                    b.Property<int>("HttpVerb")
                        .HasColumnName("http_verb")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.ToTable("Method");
                });

            modelBuilder.Entity("Bug.Logic.DomainModel.ResourceName", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("id")
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnName("name")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("ResourceName");
                });

            modelBuilder.Entity("Bug.Logic.DomainModel.ResourceStore", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("id")
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<int>("FkFhirVersionId")
                        .HasColumnName("fk_fhir_version_id")
                        .HasColumnType("integer");

                    b.Property<int>("FkMethodId")
                        .HasColumnName("fk_method_id")
                        .HasColumnType("integer");

                    b.Property<int>("FkResourceNameId")
                        .HasColumnName("fk_resource_name_id")
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
                        .HasColumnType("text");

                    b.Property<int>("VersionId")
                        .HasColumnName("version_id")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("FkMethodId");

                    b.HasIndex("FkResourceNameId");

                    b.HasIndex("FkFhirVersionId", "FkResourceNameId", "ResourceId", "VersionId")
                        .IsUnique()
                        .HasName("Unique_FhirVer_ResName_ResId_ResVer");

                    b.ToTable("ResourceStore");
                });

            modelBuilder.Entity("Bug.Logic.DomainModel.ResourceStore", b =>
                {
                    b.HasOne("Bug.Logic.DomainModel.FhirVersion", "FhirVersion")
                        .WithMany()
                        .HasForeignKey("FkFhirVersionId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Bug.Logic.DomainModel.Method", "Method")
                        .WithMany()
                        .HasForeignKey("FkMethodId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Bug.Logic.DomainModel.ResourceName", "ResourceName")
                        .WithMany()
                        .HasForeignKey("FkResourceNameId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });
#pragma warning restore 612, 618
        }
    }
}