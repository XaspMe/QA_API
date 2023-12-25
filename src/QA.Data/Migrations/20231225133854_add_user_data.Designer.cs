﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using QA.Data;

#nullable disable

namespace QA.Data.Migrations
{
    [DbContext(typeof(QaContext))]
    [Migration("20231225133854_add_user_data")]
    partial class add_user_data
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.13")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("QA.Models.Models.FeedBack", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Message")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("Feedbacks");
                });

            modelBuilder.Entity("QA.Models.Models.QACategory", b =>
                {
                    b.Property<int>("Id")
                        .HasColumnType("int");

                    b.Property<Guid?>("AuthorId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(250)
                        .HasColumnType("nvarchar(250)");

                    b.HasKey("Id");

                    b.HasIndex("AuthorId");

                    b.HasIndex("Name")
                        .IsUnique();

                    b.ToTable("Categories");
                });

            modelBuilder.Entity("QA.Models.Models.QAElement", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Answer")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("CategoryId")
                        .HasColumnType("int");

                    b.Property<string>("Question")
                        .IsRequired()
                        .HasMaxLength(250)
                        .HasColumnType("nvarchar(250)");

                    b.HasKey("Id");

                    b.HasIndex("CategoryId");

                    b.ToTable("Elements");
                });

            modelBuilder.Entity("QA.Models.Models.User", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<int?>("CurrentQuestionId")
                        .HasColumnType("int");

                    b.Property<string>("DisplayName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<long>("TelegramChatId")
                        .HasColumnType("bigint");

                    b.Property<string>("UserCurrentStep")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UserInputMode")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UserName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("isAdmin")
                        .HasColumnType("bit");

                    b.HasKey("Id");

                    b.HasIndex("CurrentQuestionId");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("UserFavoriteCategories", b =>
                {
                    b.Property<Guid>("UserId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<int>("QACategoryId")
                        .HasColumnType("int");

                    b.HasKey("UserId", "QACategoryId");

                    b.HasIndex("QACategoryId");

                    b.ToTable("UserFavoriteCategories");
                });

            modelBuilder.Entity("UserFavoriteElements", b =>
                {
                    b.Property<Guid>("UserId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<int>("QAElementId")
                        .HasColumnType("int");

                    b.HasKey("UserId", "QAElementId");

                    b.HasIndex("QAElementId");

                    b.ToTable("UserFavoriteElements");
                });

            modelBuilder.Entity("QA.Models.Models.FeedBack", b =>
                {
                    b.HasOne("QA.Models.Models.User", "User")
                        .WithMany("FeedBacks")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("QA.Models.Models.QACategory", b =>
                {
                    b.HasOne("QA.Models.Models.User", "Author")
                        .WithMany("CategoriesCreated")
                        .HasForeignKey("AuthorId")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.Navigation("Author");
                });

            modelBuilder.Entity("QA.Models.Models.QAElement", b =>
                {
                    b.HasOne("QA.Models.Models.QACategory", "Category")
                        .WithMany()
                        .HasForeignKey("CategoryId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Category");
                });

            modelBuilder.Entity("QA.Models.Models.User", b =>
                {
                    b.HasOne("QA.Models.Models.QAElement", "CurrentQuestion")
                        .WithMany()
                        .HasForeignKey("CurrentQuestionId");

                    b.Navigation("CurrentQuestion");
                });

            modelBuilder.Entity("UserFavoriteCategories", b =>
                {
                    b.HasOne("QA.Models.Models.QACategory", null)
                        .WithMany()
                        .HasForeignKey("QACategoryId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.HasOne("QA.Models.Models.User", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();
                });

            modelBuilder.Entity("UserFavoriteElements", b =>
                {
                    b.HasOne("QA.Models.Models.QAElement", null)
                        .WithMany()
                        .HasForeignKey("QAElementId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("QA.Models.Models.User", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("QA.Models.Models.User", b =>
                {
                    b.Navigation("CategoriesCreated");

                    b.Navigation("FeedBacks");
                });
#pragma warning restore 612, 618
        }
    }
}
