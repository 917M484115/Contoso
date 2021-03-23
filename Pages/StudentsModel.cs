using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Contoso.Infra;
using Contoso.Facade;
using Contoso.Data;
using Microsoft.Extensions.Logging;

namespace Contoso.Pages {
   
    public class StudentsModel :BasePageModel {
        private readonly ApplicationDbContext _context;
        public StudentsModel(ApplicationDbContext c) => _context = c;
        public IActionResult OnGetCreate() {
            return Page();
        }
        [BindProperty]
        public StudentViewModel StudentVM { get; set; }
        public async Task<IActionResult> OnPostCreateAsync() {
            if (!ModelState.IsValid) {
                return Page();
            }
            var student = toDataModel(StudentVM);
            _context.Add(student);
            await _context.SaveChangesAsync();
            return RedirectToPage("./Index");
        }
        private Student toDataModel(StudentViewModel v) {
            var s = new Student();
            s.LastName = v.LastName;
            s.FirstMidName = v.FirstMidName;
            s.EnrollmentDate = v.EnrollmentDate;
            if (string.IsNullOrEmpty(v?.Photo?.FileName)) return s;
            var stream = new MemoryStream();
            v?.Photo?.CopyTo(stream);
            if (stream.Length < 2097152)
                s.Photo = stream.ToArray();
            return s;
        }
        public string ErrorMessage { get; set; }
        public StudentViewModel Student { get; private set; }

        public async Task<IActionResult> OnGetDeleteAsync(int? id, bool? saveChangesError = false) {
            if (id == null) {
                return NotFound();
            }
            var student = await _context.Students
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.ID == id);
            if (student == null) {
                return NotFound();
            }
            Student = toViewModel(student);
            if (saveChangesError.GetValueOrDefault()) {
                ErrorMessage = "Delete failed. Try again";
            }
            return Page();
        }
        private readonly ILogger _logger;
        public async Task<IActionResult> OnPostDeleteAsync(int? id) {
            if (id == null) {
                return NotFound();
            }

            var student = await _context.Students.FindAsync(id);

            if (student == null) {
                return NotFound();
            }
            try {
                _context.Students.Remove(student);
                await _context.SaveChangesAsync();
                return RedirectToPage("./Index");
            } catch (DbUpdateException ex) {
                _logger.LogInformation(Convert.ToString(ex));
                return RedirectToAction("./Delete",
                                     new { id, saveChangesError = true });
            }
        }
        public ICollection<Enrollment> Enrollments { get; private set; }

        public async Task<IActionResult> OnGetDetailsAsync(int? id) {
            if (id == null) {
                return NotFound();
            }
            var student = await _context.Students
                   .Include(s => s.Enrollments)
                   .ThenInclude(e => e.Course)
                   .AsNoTracking()
                   .FirstOrDefaultAsync(m => m.ID == id);
            if (student == null) {
                return NotFound();
            }
            Enrollments = student.Enrollments;
            Student = toViewModel(student);
            return Page();
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
        public async Task<IActionResult> OnGetEditAsync(int? id) {
            if (id == null) {
                return NotFound();
            }
            var student = await _context.Students.FindAsync(id);
            if (student == null) {
                return NotFound();
            }
            Student = toViewModel(student);
            return Page();
        }
        public async Task<IActionResult> OnPostEditAsync(int id) {
            var studentToUpdate = await _context.Students.FindAsync(id);

            if (studentToUpdate == null) {
                return NotFound();
            }

            toDataModel(Student, studentToUpdate);
            try {
                _context.Update(studentToUpdate);
                await _context.SaveChangesAsync();
                return RedirectToPage("./Index");
            } catch {
                return Page();
            }
        }
        private void toDataModel(StudentViewModel v, Student s) {
            s.LastName = v.LastName;
            s.FirstMidName = v.FirstMidName;
            s.EnrollmentDate = v.EnrollmentDate;
            if (string.IsNullOrEmpty(v?.Photo?.FileName)) return;
            var stream = new MemoryStream();
            v?.Photo?.CopyTo(stream);
            if (stream.Length < 2097152)
                s.Photo = stream.ToArray();
        }
    }
}