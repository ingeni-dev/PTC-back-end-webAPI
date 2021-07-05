using PTC_back_end_webAPI.Methods.ColorFolder;
using webAPI.Security;

namespace PTC_back_end_webAPI.Models.ColorFolder
{
    public class Files
    {
        public string PROD_ID { get; set; }
        public string PROD_DESC { get; set; }
        public string QR_CODE { get { return new CFGenerateQrcode().generateQrcode(PROD_ID); } set { } }
        public int NUM { get { return 1; } set { } }
    }
    public class FilesMap
    {
        public string prodID { get; set; }
        public string prodDesc { get; set; }
        public string qrCode { get; set; }
        public int num { get; set; }
    }
    public class Folder
    {
        public string CUST_NAME { get; set; }
        public string PROD_DESC { get; set; }
        public string APP_DATE { get; set; }
        public string EXPIRE_DATE { get; set; }
        public string RUNNING_NO { get; set; }
        public string CF_SN { get; set; }
        public string APPROVAL { get; set; }
        public string QR_CODE { get { return new CFGenerateQrcode().generateQrcode(CF_SN); } set { } }
        public int NUM { get { return 1; } set { } }
    }
    public class FolderMap
    {
        public string custName { get; set; }
        public string prodDesc { get; set; }
        public string appDate { get; set; }
        public string expireDate { get; set; }
        public string runningNo { get; set; }
        public string cfSn { get; set; }
        public string approval { get; set; }
        public string qrCode { get; set; }
        public int num { get; set; }
    }
}