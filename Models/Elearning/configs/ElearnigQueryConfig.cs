using System.IO;

namespace webAPI.Models.Elearning.configs
{
    public class ElearnigQueryConfig
    {
        public string Q_GET_COURE { get; set; }
        public string Q_GET_COURE_FORM { get; set; }
        public string Q_GET_DEPARTMENT { get; set; }
        public string Q_GET_APPLICANT { get; set; }
        public string Q_CHECK_APPLICANT_ID { get; set; }
        public string Q_GET_ALL_LECTURE { get; set; }
        public string Q_GET_QUERY_ID { get; set; }
        public string Q_ALL_COURSE { get; set; }
        public string Q_ALL_ISO { get; set; }
        public string I_SET_CHECK { get; set; }
        public string Q_GET_ALL_APPLICANT { get; set; }
        public string I_COURSE_QUERY { get; set; }
        public string I_COURSE_QUERY_TIME { get; set; }
        public string Q_CHECK_TIME_SEQ_BY_ID { get; set; }
        public string I_COURSE_LECTURER { get; set; }
        public string U_NEW_QUERY_ID_RUNNING { get; set; }
        public string Q_SORT_COURSE_DATE { get; set; }
        public string Q_APPLICANT_DETAIL { get; set; }
        public string I_COURSE_APPLICANT { get; set; }
        public string U_COURSE_MASTER { get; set; }
        public string U_COURSE_QUERY { get; set; }
        public string U_COURSE_QUERY_TIME { get; set; }
        public string U_COURSE_QUERY_DATE { get; set; }
        public string S_COURSE_TOPIC { get; set; }
        public string S_TOPIC_MASTER { get; set; }
        public string S_DOC_TOPIC { get; set; }
        public string S_COUNT_TOPIC_MASTER { get; set; }
        public string I_TOPIC_MASTER { get; set; }
        public string I_COURSE_TOPIC { get; set; }
        public string UG_TOPIC_MASTER { get; set; }
        public string UG_COURSE_TOPIC { get; set; }
        public string UT_TOPIC_MASTER { get; set; }
        public string S_COUNT_COURSE_DOC_MASTER { get; set; }
        public string I_COURSE_DOC_MASTER { get; set; }
        public string I_TOPIC_DOC { get; set; }
        public string U_COURSE_DOC_MASTER { get; set; }
        public string U_TOPIC_DOC { get; set; }
        private string PATH = @"\configs";
        // private string PATH = @"\Models\Elearning\configs";
        public ElearnigQueryConfig()
        {
            string curr_dir = Directory.GetCurrentDirectory() + PATH;
            Q_GET_COURE = System.IO.File.ReadAllText(@$"{curr_dir}\Q_GET_COURE.txt");
            Q_GET_COURE_FORM = System.IO.File.ReadAllText(@$"{curr_dir}\Q_GET_COURE_FORM.TXT");
            Q_GET_DEPARTMENT = System.IO.File.ReadAllText(@$"{curr_dir}\Q_GET_DEPARTMENT.TXT");
            Q_GET_APPLICANT = System.IO.File.ReadAllText(@$"{curr_dir}\Q_GET_APPLICANT.TXT");
            Q_CHECK_APPLICANT_ID = System.IO.File.ReadAllText(@$"{curr_dir}\Q_CHECK_APPLICANT_ID.TXT");
            Q_GET_ALL_LECTURE = System.IO.File.ReadAllText(@$"{curr_dir}\Q_GET_ALL_LECTURE.TXT");
            Q_GET_QUERY_ID = System.IO.File.ReadAllText(@$"{curr_dir}\Q_GET_QUERY_ID.TXT");
            Q_ALL_COURSE = System.IO.File.ReadAllText(@$"{curr_dir}\Q_ALL_COURSE.TXT");
            Q_ALL_ISO = System.IO.File.ReadAllText(@$"{curr_dir}\Q_ALL_ISO.TXT");
            I_SET_CHECK = System.IO.File.ReadAllText(@$"{curr_dir}\I_SET_CHECK.TXT");
            Q_GET_ALL_APPLICANT = System.IO.File.ReadAllText(@$"{curr_dir}\Q_GET_ALL_APPLICANT.TXT");
            I_COURSE_QUERY = System.IO.File.ReadAllText(@$"{curr_dir}\I_COURSE_QUERY.TXT");
            I_COURSE_QUERY_TIME = System.IO.File.ReadAllText(@$"{curr_dir}\I_COURSE_QUERY_TIME.TXT");
            Q_CHECK_TIME_SEQ_BY_ID = System.IO.File.ReadAllText(@$"{curr_dir}\Q_CHECK_TIME_SEQ_BY_ID.TXT");
            I_COURSE_LECTURER = System.IO.File.ReadAllText(@$"{curr_dir}\I_COURSE_LECTURER.TXT");
            U_NEW_QUERY_ID_RUNNING = System.IO.File.ReadAllText(@$"{curr_dir}\U_NEW_QUERY_ID_RUNNING.TXT");
            Q_SORT_COURSE_DATE = System.IO.File.ReadAllText(@$"{curr_dir}\Q_SORT_COURSE_DATE.TXT");
            Q_APPLICANT_DETAIL = System.IO.File.ReadAllText(@$"{curr_dir}\Q_APPLICANT_DETAIL.TXT");
            I_COURSE_APPLICANT = System.IO.File.ReadAllText(@$"{curr_dir}\I_COURSE_APPLICANT.TXT");
            U_COURSE_MASTER = System.IO.File.ReadAllText(@$"{curr_dir}\U_COURSE_MASTER.TXT");
            U_COURSE_QUERY = System.IO.File.ReadAllText(@$"{curr_dir}\U_COURSE_QUERY.TXT");
            U_COURSE_QUERY_TIME = System.IO.File.ReadAllText(@$"{curr_dir}\U_COURSE_QUERY_TIME.TXT");
            U_COURSE_QUERY_DATE = System.IO.File.ReadAllText(@$"{curr_dir}\U_COURSE_QUERY_DATE.TXT");
            S_COURSE_TOPIC = System.IO.File.ReadAllText(@$"{curr_dir}\create-topic\S_COURSE_TOPIC.sql");
            S_TOPIC_MASTER = System.IO.File.ReadAllText(@$"{curr_dir}\create-topic\S_TOPIC_MASTER.sql");
            S_DOC_TOPIC = System.IO.File.ReadAllText(@$"{curr_dir}\create-topic\S_DOC_TOPIC.sql");
            S_COUNT_TOPIC_MASTER = System.IO.File.ReadAllText(@$"{curr_dir}\create-topic\S_COUNT_TOPIC_MASTER.sql");
            I_TOPIC_MASTER = System.IO.File.ReadAllText(@$"{curr_dir}\create-topic\I_TOPIC_MASTER.sql");
            I_COURSE_TOPIC = System.IO.File.ReadAllText(@$"{curr_dir}\create-topic\I_COURSE_TOPIC.sql");
            UG_TOPIC_MASTER = System.IO.File.ReadAllText(@$"{curr_dir}\create-topic\UG_TOPIC_MASTER.sql");
            UG_COURSE_TOPIC = System.IO.File.ReadAllText(@$"{curr_dir}\create-topic\UG_COURSE_TOPIC.sql");
            UT_TOPIC_MASTER = System.IO.File.ReadAllText(@$"{curr_dir}\create-topic\UT_TOPIC_MASTER.sql");
            S_COUNT_COURSE_DOC_MASTER = System.IO.File.ReadAllText(@$"{curr_dir}\create-topic\S_COUNT_COURSE_DOC_MASTER.sql");
            I_COURSE_DOC_MASTER = System.IO.File.ReadAllText(@$"{curr_dir}\create-topic\I_COURSE_DOC_MASTER.sql");
            I_TOPIC_DOC = System.IO.File.ReadAllText(@$"{curr_dir}\create-topic\I_TOPIC_DOC.sql");
            U_COURSE_DOC_MASTER = System.IO.File.ReadAllText(@$"{curr_dir}\create-topic\U_COURSE_DOC_MASTER.sql");
            U_TOPIC_DOC = System.IO.File.ReadAllText(@$"{curr_dir}\create-topic\U_TOPIC_DOC.sql");
        }
    }
}