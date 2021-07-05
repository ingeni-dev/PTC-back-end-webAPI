namespace PTC_back_end_webAPI.Models.ColorFolder
{
    public class RejectMessage
    {
        public string CF_REJ_REASON { get; set; }
        public string CF_REJ_REASON_DESC { get; set; }
    }
    public class RejectMessageMap
    {
        public string rejID { get; set; }
        public string rejMessage { get; set; }
    }
}