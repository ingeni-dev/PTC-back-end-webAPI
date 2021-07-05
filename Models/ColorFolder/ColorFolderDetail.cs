namespace PTC_back_end_webAPI.Models.ColorFolder
{
    public class ColorFolderDetail
    {
        public string PROD_ID { get; set; }
        public string PROD_DESC { get; set; }
        public string SN_NO { get; set; }
        public string CF_STATUS_DESC { get; set; }
        public string LOC_DETAIL { get; set; }
        public string TRAN_DATE { get; set; }
        public string CF_SN { get; set; }
    }

    public class ColorFolderDetailMap
    {
        public string prodID { get; set; }
        public string prodDESC { get; set; }
        public string snNO { get; set; }
        public string cfStatusDESC { get; set; }
        public string locDetail { get; set; }
        public string tranDate { get; set; }
        public string cfSN { get; set; }
    }
}