using lab2_pan_les.Models;
using Microsoft.AspNetCore.Mvc;

namespace lab2_pan_les.Controllers
{
    public class HomeController : Controller
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
            // Obliczamy sumê liczb
            model.Result = model.Number1 + model.Number2;

            // Zwracamy model do widoku, aby zachowaæ dane i wyœwietliæ wynik
            return View(model);
        }
    }
}
