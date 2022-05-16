using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Hotel_Booking.Models
{
     public class BookingModel
     {
          [Key]
          public int BookingId { get; set; }
          [Required(ErrorMessage = "Customer Name is required.")]
          public string CustomerName { get; set; }
          [Required(ErrorMessage = "Customer Address is required.")]
          public string CustomerAddress { get; set; }
          [Required, Range(0, 99999)]
          public float TotalPrice { get; set; }
          [Required(ErrorMessage = "Check in date is required.")]
          public DateTime BookingFrom { get; set; }
          [Required(ErrorMessage = "Check out date is required.")]
          public DateTime BookingTo { get; set; }
          [Required, Range(0, 99)]
          public int NumberOfMembers { get; set; }
          [Required, DefaultValue(false)]
          public bool IsConfirmed { get; set; }
          [ForeignKey("UserId")]
          public int UserId { get; set; }
          public virtual UserModel User { get; set; }
          [Required, DefaultValue(false)]
          public bool IsPaid { get; set; }
          [ForeignKey("Room")]
          public int RoomId { get; set; }
          public virtual RoomModel Room { get; set; }
          [ForeignKey("PaymentTypes")]
          public int PaymentTypeId { get; set; }
          public virtual PaymentTypesModel PaymentTypes { get; set; }
     }
}
