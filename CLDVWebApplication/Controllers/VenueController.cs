using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CLDVWebApplication.Models;
using Microsoft.AspNetCore.Http;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using System.IO;

namespace CLDVWebApplication.Controllers
{
    public class VenueController : Controller
    {
        private readonly EventEaseDbContext _context;

        public VenueController(EventEaseDbContext context)
        {
            _context = context;
        }

        // GET: Venue
        public async Task<IActionResult> Index()
        {
            var venues = await _context.Venues.ToListAsync();
            return View(venues);
        }

        // GET: Venue/Details
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var venue = await _context.Venues
                .FirstOrDefaultAsync(m => m.VenueId == id);

            if (venue == null) return NotFound();
            return View(venue);
        }

        // GET: Venue/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Venue/Create       
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Venue venue, IFormFile ImageFile)
        {
            if (ModelState.IsValid)
            {
                if (ImageFile != null && ImageFile.Length > 0)
                {
                    try
                    {
                        var blobUrl = await UploadImageToBlobAsync(ImageFile);
                        venue.ImageUrl = blobUrl;
                    }
                    catch (Exception ex)
                    {
                        ModelState.AddModelError("ImageFile", $"Error uploading image: {ex.Message}");
                        return View(venue);
                    }
                }

                _context.Add(venue);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = $"Venue '{venue.VenueName}' created successfully!";
                return RedirectToAction(nameof(Index));
            }
            return View(venue);
        }

        // GET: Venue/Edit
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var venue = await _context.Venues.FindAsync(id);
            if (venue == null) return NotFound();

            return View(venue);
        }

        // POST: Venue/Edit/5        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Venue venue, IFormFile ImageFile)
        {
            if (id != venue.VenueId) return NotFound();

            var existingVenue = await _context.Venues.FindAsync(id);
            if (existingVenue == null) return NotFound();

            // Update only if new values are provided
            if (!string.IsNullOrWhiteSpace(venue.VenueName))
            {
                existingVenue.VenueName = venue.VenueName;
            }

            if (!string.IsNullOrWhiteSpace(venue.Location))
            {
                existingVenue.Location = venue.Location;
            }

            if (venue.Capacity > 0) // Only update if valid capacity is provided
            {
                existingVenue.Capacity = venue.Capacity;
            }

            if (ImageFile != null && ImageFile.Length > 0)
            {
                try
                {
                    var blobUrl = await UploadImageToBlobAsync(ImageFile);
                    existingVenue.ImageUrl = blobUrl;
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("ImageFile", $"Error uploading image: {ex.Message}");
                    return View(venue);
                }
            }

            try
            {
                _context.Update(existingVenue);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = $"Venue '{existingVenue.VenueName}' updated successfully!";
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!VenueExists(venue.VenueId)) return NotFound();
                else throw;
            }
        }

        // GET: Venue/Delete
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var venue = await _context.Venues
                .FirstOrDefaultAsync(m => m.VenueId == id);

            if (venue == null) return NotFound();
            return View(venue);
        }

        // POST: Venue/Delete
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var hasBookings = await _context.Bookings
                .AnyAsync(b => b.VenueId == id);

            if (hasBookings)
            {
                TempData["ErrorMessage"] = "Cannot delete this venue because it has active bookings.";
                return RedirectToAction(nameof(Index));
            }

            var venue = await _context.Venues.FindAsync(id);
            if (venue != null)
            {
                try
                {
                    _context.Venues.Remove(venue);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Venue deleted successfully!";
                }
                catch (DbUpdateException ex)
                {
                    TempData["ErrorMessage"] = "An error occurred while deleting the venue. Please try again.";
                    // Log the exception (ex)
                }
            }
            return RedirectToAction(nameof(Index));
        }

        private async Task<string> UploadImageToBlobAsync(IFormFile imageFile)
        {
            

            var blobServiceClient = new BlobServiceClient(connectionString);
            var containerClient = blobServiceClient.GetBlobContainerClient(containerName);

            // Create container if it doesn't exist
            await containerClient.CreateIfNotExistsAsync();
            await containerClient.SetAccessPolicyAsync(PublicAccessType.Blob);

            var blobName = Guid.NewGuid() + Path.GetExtension(imageFile.FileName);
            var blobClient = containerClient.GetBlobClient(blobName);

            var blobHttpHeaders = new BlobHttpHeaders
            {
                ContentType = imageFile.ContentType
            };

            using (var stream = imageFile.OpenReadStream())
            {
                await blobClient.UploadAsync(stream, new BlobUploadOptions
                {
                    HttpHeaders = blobHttpHeaders
                });
            }
            return blobClient.Uri.ToString();
        }

        private bool VenueExists(int id)
        {
            return _context.Venues.Any(e => e.VenueId == id);
        }
    }
}