namespace webAPI.Models.Elearning
{
    public class GetCourseQueryDoc
    {
        public string QUERY_DOC_ID { get; set; }
        public string COUNT { get; set; }
        public string CURR_TIME { get; set; }
        public string FINISH_FLAG { get; set; }
    }
    public class SetCourseQueryDoc
    {
        public string queryDocID { get; set; }
        public string count { get; set; }
        public string currTime { get; set; }
        public string finishFlag { get; set; }
    }
}