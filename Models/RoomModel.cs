using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Hotel_Booking.Models
{
     public class RoomModel
     {
          [Key]
          public int RoomId { get; set; }
          [Required, MinLength(3, ErrorMessage = "Room Number can't be shorter than 3 char.")]
          public string RoomNumber { get; set; }
          [Required, Range(0, 100000)]
          public int RoomPrice { get; set; }
          [Required, MinLength(10, ErrorMessage = "Description can't be shorter than 10 char.")]
          public string RoomDescription { get; set; }
          [Required, Range(0, 99)]
          public int RoomCapacity { get; set; }
          [Required, DefaultValue(false)]
          public bool IsActive { get; set; }
          [Required, DefaultValue(false)]
          public bool IsBooked { get; set; }
          [ForeignKey("RoomTypes")]
          public int RoomTypeId { get; set; }
          public virtual RoomTypesModel RoomTypes { get; set; }
     }
}
