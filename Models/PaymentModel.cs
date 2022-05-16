using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Hotel_Booking.Models
{
     public class PaymentModel
     {
          [Key]
          public int PaymentId { get; set; }
          [Required, Range(0, 99999)]
          public float PaymentAmount { get; set; }
          [ForeignKey("UserId")]
          public int UserId { get; set; }
          public virtual UserModel User { get; set; }
          [ForeignKey("Booking")]
          public int BookingId { get; set; }
          public virtual BookingModel Booking { get; set; }
     }
}
