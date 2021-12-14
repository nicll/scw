﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using ScwSvc.Models;

namespace ScwSvc.DataAccess.Impl.Migrations;

[DbContext(typeof(DbSysContext))]
[Migration("20201204001355_RenameDbStoreContext")]
partial class RenameDbStoreContext
{
    protected override void BuildTargetModel(ModelBuilder modelBuilder)
    {
#pragma warning disable 612, 618
        modelBuilder
            .HasDefaultSchema("scw1_sys")
            .UseIdentityByDefaultColumns()
            .HasAnnotation("Relational:MaxIdentifierLength", 63)
            .HasAnnotation("ProductVersion", "5.0.0");

        modelBuilder.Entity("ScwSvc.Models.DataSetColumn", b =>
            {
                b.Property<Guid>("TableRefId")
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

                b.HasKey("TableRefId", "Position");

                b.HasAlternateKey("TableRefId", "Name");

                b.ToTable("DataSetColumn");
            });

        modelBuilder.Entity("ScwSvc.Models.TableRef", b =>
            {
                b.Property<Guid>("TableRefId")
                    .ValueGeneratedOnAdd()
                    .HasColumnType("uuid");

                b.Property<string>("DisplayName")
                    .IsRequired()
                    .HasMaxLength(20)
                    .HasColumnType("character varying(20)");

                b.Property<Guid>("LookupName")
                    .HasMaxLength(24)
                    .HasColumnType("uuid")
                    .IsFixedLength(true);

                b.Property<Guid>("OwnerUserId")
                    .HasColumnType("uuid");

                b.Property<int>("Type")
                    .HasColumnType("integer");

                b.HasKey("TableRefId");

                b.HasIndex("OwnerUserId");

                b.ToTable("TableRefs");
            });

        modelBuilder.Entity("ScwSvc.Models.User", b =>
            {
                b.Property<Guid>("UserId")
                    .ValueGeneratedOnAdd()
                    .HasColumnType("uuid");

                b.Property<string>("Name")
                    .IsRequired()
                    .HasMaxLength(20)
                    .HasColumnType("character varying(20)");

                b.Property<byte[]>("PasswordHash")
                    .IsRequired()
                    .HasMaxLength(32)
                    .HasColumnType("bytea")
                    .IsFixedLength(true);

                b.Property<int>("Role")
                    .HasColumnType("integer");

                b.HasKey("UserId");

                b.HasAlternateKey("Name");

                b.ToTable("Users");
            });

        modelBuilder.Entity("TableRefUser", b =>
            {
                b.Property<Guid>("CollaborationsTableRefId")
                    .HasColumnType("uuid");

                b.Property<Guid>("CollaboratorsUserId")
                    .HasColumnType("uuid");

                b.HasKey("CollaborationsTableRefId", "CollaboratorsUserId");

                b.HasIndex("CollaboratorsUserId");

                b.ToTable("TableRefUser");
            });

        modelBuilder.Entity("ScwSvc.Models.DataSetColumn", b =>
            {
                b.HasOne("ScwSvc.Models.TableRef", "TableRef")
                    .WithMany("Columns")
                    .HasForeignKey("TableRefId")
                    .OnDelete(DeleteBehavior.Cascade)
                    .IsRequired();

                b.Navigation("TableRef");
            });

        modelBuilder.Entity("ScwSvc.Models.TableRef", b =>
            {
                b.HasOne("ScwSvc.Models.User", "Owner")
                    .WithMany("OwnTables")
                    .HasForeignKey("OwnerUserId")
                    .OnDelete(DeleteBehavior.Cascade)
                    .IsRequired();

                b.Navigation("Owner");
            });

        modelBuilder.Entity("TableRefUser", b =>
            {
                b.HasOne("ScwSvc.Models.TableRef", null)
                    .WithMany()
                    .HasForeignKey("CollaborationsTableRefId")
                    .OnDelete(DeleteBehavior.Cascade)
                    .IsRequired();

                b.HasOne("ScwSvc.Models.User", null)
                    .WithMany()
                    .HasForeignKey("CollaboratorsUserId")
                    .OnDelete(DeleteBehavior.Cascade)
                    .IsRequired();
            });

        modelBuilder.Entity("ScwSvc.Models.TableRef", b =>
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
