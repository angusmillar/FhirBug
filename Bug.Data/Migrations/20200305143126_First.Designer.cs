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
    [Migration("20200305143126_First")]
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
                        .HasColumnType("text");

                    b.Property<DateTime>("Created")
                        .HasColumnName("created")
                        .HasColumnType("timestamp without time zone");

                    b.Property<DateTime>("Updated")
                        .HasColumnName("updated")
                        .HasColumnType("timestamp without time zone");

                    b.HasKey("Id");

                    b.ToTable("FhirVersion");

                    b.HasData(
                        new
                        {
                            Id = 0,
                            Code = "Stu3",
                            Created = new DateTime(2020, 3, 5, 14, 31, 21, 450, DateTimeKind.Utc).AddTicks(5506),
                            Updated = new DateTime(2020, 3, 5, 14, 31, 21, 450, DateTimeKind.Utc).AddTicks(5506)
                        },
                        new
                        {
                            Id = 1,
                            Code = "R4",
                            Created = new DateTime(2020, 3, 5, 14, 31, 21, 450, DateTimeKind.Utc).AddTicks(5506),
                            Updated = new DateTime(2020, 3, 5, 14, 31, 21, 450, DateTimeKind.Utc).AddTicks(5506)
                        });
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
                        .HasColumnType("text");

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

                    b.HasData(
                        new
                        {
                            Id = 1,
                            Code = "Continue",
                            Created = new DateTime(2020, 3, 5, 14, 31, 21, 450, DateTimeKind.Utc).AddTicks(5506),
                            Number = 100,
                            Updated = new DateTime(2020, 3, 5, 14, 31, 21, 450, DateTimeKind.Utc).AddTicks(5506)
                        },
                        new
                        {
                            Id = 2,
                            Code = "SwitchingProtocols",
                            Created = new DateTime(2020, 3, 5, 14, 31, 21, 450, DateTimeKind.Utc).AddTicks(5506),
                            Number = 101,
                            Updated = new DateTime(2020, 3, 5, 14, 31, 21, 450, DateTimeKind.Utc).AddTicks(5506)
                        },
                        new
                        {
                            Id = 3,
                            Code = "Processing",
                            Created = new DateTime(2020, 3, 5, 14, 31, 21, 450, DateTimeKind.Utc).AddTicks(5506),
                            Number = 102,
                            Updated = new DateTime(2020, 3, 5, 14, 31, 21, 450, DateTimeKind.Utc).AddTicks(5506)
                        },
                        new
                        {
                            Id = 4,
                            Code = "EarlyHints",
                            Created = new DateTime(2020, 3, 5, 14, 31, 21, 450, DateTimeKind.Utc).AddTicks(5506),
                            Number = 103,
                            Updated = new DateTime(2020, 3, 5, 14, 31, 21, 450, DateTimeKind.Utc).AddTicks(5506)
                        },
                        new
                        {
                            Id = 5,
                            Code = "OK",
                            Created = new DateTime(2020, 3, 5, 14, 31, 21, 450, DateTimeKind.Utc).AddTicks(5506),
                            Number = 200,
                            Updated = new DateTime(2020, 3, 5, 14, 31, 21, 450, DateTimeKind.Utc).AddTicks(5506)
                        },
                        new
                        {
                            Id = 6,
                            Code = "Created",
                            Created = new DateTime(2020, 3, 5, 14, 31, 21, 450, DateTimeKind.Utc).AddTicks(5506),
                            Number = 201,
                            Updated = new DateTime(2020, 3, 5, 14, 31, 21, 450, DateTimeKind.Utc).AddTicks(5506)
                        },
                        new
                        {
                            Id = 7,
                            Code = "Accepted",
                            Created = new DateTime(2020, 3, 5, 14, 31, 21, 450, DateTimeKind.Utc).AddTicks(5506),
                            Number = 202,
                            Updated = new DateTime(2020, 3, 5, 14, 31, 21, 450, DateTimeKind.Utc).AddTicks(5506)
                        },
                        new
                        {
                            Id = 8,
                            Code = "NonAuthoritativeInformation",
                            Created = new DateTime(2020, 3, 5, 14, 31, 21, 450, DateTimeKind.Utc).AddTicks(5506),
                            Number = 203,
                            Updated = new DateTime(2020, 3, 5, 14, 31, 21, 450, DateTimeKind.Utc).AddTicks(5506)
                        },
                        new
                        {
                            Id = 9,
                            Code = "NoContent",
                            Created = new DateTime(2020, 3, 5, 14, 31, 21, 450, DateTimeKind.Utc).AddTicks(5506),
                            Number = 204,
                            Updated = new DateTime(2020, 3, 5, 14, 31, 21, 450, DateTimeKind.Utc).AddTicks(5506)
                        },
                        new
                        {
                            Id = 10,
                            Code = "ResetContent",
                            Created = new DateTime(2020, 3, 5, 14, 31, 21, 450, DateTimeKind.Utc).AddTicks(5506),
                            Number = 205,
                            Updated = new DateTime(2020, 3, 5, 14, 31, 21, 450, DateTimeKind.Utc).AddTicks(5506)
                        },
                        new
                        {
                            Id = 11,
                            Code = "PartialContent",
                            Created = new DateTime(2020, 3, 5, 14, 31, 21, 450, DateTimeKind.Utc).AddTicks(5506),
                            Number = 206,
                            Updated = new DateTime(2020, 3, 5, 14, 31, 21, 450, DateTimeKind.Utc).AddTicks(5506)
                        },
                        new
                        {
                            Id = 12,
                            Code = "MultiStatus",
                            Created = new DateTime(2020, 3, 5, 14, 31, 21, 450, DateTimeKind.Utc).AddTicks(5506),
                            Number = 207,
                            Updated = new DateTime(2020, 3, 5, 14, 31, 21, 450, DateTimeKind.Utc).AddTicks(5506)
                        },
                        new
                        {
                            Id = 13,
                            Code = "AlreadyReported",
                            Created = new DateTime(2020, 3, 5, 14, 31, 21, 450, DateTimeKind.Utc).AddTicks(5506),
                            Number = 208,
                            Updated = new DateTime(2020, 3, 5, 14, 31, 21, 450, DateTimeKind.Utc).AddTicks(5506)
                        },
                        new
                        {
                            Id = 14,
                            Code = "IMUsed",
                            Created = new DateTime(2020, 3, 5, 14, 31, 21, 450, DateTimeKind.Utc).AddTicks(5506),
                            Number = 226,
                            Updated = new DateTime(2020, 3, 5, 14, 31, 21, 450, DateTimeKind.Utc).AddTicks(5506)
                        },
                        new
                        {
                            Id = 15,
                            Code = "Ambiguous",
                            Created = new DateTime(2020, 3, 5, 14, 31, 21, 450, DateTimeKind.Utc).AddTicks(5506),
                            Number = 300,
                            Updated = new DateTime(2020, 3, 5, 14, 31, 21, 450, DateTimeKind.Utc).AddTicks(5506)
                        },
                        new
                        {
                            Id = 16,
                            Code = "Moved",
                            Created = new DateTime(2020, 3, 5, 14, 31, 21, 450, DateTimeKind.Utc).AddTicks(5506),
                            Number = 301,
                            Updated = new DateTime(2020, 3, 5, 14, 31, 21, 450, DateTimeKind.Utc).AddTicks(5506)
                        },
                        new
                        {
                            Id = 17,
                            Code = "Redirect",
                            Created = new DateTime(2020, 3, 5, 14, 31, 21, 450, DateTimeKind.Utc).AddTicks(5506),
                            Number = 302,
                            Updated = new DateTime(2020, 3, 5, 14, 31, 21, 450, DateTimeKind.Utc).AddTicks(5506)
                        },
                        new
                        {
                            Id = 18,
                            Code = "RedirectMethod",
                            Created = new DateTime(2020, 3, 5, 14, 31, 21, 450, DateTimeKind.Utc).AddTicks(5506),
                            Number = 303,
                            Updated = new DateTime(2020, 3, 5, 14, 31, 21, 450, DateTimeKind.Utc).AddTicks(5506)
                        },
                        new
                        {
                            Id = 19,
                            Code = "NotModified",
                            Created = new DateTime(2020, 3, 5, 14, 31, 21, 450, DateTimeKind.Utc).AddTicks(5506),
                            Number = 304,
                            Updated = new DateTime(2020, 3, 5, 14, 31, 21, 450, DateTimeKind.Utc).AddTicks(5506)
                        },
                        new
                        {
                            Id = 20,
                            Code = "UseProxy",
                            Created = new DateTime(2020, 3, 5, 14, 31, 21, 450, DateTimeKind.Utc).AddTicks(5506),
                            Number = 305,
                            Updated = new DateTime(2020, 3, 5, 14, 31, 21, 450, DateTimeKind.Utc).AddTicks(5506)
                        },
                        new
                        {
                            Id = 21,
                            Code = "Unused",
                            Created = new DateTime(2020, 3, 5, 14, 31, 21, 450, DateTimeKind.Utc).AddTicks(5506),
                            Number = 306,
                            Updated = new DateTime(2020, 3, 5, 14, 31, 21, 450, DateTimeKind.Utc).AddTicks(5506)
                        },
                        new
                        {
                            Id = 22,
                            Code = "TemporaryRedirect",
                            Created = new DateTime(2020, 3, 5, 14, 31, 21, 450, DateTimeKind.Utc).AddTicks(5506),
                            Number = 307,
                            Updated = new DateTime(2020, 3, 5, 14, 31, 21, 450, DateTimeKind.Utc).AddTicks(5506)
                        },
                        new
                        {
                            Id = 23,
                            Code = "PermanentRedirect",
                            Created = new DateTime(2020, 3, 5, 14, 31, 21, 450, DateTimeKind.Utc).AddTicks(5506),
                            Number = 308,
                            Updated = new DateTime(2020, 3, 5, 14, 31, 21, 450, DateTimeKind.Utc).AddTicks(5506)
                        },
                        new
                        {
                            Id = 24,
                            Code = "BadRequest",
                            Created = new DateTime(2020, 3, 5, 14, 31, 21, 450, DateTimeKind.Utc).AddTicks(5506),
                            Number = 400,
                            Updated = new DateTime(2020, 3, 5, 14, 31, 21, 450, DateTimeKind.Utc).AddTicks(5506)
                        },
                        new
                        {
                            Id = 25,
                            Code = "Unauthorized",
                            Created = new DateTime(2020, 3, 5, 14, 31, 21, 450, DateTimeKind.Utc).AddTicks(5506),
                            Number = 401,
                            Updated = new DateTime(2020, 3, 5, 14, 31, 21, 450, DateTimeKind.Utc).AddTicks(5506)
                        },
                        new
                        {
                            Id = 26,
                            Code = "PaymentRequired",
                            Created = new DateTime(2020, 3, 5, 14, 31, 21, 450, DateTimeKind.Utc).AddTicks(5506),
                            Number = 402,
                            Updated = new DateTime(2020, 3, 5, 14, 31, 21, 450, DateTimeKind.Utc).AddTicks(5506)
                        },
                        new
                        {
                            Id = 27,
                            Code = "Forbidden",
                            Created = new DateTime(2020, 3, 5, 14, 31, 21, 450, DateTimeKind.Utc).AddTicks(5506),
                            Number = 403,
                            Updated = new DateTime(2020, 3, 5, 14, 31, 21, 450, DateTimeKind.Utc).AddTicks(5506)
                        },
                        new
                        {
                            Id = 28,
                            Code = "NotFound",
                            Created = new DateTime(2020, 3, 5, 14, 31, 21, 450, DateTimeKind.Utc).AddTicks(5506),
                            Number = 404,
                            Updated = new DateTime(2020, 3, 5, 14, 31, 21, 450, DateTimeKind.Utc).AddTicks(5506)
                        },
                        new
                        {
                            Id = 29,
                            Code = "MethodNotAllowed",
                            Created = new DateTime(2020, 3, 5, 14, 31, 21, 450, DateTimeKind.Utc).AddTicks(5506),
                            Number = 405,
                            Updated = new DateTime(2020, 3, 5, 14, 31, 21, 450, DateTimeKind.Utc).AddTicks(5506)
                        },
                        new
                        {
                            Id = 30,
                            Code = "NotAcceptable",
                            Created = new DateTime(2020, 3, 5, 14, 31, 21, 450, DateTimeKind.Utc).AddTicks(5506),
                            Number = 406,
                            Updated = new DateTime(2020, 3, 5, 14, 31, 21, 450, DateTimeKind.Utc).AddTicks(5506)
                        },
                        new
                        {
                            Id = 31,
                            Code = "ProxyAuthenticationRequired",
                            Created = new DateTime(2020, 3, 5, 14, 31, 21, 450, DateTimeKind.Utc).AddTicks(5506),
                            Number = 407,
                            Updated = new DateTime(2020, 3, 5, 14, 31, 21, 450, DateTimeKind.Utc).AddTicks(5506)
                        },
                        new
                        {
                            Id = 32,
                            Code = "RequestTimeout",
                            Created = new DateTime(2020, 3, 5, 14, 31, 21, 450, DateTimeKind.Utc).AddTicks(5506),
                            Number = 408,
                            Updated = new DateTime(2020, 3, 5, 14, 31, 21, 450, DateTimeKind.Utc).AddTicks(5506)
                        },
                        new
                        {
                            Id = 33,
                            Code = "Conflict",
                            Created = new DateTime(2020, 3, 5, 14, 31, 21, 450, DateTimeKind.Utc).AddTicks(5506),
                            Number = 409,
                            Updated = new DateTime(2020, 3, 5, 14, 31, 21, 450, DateTimeKind.Utc).AddTicks(5506)
                        },
                        new
                        {
                            Id = 34,
                            Code = "Gone",
                            Created = new DateTime(2020, 3, 5, 14, 31, 21, 450, DateTimeKind.Utc).AddTicks(5506),
                            Number = 410,
                            Updated = new DateTime(2020, 3, 5, 14, 31, 21, 450, DateTimeKind.Utc).AddTicks(5506)
                        },
                        new
                        {
                            Id = 35,
                            Code = "LengthRequired",
                            Created = new DateTime(2020, 3, 5, 14, 31, 21, 450, DateTimeKind.Utc).AddTicks(5506),
                            Number = 411,
                            Updated = new DateTime(2020, 3, 5, 14, 31, 21, 450, DateTimeKind.Utc).AddTicks(5506)
                        },
                        new
                        {
                            Id = 36,
                            Code = "PreconditionFailed",
                            Created = new DateTime(2020, 3, 5, 14, 31, 21, 450, DateTimeKind.Utc).AddTicks(5506),
                            Number = 412,
                            Updated = new DateTime(2020, 3, 5, 14, 31, 21, 450, DateTimeKind.Utc).AddTicks(5506)
                        },
                        new
                        {
                            Id = 37,
                            Code = "RequestEntityTooLarge",
                            Created = new DateTime(2020, 3, 5, 14, 31, 21, 450, DateTimeKind.Utc).AddTicks(5506),
                            Number = 413,
                            Updated = new DateTime(2020, 3, 5, 14, 31, 21, 450, DateTimeKind.Utc).AddTicks(5506)
                        },
                        new
                        {
                            Id = 38,
                            Code = "RequestUriTooLong",
                            Created = new DateTime(2020, 3, 5, 14, 31, 21, 450, DateTimeKind.Utc).AddTicks(5506),
                            Number = 414,
                            Updated = new DateTime(2020, 3, 5, 14, 31, 21, 450, DateTimeKind.Utc).AddTicks(5506)
                        },
                        new
                        {
                            Id = 39,
                            Code = "UnsupportedMediaType",
                            Created = new DateTime(2020, 3, 5, 14, 31, 21, 450, DateTimeKind.Utc).AddTicks(5506),
                            Number = 415,
                            Updated = new DateTime(2020, 3, 5, 14, 31, 21, 450, DateTimeKind.Utc).AddTicks(5506)
                        },
                        new
                        {
                            Id = 40,
                            Code = "RequestedRangeNotSatisfiable",
                            Created = new DateTime(2020, 3, 5, 14, 31, 21, 450, DateTimeKind.Utc).AddTicks(5506),
                            Number = 416,
                            Updated = new DateTime(2020, 3, 5, 14, 31, 21, 450, DateTimeKind.Utc).AddTicks(5506)
                        },
                        new
                        {
                            Id = 41,
                            Code = "ExpectationFailed",
                            Created = new DateTime(2020, 3, 5, 14, 31, 21, 450, DateTimeKind.Utc).AddTicks(5506),
                            Number = 417,
                            Updated = new DateTime(2020, 3, 5, 14, 31, 21, 450, DateTimeKind.Utc).AddTicks(5506)
                        },
                        new
                        {
                            Id = 42,
                            Code = "MisdirectedRequest",
                            Created = new DateTime(2020, 3, 5, 14, 31, 21, 450, DateTimeKind.Utc).AddTicks(5506),
                            Number = 421,
                            Updated = new DateTime(2020, 3, 5, 14, 31, 21, 450, DateTimeKind.Utc).AddTicks(5506)
                        },
                        new
                        {
                            Id = 43,
                            Code = "UnprocessableEntity",
                            Created = new DateTime(2020, 3, 5, 14, 31, 21, 450, DateTimeKind.Utc).AddTicks(5506),
                            Number = 422,
                            Updated = new DateTime(2020, 3, 5, 14, 31, 21, 450, DateTimeKind.Utc).AddTicks(5506)
                        },
                        new
                        {
                            Id = 44,
                            Code = "Locked",
                            Created = new DateTime(2020, 3, 5, 14, 31, 21, 450, DateTimeKind.Utc).AddTicks(5506),
                            Number = 423,
                            Updated = new DateTime(2020, 3, 5, 14, 31, 21, 450, DateTimeKind.Utc).AddTicks(5506)
                        },
                        new
                        {
                            Id = 45,
                            Code = "FailedDependency",
                            Created = new DateTime(2020, 3, 5, 14, 31, 21, 450, DateTimeKind.Utc).AddTicks(5506),
                            Number = 424,
                            Updated = new DateTime(2020, 3, 5, 14, 31, 21, 450, DateTimeKind.Utc).AddTicks(5506)
                        },
                        new
                        {
                            Id = 46,
                            Code = "UpgradeRequired",
                            Created = new DateTime(2020, 3, 5, 14, 31, 21, 450, DateTimeKind.Utc).AddTicks(5506),
                            Number = 426,
                            Updated = new DateTime(2020, 3, 5, 14, 31, 21, 450, DateTimeKind.Utc).AddTicks(5506)
                        },
                        new
                        {
                            Id = 47,
                            Code = "PreconditionRequired",
                            Created = new DateTime(2020, 3, 5, 14, 31, 21, 450, DateTimeKind.Utc).AddTicks(5506),
                            Number = 428,
                            Updated = new DateTime(2020, 3, 5, 14, 31, 21, 450, DateTimeKind.Utc).AddTicks(5506)
                        },
                        new
                        {
                            Id = 48,
                            Code = "TooManyRequests",
                            Created = new DateTime(2020, 3, 5, 14, 31, 21, 450, DateTimeKind.Utc).AddTicks(5506),
                            Number = 429,
                            Updated = new DateTime(2020, 3, 5, 14, 31, 21, 450, DateTimeKind.Utc).AddTicks(5506)
                        },
                        new
                        {
                            Id = 49,
                            Code = "RequestHeaderFieldsTooLarge",
                            Created = new DateTime(2020, 3, 5, 14, 31, 21, 450, DateTimeKind.Utc).AddTicks(5506),
                            Number = 431,
                            Updated = new DateTime(2020, 3, 5, 14, 31, 21, 450, DateTimeKind.Utc).AddTicks(5506)
                        },
                        new
                        {
                            Id = 50,
                            Code = "UnavailableForLegalReasons",
                            Created = new DateTime(2020, 3, 5, 14, 31, 21, 450, DateTimeKind.Utc).AddTicks(5506),
                            Number = 451,
                            Updated = new DateTime(2020, 3, 5, 14, 31, 21, 450, DateTimeKind.Utc).AddTicks(5506)
                        },
                        new
                        {
                            Id = 51,
                            Code = "InternalServerError",
                            Created = new DateTime(2020, 3, 5, 14, 31, 21, 450, DateTimeKind.Utc).AddTicks(5506),
                            Number = 500,
                            Updated = new DateTime(2020, 3, 5, 14, 31, 21, 450, DateTimeKind.Utc).AddTicks(5506)
                        },
                        new
                        {
                            Id = 52,
                            Code = "NotImplemented",
                            Created = new DateTime(2020, 3, 5, 14, 31, 21, 450, DateTimeKind.Utc).AddTicks(5506),
                            Number = 501,
                            Updated = new DateTime(2020, 3, 5, 14, 31, 21, 450, DateTimeKind.Utc).AddTicks(5506)
                        },
                        new
                        {
                            Id = 53,
                            Code = "BadGateway",
                            Created = new DateTime(2020, 3, 5, 14, 31, 21, 450, DateTimeKind.Utc).AddTicks(5506),
                            Number = 502,
                            Updated = new DateTime(2020, 3, 5, 14, 31, 21, 450, DateTimeKind.Utc).AddTicks(5506)
                        },
                        new
                        {
                            Id = 54,
                            Code = "ServiceUnavailable",
                            Created = new DateTime(2020, 3, 5, 14, 31, 21, 450, DateTimeKind.Utc).AddTicks(5506),
                            Number = 503,
                            Updated = new DateTime(2020, 3, 5, 14, 31, 21, 450, DateTimeKind.Utc).AddTicks(5506)
                        },
                        new
                        {
                            Id = 55,
                            Code = "GatewayTimeout",
                            Created = new DateTime(2020, 3, 5, 14, 31, 21, 450, DateTimeKind.Utc).AddTicks(5506),
                            Number = 504,
                            Updated = new DateTime(2020, 3, 5, 14, 31, 21, 450, DateTimeKind.Utc).AddTicks(5506)
                        },
                        new
                        {
                            Id = 56,
                            Code = "HttpVersionNotSupported",
                            Created = new DateTime(2020, 3, 5, 14, 31, 21, 450, DateTimeKind.Utc).AddTicks(5506),
                            Number = 505,
                            Updated = new DateTime(2020, 3, 5, 14, 31, 21, 450, DateTimeKind.Utc).AddTicks(5506)
                        },
                        new
                        {
                            Id = 57,
                            Code = "VariantAlsoNegotiates",
                            Created = new DateTime(2020, 3, 5, 14, 31, 21, 450, DateTimeKind.Utc).AddTicks(5506),
                            Number = 506,
                            Updated = new DateTime(2020, 3, 5, 14, 31, 21, 450, DateTimeKind.Utc).AddTicks(5506)
                        },
                        new
                        {
                            Id = 58,
                            Code = "InsufficientStorage",
                            Created = new DateTime(2020, 3, 5, 14, 31, 21, 450, DateTimeKind.Utc).AddTicks(5506),
                            Number = 507,
                            Updated = new DateTime(2020, 3, 5, 14, 31, 21, 450, DateTimeKind.Utc).AddTicks(5506)
                        },
                        new
                        {
                            Id = 59,
                            Code = "LoopDetected",
                            Created = new DateTime(2020, 3, 5, 14, 31, 21, 450, DateTimeKind.Utc).AddTicks(5506),
                            Number = 508,
                            Updated = new DateTime(2020, 3, 5, 14, 31, 21, 450, DateTimeKind.Utc).AddTicks(5506)
                        },
                        new
                        {
                            Id = 60,
                            Code = "NotExtended",
                            Created = new DateTime(2020, 3, 5, 14, 31, 21, 450, DateTimeKind.Utc).AddTicks(5506),
                            Number = 510,
                            Updated = new DateTime(2020, 3, 5, 14, 31, 21, 450, DateTimeKind.Utc).AddTicks(5506)
                        },
                        new
                        {
                            Id = 61,
                            Code = "NetworkAuthenticationRequired",
                            Created = new DateTime(2020, 3, 5, 14, 31, 21, 450, DateTimeKind.Utc).AddTicks(5506),
                            Number = 511,
                            Updated = new DateTime(2020, 3, 5, 14, 31, 21, 450, DateTimeKind.Utc).AddTicks(5506)
                        });
                });

            modelBuilder.Entity("Bug.Logic.DomainModel.Method", b =>
                {
                    b.Property<int>("Id")
                        .HasColumnName("id")
                        .HasColumnType("integer");

                    b.Property<string>("Code")
                        .IsRequired()
                        .HasColumnName("code")
                        .HasColumnType("text");

                    b.Property<DateTime>("Created")
                        .HasColumnName("created")
                        .HasColumnType("timestamp without time zone");

                    b.Property<DateTime>("Updated")
                        .HasColumnName("updated")
                        .HasColumnType("timestamp without time zone");

                    b.HasKey("Id");

                    b.ToTable("Method");

                    b.HasData(
                        new
                        {
                            Id = 4,
                            Code = "DELETE",
                            Created = new DateTime(2020, 3, 5, 14, 31, 21, 450, DateTimeKind.Utc).AddTicks(5506),
                            Updated = new DateTime(2020, 3, 5, 14, 31, 21, 450, DateTimeKind.Utc).AddTicks(5506)
                        },
                        new
                        {
                            Id = 3,
                            Code = "GET",
                            Created = new DateTime(2020, 3, 5, 14, 31, 21, 450, DateTimeKind.Utc).AddTicks(5506),
                            Updated = new DateTime(2020, 3, 5, 14, 31, 21, 450, DateTimeKind.Utc).AddTicks(5506)
                        },
                        new
                        {
                            Id = 5,
                            Code = "HEAD",
                            Created = new DateTime(2020, 3, 5, 14, 31, 21, 450, DateTimeKind.Utc).AddTicks(5506),
                            Updated = new DateTime(2020, 3, 5, 14, 31, 21, 450, DateTimeKind.Utc).AddTicks(5506)
                        },
                        new
                        {
                            Id = 6,
                            Code = "PATCH",
                            Created = new DateTime(2020, 3, 5, 14, 31, 21, 450, DateTimeKind.Utc).AddTicks(5506),
                            Updated = new DateTime(2020, 3, 5, 14, 31, 21, 450, DateTimeKind.Utc).AddTicks(5506)
                        },
                        new
                        {
                            Id = 1,
                            Code = "POST",
                            Created = new DateTime(2020, 3, 5, 14, 31, 21, 450, DateTimeKind.Utc).AddTicks(5506),
                            Updated = new DateTime(2020, 3, 5, 14, 31, 21, 450, DateTimeKind.Utc).AddTicks(5506)
                        },
                        new
                        {
                            Id = 2,
                            Code = "PUT",
                            Created = new DateTime(2020, 3, 5, 14, 31, 21, 450, DateTimeKind.Utc).AddTicks(5506),
                            Updated = new DateTime(2020, 3, 5, 14, 31, 21, 450, DateTimeKind.Utc).AddTicks(5506)
                        });
                });

            modelBuilder.Entity("Bug.Logic.DomainModel.ResourceName", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("id")
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<DateTime>("Created")
                        .HasColumnName("created")
                        .HasColumnType("timestamp without time zone");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnName("name")
                        .HasColumnType("text");

                    b.Property<DateTime>("Updated")
                        .HasColumnName("updated")
                        .HasColumnType("timestamp without time zone");

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

                    b.Property<int>("FkResourceNameId")
                        .HasColumnName("fk_resourcename_id")
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

                    b.Property<DateTime>("Updated")
                        .HasColumnName("updated")
                        .HasColumnType("timestamp without time zone");

                    b.Property<int>("VersionId")
                        .HasColumnName("version_id")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("FkHttpStatusCodeId");

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