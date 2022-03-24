using Microsoft.EntityFrameworkCore;
using SchoolAPI.Model;

namespace SchoolAPI.Data
{
    public class SchoolContext : DbContext
    {
        public SchoolContext(DbContextOptions<SchoolContext> options) : base(options)
        {

        }
        public DbSet<Student> Students { get; set; }
        public DbSet<Grade> Grades { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<StudentAddress> StudentAddresses { get; set; }
        public DbSet<Course> Courses { get; set; }
    }
}
