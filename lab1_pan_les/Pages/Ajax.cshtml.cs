using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
namespace lab1_pan_les.Pages
{ 
    public class AjaxModel : PageModel
    {
        [BindProperty]
        public string Name { get; set; }
    
        [BindProperty]
        public string Email { get; set; }

        public IActionResult OnPost()
        {
            // Handle the submitted form data here
            // You can add validation, saving to a database, etc.

            // Return a simple JSON response
            return new JsonResult(new { success = true });
        }
    }
}
