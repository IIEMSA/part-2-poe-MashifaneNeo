using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using CLDVWebApplication.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CLDVWebApplication.Controllers
{
    public class EventController : Controller
    {
        private readonly EventEaseDbContext _context;

        public EventController(EventEaseDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string searchType, int? venueId, DateTime? startDate, DateTime? endDate)
        {
            // Start with base query including related entities
            var events = _context.EventTables
                .Include(e => e.Venue)
                .Include(e => e.EventType)  // Make sure you have this relationship
                .AsQueryable();

            // Apply filters if they are provided
            if (!string.IsNullOrEmpty(searchType))
            {
                events = events.Where(e => e.EventType.Name == searchType);
            }

            if (venueId.HasValue)
            {
                events = events.Where(e => e.VenueId == venueId);
            }

            if (startDate.HasValue && endDate.HasValue)
            {
                events = events.Where(e => e.EventDate >= startDate && e.EventDate <= endDate);
            }

            // Provide data for dropdown filters in the View
            ViewData["EventTypes"] = await _context.EventType.ToListAsync();
            ViewData["Venues"] = await _context.Venues.ToListAsync();

            // Execute the query and return the view
            return View(await events.ToListAsync());
        }

        public IActionResult Create()
        {
            // Ensure EventTypes and Venues are populated
            ViewData["EventTypes"] = _context.EventType.ToList() ?? new List<EventType>();
            ViewData["VenueId"] = new SelectList(_context.Venues, "VenueId", "VenueName");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("EventName,EventDate,Description,VenueId,EventTypeID")] EventTable eventTable)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Add(eventTable);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = $"Event '{eventTable.EventName}' created successfully!";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "An error occurred while saving the event.");
                    Console.WriteLine(ex.Message);
                }
            }

            // Repopulate dropdowns if validation fails
            ViewData["EventTypes"] = _context.EventType.ToList() ?? new List<EventType>();
            ViewData["VenueId"] = new SelectList(_context.Venues, "VenueId", "VenueName", eventTable.VenueId);
            return View(eventTable);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var eventTable = await _context.EventTables.FindAsync(id);
            if (eventTable == null) return NotFound();

            ViewData["EventTypes"] = _context.EventType.ToList() ?? new List<EventType>();
            ViewData["VenueId"] = new SelectList(_context.Venues, "VenueId", "VenueName", eventTable.VenueId);
            return View(eventTable);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("EventId,EventName,EventDate,Description,VenueId,EventTypeID")] EventTable eventTable)
        {
            if (id != eventTable.EventId) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(eventTable);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = $"Event '{eventTable.EventName}' edited successfully!";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EventTableExists(eventTable.EventId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }

            ViewData["EventTypes"] = _context.EventType.ToList() ?? new List<EventType>();
            ViewData["VenueId"] = new SelectList(_context.Venues, "VenueId", "VenueName", eventTable.VenueId);
            return View(eventTable);
        }

        private bool EventTableExists(int id)
        {
            return _context.EventTables.Any(e => e.EventId == id);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var eventTable = await _context.EventTables.Include(e => e.Venue).FirstOrDefaultAsync(m => m.EventId == id);
            if (eventTable == null) return NotFound();

            return View(eventTable);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var eventTable = await _context.EventTables.Include(e => e.Venue).FirstOrDefaultAsync(m => m.EventId == id);
            if (eventTable == null) return NotFound();

            return View(eventTable);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            // Check if there are any bookings for this event
            var hasBookings = await _context.Bookings
        .AnyAsync(b => b.EventId == id);

            if (hasBookings)
            {
                TempData["ErrorMessage"] = "Cannot delete this event because it has active bookings.";
                return RedirectToAction(nameof(Index));
            }

            var eventTable = await _context.EventTables.FindAsync(id);
            if (eventTable != null)
            {
                try
                {
                    _context.EventTables.Remove(eventTable);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Event deleted successfully!";
                }
                catch (DbUpdateException ex)
                {
                    TempData["ErrorMessage"] = "An error occurred while deleting the event. Please try again.";
                    // Log the exception (ex)
                }
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
