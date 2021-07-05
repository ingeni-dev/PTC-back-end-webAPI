using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using AutoMapper;
using HeyRed.Mime;
using Microsoft.AspNetCore.Mvc;
using PTC_back_end_webAPI.Methods.ColorFolder;
using PTC_back_end_webAPI.Models.ColorFolder;
using PTC_back_end_webAPI.Models.Elearning.configs;
using PTCwebApi;
using PTCwebApi.Interfaces;
using PTCwebApi.Models.ProfilesModels;
using webAPI.Models.Elearning;
using webAPI.Security;
//using PTC-back-end-webAPI.Models;

namespace WebApi.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class FolderController : ControllerBase
    {
        private readonly IJwtGenerator _jwtGenerator;
        private readonly IMapper _mapper;
        public FolderController(IMapper mapper, IJwtGenerator jwtGenerator)
        {
            _jwtGenerator = jwtGenerator;
            _mapper = mapper;
        }
        [Authorize]
        [HttpPost("getFolderList")]
        public async Task<ActionResult> GetFolderListModel(Search model)
        {
            string query = new ColorFolderQueryConfig().S_COLOR_FOLDER;
            var response = await new DataContext().GetResultDapperAsyncDynamic(DataBaseHostEnum.KPR, query);
            var result = _mapper.Map<IEnumerable<AllowFolder>>(response);
            var results = result as List<AllowFolder>;
            var resultReal = _mapper.Map<List<AllowFolder>, List<AllowFolderMap>>(results);
            return Ok(resultReal);
        }

        [HttpPost("getProduct")]
        public async Task<ActionResult> GetProductModel(Search model)
        {
            string q = new ColorFolderQueryConfig().S_PRODUCT;
            string query = q.Replace(":AS_TXT_SEARCH", $"'{model.strSearch}'");
            var response = await new DataContext().GetResultDapperAsyncDynamic(DataBaseHostEnum.KPR, query);
            var result = _mapper.Map<IEnumerable<Product>>(response);
            var results = result as List<Product>;
            var resultReal = _mapper.Map<List<Product>, List<ProductMap>>(results);
            return Ok(resultReal);
        }

        [HttpPost("insertColorFolder")]
        public async Task<ActionResult> InsertColorFolderModel(InsertColorFolder model)
        {
            List<string> insertQuery = new List<string>();
            StateError re = new StateError();
            var idSplit = model.id.Split("-");
            var id = idSplit[0].PadLeft(12, '0');
            var rev = idSplit[1];

            UserProfile userProfile = _jwtGenerator.DecodeToken(model.token);
            string org = userProfile.org;
            string userID = userProfile.userID;
            // TODO: Your code here
            string subID = id.Substring(6) + rev;
            string CF_SEQ = new ColorFolderQueryConfig().C_COLOR_FOLDER;
            string CF_SEQn = CF_SEQ.Replace(":AS_SUB_ID", $"'{subID}'");
            var responseCF_SEQ = await new DataContext().GetResultDapperAsyncDynamic(DataBaseHostEnum.KPR, CF_SEQn);
            decimal SEQ = ((responseCF_SEQ as List<dynamic>)[0].COUN + 1);
            string cfSEQ = "CF" + subID + SEQ.ToString("000");
            string q = new ColorFolderQueryConfig().I_COLOR_FOLDER;
            string query = q.Replace(":AS_CF_SEQ", $"'{cfSEQ}'")
                .Replace(":AS_PROD_ID", $"'{id}'")
                .Replace(":AS_CUST_ID", $"'{model.custID}'")
                .Replace(":AS_REVISION", $"'{rev}'")
                .Replace(":AS_CR_QTY", $"'{model.num}'")
                .Replace(":AS_COLLECTION_DATE", $"'{model.date}'")
                .Replace(":AS_CF_TYPE", $"'{model.type}'")
                .Replace(":AS_JOB_ID", $"'{model.jobID}'")
                .Replace(":AS_CR_ORG_ID", $"'{org}'")
                .Replace(":AS_CR_USER_ID", $"'{userID}'");
            insertQuery.Add(query);
            string subIDSN = idSplit[0] + rev;
            string qCCFSN = new ColorFolderQueryConfig().C_COLOR_FOLDER_SN;
            string qCCFSNn = qCCFSN.Replace(":AS_CF_SN", $"'{subIDSN}'");
            var responseCCFSN = await new DataContext().GetResultDapperAsyncDynamic(DataBaseHostEnum.KPR, qCCFSNn);
            decimal coun = (responseCCFSN as List<dynamic>)[0].COUN;
            for (int i = 1; i < model.num + 1; i++)
            {
                string cfSN = "CS" + subIDSN + (coun + i).ToString("000");
                string qICFSN = new ColorFolderQueryConfig().I_COLOR_FOLDER_SN;
                string queryICFSN = qICFSN.Replace(":AS_CF_SN", $"'{cfSN}'")
                    .Replace(":AS_CF_SEQ", $"'{cfSEQ}'")
                    // .Replace(":AS_CF_NO", $"'{i}'")
                    .Replace(":AS_CR_ORG_ID", $"'{org}'")
                    .Replace(":AS_CR_USER_ID", $"'{userID}'");
                insertQuery.Add(queryICFSN);
            }
            var response = await new DataContext().ExecuteDapperMultiAsync(DataBaseHostEnum.KPR, insertQuery);

            re.stateError = false;
            re.message = "";
            return Ok(re);
        }

        [HttpPost("getUserSale")]
        public async Task<ActionResult> GetUserSaleModel(Search model)
        {
            string query = new ColorFolderQueryConfig().S_USER_SALE;
            var response = await new DataContext().GetResultDapperAsyncDynamic(DataBaseHostEnum.KPR, query);
            var result = _mapper.Map<IEnumerable<UserSale>>(response);
            var results = result as List<UserSale>;
            var resultReal = _mapper.Map<List<UserSale>, List<UserSaleMap>>(results);
            return Ok(resultReal);
        }

        [HttpPost("setUserSale")]
        public async Task<ActionResult> SetUserSaleModel(WithdUserSale model)
        {
            StateError re = new StateError();
            UserProfile userProfile = _jwtGenerator.DecodeToken(model.token);
            string org = userProfile.org;
            string userID = userProfile.userID;
            string q = new ColorFolderQueryConfig().U_REVICE_FOLDER;
            string query = q.Replace(":AS_WITHD_ORG_ID", $"'{org}'")
            .Replace(":AS_WITHD_USER_ID", $"'{model.saleID}'")
            .Replace(":AS_CF_SEQ", $"'{model.cfSEQ}'")
            .Replace(":AS_UP_ORG_ID", $"'{org}'")
            .Replace(":AS_UP_USER_ID", $"'{userID}'");
            var response = await new DataContext().GetResultDapperAsyncDynamic(DataBaseHostEnum.KPR, query);
            re.stateError = false;
            re.message = "";
            return Ok(re);
        }

        [HttpPost("setSendAllow")]
        public async Task<ActionResult> SetSendAllowModel(SetSendAllow model)
        {
            List<string> insertStr = new List<string>();
            StateError re = new StateError();
            UserProfile userProfile = _jwtGenerator.DecodeToken(model.token);
            string org = userProfile.org;
            string userID = userProfile.userID;
            string q = new ColorFolderQueryConfig().U_SEND_FOLDER;
            DateTime oDate = DateTime.Parse(model.returnDate);
            string date = oDate.ToString("dd/MM/yyyy");

            if (model.appvFlag == "A")
            {
                string qUCSNG = new ColorFolderQueryConfig().U_COLOR_SN_GOOD;
                string qUCSNGn = qUCSNG.Replace(":AS_CF_SEQ", $"'{model.cfSeq}'");
                insertStr.Add(qUCSNGn);
            }
            else
            {
                string qUCSNC = new ColorFolderQueryConfig().U_COLOR_SN_CNCL;
                string qUCSNCn = qUCSNC.Replace(":AS_CF_SEQ", $"'{model.cfSeq}'");
                insertStr.Add(qUCSNCn);
            }
            string query = q.Replace(":AS_APPV_FLAG", $"'{model.appvFlag}'")
                .Replace(":AS_CF_REJ_REASON", $"'{model.cfRejReason}'")
                .Replace(":AS_REMARK", $"'{model.remark}'")
                .Replace(":AS_SUBMIT_QTY", $"'{model.submitQTY}'")
                .Replace(":AS_EXPIRE_DATE", $"'{date}'")
                .Replace(":AS_RETURN_ORG_ID", $"'{model.saleOrg}'")
                .Replace(":AS_RETURN_USER_ID", $"'{model.saleID}'")
                .Replace(":AS_UP_ORG_ID", $"'{org}'")
                .Replace(":AS_UP_USER_ID", $"'{userID}'")
                .Replace(":AS_CF_SEQ", $"'{model.cfSeq}'");
            insertStr.Add(query);

            var response = await new DataContext().ExecuteDapperMultiAsync(DataBaseHostEnum.KPR, insertStr);
            re.stateError = false;
            re.message = "";

            // for (int i = 1; i < model.submitQTY + 1; i++)
            // {
            //     var idSplit = model.prodID.Split("-");
            //     var id = idSplit[0].PadLeft(12, '0');
            //     var rev = idSplit[1];
            //     string subID = id.Substring(6) + rev;

            //     string qCCFSN = new ColorFolderQueryConfig().C_COLOR_FOLDER_SN;
            //     string qCCFSNn = qCCFSN.Replace(":AS_CF_SN", $"'{subID}'");
            //     var responseCCFSN = await new DataContext().GetResultDapperAsyncDynamic(DataBaseHostEnum.KPR, qCCFSNn);
            //     decimal coun = (responseCCFSN as List<dynamic>)[0].COUN;
            //     // string dateSN = DateTime.Now.ToString("yyMM");
            //     // string cfSEQ = dateSN + coun.ToString("000000");
            //     string cfSEQ = "CS" + subID + coun.ToString("000");

            //     string qICFSN = new ColorFolderQueryConfig().I_COLOR_FOLDER_SN;
            //     string queryICFSN = qICFSN.Replace(":AS_CF_SN", $"'{cfSEQ}'")
            //     .Replace(":AS_CF_SEQ", $"'{model.cfSeq}'")
            //     .Replace(":AS_CF_NO", $"'{i}'")
            //     .Replace(":AS_CR_ORG_ID", $"'{org}'")
            //     .Replace(":AS_CR_USER_ID", $"'{userID}'");
            //     var responseICFSN = await new DataContext().GetResultDapperAsyncDynamic(DataBaseHostEnum.KPR, queryICFSN);
            // }
            return Ok(re);
        }

        [HttpPost("getRejectMessage")]
        public async Task<ActionResult> GetRejectMessageModel(Search model)
        {
            string q = new ColorFolderQueryConfig().S_REJECT_MASTER;
            string query = q.Replace(":AS_TXT_SEARCH", $"'{model.strSearch}'");
            var response = await new DataContext().GetResultDapperAsyncDynamic(DataBaseHostEnum.KPR, query);
            var result = _mapper.Map<IEnumerable<RejectMessage>>(response);
            var results = result as List<RejectMessage>;
            var resultReal = _mapper.Map<List<RejectMessage>, List<RejectMessageMap>>(results);
            return Ok(resultReal);
        }

        [HttpPost("getColorFolderDetail")]
        public async Task<ActionResult> GetColorFolderDetailModel(Search model)
        {
            string q = new ColorFolderQueryConfig().S_COLOR_FOLDER_DETAIL;
            string query = q.Replace(":AS_TXT_SEARCH", $"'{model.strSearch}'");
            var response = await new DataContext().GetResultDapperAsyncDynamic(DataBaseHostEnum.KPR, query);
            var result = _mapper.Map<IEnumerable<ColorFolderDetail>>(response);
            var results = result as List<ColorFolderDetail>;
            var resultReal = _mapper.Map<List<ColorFolderDetail>, List<ColorFolderDetailMap>>(results);
            return Ok(resultReal);
        }

        [HttpPost("getPrintFile")]
        public async Task<ActionResult> GetPrintFileModel(Search model)
        {
            string q = new ColorFolderQueryConfig().S_PRINT_FILE;
            string query = q.Replace(":AS_TXT_SEARCH", $"'{model.strSearch}'");
            var response = await new DataContext().GetResultDapperAsyncDynamic(DataBaseHostEnum.KPR, query);
            var result = _mapper.Map<IEnumerable<Files>>(response);
            var results = result as List<Files>;
            var resultReal = _mapper.Map<List<Files>, List<FilesMap>>(results);
            return Ok(resultReal);
        }

        [HttpPost("getPrintFolder")]
        public async Task<ActionResult> GetPrintFolderModel(Search model)
        {
            string q = new ColorFolderQueryConfig().S_PRINT_FOLDER;
            string query = q.Replace(":AS_TXT_SEARCH", $"'{model.strSearch}'");
            var response = await new DataContext().GetResultDapperAsyncDynamic(DataBaseHostEnum.KPR, query);
            var result = _mapper.Map<IEnumerable<Folder>>(response);
            var results = result as List<Folder>;
            var resultReal = _mapper.Map<List<Folder>, List<FolderMap>>(results);
            return Ok(resultReal);
        }

        [HttpPost("getOnePrintFolder")]
        public async Task<ActionResult> GetOnePrintFolderModel(Search model)
        {
            string q = new ColorFolderQueryConfig().S_A_PRINT_FOLDER;
            string query = q.Replace(":AS_CF_SN", $"'{model.strSearch}'");
            var response = await new DataContext().GetResultDapperAsyncDynamic(DataBaseHostEnum.KPR, query);
            var result = _mapper.Map<IEnumerable<Folder>>(response);
            var results = result as List<Folder>;
            var resultReal = _mapper.Map<List<Folder>, List<FolderMap>>(results);
            return Ok(resultReal);
        }

        [HttpPost("getPrintAllFile")]
        public async Task<ActionResult> GetPrintAllFileModel(Search model)
        {
            string q = new ColorFolderQueryConfig().S_PRINT_ALL_FOLDER;
            string query = q.Replace(":AS_CF_SEQ", $"'{model.strSearch}'");
            var response = await new DataContext().GetResultDapperAsyncDynamic(DataBaseHostEnum.KPR, query);
            var result = _mapper.Map<IEnumerable<Folder>>(response);
            var results = result as List<Folder>;
            var resultReal = _mapper.Map<List<Folder>, List<FolderMap>>(results);
            return Ok(resultReal);
        }

        [HttpPost("getJobID")]
        public async Task<ActionResult> GetJobIDModel(GetJob model)
        {
            var idSplit = model.prodID.Split("-");
            var id = idSplit[0].PadLeft(12, '0');
            var rev = idSplit[1];
            string q = new ColorFolderQueryConfig().S_JOB_ID;
            string query = q.Replace(":AS_PROD_ID", $"'{id}'")
                            .Replace(":AS_REVISION", $"'{rev}'");
            var response = await new DataContext().GetResultDapperAsyncDynamic(DataBaseHostEnum.KPR, query);
            var result = _mapper.Map<IEnumerable<Job>>(response);
            var results = result as List<Job>;
            var resultReal = _mapper.Map<List<Job>, List<JobMap>>(results);
            return Ok(resultReal);
        }

        [HttpPost("getCustDetail")]
        public async Task<ActionResult> GetCustDetailModel(GetJob model)
        {
            var idSplit = model.prodID.Split("-");
            var id = idSplit[0].PadLeft(12, '0');
            var rev = idSplit[1];
            string q = new ColorFolderQueryConfig().S_CUST_DETAIL;
            string query = q.Replace(":AS_PROD_ID", $"'{id}'")
                            .Replace(":AS_REVISION", $"'{rev}'");
            var response = await new DataContext().GetResultDapperAsyncDynamic(DataBaseHostEnum.KPR, query);
            var result = _mapper.Map<IEnumerable<CustDetail>>(response);
            var results = result as List<CustDetail>;
            var resultReal = _mapper.Map<List<CustDetail>, List<CustDetailMap>>(results);
            return Ok(resultReal);
        }

        [HttpPost("getColorFilePDF")]
        public async Task<ActionResult> GetColorFilePDFTModel(List<FilePrint> model)
        {
            string fileName = "FC_File_" + DateTime.Now.ToString("ddMMyyhhmmss");
            var pathUrl = string.Empty;
            try
            {
                string pathRoot = "\\DEPLOY\\file";
                if (!Directory.Exists(pathRoot))
                {
                    Directory.CreateDirectory(pathRoot);
                }
                pathUrl = Path.Combine(pathRoot, fileName + ".pdf");
                FilePrintReport fpr = new FilePrintReport();
                fpr.DataSource = model;
                fpr.CreateDocument();
                fpr.ExportToPdf(pathUrl);
                return await new CFGenerateQrcode().GetFilePDF(pathRoot + "\\" + fileName + ".pdf");
            }
            catch (Exception e)
            {
                return Ok(e.Message);

            }
        }

        [HttpPost("getColorFolderPDF")]
        public async Task<ActionResult> GetColorFolderPDFTModel(List<FolderPrint> model)
        {
            string fileName = "FC_Folder_" + DateTime.Now.ToString("ddMMyyhhmmss");
            var pathUrl = string.Empty;
            try
            {
                string pathRoot = "\\DEPLOY\\folder";
                if (!Directory.Exists(pathRoot))
                {
                    Directory.CreateDirectory(pathRoot);
                }
                pathUrl = Path.Combine(pathRoot, fileName + ".pdf");
                FolderPrintReport fpr = new FolderPrintReport();
                fpr.DataSource = model;
                fpr.CreateDocument();
                fpr.ExportToPdf(pathUrl);
                return await new CFGenerateQrcode().GetFilePDF(pathRoot + "\\" + fileName + ".pdf");

                // return new FileStreamResult(System.IO.File.OpenRead(pathUrl), "application/octet-stream");
            }
            catch (Exception e)
            {
                return Ok(e.Message);

            }
        }

        [HttpPost("keepScanLog")]
        public async Task<ActionResult> KeepScanLogTModel(Search model)
        {
            ReturnKeepScanLoc returns = new ReturnKeepScanLoc();
            var code = model.strSearch.Split(":");
            if (code.Length != 2)
            {
                returns.stateError = true;
                returns.messageError = "ไม่พบข้อมูล Location นี้ในฐานข้อมูล";
                return Ok(returns);
            }
            string wh = code[0];
            string locID = code[1];
            string q = new ColorFolderQueryConfig().S_GET_LOC_DETAIL;
            string query = q.Replace(":AS_LOC_ID", $"'{locID}'")
                            .Replace(":AS_WAREHOUSE_ID", $"'{wh}'");
            var response = await new DataContext().GetResultDapperAsyncDynamic(DataBaseHostEnum.KPR, query);
            var c = (response as List<dynamic>).Count;
            if (c == 0)
            {
                returns.stateError = true;
                returns.messageError = "ไม่พบข้อมูล Location นี้ในฐานข้อมูล";
                return Ok(returns);
            }
            else
            {
                var result = _mapper.Map<IEnumerable<KeepScanLoc>>(response);
                var results = result as List<KeepScanLoc>;
                var resultReal = _mapper.Map<List<KeepScanLoc>, List<KeepScanLocMap>>(results);
                returns.stateError = false;
                returns.lists = resultReal;
                return Ok(returns);
            }
        }
        [HttpPost("keepScanSN")]
        public async Task<ActionResult> KeepScanSNTModel(Search model)
        {
            ReturnKeepScanSN returns = new ReturnKeepScanSN();
            string q = new ColorFolderQueryConfig().S_GET_COLOR_FOLDER_DETAIL;
            string query = q.Replace(":S_CF_SN", $"'{model.strSearch}'");
            var response = await new DataContext().GetResultDapperAsyncDynamic(DataBaseHostEnum.KPR, query);
            var c = (response as List<dynamic>).Count;
            if (c == 0)
            {
                returns.stateError = true;
                returns.messageError = "ไม่พบข้อมูลแฟ้มสีนี้ในฐานข้อมูล";
                return Ok(returns);
            }
            else
            {
                var result = _mapper.Map<IEnumerable<KeepScanSN>>(response);
                var results = result as List<KeepScanSN>;
                var resultReal = _mapper.Map<List<KeepScanSN>, List<KeepScanSNMap>>(results);
                var flag = resultReal[0].availableFlag;
                switch (flag)
                {
                    case "F":
                        returns.stateError = true;
                        returns.messageError = "แฟ้มสีนี้ไม่สามารถใช้งานได้";
                        return Ok(returns);
                    case "T":
                        string qQTY = new ColorFolderQueryConfig().C_QTY;
                        string queryQTY = qQTY.Replace(":S_CF_SN", $"'{model.strSearch}'");
                        var responseQTY = await new DataContext().GetResultDapperAsyncDynamic(DataBaseHostEnum.KPR, queryQTY);

                        var cqty = (responseQTY as List<dynamic>)[0].REMAIN_QTY;
                        if (cqty == 1)
                        {
                            returns.stateError = false;
                            returns.lists = resultReal;
                            return Ok(returns);
                        }
                        else
                        {
                            returns.stateError = true;
                            returns.messageError = "ไม่มีแฟ้มสีเหลือให้จัดเก็บ";
                            return Ok(returns);
                        }
                    default:
                        returns.stateError = false;
                        returns.lists = resultReal;
                        return Ok(returns);
                }
            }
        }
        [HttpPost("keepTrans")]
        public async Task<ActionResult> KeepTransTModel(keepTrans model)
        {
            StateError returns = new StateError();
            if (model.token == null || model.token == string.Empty)
            {
                returns.stateError = true;
                returns.message = "token is empty!!";
                return Ok(returns);
            }
            UserProfile userProfile = _jwtGenerator.DecodeToken(model.token);
            string org = userProfile.org;
            string userID = userProfile.userID;

            string qtd = new ColorFolderQueryConfig().S_TRAN_DATE;
            var responseQTD = await new DataContext().GetResultDapperAsyncDynamic(DataBaseHostEnum.KPR, qtd);
            var start = (responseQTD as List<dynamic>)[0].S;
            var end = (responseQTD as List<dynamic>)[0].END;
            if (model.compID == "EMP")
            {
                foreach (var folder in model.folderList)
                {
                    string qTranID = new ColorFolderQueryConfig().C_TRAN_ID;
                    var responseTranID = await new DataContext().GetResultDapperAsyncDynamic(DataBaseHostEnum.KPR, qTranID);
                    decimal coun = (responseTranID as List<dynamic>)[0].COUN;
                    string dateSN = DateTime.Now.ToString("yyMMdd");
                    string tranID = "T" + dateSN + coun.ToString("00000000");
                    string qicf = new ColorFolderQueryConfig().I_CF_STOCK_DETAIL;
                    if (folder.availableFlag == "T")
                    {
                        string qCheck = new ColorFolderQueryConfig().CHECK_FOLDER_SN_DETAIL;
                        string qCheckn = qCheck.Replace(":AS_CF_SN", $"'{folder.cfSN}'");
                        var responseCheck = await new DataContext().GetResultDapperAsyncDynamic(DataBaseHostEnum.KPR, qCheckn);
                        var loc = (responseCheck as List<dynamic>)[0].LOC_ID;
                        var emp = (responseCheck as List<dynamic>)[0].EMP_ID;
                        var wh = (responseCheck as List<dynamic>)[0].WAREHOUSE_ID;
                        var comp = (responseCheck as List<dynamic>)[0].COMP_ID;

                        if (emp != null)
                        {
                            List<string> insertQueryEMPtoEMP = new List<string>();
                            string qEMPs = qicf.Replace(":AS_TRAN_ID", $"'{tranID}'")
                                                    .Replace(":AS_TRAN_SEQ", $"'1'")
                                                    .Replace(":AS_TRAN_TYPE", $"'25'")
                                                    .Replace(":AS_TRAN_DATE", $"'{start}'")
                                                    .Replace(":AS_CF_SN", $"'{folder.cfSN}'")
                                                    .Replace(":AS_QTY", $"'-1'")
                                                    .Replace(":AS_COMP_ID", $"''")
                                                    .Replace(":AS_WAREHOUSE_ID", $"''")
                                                    .Replace(":AS_LOC_ID", $"''")
                                                    .Replace(":AS_EMP_ID", $"'{emp}'")
                                                    .Replace(":AS_STATUS", $"'T'")
                                                    .Replace(":AS_REMARK", $"'ออกจากบุคคลสู่บุคคล'")
                                                    .Replace(":AS_CR_DATE", $"'{start}'")
                                                    .Replace(":AS_CR_ORG_ID", $"'{org}'")
                                                    .Replace(":AS_CR_USER_ID", $"'{userID}'");
                            insertQueryEMPtoEMP.Add(qEMPs);
                            string qEMPe = qicf.Replace(":AS_TRAN_ID", $"'{tranID}'")
                                                    .Replace(":AS_TRAN_SEQ", $"'2'")
                                                    .Replace(":AS_TRAN_TYPE", $"'24'")
                                                    .Replace(":AS_TRAN_DATE", $"'{end}'")
                                                    .Replace(":AS_CF_SN", $"'{folder.cfSN}'")
                                                    .Replace(":AS_QTY", $"'1'")
                                                    .Replace(":AS_COMP_ID", $"''")
                                                    .Replace(":AS_WAREHOUSE_ID", $"''")
                                                    .Replace(":AS_LOC_ID", $"''")
                                                    .Replace(":AS_EMP_ID", $"'{model.locID}'")
                                                    .Replace(":AS_STATUS", $"'T'")
                                                    .Replace(":AS_REMARK", $"'ออกจากบุคคลสู่บุคคล'")
                                                    .Replace(":AS_CR_DATE", $"'{start}'")
                                                    .Replace(":AS_CR_ORG_ID", $"'{org}'")
                                                    .Replace(":AS_CR_USER_ID", $"'{userID}'");
                            insertQueryEMPtoEMP.Add(qEMPe);
                            var responseTEMP = await new DataContext().ExecuteDapperMultiAsync(DataBaseHostEnum.KPR, insertQueryEMPtoEMP);
                        }
                        else
                        {
                            List<string> insertQueryLoctoEMP = new List<string>();
                            string qEMPs = qicf.Replace(":AS_TRAN_ID", $"'{tranID}'")
                                                    .Replace(":AS_TRAN_SEQ", $"'1'")
                                                    .Replace(":AS_TRAN_TYPE", $"'5'")
                                                    .Replace(":AS_TRAN_DATE", $"'{start}'")
                                                    .Replace(":AS_CF_SN", $"'{folder.cfSN}'")
                                                    .Replace(":AS_QTY", $"'-1'")
                                                    .Replace(":AS_COMP_ID", $"'{comp}'")
                                                    .Replace(":AS_WAREHOUSE_ID", $"'{wh}'")
                                                    .Replace(":AS_LOC_ID", $"'{loc}'")
                                                    .Replace(":AS_EMP_ID", $"'{emp}'")
                                                    .Replace(":AS_STATUS", $"'T'")
                                                    .Replace(":AS_REMARK", $"'ออกจากตำแหน่งที่วางสู่บุคคล'")
                                                    .Replace(":AS_CR_DATE", $"'{start}'")
                                                    .Replace(":AS_CR_ORG_ID", $"'{org}'")
                                                    .Replace(":AS_CR_USER_ID", $"'{userID}'");
                            insertQueryLoctoEMP.Add(qEMPs);
                            string qEMPe = qicf.Replace(":AS_TRAN_ID", $"'{tranID}'")
                                                    .Replace(":AS_TRAN_SEQ", $"'2'")
                                                    .Replace(":AS_TRAN_TYPE", $"'2'")
                                                    .Replace(":AS_TRAN_DATE", $"'{end}'")
                                                    .Replace(":AS_CF_SN", $"'{folder.cfSN}'")
                                                    .Replace(":AS_QTY", $"'1'")
                                                    .Replace(":AS_COMP_ID", $"''")
                                                    .Replace(":AS_WAREHOUSE_ID", $"''")
                                                    .Replace(":AS_LOC_ID", $"''")
                                                    .Replace(":AS_EMP_ID", $"'{model.locID}'")
                                                    .Replace(":AS_STATUS", $"'T'")
                                                    .Replace(":AS_REMARK", $"'รับจากตำแหน่งที่วาง'")
                                                    .Replace(":AS_CR_DATE", $"'{start}'")
                                                    .Replace(":AS_CR_ORG_ID", $"'{org}'")
                                                    .Replace(":AS_CR_USER_ID", $"'{userID}'");
                            insertQueryLoctoEMP.Add(qEMPe);
                            var responseTLOC = await new DataContext().ExecuteDapperMultiAsync(DataBaseHostEnum.KPR, insertQueryLoctoEMP);
                        }
                    }
                    else
                    {
                        returns.stateError = true;
                        returns.message = "ไม่พบเงื่อนไข availableFlag";
                        return Ok(returns);
                    }
                }
            }
            else
            {
                foreach (var folder in model.folderList)
                {
                    string qTranID = new ColorFolderQueryConfig().C_TRAN_ID;
                    var responseTranID = await new DataContext().GetResultDapperAsyncDynamic(DataBaseHostEnum.KPR, qTranID);
                    decimal coun = (responseTranID as List<dynamic>)[0].COUN;
                    string dateSN = DateTime.Now.ToString("yyMMdd");
                    string tranID = "T" + dateSN + coun.ToString("00000000");
                    string qicf = new ColorFolderQueryConfig().I_CF_STOCK_DETAIL;
                    if (folder.availableFlag == "P")
                    {
                        List<string> insertQueryP = new List<string>();
                        string qicfNew = qicf.Replace(":AS_TRAN_ID", $"'{tranID}'")
                        .Replace(":AS_TRAN_SEQ", $"'1'")
                        .Replace(":AS_TRAN_TYPE", $"'1'")
                        .Replace(":AS_TRAN_DATE", $"'{start}'")
                        .Replace(":AS_CF_SN", $"'{folder.cfSN}'")
                        .Replace(":AS_QTY", $"'1'")
                        .Replace(":AS_COMP_ID", $"'{model.compID}'")
                        .Replace(":AS_WAREHOUSE_ID", $"'{model.warehouseID}'")
                        .Replace(":AS_LOC_ID", $"'{model.locID}'")
                        .Replace(":AS_EMP_ID", $"''")
                        .Replace(":AS_STATUS", $"'T'")
                        .Replace(":AS_REMARK", $"'รับเข้าคลังหลังการลงทะเบียน'")
                        .Replace(":AS_CR_DATE", $"'{start}'")
                        .Replace(":AS_CR_ORG_ID", $"'{org}'")
                        .Replace(":AS_CR_USER_ID", $"'{userID}'");
                        insertQueryP.Add(qicfNew);
                        string qUSTSN = new ColorFolderQueryConfig().U_STATUS_COLOR_SN;
                        string qUSTSNn = qUSTSN.Replace(":AS_CF_SN", $"'{folder.cfSN}'");
                        insertQueryP.Add(qUSTSNn);
                        var responseP = await new DataContext().ExecuteDapperMultiAsync(DataBaseHostEnum.KPR, insertQueryP);

                    }
                    else if (folder.availableFlag == "T")
                    {
                        string qCheck = new ColorFolderQueryConfig().CHECK_FOLDER_SN_DETAIL;
                        string qCheckn = qCheck.Replace(":AS_CF_SN", $"'{folder.cfSN}'");
                        var responseCheck = await new DataContext().GetResultDapperAsyncDynamic(DataBaseHostEnum.KPR, qCheckn);
                        var loc = (responseCheck as List<dynamic>)[0].LOC_ID;
                        var emp = (responseCheck as List<dynamic>)[0].EMP_ID;
                        var wh = (responseCheck as List<dynamic>)[0].WAREHOUSE_ID;
                        var comp = (responseCheck as List<dynamic>)[0].COMP_ID;

                        if (emp != null)
                        {
                            List<string> insertQueryEMP = new List<string>();
                            string qEMPs = qicf.Replace(":AS_TRAN_ID", $"'{tranID}'")
                                                    .Replace(":AS_TRAN_SEQ", $"'1'")
                                                    .Replace(":AS_TRAN_TYPE", $"'25'")
                                                    .Replace(":AS_TRAN_DATE", $"'{start}'")
                                                    .Replace(":AS_CF_SN", $"'{folder.cfSN}'")
                                                    .Replace(":AS_QTY", $"'-1'")
                                                    .Replace(":AS_COMP_ID", $"''")
                                                    .Replace(":AS_WAREHOUSE_ID", $"''")
                                                    .Replace(":AS_LOC_ID", $"''")
                                                    .Replace(":AS_EMP_ID", $"'{emp}'")
                                                    .Replace(":AS_STATUS", $"'T'")
                                                    .Replace(":AS_REMARK", $"'บุคคลส่งมอบคืนคลัง'")
                                                    .Replace(":AS_CR_DATE", $"'{start}'")
                                                    .Replace(":AS_CR_ORG_ID", $"'{org}'")
                                                    .Replace(":AS_CR_USER_ID", $"'{userID}'");
                            insertQueryEMP.Add(qEMPs);
                            string qEMPe = qicf.Replace(":AS_TRAN_ID", $"'{tranID}'")
                                                    .Replace(":AS_TRAN_SEQ", $"'2'")
                                                    .Replace(":AS_TRAN_TYPE", $"'3'")
                                                    .Replace(":AS_TRAN_DATE", $"'{end}'")
                                                    .Replace(":AS_CF_SN", $"'{folder.cfSN}'")
                                                    .Replace(":AS_QTY", $"'1'")
                                                    .Replace(":AS_COMP_ID", $"'{model.compID}'")
                                                    .Replace(":AS_WAREHOUSE_ID", $"'{model.warehouseID}'")
                                                    .Replace(":AS_LOC_ID", $"'{model.locID}'")
                                                    .Replace(":AS_EMP_ID", $"''")
                                                    .Replace(":AS_STATUS", $"'T'")
                                                    .Replace(":AS_REMARK", $"'รับคืนจากการจ่าย/ยืม'")
                                                    .Replace(":AS_CR_DATE", $"'{start}'")
                                                    .Replace(":AS_CR_ORG_ID", $"'{org}'")
                                                    .Replace(":AS_CR_USER_ID", $"'{userID}'");
                            insertQueryEMP.Add(qEMPe);
                            var responseTEMP = await new DataContext().ExecuteDapperMultiAsync(DataBaseHostEnum.KPR, insertQueryEMP);
                        }
                        else if (loc != null)
                        {
                            if (wh == model.warehouseID)
                            {
                                List<string> insertQueryLoc = new List<string>();
                                string qEMPs = qicf.Replace(":AS_TRAN_ID", $"'{tranID}'")
                                                        .Replace(":AS_TRAN_SEQ", $"'1'")
                                                        .Replace(":AS_TRAN_TYPE", $"'5'")
                                                        .Replace(":AS_TRAN_DATE", $"'{start}'")
                                                        .Replace(":AS_CF_SN", $"'{folder.cfSN}'")
                                                        .Replace(":AS_QTY", $"'-1'")
                                                        .Replace(":AS_COMP_ID", $"'{comp}'")
                                                        .Replace(":AS_WAREHOUSE_ID", $"'{wh}'")
                                                        .Replace(":AS_LOC_ID", $"'{loc}'")
                                                        .Replace(":AS_EMP_ID", $"'{emp}'")
                                                        .Replace(":AS_STATUS", $"'T'")
                                                        .Replace(":AS_REMARK", $"'ย้ายไป {model.warehouseID}{model.locID}'")
                                                        .Replace(":AS_CR_DATE", $"'{start}'")
                                                        .Replace(":AS_CR_ORG_ID", $"'{org}'")
                                                        .Replace(":AS_CR_USER_ID", $"'{userID}'");
                                insertQueryLoc.Add(qEMPs);
                                string qEMPe = qicf.Replace(":AS_TRAN_ID", $"'{tranID}'")
                                                        .Replace(":AS_TRAN_SEQ", $"'2'")
                                                        .Replace(":AS_TRAN_TYPE", $"'4'")
                                                        .Replace(":AS_TRAN_DATE", $"'{end}'")
                                                        .Replace(":AS_CF_SN", $"'{folder.cfSN}'")
                                                        .Replace(":AS_QTY", $"'1'")
                                                        .Replace(":AS_COMP_ID", $"'{model.compID}'")
                                                        .Replace(":AS_WAREHOUSE_ID", $"'{model.warehouseID}'")
                                                        .Replace(":AS_LOC_ID", $"'{model.locID}'")
                                                        .Replace(":AS_EMP_ID", $"''")
                                                        .Replace(":AS_STATUS", $"'T'")
                                                        .Replace(":AS_REMARK", $"'ย้ายจาก {wh}{loc}'")
                                                        .Replace(":AS_CR_DATE", $"'{start}'")
                                                        .Replace(":AS_CR_ORG_ID", $"'{org}'")
                                                        .Replace(":AS_CR_USER_ID", $"'{userID}'");
                                insertQueryLoc.Add(qEMPe);
                                var responseTLOC = await new DataContext().ExecuteDapperMultiAsync(DataBaseHostEnum.KPR, insertQueryLoc);
                            }
                            else
                            {
                                List<string> insertQueryWH = new List<string>();
                                string qEMPs = qicf.Replace(":AS_TRAN_ID", $"'{tranID}'")
                                                        .Replace(":AS_TRAN_SEQ", $"'1'")
                                                        .Replace(":AS_TRAN_TYPE", $"'15'")
                                                        .Replace(":AS_TRAN_DATE", $"'{start}'")
                                                        .Replace(":AS_CF_SN", $"'{folder.cfSN}'")
                                                        .Replace(":AS_QTY", $"'-1'")
                                                        .Replace(":AS_COMP_ID", $"'{comp}'")
                                                        .Replace(":AS_WAREHOUSE_ID", $"'{wh}'")
                                                        .Replace(":AS_LOC_ID", $"'{loc}'")
                                                        .Replace(":AS_EMP_ID", $"'{emp}'")
                                                        .Replace(":AS_STATUS", $"'T'")
                                                        .Replace(":AS_REMARK", $"'ย้ายข้ามคลังไป  {model.warehouseID}{model.locID}'")
                                                        .Replace(":AS_CR_DATE", $"'{start}'")
                                                        .Replace(":AS_CR_ORG_ID", $"'{org}'")
                                                        .Replace(":AS_CR_USER_ID", $"'{userID}'");
                                insertQueryWH.Add(qEMPs);
                                string qEMPe = qicf.Replace(":AS_TRAN_ID", $"'{tranID}'")
                                                        .Replace(":AS_TRAN_SEQ", $"'2'")
                                                        .Replace(":AS_TRAN_TYPE", $"'14'")
                                                        .Replace(":AS_TRAN_DATE", $"'{end}'")
                                                        .Replace(":AS_CF_SN", $"'{folder.cfSN}'")
                                                        .Replace(":AS_QTY", $"'1'")
                                                        .Replace(":AS_COMP_ID", $"'{model.compID}'")
                                                        .Replace(":AS_WAREHOUSE_ID", $"'{model.warehouseID}'")
                                                        .Replace(":AS_LOC_ID", $"'{model.locID}'")
                                                        .Replace(":AS_EMP_ID", $"''")
                                                        .Replace(":AS_STATUS", $"'T'")
                                                        .Replace(":AS_REMARK", $"'ย้ายข้ามคลังจาก {wh}{loc}'")
                                                        .Replace(":AS_CR_DATE", $"'{start}'")
                                                        .Replace(":AS_CR_ORG_ID", $"'{org}'")
                                                        .Replace(":AS_CR_USER_ID", $"'{userID}'");
                                insertQueryWH.Add(qEMPe);
                                var responseTWH = await new DataContext().ExecuteDapperMultiAsync(DataBaseHostEnum.KPR, insertQueryWH);
                            }
                        }
                    }
                    else
                    {
                        returns.stateError = true;
                        returns.message = "ไม่พบเงื่อนไข availableFlag";
                        return Ok(returns);
                    }
                }
            }
            returns.stateError = false;
            return Ok(returns);
        }
        [HttpPost("getQrcode")]
        public ActionResult GetQrcodeTModel(Search model)
        {
            UserProfile userProfile = _jwtGenerator.DecodeToken(model.token);
            string org = userProfile.org;
            string userID = userProfile.userID;
            SetQrCode response = new SetQrCode();
            var stringBase = new GenerateQrcode().generateKeepQrcode(userID);
            response.qrCode = stringBase;
            return Ok(response);
        }

        [Authorize]
        [HttpPost("getMyFolder")]
        public async Task<ActionResult> GetMyFolderTModel(Search model)
        {
            UserProfile userProfile = _jwtGenerator.DecodeToken(model.token);
            string org = userProfile.org;
            string userID = userProfile.userID;
            return Ok(true);
        }


        [HttpPost("reviceScanLocEmp")]
        public async Task<ActionResult> ReviceScanLocEmpTModel(Search model)
        {
            ReturnKeepScanLoc returns = new ReturnKeepScanLoc();
            var code = model.strSearch.Split(":");
            if (code.Length == 1)
            {
                string qEMP = new ColorFolderQueryConfig().S_EMP_DETAIL;
                string queryEMP = qEMP.Replace(":AS_EMP_ID", $"'{model.strSearch}'");
                var response = await new DataContext().GetResultDapperAsyncDynamic(DataBaseHostEnum.KPR, queryEMP);
                var c = (response as List<dynamic>).Count;
                if (c == 0)
                {
                    returns.stateError = true;
                    returns.messageError = "ไม่พบพนักงานหรือพนักงานลาออกแล้ว ไม่สามารถส่งมอบให้ได้";
                    return Ok(returns);
                }
                else
                {
                    returns.stateError = false;
                    List<KeepScanLocMap> EMPS = new List<KeepScanLocMap>();
                    KeepScanLocMap EMP = new KeepScanLocMap();
                    EMP.locID = (response as List<dynamic>)[0].EMP_ID;
                    EMP.locDetail = (response as List<dynamic>)[0].EMP_NAME;
                    EMP.compID = "EMP";
                    EMP.warehouseID = null;
                    EMPS.Add(EMP);
                    returns.lists = EMPS;
                    return Ok(returns);
                }
            }
            else if (code.Length == 2)
            {
                string wh = code[0];
                string locID = code[1];
                string q = new ColorFolderQueryConfig().S_GET_LOC_DETAIL;
                string query = q.Replace(":AS_LOC_ID", $"'{locID}'")
                                .Replace(":AS_WAREHOUSE_ID", $"'{wh}'");
                var response = await new DataContext().GetResultDapperAsyncDynamic(DataBaseHostEnum.KPR, query);
                var c = (response as List<dynamic>).Count;
                if (c == 0)
                {
                    returns.stateError = true;
                    returns.messageError = "ไม่พบตำแหน่งที่เก็บ";
                    return Ok(returns);
                }
                else
                {
                    var result = _mapper.Map<IEnumerable<KeepScanLoc>>(response);
                    var results = result as List<KeepScanLoc>;
                    var resultReal = _mapper.Map<List<KeepScanLoc>, List<KeepScanLocMap>>(results);
                    returns.stateError = false;
                    returns.lists = resultReal;
                    return Ok(returns);
                }
            }
            else
            {
                returns.stateError = true;
                returns.messageError = "ไม่พบรหัสนี้ในฐานข้อมูล";
                return Ok(returns);
            }
        }
        [HttpPost("reciveScanSN")]
        public async Task<ActionResult> ReciveScanSNTModel(Search model)
        {
            ReturnKeepScanSN returns = new ReturnKeepScanSN();
            string q = new ColorFolderQueryConfig().S_GET_COLOR_FOLDER_DETAIL;
            string query = q.Replace(":S_CF_SN", $"'{model.strSearch}'");
            var response = await new DataContext().GetResultDapperAsyncDynamic(DataBaseHostEnum.KPR, query);
            var c = (response as List<dynamic>).Count;
            if (c == 0)
            {
                returns.stateError = true;
                returns.messageError = "ไม่พบข้อมูลแฟ้มสีนี้ในฐานข้อมูล";
                return Ok(returns);
            }
            else
            {
                var result = _mapper.Map<IEnumerable<KeepScanSN>>(response);
                var results = result as List<KeepScanSN>;
                var resultReal = _mapper.Map<List<KeepScanSN>, List<KeepScanSNMap>>(results);
                var flag = resultReal[0].availableFlag;
                switch (flag)
                {
                    case "F":
                        returns.stateError = true;
                        returns.messageError = "แฟ้มสีนี้ไม่สามารถใช้งานได้";
                        return Ok(returns);
                    case "P":
                        returns.stateError = true;
                        returns.messageError = "แฟ้มสีนี้ไม่สามารถใช้งานได้";
                        return Ok(returns);
                    case "T":
                        string qQTY = new ColorFolderQueryConfig().C_QTY;
                        string queryQTY = qQTY.Replace(":S_CF_SN", $"'{model.strSearch}'");
                        var responseQTY = await new DataContext().GetResultDapperAsyncDynamic(DataBaseHostEnum.KPR, queryQTY);

                        var cqty = (responseQTY as List<dynamic>)[0].REMAIN_QTY;
                        if (cqty == 1)
                        {
                            returns.stateError = false;
                            returns.lists = resultReal;
                            return Ok(returns);
                        }
                        else
                        {
                            returns.stateError = true;
                            returns.messageError = "ไม่มีแฟ้มสีเหลือให้จัดเก็บ";
                            return Ok(returns);
                        }
                    default:
                        returns.stateError = false;
                        returns.lists = resultReal;
                        return Ok(returns);
                }
            }
        }
    }
}