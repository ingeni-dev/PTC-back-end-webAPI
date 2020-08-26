namespace PTCwebApi.Models.PTCModels.MethodModels {
    public class ReturnCurrentPlans {
        public string PTC_ID { get; set; }
        public string PTC_NAME { get; set; }
        public string MACH_ID { get; set; }
        public string LOC_ID { get; set; }
        public string LOC_NAME { get; set; }
        public string TYPE_PTC { get; set; }
        public string DAY { get; set; }
        public string MONTH { get; set; }
        public string TIME { get; set; }
        public string FLAG { get; set; }
        public string TEXT { get; set; }
    }
}