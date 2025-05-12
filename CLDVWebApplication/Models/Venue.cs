using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CLDVWebApplication.Models;

public partial class Venue
{
    public int VenueId { get; set; }

    [Required]
    public string VenueName { get; set; } = null!;

    [Required]
    public string Location { get; set; } = null!;

    [Required]
    [Range(1, int.MaxValue, ErrorMessage = "Capacity must be greater thane 0.")]
    public int Capacity { get; set; }

    public string? ImageUrl { get; set; }

    [NotMapped]
    public IFormFile? ImageFile { get; set; } 

    public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();

    public virtual ICollection<EventTable> EventTables { get; set; } = new List<EventTable>();
}
