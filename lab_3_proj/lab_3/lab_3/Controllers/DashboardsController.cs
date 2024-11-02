using lab_3.Models;
using lab_3.Services;
using Microsoft.AspNetCore.Mvc;

namespace lab_3.Controllers;

public class DashboardsController : Controller
{
    private readonly JsonService<ProductModel> _jsonService;

    public DashboardsController()
    {
        _jsonService = new JsonService<ProductModel>("C:\\Users\\mateu\\Documents\\GitHub\\zes_lul\\lab_3_proj\\lab_3\\lab_3\\Data\\Products.json");
    }
    
    public IActionResult Index(string searchTerm, string category, string sortBy)
    {
        var productModels = _jsonService.ReadAll();
        return View(productModels);
    }
}