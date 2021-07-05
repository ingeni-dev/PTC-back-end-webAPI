namespace PTC_back_end_webAPI.Models.ColorFolder
{
    public class SetSendAllow
    {
        public string token { get; set; }
        public string cfSeq { get; set; }
        public string prodID { get; set; }
        public string saleID { get; set; }
        public string saleOrg { get; set; }
        public int submitQTY { get; set; }
        public string appvFlag { get; set; }
        public string cfRejReason { get; set; }
        public string remark { get; set; }
        public string returnDate { get; set; }

    }
}