using Contoso.Data;
using Contoso.Infra;
using Contoso.Pages;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Contoso.Soft.Pages.Departments {
    public class IndexModel :BasePageModel {
        //TODO 1.2 Vii sisu kursuste mudelisse ja kasuta händlerit
        private readonly ApplicationDbContext _context;

        public IndexModel(ApplicationDbContext context) {
            _context = context;
        }

        public IList<Department> Department { get; set; }

        public async Task OnGetAsync() {
            Department = await _context.Departments
                .Include(d => d.Administrator).ToListAsync();
        }
    }
}
