using System;
using System.Collections.Generic;
using DemoSecurityApp.EntityModel;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace DemoSecurityApp.Context
{
    public class SecurityDBContext : DbContext
    {
        public SecurityDBContext(DbContextOptions<SecurityDBContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>(entity =>
            {

            });
        }
        public virtual DbSet<User> Users { get; set; } = null!;

    }
}
