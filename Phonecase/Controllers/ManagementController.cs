





//using Microsoft.AspNetCore.Mvc;
//using Phonecase.Data;
//using Phonecase.Models;
//using Microsoft.EntityFrameworkCore;
//using System.Threading.Tasks;
//using Phonecase.Repositories;

//namespace Phonecase.Controllers {
//    public class ManagementController : Controller 
//    {

//        private readonly PhoneCaseDbContext _dbContext;
//        private readonly ICaseManufacturerRepository _repository;

//        public ManagementController(PhoneCaseDbContext dbContext, ICaseManufacturerRepository repository) {
//            _dbContext = dbContext;
//            _repository = repository;
//        }

//        // Action to display the management page
//        public async Task<IActionResult> Index() {
//            var CaseCompanies = await _dbContext.CaseManufacturers.ToListAsync();
//            var PhoneModels = await _dbContext.Models.ToListAsync();
//            ViewBag.CaseCompanies = CaseCompanies;
//            ViewBag.PhoneModels = PhoneModels;
//            return View();
//        }

//        [HttpGet]
//        public async Task<IActionResult> GetAll()
//        {
//            var manufacturers = await _repository.GetAllAsync();
//            return Ok(manufacturers);
//        }
//        // Action to add a case company
//        [HttpPost]
//        public async Task<IActionResult> AddCaseCompany(string companyName) 
//        {
//            if (companyName == null || string.IsNullOrWhiteSpace(companyName))
//            {
//                return BadRequest("Invalid case manufacturer data.");
//            }
//            var companyname = new CaseManufacturer
//            {
//                Name = companyName,
//            };
//                await _repository.CreateAsync(companyname);
//            return Ok(companyname);
//        }

//        // Action to delete a case company
//        [HttpDelete("{id}")]
//        public async Task<IActionResult> Delete(int id)
//        {
//            var result = await _repository.DeleteAsync(id);
//            if (result == null)
//            {
//                return NotFound();
//            }
//            return NoContent();
//        }

        // Action to display the management page
        public async Task<IActionResult> Index() {
            var CaseCompanies = await _dbContext.CaseManufacturers.ToListAsync();
            var PhoneModels = await _dbContext.PhoneModels.ToListAsync();
            ViewBag.CaseCompanies = CaseCompanies;
            ViewBag.PhoneModels = PhoneModels;
            return View();
        }


//        // Action to add a phone model
//        [HttpPost]
//        public async Task<IActionResult> AddPhoneModel(Model modelName) {
            
//                var model = new Model {
//                    Name = modelName.Name
//                };

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