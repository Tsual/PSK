using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using PSK.Models;

namespace PSK.Migrations
{
    [DbContext(typeof(APPDbContext))]
    [Migration("20170731023127_init")]
    partial class init
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("ProductVersion", "1.1.2");

            modelBuilder.Entity("PSK.Models.Recording", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("key");

                    b.Property<int>("uid");

                    b.Property<string>("value");

                    b.HasKey("ID");

                    b.ToTable("Recording");
                });

            modelBuilder.Entity("PSK.Models.StringSequenceObjA", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Data");

                    b.HasKey("ID");

                    b.ToTable("StringSequenceA");
                });

            modelBuilder.Entity("PSK.Models.StringSequenceObjB", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Data");

                    b.HasKey("ID");

                    b.ToTable("StringSequenceB");
                });

            modelBuilder.Entity("PSK.Models.User", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("pid");

                    b.Property<string>("pwd");

                    b.HasKey("ID");

                    b.ToTable("User");
                });
        }
    }
}
