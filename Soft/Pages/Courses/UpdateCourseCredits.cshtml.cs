using Contoso.Infra;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace Contoso.Soft.Pages.Courses {
    public class UpdateCourseCreditsModel :PageModel {
        private readonly ApplicationDbContext _context;

        public UpdateCourseCreditsModel(ApplicationDbContext context) {
            _context = context;
        }
        public IActionResult OnGet() {
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(double? multiplier) {
            if (multiplier != null) {
                ViewData["RowsAffected"] =
                    await _context.Database.ExecuteSqlRawAsync(
                        "UPDATE Course SET Credits = Credits * {0}",
                        parameters: multiplier);
            }
            return Page();
        }
    }
}
