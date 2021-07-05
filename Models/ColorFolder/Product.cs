namespace PTC_back_end_webAPI.Models.ColorFolder
{
    public class Search
    {
        public string strSearch { get; set; }
        public string token { get; set; }
    }
    public class Product
    {
        public string PROD_ID { get; set; }
        public string REVISION { get; set; }
        public string PROD_DESC { get; set; }
    }
    public class ProductMap
    {
        public string prodID { get; set; }
        public string revision { get; set; }
        public string prodName { get; set; }
    }
}