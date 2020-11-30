﻿// <auto-generated />
using System;
using DMicroservices.DataAccess.Tests;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace DMicroservices.DataAccess.Tests.Migrations
{
    [DbContext(typeof(MasterContext))]
    [Migration("20201130115055_Init")]
    partial class Init
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.1.14-servicing-32113");

            modelBuilder.Entity("DMicroservices.DataAccess.Tests.Models.ClassModel", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Name");

                    b.HasKey("Id");

                    b.ToTable("Classes");
                });

            modelBuilder.Entity("DMicroservices.DataAccess.Tests.Models.StudentModel", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<long?>("ClassModelId");

                    b.Property<string>("Name");

                    b.Property<string>("Surname");

                    b.HasKey("Id");

                    b.HasIndex("ClassModelId");

                    b.ToTable("Students");
                });

            modelBuilder.Entity("DMicroservices.DataAccess.Tests.Models.StudentModel", b =>
                {
                    b.HasOne("DMicroservices.DataAccess.Tests.Models.ClassModel")
                        .WithMany("Students")
                        .HasForeignKey("ClassModelId");
                });
#pragma warning restore 612, 618
        }
    }
}
