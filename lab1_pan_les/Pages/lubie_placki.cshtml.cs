using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace lab1_pan_les.Pages
{
    public class PlackiModel : PageModel
    {
        private readonly ILogger<PrivacyModel> _logger;

        public PlackiModel(ILogger<PrivacyModel> logger)
        {
            _logger = logger;
        }

        public void OnGet()
        {
        }
    }

}
