using System;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using PTCwebApi.DataService;
using PTCwebApi.Interfaces;
using PTCwebApi.Models.ProfilesModels;
using PTCwebApi.Models.PTCModels;
using PTCwebApi.Models.PTCModels.MethodModels;
using PTCwebApi.Models.PTCModels.MethodModels.FindTools;
using PTCwebApi.Models.PTCModels.MethodModels.MoveLoc;

namespace PTCwebApi.Methods.PTCMethods {
    public class PTCMethods {
        private readonly IJwtGenerator _jwtGenerator;
        private readonly IMapper _mapper;
        public PTCMethods (IMapper mapper, IJwtGenerator jwtGenerator) {
            _jwtGenerator = jwtGenerator;
            _mapper = mapper;
        }

        //*Find Tool
        public async Task<ReturnDataTool> FindTooling (RequestAllModelsPTC model) {
            string _returnFlag = "0";
            string _returnText = "ผ่าน";
            GetLoc _dataLoc = new GetLoc ();

            //* check A tool is real in warehourse.
            var queryCheck = $"SELECT COUNT(1) AS COUN FROM KPDBA.DIECUT_SN WHERE DIECUT_SN ='{model.PTC_ID}'";
            var resultCheck = await new DataContext ().GetResultDapperAsyncDynamic (DataBaseHostEnum.KPR, queryCheck);
            decimal toolValid = (resultCheck as List<dynamic>) [0].COUN;

            if (toolValid == 1) {

                //* check location of the tool.
                model.WAREHOUSE_ID = "7";
                var query = $"SELECT SD.LOC_ID, LOC.LOC_DETAIL, SD.QTY FROM (SELECT SD.WAREHOUSE_ID, SD.LOC_ID, SUM (SD.QTY) QTY FROM KPDBA.PTC_STOCK_DETAIL SD WHERE SD.WAREHOUSE_ID = '{model.WAREHOUSE_ID}' AND SD.PTC_ID = '{model.PTC_ID}' GROUP BY SD.WAREHOUSE_ID, SD.LOC_ID HAVING SUM (SD.QTY) > 0) SD JOIN (SELECT WAREHOUSE_ID, LOC_ID, LOC_DETAIL FROM KPDBA.LOCATION_PTC WHERE RECEIVE_LOC_FLAG = 'T') LOC ON (SD.WAREHOUSE_ID = LOC.WAREHOUSE_ID AND SD.LOC_ID = LOC.LOC_ID)";
                var result = await new DataContext ().GetResultDapperAsyncObject (DataBaseHostEnum.KPR, query);
                decimal count = (result as List<object>).Count;
                if (count == 0) {
                    _dataLoc.LOC_ID = null;
                    _dataLoc.LOC_DETAIL = null;
                    _returnFlag = "1";
                    _returnText = "\"error: ไม่มีอุปกรณ์อุปกรณ์อยู่ภายในคลัง\"";
                } else {
                    var results = _mapper.Map<IEnumerable<GetLoc>> (result);
                    _dataLoc = results.ElementAt (0);
                }
            } else {
                _returnFlag = "1";
                _returnText = "\"error: ไม่พบหมายเลขของอุปกรณ์นี้ในฐานข้อมูล\"";
            }
            var returnResult = new ReturnDataTool {
                LOC_ID = _dataLoc.LOC_ID,
                LOC_NAME = _dataLoc.LOC_DETAIL,
                FLAG = _returnFlag,
                TEXT = _returnText
            };
            return returnResult;
        }

