using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

#nullable disable

namespace Repository.Models
{
    public partial class ReviewDBContext : DbContext
    {
        public ReviewDBContext()
        {
        }

        public ReviewDBContext(DbContextOptions<ReviewDBContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Review> Reviews { get; set; }
        public virtual DbSet<Setting> Settings { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer("Server=.\\SQLExpress;Database=ReviewDB;Trusted_Connection=True;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("Relational:Collation", "SQL_Latin1_General_CP1_CI_AS");

            modelBuilder.Entity<Review>(entity =>
            {
                entity.ToTable("Review");

                entity.Property(e => e.ReviewId)
                    .HasColumnName("reviewId")
                    .HasDefaultValueSql("(newid())");

                entity.Property(e => e.CreationTime)
                    .HasColumnName("creationTime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.ImdbId)
                    .IsRequired()
                    .HasMaxLength(255)
                    .HasColumnName("imdbId");

                entity.Property(e => e.Review1)
                    .HasColumnType("text")
                    .HasColumnName("review");

                entity.Property(e => e.Score).HasColumnName("score");

                entity.Property(e => e.UsernameId).HasColumnName("usernameId");
            });

            modelBuilder.Entity<Setting>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("Setting");

                entity.Property(e => e.IntValue).HasColumnName("intValue");

                entity.Property(e => e.Setting1)
                    .IsRequired()
                    .HasMaxLength(255)
                    .HasColumnName("setting")
                    .HasDefaultValueSql("(newid())");

                entity.Property(e => e.StringValue)
                    .HasMaxLength(255)
                    .HasColumnName("stringValue");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
