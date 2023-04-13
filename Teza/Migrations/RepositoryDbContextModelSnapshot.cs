﻿// <auto-generated />
using System;
using Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace Teza.Migrations
{
    [DbContext(typeof(RepositoryDbContext))]
    partial class RepositoryDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Relational:MaxIdentifierLength", 63)
                .HasAnnotation("ProductVersion", "5.0.17")
                .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn);

            modelBuilder.Entity("Data.Entities.Collection", b =>
                {
                    b.Property<Guid?>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<string>("Name")
                        .HasColumnType("text");

                    b.Property<Guid?>("WorkspaceId")
                        .HasColumnType("uuid");

                    b.HasKey("Id");

                    b.HasIndex("WorkspaceId");

                    b.ToTable("Collection");
                });

            modelBuilder.Entity("Data.Entities.Folder", b =>
                {
                    b.Property<Guid?>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<Guid?>("CollectionId")
                        .HasColumnType("uuid");

                    b.Property<string>("Name")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("CollectionId");

                    b.ToTable("Folder");
                });

            modelBuilder.Entity("Data.Entities.History", b =>
                {
                    b.Property<Guid?>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.HasKey("Id");

                    b.ToTable("History");
                });

            modelBuilder.Entity("Data.Entities.Query", b =>
                {
                    b.Property<Guid?>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<Guid?>("CollectionId")
                        .HasColumnType("uuid");

                    b.Property<int?>("Count")
                        .HasColumnType("integer");

                    b.Property<string>("Documentation")
                        .HasColumnType("text");

                    b.Property<Guid?>("FolderId")
                        .HasColumnType("uuid");

                    b.Property<string>("Name")
                        .HasColumnType("text");

                    b.Property<string>("RawSql")
                        .HasColumnType("text");

                    b.Property<int?>("Size")
                        .HasColumnType("integer");

                    b.Property<string>("Snapshot")
                        .HasColumnType("jsonb");

                    b.HasKey("Id");

                    b.HasIndex("FolderId");

                    b.ToTable("Query");
                });

            modelBuilder.Entity("Data.Entities.User", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<string>("Email")
                        .HasColumnType("text");

                    b.Property<int?>("Role")
                        .HasColumnType("integer");

                    b.Property<Guid?>("UserId")
                        .HasColumnType("uuid");

                    b.Property<Guid?>("WorkspaceId")
                        .HasColumnType("uuid");

                    b.HasKey("Id");

                    b.HasIndex("WorkspaceId");

                    b.ToTable("User");
                });

            modelBuilder.Entity("Data.Entities.UserEmailConfirmation", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<string>("Email")
                        .HasColumnType("text");

                    b.Property<string>("EmailConfirmationToken")
                        .HasColumnType("text");

                    b.Property<bool>("IsEmailConfirmed")
                        .HasColumnType("boolean");

                    b.Property<DateTime>("ValidTo")
                        .HasColumnType("timestamp with time zone");

                    b.HasKey("Id");

                    b.ToTable("UserEmailConfirmation");
                });

            modelBuilder.Entity("Data.Entities.Workspace", b =>
                {
                    b.Property<Guid?>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<string>("DbConnectionString")
                        .HasColumnType("text");

                    b.Property<string>("EnvVariables")
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .HasColumnType("text");

                    b.Property<Guid?>("UserId")
                        .HasColumnType("uuid");

                    b.HasKey("Id");

                    b.ToTable("Workspace");
                });

            modelBuilder.Entity("Data.Entities.Collection", b =>
                {
                    b.HasOne("Data.Entities.Workspace", "Workspace")
                        .WithMany("Collections")
                        .HasForeignKey("WorkspaceId");

                    b.Navigation("Workspace");
                });

            modelBuilder.Entity("Data.Entities.Folder", b =>
                {
                    b.HasOne("Data.Entities.Collection", "Collection")
                        .WithMany("Folders")
                        .HasForeignKey("CollectionId");

                    b.Navigation("Collection");
                });

            modelBuilder.Entity("Data.Entities.Query", b =>
                {
                    b.HasOne("Data.Entities.Folder", "Folder")
                        .WithMany("Queries")
                        .HasForeignKey("FolderId");

                    b.Navigation("Folder");
                });

            modelBuilder.Entity("Data.Entities.User", b =>
                {
                    b.HasOne("Data.Entities.Workspace", "Workspace")
                        .WithMany("Collaborators")
                        .HasForeignKey("WorkspaceId");

                    b.Navigation("Workspace");
                });

            modelBuilder.Entity("Data.Entities.Collection", b =>
                {
                    b.Navigation("Folders");
                });

            modelBuilder.Entity("Data.Entities.Folder", b =>
                {
                    b.Navigation("Queries");
                });

            modelBuilder.Entity("Data.Entities.Workspace", b =>
                {
                    b.Navigation("Collaborators");

                    b.Navigation("Collections");
                });
#pragma warning restore 612, 618
        }
    }
}
