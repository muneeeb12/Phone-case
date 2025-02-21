using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Phonecase.Data;
using Phonecase.Models;
using Phonecase.Repositories;
using System.Threading.Tasks;

namespace Phonecase.Controllers {
    public class ProductController : Controller {
        private readonly PhoneCaseDbContext _dbContext;
        private readonly IProductRepository _productrepository;
        private readonly IManagementRepository _managementrepository;

        public ProductController(
            PhoneCaseDbContext dbContext,
            IProductRepository productrepository,
            IManagementRepository managementRepository) {

            _dbContext = dbContext;
            _productrepository = productrepository;
            _managementrepository = managementRepository;
        }

        // Action to display the product management page
        public async Task<IActionResult> Index() {
            // Fetch products with related data
            var products = await _productrepository.GetAllAsync();

            // Fetch models and case manufacturers for dropdowns
            ViewBag.Models = await _managementrepository.GetAllModelAsync();
            ViewBag.CaseManufacturers = await _managementrepository.GetAllCompanyAsync();

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

                await _productrepository.CreateProductAsync(product);
                
            }
            return RedirectToAction("Index");
        }
    }
}