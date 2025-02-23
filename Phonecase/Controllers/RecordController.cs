using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Phonecase.Data;
using Phonecase.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Phonecase.Controllers {
    public class RecordController : Controller {
        private readonly PhoneCaseDbContext _context;

        public RecordController(PhoneCaseDbContext context) {
            _context = context;
        }

        // Action to display the record purchase page
        public async Task<IActionResult> Index() {
            // Fetch vendors and products for dropdowns
            ViewBag.Vendors = await _context.Vendors.ToListAsync();
            ViewBag.Products = await _context.Products
                .Include(p => p.Model)
                .Include(p => p.CaseManufacturer)
                .ToListAsync();
            ViewBag.Models = await _context.PhoneModels.ToListAsync();
            ViewBag.CaseManufacturers = await _context.CaseManufacturers.ToListAsync();

            return View();
        }

        public async Task<IActionResult> SavePurchase(int vendorId, int[] productIds, int[] quantities, decimal[] unitPrices, DateTime purchaseDate)
        {
            if (vendorId <= 0 || productIds == null || quantities == null || unitPrices == null)
            {
                return BadRequest("Invalid purchase data. Please check input values.");
            }

            if (productIds.Length != quantities.Length || productIds.Length != unitPrices.Length)
            {
                return BadRequest("Mismatched product, quantity, and price data.");
            }

            var vendor = await _context.Vendors.FindAsync(vendorId);
            if (vendor == null)
            {
                return NotFound("Vendor not found.");
            }

            // Ensure purchase date is within the last 7 days
            DateTime today = DateTime.Today;
            if (purchaseDate > today || purchaseDate < today.AddDays(-7))
            {
                return BadRequest("Invalid purchase date. The date must be today or within the last 7 days.");
            }

            decimal totalPurchaseAmount = 0;

            for (int i = 0; i < productIds.Length; i++)
            {
                var purchase = new Purchase
                {
                    VendorId = vendorId,
                    ProductId = productIds[i],
                    Quantity = quantities[i],
                    UnitPrice = unitPrices[i],
                    PurchaseDate = purchaseDate
                };

                totalPurchaseAmount += purchase.Quantity * purchase.UnitPrice;
                await _context.Purchases.AddAsync(purchase);
            }

            // Update vendor's total credit
            vendor.TotalCredit += totalPurchaseAmount;
            _context.Vendors.Update(vendor);

            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }


        // Action to add a new product
        [HttpPost]
        public async Task<IActionResult> AddProduct(string caseName, int modelId, int caseManufacturerId) {
            if (!string.IsNullOrEmpty(caseName) && modelId > 0 && caseManufacturerId > 0) {
                var product = new Product {
                    CaseName = caseName,
                    ModelId = modelId,
                    CaseManufacturerId = caseManufacturerId
                };

                await _context.Products.AddAsync(product);
                await _context.SaveChangesAsync();

                return Json(new { success = true, productId = product.ProductId, caseName = product.CaseName });
            }
            return Json(new { success = false });
        }
    }
}