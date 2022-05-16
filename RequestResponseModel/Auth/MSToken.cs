namespace Hotel_Booking.RequestResponseModel.Auth
{
    public class MSToken
    {
        public string Access_Token { get; set; }
        public string Token_Type { get; set; }
        public string Scope { get; set; }
        public string Refresh_Token { get; set; }
        public string Id_Token { get; set; }
    }
}
