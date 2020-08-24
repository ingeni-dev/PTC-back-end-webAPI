using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PTCwebApi.Interfaces;
using PTCwebApi.Methods;
using PTCwebApi.Models.ProfilesModels;
using PTCwebApi.Models.PTCModels;
using PTCwebApi.Models.PTCModels.Entities;
using PTCwebApi.Models.PTCModels.MethodModels.FindTools;
using PTCwebApi.Models.PTCModels.MethodModels.MoveLoc;
using PTCwebApi.Models.RequestModels;
//using PTC-back-end-webAPI.Models;

namespace PTCwebApi.Controllers {
    [Route ("[controller]")]
    [ApiController]
    public class PtcController : ControllerBase {
        private readonly IJwtGenerator _jwtGenerator;
        private readonly IMapper _mapper;
        public PtcController (IMapper mapper, IJwtGenerator jwtGenerator) {
            _jwtGenerator = jwtGenerator;
            _mapper = mapper;
        }

        [Authorize]
        [HttpPost ("findtooling")]
        public async Task<ActionResult<ReturnDataTool>> FindTooling (SerialNumber model) {
            string _returnFlag = "0";
            string _returnText = "ผ่าน";
            GetLoc _dataLoc = new GetLoc ();

            //* check A tool is real in warehourse.
            var queryCheck = $"SELECT COUNT(1) AS COUN FROM KPDBA.DIECUT_SN WHERE DIECUT_SN ='{model.PTC_ID}'";
            var resultCheck = await new DataContext ().GetResultDapperAsyncDynamic (DataBaseHostEnum.KPR, queryCheck);
            decimal toolReal = (resultCheck as List<dynamic>) [0].COUN;

            if (toolReal == 1) {

                //* check location of the tool.
                var query = $@"SELECT KPDBA.PTC_STOCK_DETAIL.LOC_ID ,KPDBA.LOCATION_PTC.LOC_DETAIL
                        FROM KPDBA.PTC_STOCK_DETAIL 
                        INNER JOIN KPDBA.LOCATION_PTC 
                        ON KPDBA.PTC_STOCK_DETAIL.LOC_ID = KPDBA.LOCATION_PTC.LOC_ID
                        WHERE KPDBA.PTC_STOCK_DETAIL.PTC_ID='{model.PTC_ID}'";
                var result = await new DataContext ().GetResultDapperAsyncObject (DataBaseHostEnum.KPR, query);
                var results = _mapper.Map<IEnumerable<GetLoc>> (result);
                _dataLoc = results.ElementAt (0);
            } else {
                _returnFlag = "1";
                _returnText = "\"error\": \"อุปกรณ์ไม่มีอยู่ในฐานข้อมูล\"";
            }
            //* Prepare data to return to Frontend
            ReturnDataTool retuenResult = new ReturnDataTool {
                PTC_NAME = "ไม่ระบุ",
                LOC_ID = _dataLoc.LOC_ID,
                LOC_NAME = _dataLoc.LOC_DETAIL,
                FLAG = _returnFlag,
                TEXT = _returnText
            };
            return Ok (retuenResult);
        }

