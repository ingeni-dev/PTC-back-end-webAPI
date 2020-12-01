using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using PTCwebApi.Interfaces;
using PTCwebApi.Models.ProfilesModels;
using PTCwebApi.Models.PTCModels.MethodModels;
using PTCwebApi.Models.PTCModels.MethodModels.FindTools;
using PTCwebApi.Models.PTCModels.MethodModels.MoveLoc;
using PTCwebApi.Models.PTCModels.MethodModels.ReturnTooling;

namespace PTCwebApi.Methods.PTCMethods
{
    public class PTCMethodsReturnTool
    {
        private readonly IJwtGenerator _jwtGenerator;
        private readonly IMapper _mapper;
        public PTCMethodsReturnTool(IMapper mapper, IJwtGenerator jwtGenerator)
        {
            _jwtGenerator = jwtGenerator;
            _mapper = mapper;
        }
        public async Task<object> getWithdrawalHistory(RequestAllModelsPTC model)
        {
            string _returnFlag = "0";
            string _returnText = "ผ่าน";
            List<ResponseWthdralwalHistoryList> _resultHistory = null;
            if (model.token != null)
            {
                UserProfile userProfile = _jwtGenerator.DecodeToken(model.token);
                var query = $"SELECT * FROM KPDBA.PTC_JS_PLAN_DETAIL WHERE RETURN_DATE IS NULL AND RETURN_USER_ID IS NULL AND WITHD_USER_ID = '{userProfile.userID}'";
                var result = await new DataContext().GetResultDapperAsyncObject(DataBaseHostEnum.KPR, query);
                // return result;
                if ((result as List<object>).Count != 0)
                {
                    var results = _mapper.Map<IEnumerable<RequestWthdralwalHistoryList>>(result);
                    _resultHistory = _mapper.Map<IEnumerable<RequestWthdralwalHistoryList>, IEnumerable<ResponseWthdralwalHistoryList>>(results).ToList();
                }
                else
                {
                    _returnFlag = "1";
                    _returnText = "ไม่พบประวัติการเบิก";
                }
            }
            else
            {
                _returnFlag = "1";
                _returnText = "ไม่พบข้อมูลผู้ใช้ กรุณา Login อีกครั้ง หรือติดต่อฝ่าย IT ";
            }
            var returnResult = new ResponseWthdralwalHistory
            {
                historyList = _resultHistory,
                flag = _returnFlag,
                text = _returnText
            };
            return returnResult;
        }

        public async Task<object> scanCheckNewGet(RequestAllModelsPTC model)
        {
            ReturnDataMoveLoc result = new ReturnDataMoveLoc();
            var queryCheck = $"SELECT COUNT(1) AS COUN FROM KPDBA.DIECUT_SN WHERE DIECUT_SN ='{model.ptcID}'";
            var resultCheck = await new DataContext().GetResultDapperAsyncDynamic(DataBaseHostEnum.KPR, queryCheck);
            decimal toolReal = (resultCheck as List<dynamic>)[0].COUN;
            return result;
        }

