using Contoso.Facade.SchoolViewModels;
using Contoso.Infra;
using Contoso.Pages;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Contoso.Soft.Pages.Courses {
    public class IndexModel :BasePageModel {
        //TODO 1.1 Vii sisu kursuste mudelisse ja kasuta händlerit
        private readonly ApplicationDbContext _context;

        public IndexModel(ApplicationDbContext context) {
            _context = context;
        }

        public IList<CourseViewModel> CourseVM { get; set; }

        public async Task OnGetAsync() {
            CourseVM = await _context.Courses
                    .Select(p => new CourseViewModel {
                        CourseID = p.CourseID,
                        Title = p.Title,
                        Credits = p.Credits,
                        DepartmentName = p.Department.Name
                    }).ToListAsync();
        }
    }
}
