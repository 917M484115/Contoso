using Contoso.Data;
using System.Collections.Generic;

namespace Contoso.Facade.SchoolViewModels {
    public class InstructorIndexData {
        public IEnumerable<Instructor> Instructors { get; set; }
        public IEnumerable<Course> Courses { get; set; }
        public IEnumerable<Enrollment> Enrollments { get; set; }
    }
}