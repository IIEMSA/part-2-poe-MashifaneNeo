using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CLDVWebApplication.Models
{
    public class Booking
    {
        [Key]
        public int BookingId { get; set; }

        [Required]
        public int EventId { get; set; }  // Make sure this is required

        [Required]
        public int VenueId { get; set; }  // Make sure this is required

        [Required]
        public DateTime BookingDate { get; set; }  // Make this required, or set it as nullable with validation

        public virtual EventTable Event { get; set; } = null!;

        public virtual Venue Venue { get; set; } = null!;
    }
}