using System.ComponentModel.DataAnnotations;

namespace CLDVWebApplication.Models
{
    public class EventType
    {
        [Key]
        public int EventTypeID { get; set; }

        [Required]
        public string Name { get; set; } = null!;

        public virtual ICollection<EventTable> Events { get; set; } = new List<EventTable>();
    }
}
