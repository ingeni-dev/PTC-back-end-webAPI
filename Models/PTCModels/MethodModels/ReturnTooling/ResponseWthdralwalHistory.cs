using System.Collections.Generic;

namespace PTCwebApi.Models.PTCModels.MethodModels.ReturnTooling {
    public class ResponseWthdralwalHistory {
        public List<ResponseWthdralwalHistoryList> historyList { get; set; }
        public string flag { get; set; }
        public string text { get; set; }
    }
    public class ResponseWthdralwalHistoryList {
        public string ptcID { get; set; }
        public string ptcType { get; set; }
        public string diecutSN { get; set; }
        public string machID { get; set; }
        public string locID { get; set; }
        public string locName { get; set; }
        public string time { get; set; }
        public string jobID { get; set; }
    }
    public class RequestWthdralwalHistoryList {
        public string JOB_ID { get; set; }
        public string PTC_ID { get; set; }
        public string PTC_TYPE { get; set; }
        public string DIECUT_SN { get; set; }
        public string MACH_ID { get; set; }
        public string LOC_ID { get; set; }
        public string LOC_NAME { get; set; }
        public string TIME { get; set; }

    }
}