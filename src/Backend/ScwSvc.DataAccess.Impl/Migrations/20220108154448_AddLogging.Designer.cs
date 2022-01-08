﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using ScwSvc.DataAccess.Impl;

#nullable disable

namespace ScwSvc.DataAccess.Impl.Migrations
{
    [DbContext(typeof(DbSysContext))]
    [Migration("20220108154448_AddLogging")]
    partial class AddLogging
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasDefaultSchema("scw1_sys")
                .HasAnnotation("ProductVersion", "6.0.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("ScwSvc.Models.DataSetColumn", b =>
                {
                    b.Property<Guid>("TableId")
                        .HasColumnType("uuid");

                    b.Property<byte>("Position")
                        .HasColumnType("smallint");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(20)
                        .HasColumnType("character varying(20)");

                    b.Property<bool>("Nullable")
                        .HasColumnType("boolean");

                    b.Property<int>("Type")
                        .HasColumnType("integer");

                    b.HasKey("TableId", "Position");

                    b.HasAlternateKey("TableId", "Name");

                    b.ToTable("DataSetColumn", "scw1_sys");
                });

            modelBuilder.Entity("ScwSvc.Models.LogEvent", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<string>("Discriminator")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<DateTime>("Timestamp")
                        .HasColumnType("timestamp with time zone");

                    b.HasKey("Id");

                    b.ToTable("Log", "scw1_sys");

                    b.HasDiscriminator<string>("Discriminator").HasValue("LogEvent");
                });

            modelBuilder.Entity("ScwSvc.Models.Table", b =>
                {
                    b.Property<Guid>("TableId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<DateTime>("CreationDate")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("DisplayName")
                        .IsRequired()
                        .HasMaxLength(20)
                        .HasColumnType("character varying(20)");

                    b.Property<Guid>("LookupName")
                        .HasMaxLength(24)
                        .HasColumnType("uuid")
                        .IsFixedLength();

                    b.Property<Guid>("OwnerUserId")
                        .HasColumnType("uuid");

                    b.Property<int>("TableType")
                        .HasColumnType("integer");

                    b.HasKey("TableId");

                    b.HasIndex("OwnerUserId");

                    b.ToTable("Tables", "scw1_sys");
                });

            modelBuilder.Entity("ScwSvc.Models.User", b =>
                {
                    b.Property<Guid>("UserId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<DateTime>("CreationDate")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(20)
                        .HasColumnType("character varying(20)");

                    b.Property<byte[]>("PasswordHash")
                        .IsRequired()
                        .HasMaxLength(32)
                        .HasColumnType("bytea")
                        .IsFixedLength();

                    b.Property<int>("Role")
                        .HasColumnType("integer");

                    b.HasKey("UserId");

                    b.HasIndex("Name")
                        .IsUnique();

                    b.ToTable("Users", "scw1_sys");
                });

            modelBuilder.Entity("TableUser", b =>
                {
                    b.Property<Guid>("CollaborationsTableId")
                        .HasColumnType("uuid");

                    b.Property<Guid>("CollaboratorsUserId")
                        .HasColumnType("uuid");

                    b.HasKey("CollaborationsTableId", "CollaboratorsUserId");

                    b.HasIndex("CollaboratorsUserId");

                    b.ToTable("TableUser", "scw1_sys");
                });

            modelBuilder.Entity("ScwSvc.Models.LookupLogEvent", b =>
                {
                    b.HasBaseType("ScwSvc.Models.LogEvent");

                    b.Property<Guid>("TableId")
                        .HasColumnType("uuid");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uuid");

                    b.HasDiscriminator().HasValue("LookupLogEvent");
                });

            modelBuilder.Entity("ScwSvc.Models.TableLogEvent", b =>
                {
                    b.HasBaseType("ScwSvc.Models.LogEvent");

                    b.Property<Guid>("TableId")
                        .HasColumnType("uuid")
                        .HasColumnName("TableLogEvent_TableId");

                    b.HasDiscriminator().HasValue("TableLogEvent");
                });

            modelBuilder.Entity("ScwSvc.Models.UserLogEvent", b =>
                {
                    b.HasBaseType("ScwSvc.Models.LogEvent");

                    b.Property<int>("UserAction")
                        .HasColumnType("integer");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uuid")
                        .HasColumnName("UserLogEvent_UserId");

                    b.HasDiscriminator().HasValue("UserLogEvent");
                });

            modelBuilder.Entity("ScwSvc.Models.TableCollaboratorLogEvent", b =>
                {
                    b.HasBaseType("ScwSvc.Models.TableLogEvent");

                    b.Property<int>("CollaboratorAction")
                        .HasColumnType("integer");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uuid")
                        .HasColumnName("TableCollaboratorLogEvent_UserId");

                    b.HasDiscriminator().HasValue("TableCollaboratorLogEvent");
                });

            modelBuilder.Entity("ScwSvc.Models.TableDefinitionLogEvent", b =>
                {
                    b.HasBaseType("ScwSvc.Models.TableLogEvent");

                    b.Property<int>("TableAction")
                        .HasColumnType("integer");

                    b.Property<int>("TableType")
                        .HasColumnType("integer");

                    b.HasDiscriminator().HasValue("TableDefinitionLogEvent");
                });

            modelBuilder.Entity("ScwSvc.Models.DataSetColumn", b =>
                {
                    b.HasOne("ScwSvc.Models.Table", "Table")
                        .WithMany("Columns")
                        .HasForeignKey("TableId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Table");
                });

            modelBuilder.Entity("ScwSvc.Models.Table", b =>
                {
                    b.HasOne("ScwSvc.Models.User", "Owner")
                        .WithMany("OwnTables")
                        .HasForeignKey("OwnerUserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Owner");
                });

            modelBuilder.Entity("TableUser", b =>
                {
                    b.HasOne("ScwSvc.Models.Table", null)
                        .WithMany()
                        .HasForeignKey("CollaborationsTableId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("ScwSvc.Models.User", null)
                        .WithMany()
                        .HasForeignKey("CollaboratorsUserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("ScwSvc.Models.Table", b =>
                {
                    b.Navigation("Columns");
                });

            modelBuilder.Entity("ScwSvc.Models.User", b =>
                {
                    b.Navigation("OwnTables");
                });
#pragma warning restore 612, 618
        }
    }
}
