using System.Collections.Generic;

namespace webAPI.Models.Elearning
{
    public class RequestTopicDetail
    {
        public string courseID { get; set; }
        public string courseRevision { get; set; }
    }
    public class GetTopicGroup
    {
        public string COURSE_TYPE { get; set; }
        public string COURSE_ID { get; set; }
        public string COURSE_REVSION { get; set; }
        public string GROUP_ID { get; set; }
        public string GROUP_ORDER { get; set; }
        public string GROUP_NAME { get; set; }

    }
    public class SetTopicGroup
    {
        public string courseType { get; set; }
        public string courseID { get; set; }
        public string courseRevision { get; set; }
        public string groupID { get; set; }
        public string groupOrder { get; set; }
        public string groupName { get; set; }

    }
    public class GetTopic
    {
        public string TOPIC_ID { get; set; }
        public string TOPIC_ORDER { get; set; }
        public string TOPIC_NAME { get; set; }
        public string GROUP_ID { get; set; }
    }
    public class SetTopic
    {
        public string topicID { get; set; }
        public string topicOrder { get; set; }
        public string topicName { get; set; }
        public string groupID { get; set; }
    }
    public class GetDoc
    {
        public string TOPIC_ID { get; set; }
        public string COURSE_DOC_ID { get; set; }
        public string DOC_ORDER { get; set; }
        public string DOC_TYPE { get; set; }
        public string DOC_NAME { get; set; }
        public string DOC_PATH { get; set; }
        public string VIDEO_COVER { get; set; }
        public string VIDEO_LENGTH { get; set; }
    }

    public class SetDoc
    {
        public bool stateDelete { get => false; set { } }
        public string topicID { get; set; }
        public string docID { get; set; }
        public string docOrder { get; set; }
        public string docType { get; set; }
        public string docName { get; set; }
        public string docPath { get; set; }
        public string videoCover { get; set; }
        public string videoLength { get; set; }
    }

    public class TopicDetail
    {
        public bool stateDelete { get => false; set { } }
        public string topicID { get; set; }
        public string topicOrder { get; set; }
        public string topicName { get; set; }
        public string groupID { get; set; }
        public List<SetDoc> docs { get; set; }
    }
    public class TopicGroupDetail
    {
        public bool stateDelete { get => false; set { } }
        public string courseType { get; set; }
        public string courseID { get; set; }
        public string courseRevision { get; set; }
        public string groupID { get; set; }
        public string groupOrder { get; set; }
        public string groupName { get; set; }
        public List<TopicDetail> topics { get; set; }
    }
}