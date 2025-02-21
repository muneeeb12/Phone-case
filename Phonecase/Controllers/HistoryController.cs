using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Phonecase.Data;
using Phonecase.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Phonecase.Controllers {
    public class HistoryController : Controller {
        private readonly PhoneCaseDbContext _context;

        public HistoryController(PhoneCaseDbContext context) {
            _context = context;
        }

        public async Task<IActionResult> PurchaseHistory() {
            var vendors = await _context.Vendors.ToListAsync();
            ViewBag.Vendors = vendors; // Pass vendors to view
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> PurchaseHistory(int vendorId, string filter) {
            var vendor = await _context.Vendors.FirstOrDefaultAsync(v => v.VendorId == vendorId);
            if (vendor == null) {
                return NotFound("Vendor not found.");
            }

            IQueryable<Purchase> query = _context.Purchases
                .Where(p => p.VendorId == vendorId)
                .Include(p => p.Product)
                .ThenInclude(m => m.Model)
                .Include(p => p.Product)
                .ThenInclude(cm => cm.CaseManufacturer);

            if (filter == "week") {
                DateTime startDate = DateTime.Now.AddDays(-7);
                query = query.Where(p => p.PurchaseDate >= startDate);
            } else if (filter == "month") {
                DateTime startDate = DateTime.Now.AddMonths(-1);
                query = query.Where(p => p.PurchaseDate >= startDate);
            }

            var purchaseHistory = await query.OrderByDescending(p => p.PurchaseDate).ToListAsync();

            ViewBag.Vendors = await _context.Vendors.ToListAsync();
            ViewBag.SelectedVendor = vendorId;
            ViewBag.SelectedFilter = filter;

            return View(purchaseHistory);
        }

    }
}
