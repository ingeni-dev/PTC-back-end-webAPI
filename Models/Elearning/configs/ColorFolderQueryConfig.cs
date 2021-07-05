using System.IO;
using System.Text;

namespace PTC_back_end_webAPI.Models.Elearning.configs
{
    public class ColorFolderQueryConfig
    {
        public string S_COLOR_FOLDER { get; set; }
        public string S_PRODUCT { get; set; }
        public string I_COLOR_FOLDER { get; set; }
        public string C_COLOR_FOLDER { get; set; }
        public string S_USER_SALE { get; set; }
        public string U_REVICE_FOLDER { get; set; }
        public string U_SEND_FOLDER { get; set; }
        public string I_COLOR_FOLDER_SN { get; set; }
        public string C_COLOR_FOLDER_SN { get; set; }
        public string S_REJECT_MASTER { get; set; }
        public string S_COLOR_FOLDER_DETAIL { get; set; }
        public string S_PRINT_FILE { get; set; }
        public string S_PRINT_FOLDER { get; set; }
        public string S_A_PRINT_FOLDER { get; set; }
        public string S_PRINT_ALL_FOLDER { get; set; }
        public string S_JOB_ID { get; set; }
        public string S_GET_LOC_DETAIL { get; set; }
        public string S_GET_COLOR_FOLDER_DETAIL { get; set; }
        public string C_QTY { get; set; }
        public string S_TRAN_DATE { get; set; }
        public string I_CF_STOCK_DETAIL { get; set; }
        public string U_STATUS_COLOR_SN { get; set; }
        public string CHECK_FOLDER_SN_DETAIL { get; set; }
        public string C_TRAN_ID { get; set; }
        public string S_EMP_DETAIL { get; set; }
        public string U_COLOR_SN_GOOD { get; set; }
        public string U_COLOR_SN_CNCL { get; set; }
        public string S_CUST_DETAIL { get; set; }
        private string PATH = @"\configs";
        public ColorFolderQueryConfig()
        {
            string curr_dir = Directory.GetCurrentDirectory() + PATH;

            S_PRODUCT = System.IO.File.ReadAllText(@$"{curr_dir}\color-folder\S_PRODUCT.sql");
            I_COLOR_FOLDER = System.IO.File.ReadAllText(@$"{curr_dir}\color-folder\I_COLOR_FOLDER.sql");
            S_COLOR_FOLDER = System.IO.File.ReadAllText(@$"{curr_dir}\color-folder\S_COLOR_FOLDER.sql");
            C_COLOR_FOLDER = System.IO.File.ReadAllText(@$"{curr_dir}\color-folder\C_COLOR_FOLDER.sql");
            S_USER_SALE = System.IO.File.ReadAllText(@$"{curr_dir}\color-folder\S_USER_SALE.sql");
            U_REVICE_FOLDER = System.IO.File.ReadAllText(@$"{curr_dir}\color-folder\U_REVICE_FOLDER.sql");
            U_SEND_FOLDER = System.IO.File.ReadAllText(@$"{curr_dir}\color-folder\U_SEND_FOLDER.sql");
            I_COLOR_FOLDER_SN = System.IO.File.ReadAllText(@$"{curr_dir}\color-folder\I_COLOR_FOLDER_SN.sql");
            C_COLOR_FOLDER_SN = System.IO.File.ReadAllText(@$"{curr_dir}\color-folder\C_COLOR_FOLDER_SN.sql");
            S_REJECT_MASTER = System.IO.File.ReadAllText(@$"{curr_dir}\color-folder\S_REJECT_MASTER.sql");
            S_COLOR_FOLDER_DETAIL = System.IO.File.ReadAllText(@$"{curr_dir}\color-folder\S_COLOR_FOLDER_DETAIL.sql", Encoding.UTF8);
            S_PRINT_FILE = System.IO.File.ReadAllText(@$"{curr_dir}\color-folder\S_PRINT_FILE.sql");
            S_PRINT_FOLDER = System.IO.File.ReadAllText(@$"{curr_dir}\color-folder\S_PRINT_FOLDER.sql", Encoding.UTF8);
            S_A_PRINT_FOLDER = System.IO.File.ReadAllText(@$"{curr_dir}\color-folder\S_A_PRINT_FOLDER.sql", Encoding.UTF8);
            S_PRINT_ALL_FOLDER = System.IO.File.ReadAllText(@$"{curr_dir}\color-folder\S_PRINT_ALL_FOLDER.sql", Encoding.UTF8);
            S_JOB_ID = System.IO.File.ReadAllText(@$"{curr_dir}\color-folder\S_JOB_ID.sql", Encoding.UTF8);
            S_GET_LOC_DETAIL = System.IO.File.ReadAllText(@$"{curr_dir}\color-folder\S_GET_LOC_DETAIL.sql", Encoding.UTF8);
            S_GET_COLOR_FOLDER_DETAIL = System.IO.File.ReadAllText(@$"{curr_dir}\color-folder\S_GET_COLOR_FOLDER_DETAIL.sql", Encoding.UTF8);
            C_QTY = System.IO.File.ReadAllText(@$"{curr_dir}\color-folder\C_QTY.sql", Encoding.UTF8);
            S_TRAN_DATE = System.IO.File.ReadAllText(@$"{curr_dir}\color-folder\S_TRAN_DATE.sql", Encoding.UTF8);
            I_CF_STOCK_DETAIL = System.IO.File.ReadAllText(@$"{curr_dir}\color-folder\I_CF_STOCK_DETAIL.sql", Encoding.UTF8);
            U_STATUS_COLOR_SN = System.IO.File.ReadAllText(@$"{curr_dir}\color-folder\U_STATUS_COLOR_SN.sql", Encoding.UTF8);
            CHECK_FOLDER_SN_DETAIL = System.IO.File.ReadAllText(@$"{curr_dir}\color-folder\CHECK_FOLDER_SN_DETAIL.sql", Encoding.UTF8);
            C_TRAN_ID = System.IO.File.ReadAllText(@$"{curr_dir}\color-folder\C_TRAN_ID.sql", Encoding.UTF8);
            S_EMP_DETAIL = System.IO.File.ReadAllText(@$"{curr_dir}\color-folder\S_EMP_DETAIL.sql", Encoding.UTF8);
            U_COLOR_SN_GOOD = System.IO.File.ReadAllText(@$"{curr_dir}\color-folder\U_COLOR_SN_GOOD.sql", Encoding.UTF8);
            U_COLOR_SN_CNCL = System.IO.File.ReadAllText(@$"{curr_dir}\color-folder\U_COLOR_SN_CNCL.sql", Encoding.UTF8);
            S_CUST_DETAIL = System.IO.File.ReadAllText(@$"{curr_dir}\color-folder\S_CUST_DETAIL.sql", Encoding.UTF8);

        }
    }
}