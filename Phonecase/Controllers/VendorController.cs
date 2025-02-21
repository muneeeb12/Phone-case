using Microsoft.AspNetCore.Mvc;
using Phonecase.Models;
using System.Collections.Generic;

namespace Phonecase.Controllers {
    public class VendorController : Controller {
        // In-memory list for demo purposes (replace with database in production)
        private static List<Vendor> Vendors = new List<Vendor>();

        // Action to display the vendor management page
        public IActionResult Index() {
            ViewBag.Vendors = Vendors;
            return View();
        }

        // Action to add a new vendor
        [HttpPost]
        public IActionResult AddVendor(string name, string contactInfo) {
            if (!string.IsNullOrEmpty(name)) {
                Vendors.Add(new Vendor {
                    VendorId = Vendors.Count + 1, // Auto-generate ID (replace with database logic)
                    Name = name,
                    ContactInfo = contactInfo,
                    TotalCredit = 0.00m
                });
            }
            return RedirectToAction("Index");
        }

        // Action to delete a vendor
        [HttpPost]
        public IActionResult DeleteVendor(int vendorId) {
            var vendor = Vendors.FirstOrDefault(v => v.VendorId == vendorId);
            if (vendor != null) {
                Vendors.Remove(vendor);
            }
            return RedirectToAction("Index");
        }
    }
}