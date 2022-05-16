using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Hotel_Booking.Models
{
     public class RoomImagesModel
     {
          [Key]
          public int RoomImageId { get; set; }
          [Required, MinLength(3, ErrorMessage = "Something needs to be there in Room Name.")]
          public string RoomImage { get; set; }
          [Required]
          public string ImageType { get; set; }
          [ForeignKey("Room")]
          public int RoomId { get; set; }
          public virtual RoomModel Room { get; set; }
     }
}
