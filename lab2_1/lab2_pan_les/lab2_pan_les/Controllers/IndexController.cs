using lab2_pan_les.Models;
using Microsoft.AspNetCore.Mvc;

namespace lab2_pan_les.Controllers
{
    public class IndexController : Controller
    {
        // Default action for rendering the form
        public IActionResult Index()
        {
            return View(new CalculatorModel());
        }

        // Action for handling the form submission
        [HttpPost]
        public IActionResult Index(CalculatorModel model)
        {
            // Obliczamy sum� liczb
            model.Result = model.Number1 + model.Number2;

            // Zwracamy model do widoku, aby zachowa� dane i wy�wietli� wynik
            return View(model);
        }
    }
}
