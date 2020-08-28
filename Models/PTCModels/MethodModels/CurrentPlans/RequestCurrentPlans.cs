namespace PTCwebApi.Models.PTCModels.MethodModels.CurrentPlans {
    public class RequestCurrentPlans {
        public string JOB_ID { get; set; }
        public string STEP_ID { get; set; }
        public string SPLIT_SEQ { get; set; }
        public string PLAN_SUB_SEQ { get; set; }
        public string SEQ_RUN { get; set; }
        public string WDEPT_ID { get; set; }
        public string REVISION { get; set; }
        public string ACT_DATE { get; set; }
        public string MACH_ID { get; set; }
        public string COMP_ID { get; set; }
        public string PERIOD { get; set; }
        public string PTC_TYPE { get; set; }
        public string PTC_ID { get; set; }
        public string LOC_ID { get; set; }
        public string LOC_NAME { get; set; }
        public string DIECUT_SN { get; set; }
        // public string WITHD_DATE { get; set; }
        // public string WITHD_USER_ID { get; set; }
        // public string RETURN_DATE { get; set; }
        // public string RETURN_USER_ID { get; set; }
    }
}