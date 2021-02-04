using System.Collections.Generic;

namespace webAPI.Models.Elearning
{
    public class GetTopicDetail
    {
        public string TOPIC_ID { get; set; }
        public string TOPIC_NAME { get; set; }
        public string TOPIC_ORDER { get; set; }
        public string COURSE_REVISION { get; set; }
    }
    public class GetDocDetail
    {
        public string COURSE_DOC_ID { get; set; }
        public string DOC_ORDER { get; set; }
        public string DOC_NAME { get; set; }
        public string DOC_TYPE { get; set; }
        public string DOC_PATH { get; set; }
        public string VIDEO_COVER { get; set; }
        public string VIDEO_LENGTH { get; set; }
    }
    public class SetTopicDetail
    {
        public string topicID { get; set; }
        public string topicName { get; set; }
        public string topicOrder { get; set; }
        public string courseRevision { get; set; }
    }
    public class SetDocDetail
    {
        public string courseDocID { get; set; }
        public string docOrder { get; set; }
        public string docName { get; set; }
        public string docType { get; set; }
        public string docPath { get; set; }
        public string videoCover { get; set; }
        public string videoLength { get; set; }
    }
    public class RequestTopicDetail
    {
        public string courseID { get; set; }
    }
    public class ResponseTopicDetail
    {
        public string topicID { get; set; }
        public string topicName { get; set; }
        public string topicOrder { get; set; }
        public string courseRevision { get; set; }
        public List<SetDocDetail> items { get; set; }
    }
}