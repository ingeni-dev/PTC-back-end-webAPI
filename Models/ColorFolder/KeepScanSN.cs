using System.Collections.Generic;

namespace PTC_back_end_webAPI.Models.ColorFolder
{
    public class KeepScanSN
    {
        public string CF_SN { get; set; }
        public string CF_SEQ { get; set; }
        public string CF_STATUS { get; set; }
        public string PROD_DESC { get; set; }
        public string SN_NO { get; set; }
        public string AVAILABLE_FLAG { get; set; }
    }
    public class KeepScanSNMap
    {
        public string cfSN { get; set; }
        public string cfSEQ { get; set; }
        public string cfStatus { get; set; }
        public string prodDESC { get; set; }
        public string snNO { get; set; }
        public string availableFlag { get; set; }
    }
    public class ReturnKeepScanSN
    {
        public bool stateError { get; set; }
        public string messageError { get; set; }
        public List<KeepScanSNMap> lists { get; set; }
    }
}