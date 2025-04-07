using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CLDVWebApplication.Models
{
    public class EventDetail
    {
        [Key] // Primary Key
        public int EventId { get; set; }

        [Required]
        public string EventName { get; set; }

        public DateTime EventDate { get; set; }

        public string? Description { get; set; } // Nullable

        // Foreign Key for Venue
        [ForeignKey("Venue")]
        public int VenueId { get; set; }
        public Venue? Venue { get; set; } // Navigation property
    }
}