        //* Move Tool
        public async Task<ReturnDataMoveLoc> MoveTooling (RequestAllModelsPTC model) {
            string _returnFlag = "0";
            string _returnText = "ผ่าน";

            var queryCheck = $"SELECT COUNT(1) AS COUN FROM KPDBA.DIECUT_SN WHERE DIECUT_SN ='{model.PTC_ID}'";
            var resultCheck = await new DataContext ().GetResultDapperAsyncDynamic (DataBaseHostEnum.KPR, queryCheck);
            decimal toolReal = (resultCheck as List<dynamic>) [0].COUN;

            if (toolReal == 1) {

                if (model.TOKEN != null) {
                    UserProfile userProfile = _jwtGenerator.DecodeToken (model.TOKEN);
                    if (userProfile != null) {
                        var query = $"SELECT * FROM KPDBA.PTC_STOCK_DETAIL WHERE PTC_ID ='{model.PTC_ID}'";
                        var result = await new DataContext ().GetResultDapperAsyncObject (DataBaseHostEnum.KPR, query);
                        var results = _mapper.Map<IEnumerable<ModelTablePTCDetail>> (result);
                        ModelTablePTCDetail getResult = results.ElementAt (0);


                        if (model.LOC_ID != getResult.LOC_ID) {
                            _returnText = "\"error: Location ถูกเปลี่ยนแปลง\"";
                        }
                        string tran_id = await new StoreConnectionMethod (_mapper).PtcGetTranID (getResult);

                        getResult.WAREHOUSE_ID = "7";
                        getResult.TRAN_SEQ = 1;
                        getResult.TRAN_TYPE = "4";
                        getResult.TRAN_DATE = DateTime.Now.ToString ("dd/MM/yyyy HH:mm:ss", new CultureInfo ("en-US"));
                        var insertGetQuery = $"INSERT INTO KPDBA.PTC_STOCK_DETAIL (TRAN_ID, TRAN_SEQ, TRAN_TYPE, TRAN_DATE,PTC_ID, QTY, COMP_ID, WAREHOUSE_ID, LOC_ID, STATUS, CR_DATE, CR_ORG_ID, CR_USER_ID) VALUES ('{tran_id}', TO_NUMBER('{getResult.TRAN_SEQ}'), TO_NUMBER('{getResult.TRAN_TYPE}'), TO_DATE('{getResult.TRAN_DATE}', 'dd/mm/yyyy hh24:mi:ss'),'{getResult.PTC_ID}', TO_NUMBER('-1'), TO_CHAR('{getResult.COMP_ID}'),'{getResult.WAREHOUSE_ID}','{getResult.LOC_ID}', 'T', SYSDATE, '{userProfile.org}', '{userProfile.userID}')";
                        var resultInsert = await new DataContext ().InsertResultDapperAsync (DataBaseHostEnum.KPR, insertGetQuery);

                        getResult.WAREHOUSE_ID = "7";
                        getResult.TRAN_SEQ = 2;
                        getResult.LOC_ID = model.SUGG_LOG_ID;
                        getResult.TRAN_DATE = DateTime.Now.ToString ("dd/MM/yyyy HH:mm:ss", new CultureInfo ("en-US"));
                        var insertOutQuery = $"INSERT INTO KPDBA.PTC_STOCK_DETAIL (TRAN_ID, TRAN_SEQ, TRAN_TYPE, TRAN_DATE,PTC_ID, QTY, COMP_ID, WAREHOUSE_ID, LOC_ID, STATUS, CR_DATE, CR_ORG_ID, CR_USER_ID) VALUES ('{tran_id}', TO_NUMBER('{getResult.TRAN_SEQ}'), TO_NUMBER('{getResult.TRAN_TYPE}'), TO_DATE('{getResult.TRAN_DATE}', 'dd/mm/yyyy hh24:mi:ss'),'{getResult.PTC_ID}', TO_NUMBER('1'), TO_CHAR('{getResult.COMP_ID}'),'{getResult.WAREHOUSE_ID}','{getResult.LOC_ID}', 'T', SYSDATE, '{userProfile.org}', '{userProfile.userID}')";
                        var resultOutInsert = await new DataContext ().InsertResultDapperAsync (DataBaseHostEnum.KPR, insertOutQuery);
                    } else {
                        _returnFlag = "1";
                        _returnText = "\"error: ข้อมูล Token ผิดพลาด กรุณาติดต่อฝ่ายIT \"";
                    }
                } else {
                    _returnFlag = "1";
                    _returnText = "\"error: ระบบไม่ได้รับ Token กรุณา Login ใหม่อีกครั้งหรือ ติดต่อแผนก IT\"";
                }
            } else {
                _returnFlag = "1";
                _returnText = "\"error: อุปกรณ์นี้ไม่มีอยู่ในฐานข้อมูล\"";
            }

            //*Prepare data to return to Frontend
            var retuenResult = new ReturnDataMoveLoc {
                FLAG = _returnFlag,
                TEXT = _returnText
            };
            return retuenResult;
        }

