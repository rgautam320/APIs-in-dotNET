using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SchoolAPI.Model
{
    public class Student
    {
        [Key]
        public int StudentID { get; set; }
        [Required]
        public string StudentName { get; set; }
        [Required]
        public DateTime? DateOfBirth { get; set; }
        [Required]
        public string Photo { get; set; }
        public int GradeId { get; set; }
        [ForeignKey("GradeId")]
        public virtual Grade Grade { get; set; }
        public int StudentAddressId { get; set; }
        [ForeignKey("StudentAddressId")]
        public virtual StudentAddress StudentAddress { get; set; }
        public int DepartmentId { get; set; }
        [ForeignKey("DepartmentId")]
        public virtual Department Department { get; set; }
        public int CourseId { get; set; }
        [ForeignKey("CourseId")]
        public virtual Course Course { get; set; }
    }

    public class Grade
    {
        [Key]
        public int GradeId { get; set; }
        [Required]
        public string GradeName { get; set; }
    }

    public class StudentAddress
    {
        [Key]
        public int StudentAddressId { get; set; }
        [Required]
        public string Address { get; set; }
        public string City { get; set; }
        [Required]
        public int Zipcode { get; set; }
        public string State { get; set; }
        [Required]
        public string Country { get; set; }
        public virtual Student Student { get; set; }
    }

    public class Department
    {
        [Key]
        public int DepartmentId { get; set; }
        [Required]
        public string DepartmentName { get; set; }
        public string DepartmentLocation { get; set; }
    }

    public class Course
    {
        [Key]
        public int CourseId { get; set; }
        [Required]
        public string CourseName { get; set; }
        [Required]
        public int CourseCredit { get; set; }
    }
}
