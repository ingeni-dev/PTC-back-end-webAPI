using System.Collections.Generic;

namespace webAPI.Models.Elearning
{
    public partial class LecturerForms
    {
        public SetLecturerForm setLecturer { get; set; }
        public string token { get; set; }
    }
    public partial class SetLecturerForm
    {
        public string queryID { get; set; }
        public string queryBegin { get; set; }
        public string queryEnd { get; set; }
        public string courseID { get; set; }
        public string courseDESC { get; set; }
        public decimal applicantCount { get; set; }
        public string day { get; set; }
        public string dayHour { get; set; }
        public string dayMin { get; set; }
        public string docType { get; set; }
        public string instantFlag { get; set; }
        public List<LectList> lectList { get; set; }
        public string lectName { get; set; }
        public string place { get; set; }
        public string remark { get; set; }
        public string timeSeq { get; set; }

        public List<ListDate> listDate { get; set; }
        public List<FormListApplicant> listApplicant { get; set; }
    }
    public partial class ListDate
    {
        public string day { get; set; }
        public string startTime { get; set; }
        public string endTime { get; set; }
        public string dayHr { get; set; }
        public string dayMin { get; set; }
    }

    public partial class LectList
    {
        public string type { get; set; }
        public string lectID { get; set; }
        public string thaiTitle { get; set; }
        public string thaiFname { get; set; }
        public string thaiSname { get; set; }
        public string refEmpID { get; set; }
    }

    public partial class FormListApplicant
    {
        public string empID { get; set; }
        public string empFname { get; set; }
        public string empLname { get; set; }
        public string posID { get; set; }
        public string posDESC { get; set; }
        public string roleID { get; set; }
        public string roleDESC { get; set; }
        public string trainingFlag { get; set; }
        public bool selectedFlag { get; set; }
    }

    public partial class StateLectError
    {
        public bool stateError { get; set; }
        public string messageError { get; set; }
    }

    public partial class SetFormStoreLecture
    {
        public string RESULT { get; set; }
        public string COURSE_ID { get; set; }
        public string LECT_ID { get; set; }
        public string ERR_TEXT { get; set; }

    }

}