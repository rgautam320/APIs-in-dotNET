namespace Hotel_Booking.RequestResponseModel.Auth
{
    public class DecodedData
    {
        public string Aud { get; set; }
        public int Iat { get; set; }
        public int Exp { get; set; }
        public string Name { get; set; }
        public string Preferred_username { get; set; }
        public string Scp { get; set; }
        public string Ver { get; set; }
    }
}
