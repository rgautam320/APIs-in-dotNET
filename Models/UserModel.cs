using System.ComponentModel.DataAnnotations;

namespace Hotel_Booking.Models
{
    public class UserModel
    {
        [Key]
        public int UserId { get; set; }
        [Required, MinLength(3, ErrorMessage = "Name can't be shorter than 3 char."), MaxLength(55, ErrorMessage = "Name can't be longer than 55 char.")]
        public string FullName { get; set; }
        [Required, MinLength(3, ErrorMessage = "Email can't be shorter than 3 char."), MaxLength(55, ErrorMessage = "Email can't be longer than 55 char.")]
        public string Email { get; set; }
        public string ProfilePicture { get; set; }
        public string PhoneNumber { get; set; }
    }
}
