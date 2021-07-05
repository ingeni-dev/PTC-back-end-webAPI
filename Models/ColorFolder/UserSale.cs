namespace PTC_back_end_webAPI.Models.ColorFolder
{
    public class UserSale
    {
        public string SALE_ID { get; set; }
        public string SALE_NAME { get; set; }
    }
    public class UserSaleMap
    {
        public string saleID { get; set; }
        public string saleName { get; set; }
    }

    public class WithdUserSale
    {
        public string token { get; set; }
        public string saleID { get; set; }
        public string saleOrg { get; set; }
        public string cfSEQ { get; set; }

    }
}