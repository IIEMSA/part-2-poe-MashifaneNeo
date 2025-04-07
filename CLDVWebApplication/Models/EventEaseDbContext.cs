using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using CLDVWebApplication.Models;

namespace CLDVWebApplication.Models;

public partial class EventEaseDbContext : DbContext
{
    public EventEaseDbContext()
    {
    }

    public EventEaseDbContext(DbContextOptions<EventEaseDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Booking> Bookings { get; set; }

    public virtual DbSet<EventTable> EventTables { get; set; }

    public virtual DbSet<Venue> Venues { get; set; }
   

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Booking>(entity =>
        {
            entity.HasKey(e => e.BookingId).HasName("PK__Booking__73951AEDC0FD34F1");

            entity.ToTable("Booking");

            entity.HasIndex(e => new { e.VenueId, e.EventId }, "unique_booking").IsUnique();

            entity.Property(e => e.BookingDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.Event)
                .WithMany(p => p.Bookings)
                .HasForeignKey(d => d.EventId)
                .OnDelete(DeleteBehavior.Cascade) // THIS LINE ENABLES CASCADE DELETE
                .HasConstraintName("FK__Booking__EventId__05D8E0BE");

            entity.HasOne(d => d.Venue)
                .WithMany(p => p.Bookings)
                .HasForeignKey(d => d.VenueId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Booking__VenueId__06CD04F7");
        });

        modelBuilder.Entity<EventTable>(entity =>
        {
            entity.HasKey(e => e.EventId);
            entity.ToTable("EventTable");
            entity.Property(e => e.EventName).HasMaxLength(255).IsUnicode(false);
            entity.Property(e => e.Description).HasColumnType("text");
            entity.HasOne(d => d.Venue).WithMany(p => p.EventTables).HasForeignKey(d => d.VenueId).HasConstraintName("FK_EventTable_Venue");
        });

        modelBuilder.Entity<Venue>(entity =>
        {
            entity.HasKey(e => e.VenueId).HasName("PK__Venue__3C57E5F2B043C9AE");

            entity.ToTable("Venue");

            entity.Property(e => e.ImageUrl)
                .HasMaxLength(500)
                .IsUnicode(false);
            entity.Property(e => e.Location)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.VenueName)
                .HasMaxLength(255)
                .IsUnicode(false);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);


}
