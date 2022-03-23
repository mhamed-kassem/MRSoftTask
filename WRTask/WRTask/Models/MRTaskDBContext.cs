using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

#nullable disable

namespace WRTask.Models
{
    public partial class MRTaskDBContext : DbContext
    {
        public MRTaskDBContext()
        {
        }

        public MRTaskDBContext(DbContextOptions<MRTaskDBContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Person> People { get; set; }
        public virtual DbSet<Sponsor> Sponsors { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                //#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                //optionsBuilder.UseSqlServer("Server=.; Database=MRTaskDB; Trusted_Connection=True;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Person>(entity =>
            {
                entity.ToTable("Person");

                entity.HasIndex(e => e.UserName, "UQ__Person__C9F28456E70C4732")
                    .IsUnique();

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(30);

                entity.Property(e => e.Password)
                    .IsRequired()
                    .HasMaxLength(20);

                entity.Property(e => e.UserName)
                    .IsRequired()
                    .HasMaxLength(20);
            });

            modelBuilder.Entity<Sponsor>(entity =>
            {
                entity.HasKey(e => e.Code)
                    .HasName("SponsorPK");

                entity.ToTable("Sponsor");

                entity.Property(e => e.Code).ValueGeneratedNever();

                entity.Property(e => e.AuthorizedByPersonId).HasColumnName("AuthorizedByPersonID");

                entity.Property(e => e.CreatedByPersonId).HasColumnName("CreatedByPersonID");

                entity.Property(e => e.Name).HasMaxLength(20);

                entity.HasOne(d => d.AuthorizedByPerson)
                    .WithMany(p => p.SponsorsAuthorizedByPerson)
                    .HasForeignKey(d => d.AuthorizedByPersonId)
                    .HasConstraintName("SponsorAuthorizedByPersonFK");

                entity.HasOne(d => d.CreatedByPerson)
                    .WithMany(p => p.SponsorSCreatedByPerson)
                    .HasForeignKey(d => d.CreatedByPersonId)
                    .HasConstraintName("SponsorCreatedByPersonFK");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