        [Authorize]
        [HttpPost ("movetolocation")]
        public async Task<ActionResult<ReturnDataMoveLoc>> MoveTooling (RequestDataMoveLoc model) {

            string _returnFlag = "0";
            string _returnText = "ผ่าน";

            //* check A tool is real in warehourse.
            var queryCheck = $"SELECT COUNT(1) AS COUN FROM KPDBA.DIECUT_SN WHERE DIECUT_SN ='{model.PTC_ID}'";
            var resultCheck = await new DataContext ().GetResultDapperAsyncDynamic (DataBaseHostEnum.KPR, queryCheck);
            decimal toolReal = (resultCheck as List<dynamic>) [0].COUN;

            if (toolReal == 1) {

                if (model.TOKEN != null) {

                    // *Get profile's user from JWT
                    UserProfile userProfile = _jwtGenerator.DecodeToken (model.TOKEN);

                    var query = $"SELECT * FROM KPDBA.PTC_STOCK_DETAIL WHERE PTC_ID ='{model.PTC_ID}'";
                    var result = await new DataContext ().GetResultDapperAsyncObject (DataBaseHostEnum.KPR, query);
                    var results = _mapper.Map<IEnumerable<ModelTablePTCDetail>> (result);
                    ModelTablePTCDetail getResult = results.ElementAt (0);

                    if (model.LOC_ID != getResult.LOC_ID) {
                        _returnText = "\"error\": \"Location ไม่ถูกต้อง\"";
                    }

                    //*Get tran ID from store
                    string tran_id = await new StoreConnectionMethod (_mapper).PtcGetTranID (getResult);

                    //*Insert data to Table
                    getResult.TRAN_SEQ = 1;
                    getResult.TRAN_TYPE = "3";
                    getResult.TRAN_DATE = DateTime.Now.ToString ("dd/MM/yyyy HH:mm:ss", new CultureInfo ("en-US"));
                    getResult.CR_DATE = DateTime.Now.ToString ("dd/MM/yyyy HH:mm:ss", new CultureInfo ("en-US"));
                    var insertGetQuery = $"INSERT INTO KPDBA.PTC_STOCK_DETAIL (TRAN_ID, TRAN_SEQ, TRAN_TYPE, TRAN_DATE,PTC_ID, QTY, COMP_ID, WAREHOUSE_ID, LOC_ID, STATUS, CR_DATE, CR_ORG_ID, CR_USER_ID) VALUES ('{tran_id}', TO_NUMBER('{getResult.TRAN_SEQ}'), TO_NUMBER('{getResult.TRAN_TYPE}'), TO_DATE('{getResult.TRAN_DATE}', 'dd/mm/yyyy hh24:mi:ss'),'{getResult.PTC_ID}', TO_NUMBER('{getResult.QTY-1}'), {getResult.COMP_ID},'{getResult.WAREHOUSE_ID}','{getResult.LOC_ID}', 'F',TO_DATE('{getResult.CR_DATE}', 'dd/mm/yyyy hh24:mi:ss') , '{userProfile.org}', '{userProfile.userID}')";
                    var resultInsert = await new DataContext ().InsertResultDapperAsync (DataBaseHostEnum.KPR, insertGetQuery);

                    //*Insert data to Table
                    // getResult.TRAN_SEQ = 2;
                    // var insertOutQuery = $@"INSERT INTO KPDBA.PTC_STOCK_DETAIL 
                    //                 (TRAN_ID, TRAN_SEQ, TRAN_TYPE, TRAN_DATE,PTC_ID, QTY, COMP_ID, WAREHOUSE_ID, LOC_ID, STATUS, CR_DATE, CR_ORG_ID, CR_USER_ID) 
                    //                 VALUES ({tran_id}, TO_NUMBER({getResult.TRAN_SEQ}), TO_NUMBER({getResult.TRAN_TYPE}), 
                    //                 TO_DATE({getResult.TRAN_DATE}, 'dd/mm/yyyy hh24:mi:ss'),{getResult.PTC_ID}, 
                    //                 TO_NUMBER('{getResult.QTY-1}'),  {getResult.COMP_ID},{getResult.WAREHOUSE_ID}, {getResult.LOC_ID}, 'F', 
                    //                 {getResult.CR_DATE}, {userProfile.org}, {userProfile.userID})";
                } else {
                    _returnFlag = "1";
                    _returnText = "\"error\": \"ระบบไม่ได้รับ Token กรุณา Login\"";
                }
            } else {
                _returnFlag = "1";
                _returnText = "\"error\": \"อุปกรณ์นี้ไม่มีอยู่ในฐานข้อมูล\"";
            }

            //*Prepare data to return to Frontend
            var retuenResult = new ReturnDataMoveLoc {
                FLAG = _returnFlag,
                TEXT = _returnText
            };
            return Ok (retuenResult);

        }

    }
}