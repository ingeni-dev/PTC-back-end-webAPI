namespace webAPI.Models.Elearning
{
    public class RequestCourses
    {
        public string instantFlag { get; set; }
        public string userID { get; set; }
    }
    public class RequestApplicants
    {
        public string queryID { get; set; }
        public string timeSeq { get; set; }
    }
    public class SetQrCode
    {
        public string qrCode { get; set; }
    }
}