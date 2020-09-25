using System.Collections.Generic;

namespace PTCwebApi.Models.PTCModels.MethodModels.CurrentPlans
{
    public class ResponseCurrentPlans
    {
        public List<RequestCurrentPlansList> ptcList { get; set; }
        public string flag { get; set; }
        public string text { get; set; }
    }
    public class RequestCurrentPlansList
    {
        public string jobID { get; set; }
        public string stepID { get; set; }
        public string splitSeq { get; set; }
        public string planSubSeq { get; set; }
        public string seqRun { get; set; }
        public string wdeptID { get; set; }
        public string revision { get; set; }
        public string actDate { get; set; }
        public string machID { get; set; }
        public string compID { get; set; }
        public string period { get; set; }
        public string ptcType { get; set; }
        public string ptcID { get; set; }
        public string diecutSN { get; set; }


        public string withdDate { get; set; }
        public string withdUserID { get; set; }
        public string returnDate { get; set; }
        public string returnUserID { get; set; }
        public string ptcSN { get; set; }
        public string locID { get; set; }
        public string locName { get; set; }
        public string checkShow { get; set; }
        // private string checkshow = "T";
        // public string checkShow { get { return checkshow; } set { checkshow = "T"; } }
    }
}