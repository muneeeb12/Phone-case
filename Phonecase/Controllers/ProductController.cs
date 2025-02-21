using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Phonecase.Data;
using Phonecase.Models;
using System.Threading.Tasks;

namespace Phonecase.Controllers {
    public class ProductController : Controller {
        private readonly PhoneCaseDbContext _dbContext;

        public ProductController(PhoneCaseDbContext dbContext) {
            _dbContext = dbContext;
        }

        // Action to display the product management page
        public async Task<IActionResult> Index() {
            // Fetch products with related data
            var products = await _dbContext.Products
                .Include(p => p.Model)
                .Include(p => p.CaseManufacturer)
                .ToListAsync();

            // Fetch models and case manufacturers for dropdowns
            ViewBag.Models = await _dbContext.Models.ToListAsync();
            ViewBag.CaseManufacturers = await _dbContext.CaseManufacturers.ToListAsync();

            return View(products);
        }

        // Action to add a new product
        [HttpPost]
        public async Task<IActionResult> AddProduct(int modelId, int caseManufacturerId, string caseName) {
            if (modelId > 0 && caseManufacturerId > 0 && !string.IsNullOrEmpty(caseName)) {
                var product = new Product {
                    ModelId = modelId,
                    CaseManufacturerId = caseManufacturerId,
                    CaseName = caseName
                };

                await _dbContext.Products.AddAsync(product);
                await _dbContext.SaveChangesAsync();
            }
            return RedirectToAction("Index");
        }
    }
}