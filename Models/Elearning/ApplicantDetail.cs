using System.Collections.Generic;
using webAPI.Security;

namespace PTCwebApi.Models.Elearning
{
    public class ListApplicantResult
    {
        public string departmentID { get; set; }
        public string department { get; set; }
        public List<ApplicantDetailResult> listApplicant { get; set; }
    }
    public class ApplicantDetailResponse
    {
        public string DOC_TYPE { get; set; }
        public string QUERY_ID { get; set; }
        // public string QUERY_IDS { get { return new GenerateQrcode().EndoceBase64(QUERY_ID); } set { } }
        public int TIME_SEQ { get; set; }
        public string APP_EMP_ID { get; set; }
        public string EMP_NAME { get; set; }
        public string TRAINING_FLAG { get; set; }
        // public string UNIT_ID { get; set; }
        // public string UNIT_DESC { get; set; }
    }
    public class ApplicantDetailResult
    {
        public string docType { get; set; }
        public string queryID { get; set; }
        public int timeSeq { get; set; }
        public string appEmpID { get; set; }
        public string empName { get; set; }
        public string trainingFlag { get; set; }
        // public string unitID { get; set; }
        // public string unitDesc { get; set; }
    }
}
