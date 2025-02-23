using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Phonecase.Data;
using Phonecase.Models;
using Phonecase.Repositories;
using System.Threading.Tasks;

namespace Phonecase.Controllers {
    public class ProductController : Controller {
        private readonly IProductRepository _productrepository;
        private readonly IManagementRepository _managementrepository;

        public ProductController(
            IProductRepository productrepository,
            IManagementRepository managementRepository) {

            _productrepository = productrepository;
            _managementrepository = managementRepository;
        }

        // Action to display the product management page
        [HttpGet]
        public async Task<IActionResult> Index(int? id)
        {
            var products = await _productrepository.GetAllAsync();
            ViewBag.Models = await _managementrepository.GetAllModelAsync();
            ViewBag.CaseManufacturers = await _managementrepository.GetAllCompanyAsync();

            if (id.HasValue)
            {
                ViewBag.EditProduct = await _productrepository.GetProductByIdAsync(id.Value);
            }

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

        [HttpPut]
        public async Task<IActionResult> UpdateProduct(int id, int modelId, int caseManufacturerId, string caseName)
        {
            if (id <= 0 || modelId <= 0 || caseManufacturerId <= 0 || string.IsNullOrEmpty(caseName))
            {
                return BadRequest("Invalid product data.");
            }

            var existingProduct = await _productrepository.GetProductByIdAsync(id);
            if (existingProduct == null)
            {
                return NotFound("Product not found.");
            }

            existingProduct.ModelId = modelId;
            existingProduct.CaseManufacturerId = caseManufacturerId;
            existingProduct.CaseName = caseName;

            await _productrepository.UpdateProductAsync(existingProduct);

            return Ok("Product updated successfully.");
        }
        [HttpPost]
        public async Task<IActionResult> SaveProduct(int? ProductId, int ModelId, int CaseManufacturerId, string CaseName)
        {
            if (ModelId <= 0 || CaseManufacturerId <= 0 || string.IsNullOrEmpty(CaseName))
            {
                return BadRequest("Invalid product data.");
            }

            if (ProductId.HasValue && ProductId > 0)
            {
                // Update existing product
                var existingProduct = await _productrepository.GetProductByIdAsync(ProductId.Value);
                if (existingProduct == null)
                {
                    return NotFound("Product not found.");
                }

                existingProduct.ModelId = ModelId;
                existingProduct.CaseManufacturerId = CaseManufacturerId;
                existingProduct.CaseName = CaseName;

                await _productrepository.UpdateProductAsync(existingProduct);
            }
            else
            {
                // Create new product
                var newProduct = new Product
                {
                    ModelId = ModelId,
                    CaseManufacturerId = CaseManufacturerId,
                    CaseName = CaseName
                };

                await _productrepository.CreateProductAsync(newProduct);
            }

            return RedirectToAction("Index");
        }
    }
}