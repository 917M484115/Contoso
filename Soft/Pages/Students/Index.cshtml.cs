using Contoso.Data;
using Contoso.Facade;
using Contoso.Infra;
using Contoso.Pages;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Contoso.Soft.Pages.Students {
    public class IndexModel :BasePageModel {
        //TODO 1.4 Vii sisu kursuste mudelisse ja kasuta händlerit
        private readonly ApplicationDbContext _context;

        public IndexModel(ApplicationDbContext context) {
            _context = context;
        }
        public override bool HasNextPage => Students?.HasNextPage??false;
        public override bool HasPreviousPage => Students?.HasPreviousPage ?? false;
        public override int PageIndex => Students?.PageIndex ?? 0;
        public PaginatedList<StudentViewModel> Students { get; set; }
        public async Task OnGetAsync(string sortOrder,
            string currentFilter, string searchString, int? pageIndex) {
            CurrentSort = sortOrder;
            NameSort = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            DateSort = sortOrder == "Date" ? "date_desc" : "Date";
            if (searchString != null) {
                pageIndex = 1;
            } else {
                searchString = currentFilter;
            }
            CurrentFilter = searchString;

            IQueryable<Student> studentsIQ = from s in _context.Students
                                             select s;
            if (!String.IsNullOrEmpty(searchString)) {
                studentsIQ = studentsIQ.Where(s => s.LastName.Contains(searchString)
                                       || s.FirstMidName.Contains(searchString));
            }

            switch (sortOrder) {
                case "name_desc":
                    studentsIQ = studentsIQ.OrderByDescending(s => s.LastName);
                    break;
                case "Date":
                    studentsIQ = studentsIQ.OrderBy(s => s.EnrollmentDate);
                    break;
                case "date_desc":
                    studentsIQ = studentsIQ.OrderByDescending(s => s.EnrollmentDate);
                    break;
                default:
                    studentsIQ = studentsIQ.OrderBy(s => s.LastName);
                    break;
            }

            int pageSize = 3;
            var students = await PaginatedList<Student>.CreateAsync(
                studentsIQ.AsNoTracking(), pageIndex ?? 1, pageSize);
            var studentsViewModels = new List<StudentViewModel>();
            foreach (var s in students) {
                studentsViewModels.Add(toViewModel(s));
            }
            var count = await studentsIQ.AsNoTracking().CountAsync();
            Students = new PaginatedList<StudentViewModel>(studentsViewModels, count, pageIndex ?? 1, pageSize);
        }

        private StudentViewModel toViewModel(Student s) {
            var v = new StudentViewModel();
            v.ID = s.ID;
            v.LastName = s.LastName;
            v.FirstMidName = s.FirstMidName;
            v.EnrollmentDate = s.EnrollmentDate;
            var photo = Convert.ToBase64String(s?.Photo ?? Array.Empty<byte>(), 0, s?.Photo?.Length ?? 0);
            v.PhotoAsString = "data:image/jpg;base64," + photo;
            return v;
        }
    }
}