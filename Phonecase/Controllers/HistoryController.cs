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
        public async Task<IActionResult> EditPurchase(int id) {
            var purchase = await _context.Purchases
                .Include(p => p.Product)
                .ThenInclude(m => m.Model)
                .Include(p => p.Product)
                .ThenInclude(cm => cm.CaseManufacturer)
                .FirstOrDefaultAsync(p => p.PurchaseId == id);

            if (purchase == null) {
                return NotFound("Purchase not found.");
            }

            return View(purchase);
        }
        [HttpPost]
        public async Task<IActionResult> EditPurchase(int id, int quantity, decimal unitPrice) {
            var purchase = await _context.Purchases
                .Include(p => p.Vendor)
                .FirstOrDefaultAsync(p => p.PurchaseId == id);

            if (purchase == null) {
                return NotFound("Purchase not found.");
            }

            decimal oldTotal = purchase.Quantity * purchase.UnitPrice;
            decimal newTotal = quantity * unitPrice;
            decimal difference = newTotal - oldTotal;

            // Adjust vendor credit
            if (difference > 0) {
                // Increase credit
                purchase.Vendor.TotalCredit += difference;
            } else if (difference < 0) {
                // Decrease credit only if credit is greater than zero
                if (purchase.Vendor.TotalCredit > 0) {
                    decimal deductionAmount = Math.Min(purchase.Vendor.TotalCredit, Math.Abs(difference));
                    purchase.Vendor.TotalCredit -= deductionAmount;
                }
            }

            // Update purchase details
            purchase.Quantity = quantity;
            purchase.UnitPrice = unitPrice;

            await _context.SaveChangesAsync();
            return RedirectToAction("PurchaseHistory", new { vendorId = purchase.VendorId, filter = "all" });
        }


    }
}
