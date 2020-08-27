using System.Collections.Generic;

namespace PTCwebApi.Models.PTCModels.MethodModels {
    public class CheckWareHouseUser {
        public List<WarehouseList> warehouseList { get; set; }
        public string flag { get; set; }
        public string text { get; set; }
    }
    public class WarehouseList{
        public string warehouseID {get; set;}
        public string warehouseDESC {get; set;}
    }
}