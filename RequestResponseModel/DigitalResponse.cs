namespace Hotel_Booking.RequestResponseModel
{
    public class DigitalSuccessResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public object Data { get; set; }
    }
    public class DigitalFailureResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; }
    }
    public class DigitalLoginResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public object Data { get; set; }
        public object DecodedData { get; set; }
    }
}