        //* Check Warehouse & Warehouse User
        public async Task<object> checkWareHouse (RequestAllModelsPTC model) {
            string _returnFlag = "1";
            string _returnText = "error: คุณไม่มีสิทธิ์ในการใช้งานนี้";
            Object _resultWarehouse = "";
            Object _userTool = "";

            if (model.TOKEN != null) {
                UserProfile userProfile = _jwtGenerator.DecodeToken (model.TOKEN);
                var _userWarehouse = await new StoreConnectionMethod (_mapper).PtcGetWareHouse (userProfile.aduserID);
                _resultWarehouse = _mapper.Map<IEnumerable<UserWareHouseID>> (_userWarehouse).ToList ();
                decimal count = (_userWarehouse as List<dynamic>).Count;

                switch (count) {
                    case 1:
                        _returnFlag = "0";
                        _returnText = "ที่เดียว";
                        break;
                    case 2:
                        _returnFlag = "2";
                        _returnText = "หลายที่";
                        break;
                    default:
                        break;
                }
            } else {
                _returnText = "\"error: ระบบไม่ได้รับ Token กรุณา Login ใหม่อีกครั้งหรือ ติดต่อแผนก IT\"";
            }

            var returnResult = new CheckWareHouseUser {
                WAREHOUSE_LIST = _resultWarehouse,
                FLAG = _returnFlag,
                TEXT = _returnText
            };
            return returnResult;
        }

        //* Get current plans
        public async Task<object> GetCurrentPlans (RequestAllModelsPTC model) {
            string _returnFlag = "0";
            string _returnText = "ผ่าน";
            Object _resultCurrentPlans = "";

            if (model.TOKEN != null) {
                UserProfile userProfile = _jwtGenerator.DecodeToken (model.TOKEN);

                var userTool = await new StoreConnectionMethod (_mapper).PtcGetWareHouseTool (userProfile.aduserID);
                var today = DateTime.Today;
                string yesterday = today.AddDays (-1).ToString ("dd/MM/yyyy", new CultureInfo ("en-US"));
                string endDay = today.AddDays (3).ToString ("dd/MM/yyyy", new CultureInfo ("en-US"));

                int count = (userTool as List<dynamic>).Count;
                switch (count) {
                    case 1:
                        string toolType = (userTool as List<dynamic>) [0].TOOLING;
                        _resultCurrentPlans = await new StoreConnectionMethod (_mapper).PtcGetCurrentPlans (toolType: toolType, startDay: yesterday, endDay: endDay);
                        break;
                    case 2:
                        var allCurrentPlans = await new StoreConnectionMethod (_mapper).PtcGetCurrentPlans (toolType: "%", startDay : yesterday, endDay : endDay);
                        var DataCurrentPlans = allCurrentPlans.ToList ();

                        //TODO convert List<dynamic> to dataTable
                        var planTable = ToDatable.ToDataTable (DataCurrentPlans, "CurrentPlans");
                        // DataTable a = ToDataTable (DataCurrentPlans, "CurrentPlans");
                        for (int i = 0; i < (count + 1); i++) { };
                        break;
                    default:
                        _resultCurrentPlans = await new StoreConnectionMethod (_mapper).PtcGetCurrentPlans (toolType: "%", startDay : yesterday, endDay : endDay);
                        break;
                }
            } else {
                _returnText = "\"error: ระบบไม่ได้รับ Token กรุณา Login ใหม่อีกครั้งหรือ ติดต่อแผนก IT\"";
            }

            var returnResult = new ReturnCurrentPlans {
                FLAG = _returnFlag,
                TEXT = _returnText
            };

            return returnResult;
        }

