using System.Collections.Generic;

namespace PTC_back_end_webAPI.Models.ColorFolder
{
    public class keepTrans
    {
        public string locID { get; set; }
        public string locDetail { get; set; }
        public string warehouseID { get; set; }
        public string compID { get; set; }
        public string token { get; set; }
        public List<KeepScanSNMap> folderList { get; set; }
    }
}