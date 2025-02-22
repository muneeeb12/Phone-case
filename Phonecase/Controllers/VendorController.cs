using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Phonecase.Data;
using Phonecase.Models;
using Phonecase.Repositories;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Phonecase.Controllers {
    public class VendorController : Controller {
        private readonly IVendorRepository _vendorRepository;

        // Inject the DbContext via constructor
        public VendorController(PhoneCaseDbContext context, IVendorRepository vendorRepository) {
            _context = context;
            _vendorRepository = vendorRepository;
        }

        // Action to display the vendor management page
        public async Task<IActionResult> Index() {
            var vendors = await _vendorRepository.GetVendorAsync();
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

                await _vendorRepository.CreateVendorAsync(vendor);
                
            }
            return RedirectToAction("Index");
        }

        // Action to delete a vendor
        [HttpPost]
        public async Task<IActionResult> DeleteVendor(int vendorId) {
            var result = await _vendorRepository.DeleteVendorAsync(vendorId);
            
            if (result != null)
            {
                return Ok("success!");

            }
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> VendorHistory(int vendorId) {
            
            var result = await _vendorRepository.GetVendorByIdAsync(vendorId);
            if (result == null) {
                return NotFound("Vendor not found.");
            }

            var purchaseHistory = await _vendorRepository.GetPurchaseHistoryByIdAsync(vendorId);

            var paymentHistory = await _vendorRepository.GetPaymentHistoryByIdAsync(vendorId);

            ViewBag.Vendor = result;
            ViewBag.PurchaseHistory = purchaseHistory;
            ViewBag.PaymentHistory = paymentHistory;

            return View();
        }

        public async Task<IActionResult> PayVendor(int vendorId) {
            var vendor = await _vendorRepository.GetVendorByIdAsync(vendorId);

            if (vendor == null) {
                return NotFound("Vendor not found.");
            }

            ViewBag.Vendor = vendor;
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> PayVendor(int vendorId, decimal amount) {
            if (amount <= 0) {
                TempData["Error"] = "Invalid payment amount.";
                return RedirectToAction("VendorHistory", new { vendorId });
            }

            var vendor = await _vendorRepository.GetVendorByIdAsync(vendorId);
            if (vendor == null) {
                return NotFound("Vendor not found.");
            }

            if (vendor.TotalCredit < amount) {
                TempData["Error"] = "Payment amount exceeds the vendor's total credit.";
                return RedirectToAction("VendorHistory", new { vendorId });
            }

            var payment = new Payment {
                VendorId = vendorId,
                Amount = amount,
                PaymentDate = DateTime.Now
            };

            // Deduct payment amount from vendor's credit
            vendor.TotalCredit -= amount;

            _context.Payments.Add(payment);
            await _vendorRepository.UpdateVendorAsync(vendor);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Payment recorded successfully.";
            return RedirectToAction("VendorHistory", new { vendorId });
        }

    }
}