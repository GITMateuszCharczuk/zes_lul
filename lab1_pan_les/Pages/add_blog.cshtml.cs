using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

namespace lab1_pan_les.Pages
{
    public class AddBlog : PageModel
    {
        [BindProperty]
        public AddBlogPost AddBlogPostRequest { get; set; }

        public bool Submitted { get; set; }

        public void OnGet()
        {
        }

        public void OnPost()
        {
            if (ModelState.IsValid)
            {
                Submitted = true;
            }
        }
    }

    public class AddBlogPost
    {
        [Required]
        [StringLength(100, ErrorMessage = "Heading cannot be longer than 100 characters.")]
        public string Heading { get; set; }

        [Required]
        [StringLength(60, ErrorMessage = "PageTitle cannot be longer than 60 characters.")]
        public string PageTitle { get; set; }

        [Required]
        [MinLength(50, ErrorMessage = "Content must be at least 50 characters long.")]
        public string Content { get; set; }

        [Required]
        [StringLength(200, ErrorMessage = "ShortDescription cannot be longer than 200 characters.")]
        public string ShortDescription { get; set; }

        [Required]
        [Url(ErrorMessage = "FeaturedImageUrl must be a valid URL.")]
        public string FeaturedImageUrl { get; set; }

        [Required]
        public string UrlHandle { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [CustomValidation(typeof(AddBlogPost), nameof(ValidatePublishedDate))]
        public DateTime PublishedDate { get; set; }

        [Required]
        [StringLength(50, ErrorMessage = "Author name cannot be longer than 50 characters.")]
        public string Author { get; set; }

        [Required]
        public string Tags { get; set; }

        public bool Visible { get; set; }

        public static ValidationResult ValidatePublishedDate(DateTime date, ValidationContext context)
        {
            if (date > DateTime.Now)
            {
                return new ValidationResult("Published date cannot be in the future.");
            }
            return ValidationResult.Success;
        }
    }
  }
