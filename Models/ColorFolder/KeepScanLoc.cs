using System.Collections.Generic;

namespace PTC_back_end_webAPI.Models.ColorFolder
{
    public class KeepScanLoc
    {
        public string LOC_ID { get; set; }
        public string LOC_DETAIL { get; set; }
        public string WAREHOUSE_ID { get; set; }
        public string COMP_ID { get; set; }

    }
    public class KeepScanLocMap
    {
        public string locID { get; set; }
        public string locDetail { get; set; }
        public string warehouseID { get; set; }
        public string compID { get; set; }
    }
    public class ReturnKeepScanLoc
    {
        public bool stateError { get; set; }
        public string messageError { get; set; }
        public List<KeepScanLocMap> lists { get; set; }
    }

}