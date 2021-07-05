namespace PTC_back_end_webAPI.Models.ColorFolder
{
    public class GetJob
    {
        public string prodID { get; set; }

    }
    public class JobMap
    {
        public string jobID { get; set; }
        public string custID { get; set; }
        public string custName { get; set; }
    }
    public class Job
    {
        public string JOB_ID { get; set; }
        public string CUST_ID { get; set; }
        public string CUST_NAME { get; set; }

    }
}