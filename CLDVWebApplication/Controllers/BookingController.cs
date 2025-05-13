using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CLDVWebApplication.Models;

namespace CLDVWebApplication.Controllers
{
    public class BookingController : Controller
    {
        private readonly EventEaseDbContext _context;

        public BookingController(EventEaseDbContext context)
        {
            _context = context;
        }

        // GET: Booking
        public async Task<IActionResult> Index(string searchString)
        {
            var bookings = _context.Bookings
                .Include(b => b.Event)
                .Include(b => b.Venue)
                .OrderBy(b => b.BookingId)
                .AsQueryable();

            if (!string.IsNullOrEmpty(searchString))
            {
                // Try to parse search string as a BookingID (integer)
                bool isBookingId = int.TryParse(searchString, out int bookingId);

                bookings = bookings.Where(b =>
                    b.Venue.VenueName.Contains(searchString) ||
                    b.Event.EventName.Contains(searchString) ||
                    (isBookingId && b.BookingId == bookingId));
            }

            return View(await bookings.ToListAsync());
        }

        // GET: Booking/Details
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var booking = await _context.Bookings
                .Include(b => b.Event)
                .Include(b => b.Venue)
                .FirstOrDefaultAsync(m => m.BookingId == id);

            if (booking == null) return NotFound();

            return View(booking);
        }

        // GET: Booking/Create
        public IActionResult Create()
        {
            ViewBag.EventId = new SelectList(_context.EventTables, "EventId", "EventName");
            ViewBag.VenueId = new SelectList(_context.Venues, "VenueId", "VenueName");
            return View();
        }

        // POST: Booking/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("BookingId,EventId,VenueId,BookingDate")] Booking booking)
        {
            // Check if the model has all required fields
            if (ModelState.IsValid)
            {
                // Now it's safe to access booking.BookingDate and other properties
                var existingBooking = await _context.Bookings
                    .FirstOrDefaultAsync(b => b.VenueId == booking.VenueId
                                          && b.BookingDate.Date == booking.BookingDate.Date
                                          && b.BookingId != booking.BookingId); // Exclude current booking if editing

                if (existingBooking != null)
                {
                    // Get event name for clearer error feedback
                    var eventName = await _context.EventTables
                        .Where(e => e.EventId == existingBooking.EventId)
                        .Select(e => e.EventName)
                        .FirstOrDefaultAsync();

                    ModelState.AddModelError("BookingDate",
                        $"The venue is already booked on {booking.BookingDate.ToShortDateString()}" +
                        $" for event '{eventName}'. Please choose a different date or venue.");

                    TempData["ErrorMessage"] = "This booking cannot be created as the venue is already booked.";
                }
                else
                {
                    try
                    {
                        _context.Add(booking);
                        await _context.SaveChangesAsync();

                        TempData["SuccessMessage"] = "Booking created successfully!";
                        return RedirectToAction(nameof(Index));
                    }
                    catch (DbUpdateException ex)
                    {
                        ModelState.AddModelError("", "Unable to save changes. The venue may have been booked by another user. Please try again.");
                        // You could log the exception here if needed
                    }
                }
            }

            // Re-populate the dropdown lists in case of validation failure
            ViewBag.EventId = new SelectList(_context.EventTables, "EventId", "EventName", booking.EventId);
            ViewBag.VenueId = new SelectList(_context.Venues, "VenueId", "VenueName", booking.VenueId);
            return View(booking);
        }

        // POST: Booking/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("BookingId,EventId,VenueId,BookingDate")] Booking booking)
        {
            if (id != booking.BookingId) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(booking);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = $"Booking edited successfully!";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BookingExists(booking.BookingId)) return NotFound();
                    else throw;
                }
            }

            ViewBag.EventId = new SelectList(_context.EventTables, "EventId", "EventName", booking.EventId);
            ViewBag.VenueId = new SelectList(_context.Venues, "VenueId", "VenueName", booking.VenueId);
            return View(booking);
        }

        // GET: Booking/Delete
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var booking = await _context.Bookings
                .Include(b => b.Event)
                .Include(b => b.Venue)
                .FirstOrDefaultAsync(m => m.BookingId == id);

            if (booking == null) return NotFound();

            return View(booking);
        }

        // POST: Booking/Delete
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var booking = await _context.Bookings
                .Include(b => b.Event)
                .Include(b => b.Venue)
                .FirstOrDefaultAsync(b => b.BookingId == id);

            if (booking != null)
            {
                try
                {
                    _context.Bookings.Remove(booking);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = $"Booking for {booking.Event.EventName} at {booking.Venue.VenueName} on {booking.BookingDate.ToShortDateString()} deleted successfully!";
                }
                catch (DbUpdateException ex)
                {
                    TempData["ErrorMessage"] = "An error occurred while deleting the booking. Please try again.";
                    // Log the error (ex)
                }
            }
            return RedirectToAction(nameof(Index));
        }

        private bool BookingExists(int id)
        {
            return _context.Bookings.Any(e => e.BookingId == id);
        }
    }
}