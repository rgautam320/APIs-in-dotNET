using System.ComponentModel.DataAnnotations;

namespace Person.Models
{
    public class PersonModel
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public string email { get; set; }
        [Required, MaxLength(50)]
        public string firstname { get; set; }
        [Required, MaxLength(50)]
        public string lastname { get; set; }
        [Required]
        public int age { get; set; }
        public string phone { get; set; }
    }
}
