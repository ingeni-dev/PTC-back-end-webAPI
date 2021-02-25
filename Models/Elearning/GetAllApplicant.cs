namespace webAPI.Models.Elearning
{
    public class PostModelAllApplcant
    {
        public string queryID { get; set; }

    }
    public class GetAllApplicantMap
    {
        public string EMP_ID { get; set; }
        public string EMP_FNAME { get; set; }
        public string EMP_LNAME { get; set; }
        public string POS_ID { get; set; }
        public string POS_DESC { get; set; }
        public string ROLE_ID { get; set; }
        public string ROLE_DESC { get; set; }
        public string TRAINING_FLAG { get; set; }
        public string SELECTED_FLAG { get; set; }
        public bool SELECTED_FLAG_BOOL { get { if (SELECTED_FLAG != "T") { return false; } else { return true; } } set { } }
    }
    public class GetAllApplicant
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
}