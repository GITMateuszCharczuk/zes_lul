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
            // Return the submitted data in the JSON response
            return new JsonResult(new { success = true, name = Name, email = Email });
        }
    }
}