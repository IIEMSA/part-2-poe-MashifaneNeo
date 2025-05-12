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

        public async Task<IActionResult> Index()
        {
            var events = await _context.EventTables.Include(e => e.Venue).ToListAsync();
            return View(events);
        }

        public IActionResult Create()
        {
            ViewData["VenueId"] = new SelectList(_context.Venues, "VenueId", "VenueName");
            return View();
        }

        // POST: Event/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("EventName, EventDate, Description, VenueId")] EventTable eventTable)
        {
            if (ModelState.IsValid)  // Ensure form inputs are valid
            {
                try
                {
                    _context.EventTables.Add(eventTable);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = $"Event '{eventTable.EventName}' created successfully!";
                    return RedirectToAction(nameof(Index));  // Redirect back to event list
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "An error occurred while saving the event. Please try again.");
                    Console.WriteLine(ex.Message); // Log the error (for debugging)
                }
            }

            // If there are validation errors, return to the form
            ViewData["VenueId"] = new SelectList(_context.Venues, "VenueId", "VenueName", eventTable.VenueId);
            return View(eventTable);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var eventTable = await _context.EventTables.FindAsync(id);
            if (eventTable == null) return NotFound();

            ViewData["VenueId"] = new SelectList(_context.Venues, "VenueId", "VenueName", eventTable.VenueId);
            return View(eventTable);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("EventId, EventName, EventDate, Description, VenueId")] EventTable eventTable)
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
