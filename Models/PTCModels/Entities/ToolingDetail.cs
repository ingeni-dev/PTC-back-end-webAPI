using System;
using Dapper.Contrib.Extensions;

namespace PTCwebApi.Models.PTCModels.Entities {
    [Table ("KPDBA.DIECUT_SN")]
    public class ToolingDetail {
        [Key]
        public string DIECUT_SN { get; set; }
        public string DIECUT_ID { get; set; }
        public string DIECUT_TYPE { get; set; }
        public decimal DIECUT_AGE { get; set; }
        public string POKAYOKE_SIGN { get; set; }
        public string CUR_LOC_ID { get; set; }
        public string STATUS { get; set; }
        public DateTime LAST_MODIFY { get; set; }
        public DateTime DUE_DATE { get; set; }
    }
}