﻿using Microsoft.EntityFrameworkCore;
using QA_API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QA_API.Data
{
    public class QAContext : DbContext
    {
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<QACategory>()
                .HasIndex(c => c.Name)
                .IsUnique();
        }

        public QAContext(DbContextOptions<QAContext> opt) : base(opt)
        {

        }

        public DbSet<QACategory> Categories { get; set; }
        public DbSet<QAElement> Elements { get; set; }
    }
}
