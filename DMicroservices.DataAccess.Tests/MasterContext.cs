using System;
using System.Collections.Generic;
using System.Text;
using DMicroservices.DataAccess.Tests.Models;
using Microsoft.EntityFrameworkCore;

namespace DMicroservices.DataAccess.Tests
{
    public class MasterContext : DbContext
    {
        public MasterContext()
        {

        }
        public MasterContext(DbContextOptions<MasterContext> options)
        {

        }

        public DbSet<ClassModel> Classes { get; set; }
        public DbSet<StudentModel> Students { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ClassModel>().HasKey(s => s.Id);
            modelBuilder.Entity<StudentModel>().HasKey(i => i.Id);
        }


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseMySQL(Environment.GetEnvironmentVariable("MYSQL_URI"));
            }
            base.OnConfiguring(optionsBuilder);
        }
    }
}
