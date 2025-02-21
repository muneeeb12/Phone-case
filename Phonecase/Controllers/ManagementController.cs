using Microsoft.AspNetCore.Mvc;
using Phonecase.Data;
using System.Collections.Generic;
using Phonecase.Models;
using Microsoft.AspNetCore.Identity;

namespace Phonecase.Controllers
{
    public class ManagementController : Controller {

        private readonly PhoneCaseDbContext dbContext;
        private readonly UserManager<IdentityUser> userManager;
        // In-memory lists for demo purposes (replace with database in production)
        private static List<string> CaseCompanies = new List<string>();
        private static List<string> PhoneModels = new List<string>();

        public ManagementController(
            PhoneCaseDbContext dbContext,
            UserManager<IdentityUser> userManager)
        {
            this.dbContext = dbContext;
            this.userManager = userManager;
        }

        // Action to display the management page
        public IActionResult Index() {
            ViewBag.CaseCompanies = CaseCompanies;
            ViewBag.PhoneModels = PhoneModels;
            return View();
        }

        // Action to add a case company
        [HttpPost]
        public IActionResult AddCaseCompany (CaseManufacturer companyName) {
            if (ModelState.IsValid)
            {
                var companyname = new CaseManufacturer
                {
                    Name = companyName.Name,    
                };

                dbContext.CaseManufacturers.Add(companyName);
                dbContext.SaveChanges();
            }
            return RedirectToAction("Index");
        }

        // Action to delete a case company
        [HttpPost]
        public IActionResult DeleteCaseCompany(string companyName) {
            CaseCompanies.Remove(companyName);
            return RedirectToAction("Index");
        }

        // Action to add a phone model
        [HttpPost]
        public IActionResult AddPhoneModel(string modelName) {
            if (!string.IsNullOrEmpty(modelName)) {
                PhoneModels.Add(modelName);
                //dbContext.Models.Add(modelName);
                //dbContext.SaveChanges();
            }
            return RedirectToAction("Index");
        }
    }
}
