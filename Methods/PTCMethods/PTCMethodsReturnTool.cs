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
                    var queryCheck = $"SELECT COUNT(1) AS COUN FROM KPDBA.DIECUT_SN WHERE DIECUT_SN ='{model.ptcID}'";
                    var resultCheck = await new DataContext().GetResultDapperAsyncDynamic(DataBaseHostEnum.KPR, queryCheck);
                    decimal toolReal = (resultCheck as List<dynamic>)[0].COUN;
                    if (toolReal == 1)
                    {
                        var queryCheckLoc = $"SELECT COUNT(1) AS COUN FROM KPDBA.LOCATION_PTC WHERE LOC_ID = '{model.locID}' AND WAREHOUSE_ID = '{model.warehouseID}'";
                        var resultCheckLoc = await new DataContext().GetResultDapperAsyncDynamic(DataBaseHostEnum.KPR, queryCheckLoc);
                        decimal LocValid = (resultCheckLoc as List<dynamic>)[0].COUN;
                        if (LocValid == 1)
                        {
                            var queryQTY = $"SELECT SUM(QTY) QTY FROM KPDBA.PTC_STOCK_DETAIL WHERE WAREHOUSE_ID = '{model.warehouseID}' AND PTC_ID = '{model.ptcID}'";
                            var resultQTY = await new DataContext().GetResultDapperAsyncObject(DataBaseHostEnum.KPR, queryQTY);
                            var countQty = (resultQTY as List<dynamic>)[0].QTY;
                            if (countQty == 0)
                            {
                                var query = $"SELECT SD.LOC_ID, LOC.LOC_DETAIL, SD.QTY FROM (SELECT SD.WAREHOUSE_ID, SD.LOC_ID, SUM (SD.QTY) QTY FROM KPDBA.PTC_STOCK_DETAIL SD WHERE SD.WAREHOUSE_ID = '{model.warehouseID}' AND SD.PTC_ID = '{model.ptcID}' GROUP BY SD.WAREHOUSE_ID, SD.LOC_ID HAVING SUM (SD.QTY) < 0) SD JOIN (SELECT WAREHOUSE_ID, LOC_ID, LOC_DETAIL FROM KPDBA.LOCATION_PTC) LOC ON (SD.WAREHOUSE_ID = LOC.WAREHOUSE_ID AND SD.LOC_ID = LOC.LOC_ID)";
                                var resultt = await new DataContext().GetResultDapperAsyncObject(DataBaseHostEnum.KPR, query);
                                decimal count = (resultt as List<object>).Count;
                                if (count != 0)
                                {
                                    var results = _mapper.Map<IEnumerable<GetLoc>>(resultt);
                                    var dataLoc = results.ElementAt(0);
                                    var queryCompID = $"SELECT COMP_ID COMP FROM KPDBA.WAREHOUSE WHERE WAREHOUSE_ID ='{model.warehouseID}'";
                                    var resultCompID = await new DataContext().GetResultDapperAsyncObject(DataBaseHostEnum.KPR, queryCompID);
                                    var compID = (resultCompID as List<dynamic>)[0].COMP;
                                    var tranSEQ = 1;
                                    var QTY = "1";
                                    var S_STATUS = 'T';
                                    var tranType = "3"; // โอนย้ายออก
                                    var locID = dataLoc.LOC_ID; // old loc
                                    string tran_id = await new StoreConnectionMethod(_mapper).PtcGetTranID(compID: model.warehouseID, tranType: compID);

                                    var tranDate = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss", new CultureInfo("en-US"));
                                    var insertOutQuery = $"INSERT INTO KPDBA.PTC_STOCK_DETAIL (TRAN_ID, TRAN_SEQ, TRAN_TYPE, TRAN_DATE,PTC_ID, QTY, COMP_ID, WAREHOUSE_ID, LOC_ID, STATUS, CR_DATE, CR_ORG_ID, CR_USER_ID) VALUES ('{tran_id}', TO_NUMBER('{tranSEQ}'), TO_NUMBER('{tranType}'), TO_DATE('{tranDate}', 'dd/mm/yyyy hh24:mi:ss'),'{model.ptcID}', TO_NUMBER('{QTY}'), TO_CHAR('{compID}'),'{model.warehouseID}','{locID}', TO_CHAR('{S_STATUS}'), SYSDATE, '{userProfile.org}', '{userProfile.userID}')";
                                    var resultOutInsert = await new DataContext().InsertResultDapperAsync(DataBaseHostEnum.KPR, insertOutQuery);

                                    string DateNow = DateTime.Today.ToString("dd/MM/yyyy", new CultureInfo("en-US"));
                                    var querys = $"UPDATE KPDBA.PTC_JS_PLAN_DETAIL SET RETURN_DATE = TO_DATE ('{DateNow}', 'dd/mm/yyyy'), RETURN_USER_ID = TO_CHAR ('{userProfile.userID}') WHERE JOB_ID = '{model.jobID}'";
                                    var upDateResult = await new DataContext().GetResultDapperAsyncDynamic(DataBaseHostEnum.KPR, querys);

                                }
                                else
                                {
                                    _returnFlag = "1";
                                    _returnText = "ไม่พบอุปกรณ์ในประวัติการเบิก";
                                }
                            }
                            else if (countQty == null)
                            {
                                var queryCompID = $"SELECT COMP_ID COMP FROM KPDBA.WAREHOUSE WHERE WAREHOUSE_ID ='{model.warehouseID}'";
                                var resultCompID = await new DataContext().GetResultDapperAsyncObject(DataBaseHostEnum.KPR, queryCompID);
                                var compID = (resultCompID as List<dynamic>)[0].COMP;
                                var tranSEQ = 1;
                                var QTY = "1";
                                var S_STATUS = 'T';
                                var tranType = "12"; // New tooling
                                var locID = "$R70"; // old loc
                                string tran_id = await new StoreConnectionMethod(_mapper).PtcGetTranID(compID: model.warehouseID, tranType: compID);

                                var tranDate = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss", new CultureInfo("en-US"));
                                var insertOutQuery = $"INSERT INTO KPDBA.PTC_STOCK_DETAIL (TRAN_ID, TRAN_SEQ, TRAN_TYPE, TRAN_DATE,PTC_ID, QTY, COMP_ID, WAREHOUSE_ID, LOC_ID, STATUS, CR_DATE, CR_ORG_ID, CR_USER_ID) VALUES ('{tran_id}', TO_NUMBER('{tranSEQ}'), TO_NUMBER('{tranType}'), TO_DATE('{tranDate}', 'dd/mm/yyyy hh24:mi:ss'),'{model.ptcID}', TO_NUMBER('{QTY}'), TO_CHAR('{compID}'),'{model.warehouseID}','{locID}', TO_CHAR('{S_STATUS}'), SYSDATE, '{userProfile.org}', '{userProfile.userID}')";
                                var resultOutInsert = await new DataContext().InsertResultDapperAsync(DataBaseHostEnum.KPR, insertOutQuery);
                            }
                            else
                            {
                                _returnFlag = "1";
                                _returnText = "อุปกรณ์ยังไม่ถูกเบิกออกจากคลัง";
                            }
                        }
                        else
                        {
                            _returnFlag = "1";
                            //ไม่พบหมายเลข Location นี้ในฐานข้อมูล
                            _returnText = "ไม่พบรหัสพื้นที่นี้ภายในคลัง";
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

            // if (model.token != null)
            // {
            //     UserProfile userProfile = _jwtGenerator.DecodeToken(model.token);
            //     result = await new PTCMethods(_mapper, _jwtGenerator).MoveTooling(model, "4");
            //     if (result != null)
            //     {
            //         if (result.flag == "0")
            //         {
            //             string DateNow = DateTime.Today.ToString("dd/MM/yyyy", new CultureInfo("en-US"));
            //             var query = $"UPDATE KPDBA.PTC_JS_PLAN_DETAIL SET RETURN_DATE = TO_DATE ('{DateNow}', 'dd/mm/yyyy'), RETURN_USER_ID = TO_CHAR ('{userProfile.userID}') WHERE JOB_ID = '{model.jobID}'";
            //             var upDateResult = await new DataContext().GetResultDapperAsyncDynamic(DataBaseHostEnum.KPR, query);
            //         }
            //         else
            //         {
            //             return result;
            //         }
            //     }
            // }
            // else
            // {
            //     result = new ReturnDataMoveLoc
            //     {
            //         flag = "1",
            //         text = "ไม่พบข้อมูลผู้ใช้ กรุณา Login อีกครั้ง หรือติดต่อฝ่าย IT "
            //     };
            // }
            var resultts = new ReturnDataMoveLoc
            {
                flag = _returnFlag,
                text = _returnText
            };

            return resultts;
        }
    }
}