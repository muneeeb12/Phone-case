using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Phonecase.Data;
using Phonecase.Models;
using Phonecase.Repositories;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Phonecase.Controllers {
    public class RecordController : Controller {
        private readonly PhoneCaseDbContext _context;
        private readonly IVendorRepository _vendorrepository;
        private readonly IProductRepository _productrepository;
        private readonly IManagementRepository _managementrepository;

        public RecordController(
            PhoneCaseDbContext context,
            IVendorRepository vendorRepository,
            IProductRepository productRepository,
            IManagementRepository managementrepository) 
        {
            _context = context;
            _vendorrepository = vendorRepository;
            _productrepository = productRepository;
            _managementrepository = managementrepository;
            
        }

        // Action to display the record purchase page
        public async Task<IActionResult> Index() {
            // Fetch vendors and products for dropdowns
            ViewBag.Vendors = await _vendorrepository.GetVendorAsync();
            ViewBag.Products = await _productrepository.GetAllAsync();
            ViewBag.Models = await _managementrepository.GetAllModelAsync();
            ViewBag.CaseManufacturers = await _managementrepository.GetAllCompanyAsync();
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

            var vendor = await _vendorrepository.GetVendorByIdAsync(vendorId);
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
                await _vendorrepository.CreatePurchaseAsync(purchase);
                //await _context.Purchases.AddAsync(purchase);
            }

            // Update vendor's total credit
            vendor.TotalCredit += totalPurchaseAmount;
            await _vendorrepository.UpdateVendorAsync(vendor);

            return RedirectToAction("Index");
        }


        // Action to add a new product
        [HttpPost]
        public async Task<IActionResult> AddProduct(string caseName, int modelId, int caseManufacturerId) {
            if (!string.IsNullOrEmpty(caseName) && modelId > 0 && caseManufacturerId > 0) 
            {
                var product = new Product {
                    CaseName = caseName,
                    ModelId = modelId,
                    CaseManufacturerId = caseManufacturerId
                };

                await _productrepository.CreateProductAsync(product);

                return Json(new { success = true, productId = product.ProductId, caseName = product.CaseName });
            }
            return Json(new { success = false });
        }
    }
}

