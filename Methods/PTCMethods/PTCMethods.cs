using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using PTCwebApi.DataService;
using PTCwebApi.Interfaces;
using PTCwebApi.Models.ProfilesModels;
using PTCwebApi.Models.PTCModels;
using PTCwebApi.Models.PTCModels.MethodModels;
using PTCwebApi.Models.PTCModels.MethodModels.CurrentPlans;
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
         //*Find  Lastest location of Tool
        public async Task<ReturnDataTool> FindLocOfTooling (RequestAllModelsPTC model) {
            string _returnFlag = "0";
            string _returnText = "ผ่าน";
            GetLoc _dataLoc = new GetLoc ();

            //* check A tool is real in warehourse.
            var queryCheck = $"SELECT COUNT(1) AS COUN FROM KPDBA.DIECUT_SN WHERE DIECUT_SN ='{model.ptcID}'";
            var resultCheck = await new DataContext ().GetResultDapperAsyncDynamic (DataBaseHostEnum.KPR, queryCheck);
            decimal toolValid = (resultCheck as List<dynamic>) [0].COUN;

            if (toolValid == 1) {

                //* check location of the tool.
                var query = $"SELECT SD.LOC_ID, LOC.LOC_DETAIL, SD.QTY FROM (SELECT SD.WAREHOUSE_ID, SD.LOC_ID, SUM (SD.QTY) QTY FROM KPDBA.PTC_STOCK_DETAIL SD WHERE SD.WAREHOUSE_ID = '{model.warehouseID}' AND SD.PTC_ID = '{model.ptcID}' GROUP BY SD.WAREHOUSE_ID, SD.LOC_ID HAVING SUM (SD.QTY) > 0) SD JOIN (SELECT WAREHOUSE_ID, LOC_ID, LOC_DETAIL FROM KPDBA.LOCATION_PTC) LOC ON (SD.WAREHOUSE_ID = LOC.WAREHOUSE_ID AND SD.LOC_ID = LOC.LOC_ID)";
                var result = await new DataContext ().GetResultDapperAsyncObject (DataBaseHostEnum.KPR, query);
                decimal count = (result as List<object>).Count;
                if (count == 0) {
                    _returnFlag = "1";
                    _returnText = "\"error: อุปกรณ์หมด ไม่มีอุปกรณ์คงเหลือภายในคลัง\"";
                } else {
                    //มี
                    var results = _mapper.Map<IEnumerable<GetLoc>> (result);
                    _dataLoc = results.ElementAt (0);
                }
            } else {
                _returnFlag = "1";
                _returnText = "\"error: ไม่พบหมายเลขของอุปกรณ์นี้ในฐานข้อมูล\"";
            }
            var returnResult = new ReturnDataTool {
                locID = _dataLoc.LOC_ID,
                locName = _dataLoc.LOC_DETAIL,
                flag = _returnFlag,
                text = _returnText
            };
            return returnResult;
        }
        //*Find suggess location of Tool
        public async Task<ReturnDataTool> FindSuggLocOfTooling (RequestAllModelsPTC model) {
            string _returnFlag = "1";
            string _returnText = "\"error: ไม่มีพื้นที่แนะนำ สำหรับจัดเก็บอุปกรณ์นี้\"";
            GetLoc _dataLoc = new GetLoc ();

            //* check A tool is real in warehourse.
            var queryCheck = $"SELECT COUNT(1) AS COUN FROM KPDBA.DIECUT_SN WHERE DIECUT_SN ='{model.ptcID}'";
            var resultCheck = await new DataContext ().GetResultDapperAsyncDynamic (DataBaseHostEnum.KPR, queryCheck);
            decimal toolValid = (resultCheck as List<dynamic>) [0].COUN;

            if (toolValid == 1) {

                //* check location of the tool.
                var query = $"SELECT SD.LOC_ID, LOC.LOC_DETAIL, SD.QTY FROM (SELECT SD.WAREHOUSE_ID, SD.LOC_ID, SUM (SD.QTY) QTY FROM KPDBA.PTC_STOCK_DETAIL SD WHERE SD.WAREHOUSE_ID = '{model.warehouseID}' AND SD.PTC_ID = '{model.ptcID}' GROUP BY SD.WAREHOUSE_ID, SD.LOC_ID HAVING SUM (SD.QTY) > 0) SD JOIN (SELECT WAREHOUSE_ID, LOC_ID, LOC_DETAIL FROM KPDBA.LOCATION_PTC WHERE RECEIVE_LOC_FLAG = 'T') LOC ON (SD.WAREHOUSE_ID = LOC.WAREHOUSE_ID AND SD.LOC_ID = LOC.LOC_ID)";
                var result = await new DataContext ().GetResultDapperAsyncObject (DataBaseHostEnum.KPR, query);
                decimal count = (result as List<object>).Count;
                if (count == 0) {
                    _dataLoc.LOC_ID = "$R70";
                    _dataLoc.LOC_DETAIL = "พื้นที่รอจัดเก็บ";
                    _returnFlag = "0";
                    _returnText = "ผ่าน";
                } else {
                    //มี
                    var results = _mapper.Map<IEnumerable<GetLoc>> (result);
                    _dataLoc = results.ElementAt (0);
                }
            } else {
                _returnFlag = "1";
                _returnText = "\"error: ไม่พบหมายเลขของอุปกรณ์นี้ในฐานข้อมูล\"";
            }
            var returnResult = new ReturnDataTool {
                locID = _dataLoc.LOC_ID,
                locName = _dataLoc.LOC_DETAIL,
                flag = _returnFlag,
                text = _returnText
            };
            return returnResult;
        }

        //* Move Tool
        public async Task<ReturnDataMoveLoc> MoveTooling (RequestAllModelsPTC model) {
            string _returnFlag = "0";
            string _returnText = "ผ่าน";
            if (model.token != null) {
                UserProfile userProfile = _jwtGenerator.DecodeToken (model.token);
                if (userProfile != null) {
                    var queryCheck = $"SELECT COUNT(1) AS COUN FROM KPDBA.DIECUT_SN WHERE DIECUT_SN ='{model.ptcID}'";
                    var resultCheck = await new DataContext ().GetResultDapperAsyncDynamic (DataBaseHostEnum.KPR, queryCheck);
                    decimal toolReal = (resultCheck as List<dynamic>) [0].COUN;
                    if (toolReal == 1) {
                        var query = $"SELECT SD.LOC_ID, LOC.LOC_DETAIL, SD.QTY FROM (SELECT SD.WAREHOUSE_ID, SD.LOC_ID, SUM (SD.QTY) QTY FROM KPDBA.PTC_STOCK_DETAIL SD WHERE SD.WAREHOUSE_ID = '{model.warehouseID}' AND SD.PTC_ID = '{model.ptcID}' GROUP BY SD.WAREHOUSE_ID, SD.LOC_ID HAVING SUM (SD.QTY) > 0) SD JOIN (SELECT WAREHOUSE_ID, LOC_ID, LOC_DETAIL FROM KPDBA.LOCATION_PTC) LOC ON (SD.WAREHOUSE_ID = LOC.WAREHOUSE_ID AND SD.LOC_ID = LOC.LOC_ID)";
                        var result = await new DataContext ().GetResultDapperAsyncObject (DataBaseHostEnum.KPR, query);
                        decimal count = (result as List<object>).Count;
                        if (count == 0) {
                            _returnFlag = "1";
                            _returnText = "\"error: อุปกรณ์หมด ไม่มีอุปกรณ์คงเหลือภายในคลัง\"";
                        } else {
                            var results = _mapper.Map<IEnumerable<GetLoc>> (result);
                            var dataLoc = results.ElementAt (0);
                            var queryCompID = $"SELECT COMP_ID COMP FROM KPDBA.WAREHOUSE WHERE WAREHOUSE_ID ='{model.warehouseID}'";
                            var resultCompID = await new DataContext ().GetResultDapperAsyncObject (DataBaseHostEnum.KPR, queryCompID);
                            var compID = (resultCompID as List<dynamic>) [0].COMP;
                            var tranSEQ = 1;
                            var QTY = "-1";
                            var S_STATUS = 'T';
                            var tranType = "4"; // โอนย้ายเข้า
                            var locID = dataLoc.LOC_ID; // old loc
                            string tran_id = await new StoreConnectionMethod (_mapper).PtcGetTranID (compID: model.warehouseID, tranType: compID);

                            var tranDate = DateTime.Now.ToString ("dd/MM/yyyy HH:mm:ss", new CultureInfo ("en-US"));
                            var insertGetQuery = $"INSERT INTO KPDBA.PTC_STOCK_DETAIL (TRAN_ID, TRAN_SEQ, TRAN_TYPE, TRAN_DATE,PTC_ID, QTY, COMP_ID, WAREHOUSE_ID, LOC_ID, STATUS, CR_DATE, CR_ORG_ID, CR_USER_ID) VALUES ('{tran_id}', TO_NUMBER('{tranSEQ}'), TO_NUMBER('{tranType}'), TO_DATE('{tranDate}', 'dd/mm/yyyy hh24:mi:ss'),'{model.ptcID}', TO_NUMBER('{QTY}'), TO_CHAR('{compID}'),'{model.warehouseID}','{locID}', TO_CHAR('{S_STATUS}'), SYSDATE, '{userProfile.org}', '{userProfile.userID}')";
                            var resultInsert = await new DataContext ().InsertResultDapperAsync (DataBaseHostEnum.KPR, insertGetQuery);

                            tranSEQ = 2;
                            QTY = "1";
                            tranType = "4"; // โอนย้ายเข้า
                            locID = model.locID; // newLoc

                            tranDate = DateTime.Now.ToString ("dd/MM/yyyy HH:mm:ss", new CultureInfo ("en-US"));
                            var insertOutQuery = $"INSERT INTO KPDBA.PTC_STOCK_DETAIL (TRAN_ID, TRAN_SEQ, TRAN_TYPE, TRAN_DATE,PTC_ID, QTY, COMP_ID, WAREHOUSE_ID, LOC_ID, STATUS, CR_DATE, CR_ORG_ID, CR_USER_ID) VALUES ('{tran_id}', TO_NUMBER('{tranSEQ}'), TO_NUMBER('{tranType}'), TO_DATE('{tranDate}', 'dd/mm/yyyy hh24:mi:ss'),'{model.ptcID}', TO_NUMBER('{QTY}'), TO_CHAR('{compID}'),'{model.warehouseID}','{locID}', TO_CHAR('{S_STATUS}'), SYSDATE, '{userProfile.org}', '{userProfile.userID}')";
                            var resultOutInsert = await new DataContext ().InsertResultDapperAsync (DataBaseHostEnum.KPR, insertOutQuery);
                        }
                    } else {
                        _returnFlag = "1";
                        _returnText = "\"error: ไม่พบหมายเลขของอุปกรณ์นี้ในฐานข้อมูล\"";
                    }
                } else {
                    _returnFlag = "1";
                    _returnText = "\"error: ข้อมูล Token ผิดพลาด กรุณาติดต่อฝ่ายIT \"";
                }
            } else {
                _returnFlag = "1";
                _returnText = "\"error: ระบบไม่ได้รับ Token กรุณา Login ใหม่อีกครั้งหรือ ติดต่อแผนก IT\"";
            }
            var retuenResult = new ReturnDataMoveLoc {
                flag = _returnFlag,
                text = _returnText
            };
            return retuenResult;
        }
        // private async void InsertTostockDetail () {

        // }

        //* Check Warehouse & Warehouse User
        public async Task<object> checkWareHouse (RequestAllModelsPTC model) {
            string _returnFlag = "1";
            string _returnText = "error: คุณไม่มีสิทธิ์ในการใช้งานนี้";
            List<WarehouseList> _resultWarehouse = null;
            Object _userTool = "";

            if (model.token != null) {
                UserProfile userProfile = _jwtGenerator.DecodeToken (model.token);
                var _userWarehouse = await new StoreConnectionMethod (_mapper).PtcGetWareHouse (userProfile.aduserID);
                var wareHouse = _mapper.Map<IEnumerable<UserWareHouseID>> (_userWarehouse);
                _resultWarehouse = _mapper.Map<IEnumerable<UserWareHouseID>, IEnumerable<WarehouseList>> (wareHouse).ToList ();
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
                warehouseList = _resultWarehouse,
                flag = _returnFlag,
                text = _returnText
            };
            return returnResult;
        }
    }
}