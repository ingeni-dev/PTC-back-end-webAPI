using System.Collections.Generic;

namespace webAPI.Models.Elearning
{
    public class OnlineGetOrderModel
    {
        public string token { get; set; }
    }
    public class ReturnOnlineGetOrderLatest
    {
        public bool stateError { get; set; }
        public string message { get; set; }
        public List<SetALLOnlineCourseLatest> returns { get; set; }
    }
    public class GetALLOnlineCourseLatest
    {
        public string APP_USER_ID { get; set; }
        public string QUERY_ID { get; set; }
        public string COURSE_DESC { get; set; }
        public string BEGIN_DATE { get; set; }
        public string END_DATE { get; set; }
        public string COURSE_DOC_ID { get; set; }
        public string COURSE_ID { get; set; }
        public string COURSE_REVISION { get; set; }
        public string TOPIC_ID { get; set; }
        public string CANCEL_FLAG { get; set; }
        public string C_TOPIC_ID { get; set; }
        public int C_TOPIC_ORDER { get; set; }
        public string C_TOPIC_NAME { get; set; }
        public string C_PARENT_TOPIC_ID { get; set; }
        public string T_TOPIC_ID { get; set; }
        public int T_TOPIC_ORDER { get; set; }
        public string T_TOPIC_NAME { get; set; }
        public string T_PARENT_TOPIC_ID { get; set; }
        public string DOC_TYPE { get; set; }
        public string DOC_NAME { get; set; }
        public string DOC_PATH { get; set; }
        public string VIDEO_COVER { get; set; }
        public int VIDEO_LENGTH { get; set; }
        public int COUNT { get; set; }
        public int CURR_TIME { get; set; }
        public string LAST_VISIT { get; set; }
    }
    public class SetALLOnlineCourseLatest
    {
        public string appUserID { get; set; }
        public string queryID { get; set; }
        public string courseDESC { get; set; }
        public string beginDate { get; set; }
        public string endDate { get; set; }
        public string courseDocID { get; set; }
        public string courseID { get; set; }
        public string courseRevision { get; set; }
        public string topicID { get; set; }
        public string cancelFlag { get; set; }
        public string cTopicID { get; set; }
        public int cTopicOrder { get; set; }
        public string cTopicName { get; set; }
        public string cParentTopicID { get; set; }
        public string tTopicID { get; set; }
        public int tTopicOrder { get; set; }
        public string tTopicName { get; set; }
        public string tParentTopicID { get; set; }
        public string docType { get; set; }
        public string docName { get; set; }
        public string docPath { get; set; }
        public string videoCover { get; set; }
        public int videoLength { get; set; }
        public int count { get; set; }
        public int currTime { get; set; }
        public string lastVisit { get; set; }
    }

    public class ReturnGetALLOnlineCourse
    {
        public bool stateError { get; set; }
        public string message { get; set; }
        public List<SetALLOnlineCourse> returns { get; set; }
    }
    public class GetALLOnlineCourse
    {
        public string COURSE_ID { get; set; }
        public string COURSE_REVISION { get; set; }
        public string COURSE_DESC { get; set; }
        public string BEGIN_DATE { get; set; }
        public string END_DATE { get; set; }
        public int FULL_TIME_COURSE { get; set; }
        public int PERCENT_VIEW_COURSE { get; set; }

    }
    public class SetALLOnlineCourse
    {
        public string courseID { get; set; }
        public string courseRevision { get; set; }
        public string courseDESC { get; set; }
        public string beginDate { get; set; }
        public string endDate { get; set; }
        public int fullTimeCourse { get; set; }
        public int precentViewCourse { get; set; }

    }
}