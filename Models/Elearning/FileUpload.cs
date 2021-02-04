using Microsoft.AspNetCore.Http;

namespace webAPI.Models.Elearning
{
    public class UpLoadTopic
    {
        public string queryID { get; set; }
        public string topicID { get; set; }
        public string topicName { get; set; }
        public string topicOrder { get; set; }
        public string revision { get; set; }

    }
    public class ReturnTopicID
    {
        public string topicID { get; set; }
    }
    public class UpLoadDoc
    {
        public string queryID { get; set; }
        public string docOrder { get; set; }
        public string topicID { get; set; }
        public string courseDocID { get; set; }
        public string docName { get; set; }
        public string docType { get; set; }
        public string docPath { get; set; }
        public string videoCover { get; set; }
        public string videoLength { get; set; }
        public string revision { get; set; }
        public IFormFile fileVideo { get; set; }
        public IFormFile fileImg { get; set; }
        public IFormFile fileDoc { get; set; }
    }
}