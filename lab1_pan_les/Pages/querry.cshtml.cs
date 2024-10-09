using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace lab1_pan_les.Pages
{
    public class querry : PageModel
    {
        private readonly ILogger<PrivacyModel> _logger;

        public querry(ILogger<PrivacyModel> logger)
        {
            _logger = logger;
        }

        public void OnGet()
        {
        }
    }

}