        public async Task<object> moveToKeep(RequestAllModelsPTC model)
        {
            string _returnFlag = "0";
            string _returnText = "ผ่าน";
            ReturnDataMoveLoc result = new ReturnDataMoveLoc();
            if (model.token != null)
            {
                UserProfile userProfile = _jwtGenerator.DecodeToken(model.token);
                if (userProfile != null)
                {
                    //     var querySN = $"SELECT DIECUT_TYPE FROM KPDBA.DIECUT_SN WHERE DIECUT_SN ='{model.diecutSN}'";
                    //     var resultSN = await new DataContext().GetResultDapperAsyncDynamic(DataBaseHostEnum.KPR, querySN);
                    // if ((resultSN as List<dynamic>).Count != 0) { model.ptcID = (resultSN as List<dynamic>)[0].DIECUT_SN; }
                    var queryCheck = $"SELECT DIECUT_TYPE FROM KPDBA.DIECUT_SN WHERE DIECUT_SN ='{model.diecutSN}'";
                    var resultCheck = await new DataContext().GetResultDapperAsyncDynamic(DataBaseHostEnum.KPR, queryCheck);
                    decimal toolReal = (resultCheck as List<dynamic>).Count;
                    if (toolReal != 0)
                    {
                        var ptcTypes = (resultCheck as List<dynamic>)[0].DIECUT_TYPE;
                        var queryCheckLoc = $"SELECT COUNT(1) AS COUN FROM KPDBA.LOCATION_PTC WHERE LOC_ID = '{model.locID}' AND WAREHOUSE_ID = '{model.warehouseID}' AND PTC_TYPE = '{ptcTypes}'";
                        var resultCheckLoc = await new DataContext().GetResultDapperAsyncDynamic(DataBaseHostEnum.KPR, queryCheckLoc);
                        decimal LocValid = (resultCheckLoc as List<dynamic>)[0].COUN;
                        if (LocValid != 0)
                        {
                            var queryQTY = $"SELECT SUM(QTY) QTY FROM KPDBA.PTC_STOCK_DETAIL WHERE WAREHOUSE_ID = '{model.warehouseID}' AND PTC_ID = '{model.diecutSN}'";
                            var resultQTY = await new DataContext().GetResultDapperAsyncObject(DataBaseHostEnum.KPR, queryQTY);
                            var countQty = (resultQTY as List<dynamic>)[0].QTY;
                            if (countQty == 1)
                            {
                                var query = $"SELECT SD.LOC_ID AS LOC_ID, LOC.LOC_DETAIL, SD.COMP_ID AS COMP, SD.QTY FROM (SELECT SD.WAREHOUSE_ID, SD.LOC_ID, SD.COMP_ID, SUM (SD.QTY) QTY FROM KPDBA.PTC_STOCK_DETAIL SD WHERE SD.WAREHOUSE_ID = '{model.warehouseID}' AND SD.PTC_ID = '{model.diecutSN}' GROUP BY SD.WAREHOUSE_ID, SD.COMP_ID, SD.LOC_ID HAVING SUM (SD.QTY) > 0) SD JOIN (SELECT WAREHOUSE_ID, LOC_ID, LOC_DETAIL FROM KPDBA.LOCATION_PTC WHERE PTC_TYPE = '{ptcTypes}') LOC ON (SD.WAREHOUSE_ID = LOC.WAREHOUSE_ID AND SD.LOC_ID = LOC.LOC_ID)";
                                var resultt = await new DataContext().GetResultDapperAsyncObject(DataBaseHostEnum.KPR, query);
                                var compID = (resultt as List<dynamic>)[0].COMP;
                                decimal count = (resultt as List<object>).Count;
                                if (count != 0)
                                {
                                    var locNow = (resultt as List<dynamic>)[0].LOC_ID;
                                    if (locNow == "$W70" || locNow == "$WG0")
                                    {
                                        List<string> insertQuery = new List<string>();
                                        var results = _mapper.Map<IEnumerable<GetLoc>>(resultt);
                                        var dataLoc = results.ElementAt(0);

                                        var tranSEQ = 1;
                                        var tranType = "3"; // โอนย้ายออก
                                        var locID = dataLoc.LOC_ID; // old loc
                                        string tran_id = await new StoreConnectionMethod(_mapper).PtcGetTranID(compID: compID, tranType: "4");
                                        var tranDate = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss", new CultureInfo("en-US"));
                                        var insertGetQuery = $"INSERT INTO KPDBA.PTC_STOCK_DETAIL (TRAN_ID, TRAN_SEQ, TRAN_TYPE, TRAN_DATE,PTC_ID, QTY, COMP_ID, WAREHOUSE_ID, LOC_ID, STATUS, CR_DATE, CR_ORG_ID, CR_USER_ID, PTC_TYPE) VALUES ('{tran_id}', TO_NUMBER('{tranSEQ}'), TO_NUMBER('{tranType}'), TO_DATE('{tranDate}', 'dd/mm/yyyy hh24:mi:ss'),'{model.diecutSN}', TO_NUMBER('-1'), TO_CHAR('{compID}'),'{model.warehouseID}','{locID}', 'T', SYSDATE, '{userProfile.org}', '{userProfile.userID}', TO_CHAR('{ptcTypes}'))";
                                        insertQuery.Add(insertGetQuery);
                                        // var resultInsert = await new DataContext().InsertResultDapperAsync(DataBaseHostEnum.KPR, insertGetQuery)1;

                                        tranSEQ = 2;
                                        var QTY = "1";
                                        var S_STATUS = 'T';
                                        tranType = "3"; // โอนย้ายออก
                                        locID = "$R70"; // old loc
                                        tranDate = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss", new CultureInfo("en-US"));
                                        var insertOutQuery = $"INSERT INTO KPDBA.PTC_STOCK_DETAIL (TRAN_ID, TRAN_SEQ, TRAN_TYPE, TRAN_DATE,PTC_ID, QTY, COMP_ID, WAREHOUSE_ID, LOC_ID, STATUS, CR_DATE, CR_ORG_ID, CR_USER_ID, PTC_TYPE) VALUES ('{tran_id}', TO_NUMBER('{tranSEQ}'), TO_NUMBER('{tranType}'), TO_DATE('{tranDate}', 'dd/mm/yyyy hh24:mi:ss'),'{model.diecutSN}', TO_NUMBER('{QTY}'), TO_CHAR('{compID}'),'{model.warehouseID}','{locID}', TO_CHAR('{S_STATUS}'), SYSDATE, '{userProfile.org}', '{userProfile.userID}', TO_CHAR('{ptcTypes}'))";
                                        insertQuery.Add(insertOutQuery);
                                        // var resultOutInsert = await new DataContext().InsertResultDapperAsync(DataBaseHostEnum.KPR, insertOutQuery);

                                        string DateNow = DateTime.Today.ToString("dd/MM/yyyy", new CultureInfo("en-US"));
                                        string queHistory = $"SELECT * FROM KPDBA.PTC_JS_PLAN_DETAIL WHERE RETURN_DATE IS NULL AND RETURN_USER_ID IS NULL AND DIECUT_SN='{model.diecutSN}'";
                                        var resultQueHis = await new DataContext().GetResultDapperAsyncDynamic(DataBaseHostEnum.KPR, queHistory);
                                        // decimal countHis = (resultCompID as List<dynamic>).Count;
                                        // List<string> testUpdate = new List<string>();

                                        if (resultQueHis != null)
                                        {
                                            var querys = $"UPDATE KPDBA.PTC_JS_PLAN_DETAIL SET RETURN_DATE = TO_DATE ('{DateNow}', 'dd/mm/yyyy'), RETURN_USER_ID = TO_CHAR ('{userProfile.userID}') WHERE RETURN_DATE IS NULL AND RETURN_USER_ID IS NULL AND DIECUT_SN = '{model.diecutSN}'";
                                            insertQuery.Add(querys);
                                        }
                                        var resultInsertAll = await new DataContext().ExecuteDapperMultiAsync(DataBaseHostEnum.KPR, insertQuery);
                                    }
                                    else
                                    {
                                        _returnFlag = "1";
                                        _returnText = "ไม่พบอุปกรณ์ในพื้นที่เบิก";
                                    }
                                }
                                else
                                {
                                    _returnFlag = "1";
                                    _returnText = "ไม่พบอุปกรณ์ในประวัติการเบิก";
                                }
                            }
                            else if (countQty == null || countQty == 0)
                            {
                                var queryCompID = $"SELECT COMP_ID COMP FROM KPDBA.WAREHOUSE WHERE WAREHOUSE_ID ='{model.warehouseID}'";
                                var resultCompID = await new DataContext().GetResultDapperAsyncObject(DataBaseHostEnum.KPR, queryCompID);
                                var compID = (resultCompID as List<dynamic>)[0].COMP;
                                var tranSEQ = 1;
                                var QTY = "1";
                                var S_STATUS = 'T';
                                var tranType = "12"; // New tooling
                                var locID = "$R70"; // old loc
                                string tran_id = await new StoreConnectionMethod(_mapper).PtcGetTranID(compID: compID, tranType: "4");

                                var tranDate = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss", new CultureInfo("en-US"));
                                var insertOutQuery = $"INSERT INTO KPDBA.PTC_STOCK_DETAIL (TRAN_ID, TRAN_SEQ, TRAN_TYPE, TRAN_DATE,PTC_ID, QTY, COMP_ID, WAREHOUSE_ID, LOC_ID, STATUS, CR_DATE, CR_ORG_ID, CR_USER_ID, PTC_TYPE) VALUES ('{tran_id}', TO_NUMBER('{tranSEQ}'), TO_NUMBER('{tranType}'), TO_DATE('{tranDate}', 'dd/mm/yyyy hh24:mi:ss'),'{model.diecutSN}', TO_NUMBER('{QTY}'), TO_CHAR('{compID}'),'{model.warehouseID}','{locID}', TO_CHAR('{S_STATUS}'), SYSDATE, '{userProfile.org}', '{userProfile.userID}', TO_CHAR('{ptcTypes}'))";
                                var resultOutInsert = await new DataContext().InsertResultDapperAsync(DataBaseHostEnum.KPR, insertOutQuery);
                            }
                            else
                            {
                                _returnFlag = "1";
                                _returnText = "ไม่พบอุปกรณ์คงเหลือภายในคลัง";
                            }
                        }
                        else
                        {
                            _returnFlag = "1";
                            //ไม่พบหมายเลข Location นี้ในฐานข้อมูล
                            _returnText = "ไม่พบพื้นที่นี้ภายในคลัง";
                        }
                    }
                    else
                    {
                        _returnFlag = "1";
                        _returnText = "ไม่พบรหัส Tooling นี้ในฐานข้อมูล";
                    }
                }
                else
                {
                    _returnFlag = "1";
                    _returnText = "ข้อมูลผู้ใช้เกิดข้อผิดพลาด กรุณาติดต่อฝ่าย IT";
                }
            }
            else
            {
                _returnFlag = "1";
                _returnText = "ไม่พบข้อมูลผู้ใช้ กรุณา Login อีกครั้ง หรือติดต่อฝ่าย IT ";
            }
            var resultts = new ReturnDataMoveLoc
            {
                flag = _returnFlag,
                text = _returnText
            };

            return resultts;
        }
    }
}