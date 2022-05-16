using Hotel_Booking.Models;
using Microsoft.EntityFrameworkCore;

namespace Hotel_Booking.Data
{
    public class HotelBookingDBContext : DbContext
    {
        public HotelBookingDBContext(DbContextOptions<HotelBookingDBContext> options) : base(options)
        {

        }
        public DbSet<UserModel> Users { get; set; }
        public DbSet<PaymentTypesModel> PaymentTypes { get; set; }
        public DbSet<RoomTypesModel> RoomTypes { get; set; }
        public DbSet<RoomModel> Rooms { get; set; }
        public DbSet<RoomImagesModel> RoomImages { get; set; }
        public DbSet<BookingModel> Bookings { get; set; }
        public DbSet<PaymentModel> Payments { get; set; }
    }
}
