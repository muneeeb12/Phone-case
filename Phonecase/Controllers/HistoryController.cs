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

        // 🟢 HELPER FUNCTION: Fetch Vendors for Dropdown
        private async Task PopulateVendorDropdownAsync() {
            ViewBag.Vendors = await _context.Vendors.ToListAsync();
        }

        // 🟢 HELPER FUNCTION: Get Date Filter
        private DateTime GetStartDate(string filter) {
            return filter switch {
                "week" => DateTime.Now.AddDays(-7),
                "month" => DateTime.Now.AddMonths(-1),
                _ => DateTime.MinValue
            };
        }

        // 🟢 HELPER FUNCTION: Fetch Vendor by ID
        private async Task<Vendor> GetVendorAsync(int vendorId) {
            return await _context.Vendors.FirstOrDefaultAsync(v => v.VendorId == vendorId);
        }

        // 🟢 GET: Show Vendor Selection for Purchase History
        public async Task<IActionResult> PurchaseHistory() {
            await PopulateVendorDropdownAsync();
            ViewBag.PurchaseHistory = null; // No history on first load
            return View();
        }

        // 🟢 POST: Fetch Purchase History Based on Vendor & Date Filter
        [HttpPost]
        public async Task<IActionResult> PurchaseHistory(int vendorId, string filter) {
            var vendor = await GetVendorAsync(vendorId);
            if (vendor == null) return NotFound("Vendor not found.");

            var startDate = GetStartDate(filter);

            var purchaseHistory = await _context.Purchases
                .Where(p => p.VendorId == vendorId && p.PurchaseDate >= startDate)
                .Include(p => p.Product)
                .ThenInclude(m => m.Model)
                .Include(p => p.Product)
                .ThenInclude(cm => cm.CaseManufacturer)
                .OrderByDescending(p => p.PurchaseDate)
                .ToListAsync();

            await PopulateVendorDropdownAsync();
            ViewBag.SelectedVendor = vendorId;
            ViewBag.SelectedFilter = filter;
            ViewBag.PurchaseHistory = purchaseHistory;

            return View(purchaseHistory);
        }

        // 🟢 GET: Show Vendor Selection for Payment History
        public async Task<IActionResult> PaymentHistory() {
            await PopulateVendorDropdownAsync();
            ViewBag.PaymentHistory = null; // No history on first load
            return View();
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




// 🟢 POST: Fetch Payment History Based on Vendor & Date Filter
[HttpPost]
        public async Task<IActionResult> PaymentHistory(int vendorId, string filter) {
            var vendor = await GetVendorAsync(vendorId);
            if (vendor == null) return NotFound("Vendor not found.");

            var startDate = GetStartDate(filter);

            var paymentHistory = await _context.Payments
                .Where(p => p.VendorId == vendorId && p.PaymentDate >= startDate)
                .OrderByDescending(p => p.PaymentDate)
                .ToListAsync();

            await PopulateVendorDropdownAsync();
            ViewBag.SelectedVendor = vendorId;
            ViewBag.SelectedFilter = filter;
            ViewBag.PaymentHistory = paymentHistory;

            return View(paymentHistory);
        }

        // 🟢 GET: Edit Payment
        public async Task<IActionResult> EditPayment(int id) {
            var payment = await _context.Payments
                .Include(p => p.Vendor)
                .FirstOrDefaultAsync(p => p.PaymentId == id);

            if (payment == null) return NotFound("Payment not found.");
            return View(payment);
        }

        // 🟢 POST: Edit Payment Submission
        [HttpPost]
        public async Task<IActionResult> EditPayment(int id, decimal amount) {
            var payment = await _context.Payments
                .Include(p => p.Vendor)
                .FirstOrDefaultAsync(p => p.PaymentId == id);

            if (payment == null) return NotFound("Payment not found.");

            decimal oldAmount = payment.Amount;
            decimal difference = oldAmount - amount;

            // Adjust vendor credit
            payment.Vendor.TotalCredit += difference;

            // Update payment details
            payment.Amount = amount;

            await _context.SaveChangesAsync();
            return RedirectToAction("PaymentHistory", new { vendorId = payment.VendorId, filter = "all" });
        }


    }
}