        //* Pick up the tool
        public async Task<object> PickUpTooling (RequestAllModelsPTC model) {
            string _returnFlag = "0";
            string _returnText = "ผ่าน";
            GetLoc _dataLoc = new GetLoc ();

            if (model.TOKEN != null) {
                UserProfile userProfile = _jwtGenerator.DecodeToken (model.TOKEN);
                var queryCheck = $"SELECT COUNT(1) AS COUN FROM KPDBA.DIECUT_SN WHERE DIECUT_SN ='{model.PTC_ID}'";
                var resultCheck = await new DataContext ().GetResultDapperAsyncDynamic (DataBaseHostEnum.KPR, queryCheck);
                decimal toolValid = (resultCheck as List<dynamic>) [0].COUN;

                if (toolValid == 1) {
                    var query = $"SELECT SD.LOC_ID, LOC.LOC_DETAIL, SD.QTY FROM (SELECT SD.WAREHOUSE_ID, SD.LOC_ID, SUM (SD.QTY) QTY FROM KPDBA.PTC_STOCK_DETAIL SD WHERE SD.WAREHOUSE_ID = '{model.WAREHOUSE_ID}' AND SD.PTC_ID = '{model.PTC_ID}' GROUP BY SD.WAREHOUSE_ID, SD.LOC_ID HAVING SUM (SD.QTY) > 0) SD JOIN (SELECT WAREHOUSE_ID, LOC_ID, LOC_DETAIL FROM KPDBA.LOCATION_PTC WHERE WITHD_LOC_FLAG = 'T') LOC ON (SD.WAREHOUSE_ID = LOC.WAREHOUSE_ID AND SD.LOC_ID = LOC.LOC_ID)";
                    var result = await new DataContext ().GetResultDapperAsyncObject (DataBaseHostEnum.KPR, query);
                    decimal count = (result as List<object>).Count;
                    if (count == 0) {
                        //TODO --> Next

                        var queryCheckToolDetail = $"SELECT * FROM KPDBA.PTC_STOCK_DETAIL WHERE PTC_ID ='{model.PTC_ID}' AND TRAN_SEQ = 2 ORDER BY TRAN_ID DESC";
                        var resultToolDetail = await new DataContext ().GetResultDapperAsyncObject (DataBaseHostEnum.KPR, queryCheckToolDetail);
                        var results = _mapper.Map<IEnumerable<ModelTablePTCDetail>> (resultToolDetail);
                        ModelTablePTCDetail getResult = results.ElementAt (0);

                        string tran_id = await new StoreConnectionMethod (_mapper).PtcGetTranID (getResult);

                        getResult.WAREHOUSE_ID = model.WAREHOUSE_ID;
                        getResult.TRAN_SEQ = 1;
                        getResult.TRAN_TYPE = "4";
                        getResult.TRAN_DATE = DateTime.Now.ToString ("dd/MM/yyyy HH:mm:ss", new CultureInfo ("en-US"));
                        var insertGetQuery = $"INSERT INTO KPDBA.PTC_STOCK_DETAIL (TRAN_ID, TRAN_SEQ, TRAN_TYPE, TRAN_DATE,PTC_ID, QTY, COMP_ID, WAREHOUSE_ID, LOC_ID, STATUS, CR_DATE, CR_ORG_ID, CR_USER_ID) VALUES ('{tran_id}', TO_NUMBER('{getResult.TRAN_SEQ}'), TO_NUMBER('{getResult.TRAN_TYPE}'), TO_DATE('{getResult.TRAN_DATE}', 'dd/mm/yyyy hh24:mi:ss'),'{getResult.PTC_ID}', TO_NUMBER('-1'), TO_CHAR('{getResult.COMP_ID}'),'{getResult.WAREHOUSE_ID}','{getResult.LOC_ID}', 'T', SYSDATE, '{userProfile.org}', '{userProfile.userID}')";
                        var resultInsert = await new DataContext ().InsertResultDapperAsync (DataBaseHostEnum.KPR, insertGetQuery);

                        getResult.WAREHOUSE_ID = model.WAREHOUSE_ID;
                        getResult.TRAN_SEQ = 2;
                        getResult.LOC_ID = "$W70";
                        getResult.TRAN_DATE = DateTime.Now.ToString ("dd/MM/yyyy HH:mm:ss", new CultureInfo ("en-US"));
                        var insertOutQuery = $"INSERT INTO KPDBA.PTC_STOCK_DETAIL (TRAN_ID, TRAN_SEQ, TRAN_TYPE, TRAN_DATE,PTC_ID, QTY, COMP_ID, WAREHOUSE_ID, LOC_ID, STATUS, CR_DATE, CR_ORG_ID, CR_USER_ID) VALUES ('{tran_id}', TO_NUMBER('{getResult.TRAN_SEQ}'), TO_NUMBER('{getResult.TRAN_TYPE}'), TO_DATE('{getResult.TRAN_DATE}', 'dd/mm/yyyy hh24:mi:ss'),'{getResult.PTC_ID}', TO_NUMBER('1'), TO_CHAR('{getResult.COMP_ID}'),'{getResult.WAREHOUSE_ID}','{getResult.LOC_ID}', 'T', SYSDATE, '{userProfile.org}', '{userProfile.userID}')";
                        var resultOutInsert = await new DataContext ().InsertResultDapperAsync (DataBaseHostEnum.KPR, insertOutQuery);
                    } else {
                        _returnFlag = "1";
                        _returnText = "\"error: อุปกรณ์นี้อยู่ในพื้นที่เบิกแล้ว\"";
                    }
                } else {
                    //* check QR code is Loccation right?
                    var queryCheckLoc = $"SELECT COUNT(1) AS COUN FROM KPDBA.LOCATION_PTC WHERE LOC_ID = '{model.PTC_ID}'";
                    var resultCheckLoc = await new DataContext ().GetResultDapperAsyncDynamic (DataBaseHostEnum.KPR, queryCheckLoc);
                    decimal LocValid = (resultCheckLoc as List<dynamic>) [0].COUN;
                    if (LocValid == 1) {
                        _returnFlag = "0";
                        _returnText = "\"error: QR code นี้คือ Location\"";
                    } else {
                        _returnFlag = "1";
                        _returnText = "\"error: ไม่พบหมายเลขของอุปกรณ์นี้ในฐานข้อมูล\"";
                    }
                }
            } else {
                _returnText = "\"error: ระบบไม่ได้รับ Token กรุณา Login ใหม่อีกครั้งหรือ ติดต่อแผนก IT\"";
            }
            var returnResult = new ReturnDataMoveLoc {
                FLAG = _returnFlag,
                TEXT = _returnText
            };
            return returnResult;
        }

        //! blureprint
        //  public async Task<object> founctionName (RequestAllModelsPTC model) {
        //     string _returnFlag = "0";
        //     string _returnText = "ผ่าน";
        //     if (model.TOKEN != null) {
        //         UserProfile userProfile = _jwtGenerator.DecodeToken (model.TOKEN);
        //     } else {
        //         _returnText = "\"error: ระบบไม่ได้รับ Token กรุณา Login ใหม่อีกครั้งหรือ ติดต่อแผนก IT\"";
        //     }
        //     var returnResult = new RrturnModel {
        //         FLAG = _returnFlag,
        //         TEXT = _returnText
        //     };
        //     return returnResult;
        // }
    }
}