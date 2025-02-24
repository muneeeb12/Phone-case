using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Phonecase.Data;
using Phonecase.Models;
using Phonecase.Repositories;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Phonecase.Controllers {
    public class HistoryController : Controller {
        private readonly PhoneCaseDbContext _context;
        private readonly IVendorRepository _vendorRepository;
        private readonly IHistoryRepository _historyRepository;

        public HistoryController(
            PhoneCaseDbContext context,
            IVendorRepository vendorRepository,
            IHistoryRepository historyRepository) {
            _context = context;
            _vendorRepository = vendorRepository;
            _historyRepository = historyRepository;
        }

        // 🟢 HELPER FUNCTION: Fetch Vendors for Dropdown
        private async Task PopulateVendorDropdownAsync() {
            ViewBag.Vendors = await _vendorRepository.GetVendorAsync();
        }

        // 🟢 HELPER FUNCTION: Get Date Filter
        private DateTime GetStartDate(string filter) {
            return filter switch {
                "week" => DateTime.Now.AddDays(-7),
                "month" => DateTime.Now.AddMonths(-1),
                _ => DateTime.MinValue
            };
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
            var vendor = await _vendorRepository.GetVendorByIdAsync(vendorId);
            if (vendor == null) return NotFound("Vendor not found.");

            var startDate = GetStartDate(filter);

            var purchaseHistory = await _historyRepository.GetPurchaseHistoryAsync(vendorId, startDate);     

            await PopulateVendorDropdownAsync();
            ViewBag.SelectedVendor = vendorId;
            ViewBag.SelectedFilter = filter;
            ViewBag.PurchaseHistory = purchaseHistory;

            return View(purchaseHistory);
        }

        
        public async Task<IActionResult> EditPurchase(int id) {
            var purchase = await _historyRepository.GetPurchaseByIdAsync(id);

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

            await _historyRepository.EditPurchaseAsync(id, quantity, unitPrice);
            
            return RedirectToAction("PurchaseHistory", new { vendorId = purchase.VendorId, filter = "all" });
        }


        // 🟢 GET: Show Vendor Selection for Payment History
        public async Task<IActionResult> PaymentHistory()
        {
            await PopulateVendorDropdownAsync();
            ViewBag.PaymentHistory = null; // No history on first load
            return View();
        }


        // 🟢 POST: Fetch Payment History Based on Vendor & Date Filter
        [HttpPost]
        public async Task<IActionResult> PaymentHistory(int vendorId, string filter) {
            var vendor = await _vendorRepository.GetVendorByIdAsync(vendorId);
            if (vendor == null) return NotFound("Vendor not found.");

            var startDate = GetStartDate(filter);

            var paymentHistory = await _historyRepository.GetPaymentHistoryAsync(vendorId, startDate);

            await PopulateVendorDropdownAsync();
            ViewBag.SelectedVendor = vendorId;
            ViewBag.SelectedFilter = filter;
            ViewBag.PaymentHistory = paymentHistory;

            return View(paymentHistory);
        }

        // 🟢 GET: Edit Payment
        public async Task<IActionResult> EditPayment(int id) {
            var payment = await _historyRepository.GetPaymentByIdAsync(id);

            if (payment == null) return NotFound("Payment not found.");
            return View(payment);
        }

        // 🟢 POST: Edit Payment Submission
        [HttpPost]
        public async Task<IActionResult> EditPayment(int id, decimal amount) {
            var payment = await _historyRepository.GetPaymentByIdAsync(id);

            if (payment == null) return NotFound("Payment not found.");

            decimal oldAmount = payment.Amount;
            decimal difference = oldAmount - amount;

            // Adjust vendor credit
            payment.Vendor.TotalCredit += difference;

            await _historyRepository.EditPaymentAsync(id, amount);
            
            return RedirectToAction("PaymentHistory", new { vendorId = payment.VendorId, filter = "all" });
        }


    }
}
