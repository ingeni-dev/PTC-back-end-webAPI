using System;

namespace PTCwebApi.Models.PTCModels {
    public class ModelTablePTCDetail {
        public string TRAN_ID { get; set; }
        public decimal TRAN_SEQ { get; set; }
        public decimal TRAN_TYPE { get; set; }
        public DateTime TRAN_DATE { get; set; }
        public string PTC_ID { get; set; }
        public string LOT_ID { get; set; }
        public decimal QTY { get; set; }
        public string COMP_ID { get; set; }
        public string WAREHOUSE_ID { get; set; }
        public string LOC_ID { get; set; }
        public string STATUS { get; set; }
        public string PO_ID { get; set; }
        public decimal PO_SEQ { get; set; }
        public string SUPLOT_ID { get; set; }
        public string REMARK { get; set; }
        public DateTime CR_DATE { get; set; }
        public string CR_ORG_ID { get; set; }
        public string CR_USER_ID { get; set; }
        public DateTime CANCEL_DATE { get; set; }
        public string CANCEL_ORG_ID { get; set; }
        public string CANCEL_USER_ID { get; set; }
    }
}