namespace PTC_back_end_webAPI.Models.ColorFolder
{
    public class FilePrint
    {
        public string prodID { get; set; }
        public string prodDesc { get; set; }
        public int num { get; set; }
        public string qrCode { get; set; }
    }
    public class FolderPrint
    {
        public string appDate { get; set; }
        public string cfSn { get; set; }
        public string custName { get; set; }
        public string expireDate { get; set; }
        public int num { get; set; }
        public string prodDesc { get; set; }
        public string approval { get; set; }
        public string qrCode { get; set; }
        public string runningNo { get; set; }
    }
}