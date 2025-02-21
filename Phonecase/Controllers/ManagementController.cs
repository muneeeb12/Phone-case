using Microsoft.AspNetCore.Mvc;
using Phonecase.Data;
using Phonecase.Models;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace Phonecase.Controllers {
    public class ManagementController : Controller {
        private readonly PhoneCaseDbContext _dbContext;

        public ManagementController(PhoneCaseDbContext dbContext) {
            _dbContext = dbContext;
        }

        // Action to display the management page
        public async Task<IActionResult> Index() {
            var CaseCompanies = await _dbContext.CaseManufacturers.ToListAsync();
            var PhoneModels = await _dbContext.Models.ToListAsync();
            ViewBag.CaseCompanies = CaseCompanies;
            ViewBag.PhoneModels = PhoneModels;
            return View();
        }

        // Action to add a case company
        [HttpPost]
        public async Task<IActionResult> AddCaseCompany(string companyName) {
            if (!string.IsNullOrEmpty(companyName)) {
                var company = new CaseManufacturer {
                    Name = companyName
                };

                await _dbContext.CaseManufacturers.AddAsync(company);
                await _dbContext.SaveChangesAsync();
            }
            return RedirectToAction("Index");
        }

        // Action to delete a case company
        [HttpPost]
        public async Task<IActionResult> DeleteCaseCompany(string companyName) {
            var company = await _dbContext.CaseManufacturers.FirstOrDefaultAsync(c => c.Name == companyName);
            if (company != null) {
                _dbContext.CaseManufacturers.Remove(company);
                await _dbContext.SaveChangesAsync();
            }
            return RedirectToAction("Index");
        }

        // Action to add a phone model
        [HttpPost]
        public async Task<IActionResult> AddPhoneModel(string modelName) {
            if (!string.IsNullOrEmpty(modelName)) {
                var model = new Model {
                    Name = modelName
                };

                await _dbContext.Models.AddAsync(model);
                await _dbContext.SaveChangesAsync();
            }
            return RedirectToAction("Index");
        }
    }
}