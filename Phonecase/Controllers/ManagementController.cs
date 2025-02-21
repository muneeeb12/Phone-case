using Microsoft.AspNetCore.Mvc;

namespace Phonecase.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using System.Collections.Generic;

    public class ManagementController : Controller {
        // In-memory lists for demo purposes (replace with database in production)
        private static List<string> CaseCompanies = new List<string>();
        private static List<string> PhoneModels = new List<string>();

        // Action to display the management page
        public IActionResult Index() {
            ViewBag.CaseCompanies = CaseCompanies;
            ViewBag.PhoneModels = PhoneModels;
            return View();
        }

        // Action to add a case company
        [HttpPost]
        public IActionResult AddCaseCompany(string companyName) {
            if (!string.IsNullOrEmpty(companyName))
            {
                CaseCompanies.Add(companyName);
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
            }
            return RedirectToAction("Index");
        }
    }
}
