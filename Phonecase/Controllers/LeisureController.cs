using Microsoft.AspNetCore.Mvc;

namespace Phonecase.Controllers
{
    public class LeisureController : Controller
    {
        public IActionResult Dashboard()
        {
            return View();
        }
    }
}
