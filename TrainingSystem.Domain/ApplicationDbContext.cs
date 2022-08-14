using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using TrainingSystem.Application.DTOs.Users;

namespace TrainingSystem.Domain
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }
        public DbSet<Trainer> Trainers { get; set; }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<Trainer>().ToTable("Trainer");
        }
        public DbSet<TrainingSystem.Application.DTOs.Users.LoginDTO> LoginDTO { get; set; }
    }
}
