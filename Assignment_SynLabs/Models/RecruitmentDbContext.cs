using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace Assignment_SynLabs.Models
{
    public class RecruitmentDbContext : DbContext
    {
        public RecruitmentDbContext(DbContextOptions<RecruitmentDbContext> options) : base(options)
        {
        }

        // Define DbSet properties here
        public DbSet<User> Users { get; set; }
        public DbSet<Profile> Profiles { get; set; }
        public DbSet<Job> Jobs { get; set; }
        public DbSet<Application> Applications { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<User>()
           .HasOne(u => u.Profile)
           .WithOne(p => p.Applicant)
           .HasForeignKey<Profile>(p => p.ApplicantId);

            modelBuilder.Entity<Job>()
           .HasKey(j => j.Id);

            modelBuilder.Entity<Job>()
                .Property(j => j.Id)
                .HasColumnName("Id");


        }
    }
}
