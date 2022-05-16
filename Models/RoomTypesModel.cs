using System.ComponentModel.DataAnnotations;

namespace Hotel_Booking.Models
{
    public class RoomTypesModel
    {
        [Key]
        public int RoomTypeId { get; set; }
        [Required]  
        public string RoomType { get; set; }
    }
}
