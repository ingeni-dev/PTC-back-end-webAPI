using System.Collections.Generic;

namespace webAPI.Models.Elearning
{
    public class GetAllCoursesDB
    {
        public string DOC_TYPE { get; set; }
        public string DOC_ID { get; set; }
        public string DOC_REV { get; set; }
        public string COURSE_DESC { get; set; }
        public string REMARK { get; set; }
    }
    public class SetAllCoursesDB
    {
        public string docType { get; set; }
        public string docID { get; set; }
        public string docRev { get; set; }
        public string docDesc { get; set; }
        public string remark { get; set; }
    }
    public class SetAllCourses
    {
        public string docType { get; set; }
        public List<SetItemCourses> items { get; set; }
    }
    public class SetItemCourses
    {
        public string docID { get; set; }
        public string docRev { get; set; }
        public string docDesc { get; set; }
        public string remark { get; set; }
    }
    public class GetAllISOsDB
    {
        public string DOC_TYPE { get; set; }
        public string DOC_CODE { get; set; }
        public string DOC_REVISION { get; set; }
        public string DOC_NAME { get; set; }
        public string ISO_STD { get; set; }
    }
    public class SetAllISOsDB
    {
        public string docType { get; set; }
        public string docCode { get; set; }
        public string docRevision { get; set; }
        public string docName { get; set; }
        public string isoSTD { get; set; }
    }
    public class SetAllISOs
    {
        public string docType { get; set; }
        public List<SetItemISOs> items { get; set; }
    }
    public class SetItemISOs
    {
        public string docCode { get; set; }
        public string docRevision { get; set; }
        public string docName { get; set; }
        public string isoSTD { get; set; }
    }
}