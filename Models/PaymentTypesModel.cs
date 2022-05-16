using System.ComponentModel.DataAnnotations;

namespace Hotel_Booking.Models
{
    public class PaymentTypesModel
    {
        [Key]
        public int PaymentTypeId { get; set; }
        [Required]
        public string PaymentType { get; set; }
    }
}
