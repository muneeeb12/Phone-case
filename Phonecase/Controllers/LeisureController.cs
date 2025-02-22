using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Phonecase.Models;
using Phonecase.Data; // Assuming your DbContext is inside a `Data` folder

namespace Phonecase.Controllers
{
    public class LeisureController : Controller
    {
        private readonly PhoneCaseDbContext _context;

        public LeisureController(PhoneCaseDbContext context)
        {
            _context = context;
        }

        // Dashboard View
        public IActionResult Dashboard()
        {
            var transactions = _context.Transactions.OrderByDescending(t => t.Date).ToList();
            return View(transactions);
        }

        // View transactions for a specific vendor
        public IActionResult VendorTransactions(int vendorId)
        {
            var transactions = _context.Transactions
                .Where(t => t.VendorId == vendorId)
                .OrderByDescending(t => t.Date)
                .ToList();

            return View(transactions);
        }

        // Add a new transaction
        [HttpPost]
        public IActionResult AddTransaction(Transaction transaction)
        {
            if (ModelState.IsValid)
            {
                // Get the last transaction balance for the vendor
                var lastTransaction = _context.Transactions
                    .Where(t => t.VendorId == transaction.VendorId)
                    .OrderByDescending(t => t.Date)
                    .FirstOrDefault();

                decimal lastBalance = lastTransaction?.CashBalance ?? 0;
                transaction.CashBalance = lastBalance + (transaction.Credit ?? 0) - (transaction.Debit ?? 0);

                _context.Transactions.Add(transaction);
                _context.SaveChanges();

                return RedirectToAction("Dashboard");
            }

            return View(transaction);
        }

        // Edit a transaction
        public IActionResult EditTransaction(int id)
        {
            var transaction = _context.Transactions.Find(id);
            if (transaction == null) return NotFound();
            return View(transaction);
        }

        [HttpPost]
        public IActionResult EditTransaction(Transaction transaction)
        {
            if (ModelState.IsValid)
            {
                _context.Transactions.Update(transaction);
                _context.SaveChanges();
                return RedirectToAction("Dashboard");
            }
            return View(transaction);
        }

        // Delete a transaction
        public IActionResult DeleteTransaction(int id)
        {
            var transaction = _context.Transactions.Find(id);
            if (transaction != null)
            {
                _context.Transactions.Remove(transaction);
                _context.SaveChanges();
            }
            return RedirectToAction("Dashboard");
        }
    }
}
