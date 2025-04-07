using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CLDVWebApplication.Models;

public class EventTable
{
    [Key]
    public int EventId { get; set; }

    [Required]
    public string EventName { get; set; } = null!;

    [Required]
    public DateTime EventDate { get; set; }

    public string? Description { get; set; }

    public int VenueId { get; set; }

    public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();

    public Venue? Venue { get; set; } = null!;

}
