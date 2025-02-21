using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Phonecase.Data;
using Phonecase.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Phonecase.Controllers {
    public class VendorController : Controller {
        private readonly PhoneCaseDbContext _context;

        // Inject the DbContext via constructor
        public VendorController(PhoneCaseDbContext context) {
            _context = context;
        }

        // Action to display the vendor management page
        public async Task<IActionResult> Index() {
            var vendors = await _context.Vendors.ToListAsync();
            return View(vendors); 
        }

        // Action to add a new vendor
        [HttpPost]
        public async Task<IActionResult> AddVendor(string name, string contactInfo) {
            if (!string.IsNullOrEmpty(name)) {
                var vendor = new Vendor {
                    Name = name,
                    ContactInfo = contactInfo,
                    TotalCredit = 0.00m
                };

                await _context.Vendors.AddAsync(vendor);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("Index");
        }

        // Action to delete a vendor
        [HttpPost]
        public async Task<IActionResult> DeleteVendor(int vendorId) {
            var vendor = await _context.Vendors.FirstOrDefaultAsync(v => v.VendorId == vendorId);
            if (vendor != null) {
                _context.Vendors.Remove(vendor);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> VendorHistory(int vendorId) {
            var vendor = await _context.Vendors
                .FirstOrDefaultAsync(v => v.VendorId == vendorId);

            if (vendor == null) {
                return NotFound("Vendor not found.");
            }

            var purchaseHistory = await _context.Purchases
                .Where(p => p.VendorId == vendorId)
                .Include(p => p.Product)
                .ThenInclude(m => m.Model)
                .Include(p => p.Product)
                .ThenInclude(cm => cm.CaseManufacturer)
                .OrderByDescending(p => p.PurchaseDate)
                .ToListAsync();

            var paymentHistory = await _context.Payments
                .Where(p => p.VendorId == vendorId)
                .OrderByDescending(p => p.PaymentDate)
                .ToListAsync();

            ViewBag.Vendor = vendor;
            ViewBag.PurchaseHistory = purchaseHistory;
            ViewBag.PaymentHistory = paymentHistory;

            return View();
        }


    }
}