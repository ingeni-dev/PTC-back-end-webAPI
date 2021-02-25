using System;
using System.Collections.Generic;
using webAPI.Security;

namespace webAPI.Models.Elearning
{
    public class ResponseCourseDetails
    {
        public string DOC_TYPE { get; set; }
        public string COURSE_ID { get; set; }
        public string COURSE_DESC { get; set; }
        public string QUERY_ID { get; set; }
        // public string QUERY_IDS { get { return new GenerateQrcode().EndoceBase64(QUERY_ID); } set { } }
        public string PLACE { get; set; }
        public string TIME_SEQ { get; set; }
        public string LECT_NAME { get; set; }
        public int APPLICANT_COUNT { get; set; }
        public string QUERY_BEGIN { get; set; }
        public string QUERY_END { get; set; }
        public string DAY_HOUR { get; set; }
        public string DAY_MIN { get; set; }
        public string INSTANT_FLAG { get; set; }
        public string REMARK { get; set; }
    }
    public class ResultCourseDetails
    {
        public string day { get; set; }
        public string docType { get; set; }
        public string courseID { get; set; }
        public string courseDESC { get; set; }
        public string queryID { get; set; }
        public string place { get; set; }
        public string timeSeq { get; set; }
        public string lectName { get; set; }
        public int applicantCount { get; set; }
        public string queryBegin { get; set; }
        public string queryEnd { get; set; }
        public string dayHour { get; set; }
        public string dayMin { get; set; }
        public string instantFlag { get; set; }
        public string remark { get; set; }


    }
    public class GroupCourseDetails
    {
        public string day { get; set; }
        public List<ResultCourseDetails> items { get; set; }

    }
}