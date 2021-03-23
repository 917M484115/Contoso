using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Contoso.Data {
    public enum Grade {
        A, B, C, D, F
    }

    public class Enrollment {
        public int EnrollmentID { get; set; }
        public int CourseID { get; set; }
        public int StudentID { get; set; }
        [DisplayFormat(NullDisplayText = "No grade")]
        public Grade? Grade { get; set; }
        [DisplayName("Course Title")]
        public string CourseTitle => Course?.Title ?? string.Empty;
        public Course Course { get; set; }
        public Student Student { get; set; }
    }
}