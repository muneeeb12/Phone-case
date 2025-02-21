using Microsoft.AspNetCore.Mvc;
using Phonecase.Data;
using Phonecase.Models;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using Phonecase.Repositories;

namespace Phonecase.Controllers {
    public class ManagementController : Controller {
        private readonly PhoneCaseDbContext _dbContext;
        private readonly IManagementRepository _repository;

        public ManagementController(
            PhoneCaseDbContext dbContext,
            IManagementRepository repository) {
            _dbContext = dbContext;
            _repository = repository;
        }

        // Action to display the management page
        public async Task<IActionResult> Index() {
            var CaseCompanies = await _repository.GetAllCompanyAsync();
            var PhoneModels = await _repository.GetAllModelAsync();
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

                await _repository.CreateCompanyAsync(company);
                //await _dbContext.CaseManufacturers.AddAsync(company);
                //await _dbContext.SaveChangesAsync();
            }
            return RedirectToAction("Index");
        }

        // Action to delete a case company
        [HttpPost]
        public async Task<IActionResult> DeleteCaseCompany(int CaseManufacturerId) {
            var company = await _dbContext.CaseManufacturers.FirstOrDefaultAsync(c => c.CaseManufacturerId == CaseManufacturerId);
            if (company != null) 
            {
                await _repository.DeleteCompanyAsync(CaseManufacturerId);
                //_dbContext.CaseManufacturers.Remove(company);
                //await _dbContext.SaveChangesAsync();
            }
            return RedirectToAction("Index");
        }

        // Action to add a phone model
        [HttpPost]
        public async Task<IActionResult> AddPhoneModel(string modelName) {
            if (!string.IsNullOrEmpty(modelName)) {
                var model = new PhoneModel {
                    Name = modelName
                };

                await _repository.CreateModelAsync(model);

                //await _dbContext.PhoneModels.AddAsync(model);
                //await _dbContext.SaveChangesAsync();
            }
            return RedirectToAction("Index");
        }
    }
}