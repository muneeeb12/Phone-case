using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Phonecase.Data;
using Phonecase.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Phonecase.Controllers {
    public class LeisureController : Controller {
        private readonly PhoneCaseDbContext _context;

        public LeisureController(PhoneCaseDbContext context) {
            _context = context;
        }

        // GET: Show Vendors for search
        public async Task<IActionResult> Index() {
            ViewBag.Vendors = await _context.Vendors.ToListAsync();
            return View();
        }

        // POST: Fetch vendor transactions based on filter (week/month)
        [HttpPost]
        public async Task<IActionResult> VendorLeisure(int vendorId, string filter) {
            var vendor = await _context.Vendors.FirstOrDefaultAsync(v => v.VendorId == vendorId);
            if (vendor == null) {
                return NotFound("Vendor not found.");
            }

            DateTime startDate = DateTime.MinValue;
            if (filter == "week") {
                startDate = DateTime.Now.AddDays(-7);
            } else if (filter == "month") {
                startDate = DateTime.Now.AddMonths(-1);
            }

            // Fetch purchases and group by date
            var purchases = await _context.Purchases
                .Where(p => p.VendorId == vendorId && p.PurchaseDate >= startDate)
                .GroupBy(p => p.PurchaseDate.Date)
                .Select(g => new {
                    Date = g.Key,
                    TotalDebit = g.Sum(p => p.Quantity * p.UnitPrice),
                    PurchaseIds = g.Select(p => p.PurchaseId).ToList()
                })
                .ToListAsync();

            // Fetch payments and group by date
            var payments = await _context.Payments
                .Where(p => p.VendorId == vendorId && p.PaymentDate >= startDate)
                .GroupBy(p => p.PaymentDate.Date)
                .Select(g => new {
                    Date = g.Key,
                    TotalCredit = g.Sum(p => p.Amount)
                })
                .ToListAsync();

            var transactions = new List<LeisureTransactionViewModel>();
            decimal runningBalance = vendor.RemainingBalance;

            // Combine purchases & payments by date
            var groupedTransactions = purchases
                .Select(p => new LeisureTransactionViewModel {
                    Date = p.Date,
                    Description = $"Purchases",
                    Debit = p.TotalDebit,
                    Credit = 0,
                    RemainingBalance = runningBalance += p.TotalDebit,
                    TransactionType = "purchase",
                    PurchaseIds = p.PurchaseIds
                })
                .Union(payments.Select(pay => new LeisureTransactionViewModel {
                    Date = pay.Date,
                    Description = "Payment",
                    Debit = 0,
                    Credit = pay.TotalCredit,
                    RemainingBalance = runningBalance -= pay.TotalCredit,
                    TransactionType = "payment"
                }))
                .GroupBy(t => t.Date)  // Merge purchases & payments on the same date
                .Select(g => new LeisureTransactionViewModel {
                    Date = g.Key,
                    Description = string.Join(" | ", g.Select(t => t.Description)), // Combine descriptions
                    Debit = g.Sum(t => t.Debit),
                    Credit = g.Sum(t => t.Credit),
                    RemainingBalance = g.Last().RemainingBalance, // Keep latest balance
                    TransactionType = g.Any(t => t.TransactionType == "purchase") ? "purchase" : "payment",
                    PurchaseIds = g.SelectMany(t => t.PurchaseIds ?? new List<int>()).ToList()
                })
                .OrderBy(t => t.Date)
                .ToList();

            ViewBag.Vendors = await _context.Vendors.ToListAsync();
            ViewBag.SelectedVendor = vendorId;
            ViewBag.SelectedFilter = filter;

            return View(groupedTransactions);
        }

        // GET: Show all products for a specific purchase date
        public async Task<IActionResult> PurchaseDetails(string purchaseIds) {
            if (string.IsNullOrEmpty(purchaseIds)) {
                return NotFound("No purchase IDs provided.");
            }

            // Convert comma-separated string into list of integers
            var purchaseIdList = purchaseIds.Split(',')
                                            .Select(id => int.TryParse(id, out int parsedId) ? parsedId : (int?)null)
                                            .Where(id => id.HasValue)
                                            .Select(id => id.Value)
                                            .ToList();

            if (!purchaseIdList.Any()) {
                return NotFound("Invalid purchase IDs.");
            }

            var purchases = await _context.Purchases
                .Where(p => purchaseIdList.Contains(p.PurchaseId))
                .Include(p => p.Product)
                .ThenInclude(m => m.Model)
                .Include(p => p.Product)
                .ThenInclude(cm => cm.CaseManufacturer)
                .ToListAsync();

            if (!purchases.Any()) {
                return NotFound("No purchases found for this date.");
            }

            return View(purchases);
        }
    }

    public class LeisureTransactionViewModel {
        public DateTime Date { get; set; }
        public string Description { get; set; }
        public decimal Debit { get; set; }
        public decimal Credit { get; set; }
        public decimal RemainingBalance { get; set; }
        public string TransactionType { get; set; }
        public List<int> PurchaseIds { get; set; } = new List<int>(); // Store purchase IDs for the date
    }
}
