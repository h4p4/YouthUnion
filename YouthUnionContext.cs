using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace YouthUnion;

public partial class YouthUnionContext : DbContext
{
    public YouthUnionContext()
    {
    }

    public YouthUnionContext(DbContextOptions<YouthUnionContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Assignment> Assignments { get; set; }

    public virtual DbSet<Event> Events { get; set; }

    public virtual DbSet<Participant> Participants { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=YouthUnion;Username=postgres;Password=1488");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Assignment>(entity =>
        {
            entity.HasKey(e => e.AssignmentId).HasName("assignments_pkey");

            entity.ToTable("assignments", "youthunion");

            entity.Property(e => e.AssignmentId).HasColumnName("assignment_id");
            entity.Property(e => e.EventId).HasColumnName("event_id");
            entity.Property(e => e.ParticipantId).HasColumnName("participant_id");

            entity.HasOne(d => d.Event).WithMany(p => p.Assignments)
                .HasForeignKey(d => d.EventId)
                .HasConstraintName("assignments_event_id_fkey");

            entity.HasOne(d => d.Participant).WithMany(p => p.Assignments)
                .HasForeignKey(d => d.ParticipantId)
                .HasConstraintName("assignments_participant_id_fkey");
        });

        modelBuilder.Entity<Event>(entity =>
        {
            entity.HasKey(e => e.EventId).HasName("events_pkey");

            entity.ToTable("events", "youthunion");

            entity.Property(e => e.EventId).HasColumnName("event_id");
            entity.Property(e => e.EndDate).HasColumnName("end_date");
            entity.Property(e => e.EventLocation)
                .HasColumnType("character varying")
                .HasColumnName("event_location");
            entity.Property(e => e.EventName)
                .HasColumnType("character varying")
                .HasColumnName("event_name");
            entity.Property(e => e.Financing).HasColumnName("financing");
            entity.Property(e => e.ResponsibleParticipantId).HasColumnName("responsible_participant_id");
            entity.Property(e => e.StartDate).HasColumnName("start_date");

            entity.HasOne(d => d.ResponsibleParticipant).WithMany(p => p.Events)
                .HasForeignKey(d => d.ResponsibleParticipantId)
                .HasConstraintName("events_responsible_participant_id_fkey");
        });

        modelBuilder.Entity<Participant>(entity =>
        {
            entity.HasKey(e => e.ParticipantId).HasName("participants_pkey");

            entity.ToTable("participants", "youthunion");

            entity.Property(e => e.ParticipantId).HasColumnName("participant_id");
            entity.Property(e => e.Age).HasColumnName("age");
            entity.Property(e => e.ContactInfo)
                .HasColumnType("character varying")
                .HasColumnName("contact_info");
            entity.Property(e => e.FullName)
                .HasColumnType("character varying")
                .HasColumnName("full_name");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("users_pkey");

            entity.ToTable("users", "youthunion");

            entity.HasIndex(e => e.Username, "users_username_key").IsUnique();

            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.UserPassword)
                .HasColumnType("character varying")
                .HasColumnName("user_password");
            entity.Property(e => e.Username)
                .HasColumnType("character varying")
                .HasColumnName("username");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
