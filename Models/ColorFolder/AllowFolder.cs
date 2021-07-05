namespace PTC_back_end_webAPI.Models.ColorFolder
{
    public class AllowFolder
    {
        public string CF_SEQ { get; set; }
        public string PROD_ID { get; set; }
        public string PROD_DESC { get; set; }
        public string CR_QTY { get; set; }
        public string SALE_NAME { get; set; }
        public string WITHD_DATE { get; set; }
        public string DAYS { get; set; }
        public string COLLECTION_DATE { get; set; }
        public string CF_TYPE { get; set; }
        // public string CF_TYPE_STR
        // {
        //     get
        //     {
        //         switch (CF_TYPE)
        //         {
        //             case "N":
        //                 return "ปกติ/ใหม่";
        //             case "A":
        //                 return "แนบอาร์ตเวิร์ค";
        //             default:
        //                 return "";
        //         }
        //     }
        //     set { }
        // }
        public int STATE{ get; set; }

    }
    public class AllowFolderMap
    {
        public string cfSEQ { get; set; }
        public string prodID { get; set; }
        public string prodName { get; set; }
        public string crQTY { get; set; }
        public string saleName { get; set; }
        public string withdDate { get; set; }
        public string days { get; set; }
        public string collectionDate { get; set; }
        public string cfType { get; set; }
        public int state { get; set; }

    }
}