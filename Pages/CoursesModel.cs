using Contoso.Data;
using Contoso.Infra;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Contoso.Pages {
    //DONE 2.1 Vii Pages projekti
    //DONE 6.1 Kirjuta see kood puhtaks (elementaarseteks väikesteks meetoditeks)
    //TODO 6.2 Mõtle ka kuidas saaks lahti ViewData["Page"] ja ViewData["ItemId"]
    public class CoursesModel :BasePageModel {

        protected readonly ApplicationDbContext db;

        public CoursesModel(ApplicationDbContext c) => db = c;

        [BindProperty] public Course Course { get; private set; }

        public SelectList Departments { get; private set; }

        internal async Task save(params Action[] actions) {
            foreach (var a in actions) a();
            await db?.SaveChangesAsync();
        }
        internal DbSet<Course> courses => db?.Courses;

        internal int? departmentId => Course?.DepartmentID;

        internal void remove(Course c = null) => courses?.Remove(c??Course);

        internal void add(Course c = null) => courses?.Add(c??Course);

        internal async Task<Course> find(int? id)
            => isNull(id) ? null : isNull(courses) ? null : await courses.FindAsync(id);

        internal async Task<Course> load(int? id) {
            if (isNull(id)) return null;
            if (isNull(courses)) return null;
            return await courses
                .AsNoTracking()
                .Include(c => c.Department)
                .FirstOrDefaultAsync(m => m.CourseID == id);
        }

        internal async Task<bool> canUpdate(Course c,
            params Expression<Func<Course, object>>[] filter)
            => await TryUpdateModelAsync(c, "course", filter);

        internal IActionResult indexPage() => RedirectToPage("./Index");

        internal Expression<Func<Course, object>>[] edFilter =
            { c => c.Credits, c => c.DepartmentID, c => c.Title };
        internal Expression<Func<Course, object>>[] crFilter {
            get {
                var l = edFilter.ToList();
                l.Add(c => c.CourseID);
                return l.ToArray();
            }
        }
        internal SelectList loadDepartments(object selectedDepartment = null) {
            var q = from d in db.Departments orderby d.Name select d;
            return new SelectList(q.AsNoTracking(),
                        "DepartmentID", "Name", selectedDepartment);
        }
        public async Task<IActionResult> OnGetDetailsAsync(int? id) 
            => isNull(Course = await load(id)) ? NotFound(): Page();

        public async Task<IActionResult> OnGetDeleteAsync(int? id) 
            => isNull(Course= await load(id)) ? NotFound() : Page();

        public async Task<IActionResult> OnPostDeleteAsync(int? id) {
            if (isNull(Course = await find(id))) return NotFound();
            await save(() => remove(Course));
            return indexPage();
        }
        public async Task<IActionResult> OnPostEditAsync(int? id) {
            var c = await find(id);
            if (isNull(c)) return NotFound();
            if (!await canUpdate(c, edFilter)) await OnGetEditAsync();
            await save();
            return indexPage();
        }
        public async Task<IActionResult> OnGetEditAsync(int? id = null) {
            if (isNull(Course ??= await load(id))) return NotFound();
            Departments = loadDepartments(departmentId);
            return Page();
        }
        public async Task<IActionResult> OnPostCreateAsync() {
            var c = new Course();
            if (!await canUpdate(c, crFilter)) return OnGetCreate(departmentId);
            await save(() => add(c));
            return indexPage();
        }
        public IActionResult OnGetCreate(int? id) {
            Departments = loadDepartments(id);
            return Page();
        }
        internal static bool isNull(object c) => c is null;
    }
}