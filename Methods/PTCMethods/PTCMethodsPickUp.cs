using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using PTCwebApi.DataService;
using PTCwebApi.Interfaces;
using PTCwebApi.Models.ProfilesModels;
using PTCwebApi.Models.PTCModels.MethodModels;
using PTCwebApi.Models.PTCModels.MethodModels.CurrentPlans;
using PTCwebApi.Models.PTCModels.MethodModels.FindTools;
using PTCwebApi.Models.PTCModels.MethodModels.MoveLoc;

namespace PTCwebApi.Methods.PTCMethods
{
    public class PTCMethodsPickUp
    {
        private readonly IJwtGenerator _jwtGenerator;
        private readonly IMapper _mapper;
        public PTCMethodsPickUp(IMapper mapper, IJwtGenerator jwtGenerator)
        {
            _jwtGenerator = jwtGenerator;
            _mapper = mapper;
        }
        //* Get current plans
        public async Task<object> GetCurrentPlans(RequestAllModelsPTC model)
        {
            string _returnFlag = "0";
            string _returnText = "ผ่าน";
            List<RequestCurrentPlansList> _ptcList = null;
            if (model.token != null)
            {
                UserProfile userProfile = _jwtGenerator.DecodeToken(model.token);
                var queryCompID = $"SELECT COMP_ID COMP FROM KPDBA.WAREHOUSE WHERE WAREHOUSE_ID ='{model.warehouseID}'";
                var resultCompID = await new DataContext().GetResultDapperAsyncObject(DataBaseHostEnum.KPR, queryCompID);
                var compID = (resultCompID as List<dynamic>)[0].COMP;
                var userTool = await new StoreConnectionMethod(_mapper).PtcGetWareHouseTool(userProfile.aduserID);
                var today = DateTime.Today;
                string yesterday = today.AddDays (-1).ToString ("dd/MM/yyyy", new CultureInfo ("en-US"));
                string endDay = today.AddDays (3).ToString ("dd/MM/yyyy", new CultureInfo ("en-US"));
                // string yesterday = "1/08/2020";
                // string endDay = "24/08/2020";
                string toolsType = "%";
                int count = (userTool as List<dynamic>).Count;
                switch (count)
                {
                    case 1:
                        string toolType = (userTool as List<dynamic>)[0].TOOLING;
                        var _resultCurrentPlans = await new StoreConnectionMethod(_mapper).PtcGetCurrentPlans(compID: compID, toolType: toolType, startDay: yesterday, endDay: endDay);
                        break;
                    case 2:
                        var allCurrentPlans = await new StoreConnectionMethod(_mapper).PtcGetCurrentPlans(compID: compID, toolType: toolsType, startDay: yesterday, endDay: endDay);
                        var DataCurrentPlans = allCurrentPlans.ToList();
                        // //TODO convert List<dynamic> to dataTable
                        // var planTable = ToDatable.ToDataTable(DataCurrentPlans, "CurrentPlans");
                        // DataTable a = ToDataTable (DataCurrentPlans, "CurrentPlans");
                        // for (int i = 0; i < (count + 1); i++) { };
                        break;
                    default:
                        List<RequestCurrentPlans> _data = null;
                        _resultCurrentPlans = await new StoreConnectionMethod(_mapper).PtcGetCurrentPlans(compID: compID, toolType: toolsType, startDay: yesterday, endDay: endDay);
                        // var currentPlans = _mapper.Map<IEnumerable<RequestCurrentPlans>>(_resultCurrentPlans);
                        List<RequestCurrentPlans> currentPlans = new List<RequestCurrentPlans> {
                            new RequestCurrentPlans {  JOB_ID= "6309080",
                                                        STEP_ID= "6",
                                                        SPLIT_SEQ= "2",
                                                        PLAN_SUB_SEQ= "1",
                                                        SEQ_RUN= "7",
                                                        WDEPT_ID= "7",
                                                        REVISION= "2",
                                                        ACT_DATE= "15/10/2020",
                                                        MACH_ID= "5001",
                                                        COMP_ID= "001",
                                                        PERIOD= "A",
                                                        PTC_TYPE= "DC",
                                                        PTC_ID= "D5206003-1",
                                                        DIECUT_SN= null,
                                                        WITHD_DATE= null,
                                                        WITHD_USER_ID= null,
                                                        RETURN_DATE= null,
                                                        RETURN_USER_ID= null,
                                                        PTC_SN= "D5206003-1",
                                                        LOC_ID= "$R70",
                                                        LOC_DESC= "ตู้ 01 ชั้น 01",
                                                        CHECK_SHOW= null
                                                        },
                            new RequestCurrentPlans {JOB_ID= "6309081",
                                                        STEP_ID= "6",
                                                        SPLIT_SEQ= "2",
                                                        PLAN_SUB_SEQ= "1",
                                                        SEQ_RUN= "7",
                                                        WDEPT_ID= "7",
                                                        REVISION= "2",
                                                        ACT_DATE= "15/10/2020",
                                                        MACH_ID= "5002",
                                                        COMP_ID= "001",
                                                        PERIOD= "A",
                                                        PTC_TYPE= "PL",
                                                        PTC_ID= "D5206004-1",
                                                        DIECUT_SN= null,
                                                        WITHD_DATE= null,
                                                        WITHD_USER_ID= null,
                                                        RETURN_DATE= null,
                                                        RETURN_USER_ID= null,
                                                        PTC_SN= "D5206004-1",
                                                        LOC_ID= "$R70",
                                                        LOC_DESC= "ตู้ 01 ชั้น 02",
                                                        CHECK_SHOW= null },
                            new RequestCurrentPlans { JOB_ID= "6309082",
                                                        STEP_ID= "6",
                                                        SPLIT_SEQ= "2",
                                                        PLAN_SUB_SEQ= "1",
                                                        SEQ_RUN= "7",
                                                        WDEPT_ID= "7",
                                                        REVISION= "2",
                                                        ACT_DATE= "15/10/2020",
                                                        MACH_ID= "5003",
                                                        COMP_ID= "001",
                                                        PERIOD= "A",
                                                        PTC_TYPE= "DC",
                                                        PTC_ID= "D5206005-1",
                                                        DIECUT_SN= null,
                                                        WITHD_DATE= null,
                                                        WITHD_USER_ID= null,
                                                        RETURN_DATE= null,
                                                        RETURN_USER_ID= null,
                                                        PTC_SN= "D5206005-1",
                                                        LOC_ID= "$R70",
                                                        LOC_DESC= "ตู้ 01 ชั้น 11",
                                                        CHECK_SHOW= null},
                            new RequestCurrentPlans {JOB_ID= "6309083",
                                                        STEP_ID= "6",
                                                        SPLIT_SEQ= "2",
                                                        PLAN_SUB_SEQ= "1",
                                                        SEQ_RUN= "7",
                                                        WDEPT_ID= "7",
                                                        REVISION= "2",
                                                        ACT_DATE= "15/10/2020",
                                                        MACH_ID= "5004",
                                                        COMP_ID= "001",
                                                        PERIOD= "A",
                                                        PTC_TYPE= "CP",
                                                        PTC_ID= "D5206007-1",
                                                        DIECUT_SN= null,
                                                        WITHD_DATE= null,
                                                        WITHD_USER_ID= null,
                                                        RETURN_DATE= null,
                                                        RETURN_USER_ID= null,
                                                        PTC_SN= "D5206007-1",
                                                        LOC_ID= "$R70",
                                                        LOC_DESC= "ตู้ 01 ชั้น 02",
                                                        CHECK_SHOW= null },
                            new RequestCurrentPlans {  JOB_ID= "6309084",
                                                        STEP_ID= "6",
                                                        SPLIT_SEQ= "2",
                                                        PLAN_SUB_SEQ= "1",
                                                        SEQ_RUN= "7",
                                                        WDEPT_ID= "7",
                                                        REVISION= "2",
                                                        ACT_DATE= "14/10/2020",
                                                        MACH_ID= "5005",
                                                        COMP_ID= "001",
                                                        PERIOD= "A",
                                                        PTC_TYPE= "DC",
                                                        PTC_ID= "D5206008-1",
                                                        DIECUT_SN= null,
                                                        WITHD_DATE= null,
                                                        WITHD_USER_ID= null,
                                                        RETURN_DATE= null,
                                                        RETURN_USER_ID= null,
                                                        PTC_SN= "D5206008-1",
                                                        LOC_ID= "$R70",
                                                        LOC_DESC= "ตู้ 01 ชั้น 08",
                                                        CHECK_SHOW= null
                                                        },
                            new RequestCurrentPlans {JOB_ID= "6309085",
                                                        STEP_ID= "6",
                                                        SPLIT_SEQ= "2",
                                                        PLAN_SUB_SEQ= "1",
                                                        SEQ_RUN= "7",
                                                        WDEPT_ID= "7",
                                                        REVISION= "2",
                                                        ACT_DATE= "11/10/2020",
                                                        MACH_ID= "5006",
                                                        COMP_ID= "001",
                                                        PERIOD= "A",
                                                        PTC_TYPE= "CP",
                                                        PTC_ID= "D5206009-1",
                                                        DIECUT_SN= null,
                                                        WITHD_DATE= null,
                                                        WITHD_USER_ID= null,
                                                        RETURN_DATE= null,
                                                        RETURN_USER_ID= null,
                                                        PTC_SN= "D5206009-1",
                                                        LOC_ID= "$R70",
                                                        LOC_DESC= "ตู้ 01 ชั้น 11",
                                                        CHECK_SHOW= null },
                            new RequestCurrentPlans { JOB_ID= "6309086",
                                                        STEP_ID= "6",
                                                        SPLIT_SEQ= "2",
                                                        PLAN_SUB_SEQ= "1",
                                                        SEQ_RUN= "7",
                                                        WDEPT_ID= "7",
                                                        REVISION= "2",
                                                        ACT_DATE= "11/10/2020",
                                                        MACH_ID= "5007",
                                                        COMP_ID= "001",
                                                        PERIOD= "A",
                                                        PTC_TYPE= "DC",
                                                        PTC_ID= "D5206010-1",
                                                        DIECUT_SN= null,
                                                        WITHD_DATE= null,
                                                        WITHD_USER_ID= null,
                                                        RETURN_DATE= null,
                                                        RETURN_USER_ID= null,
                                                        PTC_SN= "D5206010-1",
                                                        LOC_ID= "$R70",
                                                        LOC_DESC= "ตู้ 01 ชั้น 07",
                                                        CHECK_SHOW= null},
                            new RequestCurrentPlans {JOB_ID= "6309087",
                                                        STEP_ID= "6",
                                                        SPLIT_SEQ= "2",
                                                        PLAN_SUB_SEQ= "1",
                                                        SEQ_RUN= "7",
                                                        WDEPT_ID= "7",
                                                        REVISION= "2",
                                                        ACT_DATE= "11/10/2020",
                                                        MACH_ID= "5008",
                                                        COMP_ID= "001",
                                                        PERIOD= "A",
                                                        PTC_TYPE= "PL",
                                                        PTC_ID= "D5206011-1",
                                                        DIECUT_SN= null,
                                                        WITHD_DATE= null,
                                                        WITHD_USER_ID= null,
                                                        RETURN_DATE= null,
                                                        RETURN_USER_ID= null,
                                                        PTC_SN= "D5206011-1",
                                                        LOC_ID= "$R70",
                                                        LOC_DESC= "ตู้ 11 ชั้น 13",
                                                        CHECK_SHOW= null },
                            new RequestCurrentPlans {  JOB_ID= "6309088",
                                                        STEP_ID= "6",
                                                        SPLIT_SEQ= "2",
                                                        PLAN_SUB_SEQ= "1",
                                                        SEQ_RUN= "7",
                                                        WDEPT_ID= "7",
                                                        REVISION= "2",
                                                        ACT_DATE= "13/10/2020",
                                                        MACH_ID= "5009",
                                                        COMP_ID= "001",
                                                        PERIOD= "A",
                                                        PTC_TYPE= "DC",
                                                        PTC_ID= "D5206012-1",
                                                        DIECUT_SN= null,
                                                        WITHD_DATE= null,
                                                        WITHD_USER_ID= null,
                                                        RETURN_DATE= null,
                                                        RETURN_USER_ID= null,
                                                        PTC_SN= "D5206012-1",
                                                        LOC_ID= "$R70",
                                                        LOC_DESC= "ตู้ 01 ชั้น 11",
                                                        CHECK_SHOW= null
                                                        },
                            new RequestCurrentPlans {JOB_ID= "6309089",
                                                        STEP_ID= "6",
                                                        SPLIT_SEQ= "2",
                                                        PLAN_SUB_SEQ= "1",
                                                        SEQ_RUN= "7",
                                                        WDEPT_ID= "7",
                                                        REVISION= "2",
                                                        ACT_DATE= "13/10/2020",
                                                        MACH_ID= "5010",
                                                        COMP_ID= "001",
                                                        PERIOD= "A",
                                                        PTC_TYPE= "CP",
                                                        PTC_ID= "D5206013-1",
                                                        DIECUT_SN= null,
                                                        WITHD_DATE= null,
                                                        WITHD_USER_ID= null,
                                                        RETURN_DATE= null,
                                                        RETURN_USER_ID= null,
                                                        PTC_SN= "D5206013-1",
                                                        LOC_ID= "$R70",
                                                        LOC_DESC= "ตู้ 01 ชั้น 05",
                                                        CHECK_SHOW= null },
                            new RequestCurrentPlans { JOB_ID= "6309090",
                                                        STEP_ID= "6",
                                                        SPLIT_SEQ= "2",
                                                        PLAN_SUB_SEQ= "1",
                                                        SEQ_RUN= "7",
                                                        WDEPT_ID= "7",
                                                        REVISION= "2",
                                                        ACT_DATE= "13/10/2020",
                                                        MACH_ID= "5011",
                                                        COMP_ID= "001",
                                                        PERIOD= "A",
                                                        PTC_TYPE= "DC",
                                                        PTC_ID= "D5206014-1",
                                                        DIECUT_SN= null,
                                                        WITHD_DATE= null,
                                                        WITHD_USER_ID= null,
                                                        RETURN_DATE= null,
                                                        RETURN_USER_ID= null,
                                                        PTC_SN= "D5206014-1",
                                                        LOC_ID= "$R70",
                                                        LOC_DESC= "ตู้ 10 ชั้น 01",
                                                        CHECK_SHOW= null},
                            new RequestCurrentPlans {JOB_ID= "6309091",
                                                        STEP_ID= "6",
                                                        SPLIT_SEQ= "2",
                                                        PLAN_SUB_SEQ= "1",
                                                        SEQ_RUN= "7",
                                                        WDEPT_ID= "7",
                                                        REVISION= "2",
                                                        ACT_DATE= "13/10/2020",
                                                        MACH_ID= "5012",
                                                        COMP_ID= "001",
                                                        PERIOD= "A",
                                                        PTC_TYPE= "PL",
                                                        PTC_ID= "D5206015-1",
                                                        DIECUT_SN= null,
                                                        WITHD_DATE= null,
                                                        WITHD_USER_ID= null,
                                                        RETURN_DATE= null,
                                                        RETURN_USER_ID= null,
                                                        PTC_SN= "D5206015-1",
                                                        LOC_ID= "$R70",
                                                        LOC_DESC= "ตู้ 05 ชั้น 04",
                                                        CHECK_SHOW= null },
                            new RequestCurrentPlans {  JOB_ID= "6309092",
                                                        STEP_ID= "6",
                                                        SPLIT_SEQ= "2",
                                                        PLAN_SUB_SEQ= "1",
                                                        SEQ_RUN= "7",
                                                        WDEPT_ID= "7",
                                                        REVISION= "2",
                                                        ACT_DATE= "14/10/2020",
                                                        MACH_ID= "5013",
                                                        COMP_ID= "001",
                                                        PERIOD= "A",
                                                        PTC_TYPE= "DC",
                                                        PTC_ID= "D5206016-1",
                                                        DIECUT_SN= null,
                                                        WITHD_DATE= null,
                                                        WITHD_USER_ID= null,
                                                        RETURN_DATE= null,
                                                        RETURN_USER_ID= null,
                                                        PTC_SN= "D5206016-1",
                                                        LOC_ID= "$R70",
                                                        LOC_DESC= "ตู้ 11 ชั้น 01",
                                                        CHECK_SHOW= null
                                                        },
                            new RequestCurrentPlans {JOB_ID= "6309093",
                                                        STEP_ID= "6",
                                                        SPLIT_SEQ= "2",
                                                        PLAN_SUB_SEQ= "1",
                                                        SEQ_RUN= "7",
                                                        WDEPT_ID= "7",
                                                        REVISION= "2",
                                                        ACT_DATE= "14/10/2020",
                                                        MACH_ID= "5014",
                                                        COMP_ID= "001",
                                                        PERIOD= "A",
                                                        PTC_TYPE= "CP",
                                                        PTC_ID= "D5206017-1",
                                                        DIECUT_SN= null,
                                                        WITHD_DATE= null,
                                                        WITHD_USER_ID= null,
                                                        RETURN_DATE= null,
                                                        RETURN_USER_ID= null,
                                                        PTC_SN= "D5206017-1",
                                                        LOC_ID= "$R70",
                                                        LOC_DESC= "ตู้ 03 ชั้น 13",
                                                        CHECK_SHOW= null },
                            new RequestCurrentPlans { JOB_ID= "6309094",
                                                        STEP_ID= "6",
                                                        SPLIT_SEQ= "2",
                                                        PLAN_SUB_SEQ= "1",
                                                        SEQ_RUN= "7",
                                                        WDEPT_ID= "7",
                                                        REVISION= "2",
                                                        ACT_DATE= "14/10/2020",
                                                        MACH_ID= "5015",
                                                        COMP_ID= "001",
                                                        PERIOD= "A",
                                                        PTC_TYPE= "DC",
                                                        PTC_ID= "D5206018-1",
                                                        DIECUT_SN= null,
                                                        WITHD_DATE= null,
                                                        WITHD_USER_ID= null,
                                                        RETURN_DATE= null,
                                                        RETURN_USER_ID= null,
                                                        PTC_SN= "D5206018-1",
                                                        LOC_ID= "$R70",
                                                        LOC_DESC= "ตู้ 04 ชั้น 02",
                                                        CHECK_SHOW= null},
                            new RequestCurrentPlans {JOB_ID= "6309095",
                                                        STEP_ID= "6",
                                                        SPLIT_SEQ= "2",
                                                        PLAN_SUB_SEQ= "1",
                                                        SEQ_RUN= "7",
                                                        WDEPT_ID= "7",
                                                        REVISION= "2",
                                                        ACT_DATE= "14/10/2020",
                                                        MACH_ID= "5016",
                                                        COMP_ID= "001",
                                                        PERIOD= "A",
                                                        PTC_TYPE= "PL",
                                                        PTC_ID= "D5206019-1",
                                                        DIECUT_SN= null,
                                                        WITHD_DATE= null,
                                                        WITHD_USER_ID= null,
                                                        RETURN_DATE= null,
                                                        RETURN_USER_ID= null,
                                                        PTC_SN= "D5206019-1",
                                                        LOC_ID= "$R70",
                                                        LOC_DESC= "ตู้ 01 ชั้น 01",
                                                        CHECK_SHOW= null },
                            new RequestCurrentPlans {JOB_ID= "6309096",
                                                        STEP_ID= "6",
                                                        SPLIT_SEQ= "2",
                                                        PLAN_SUB_SEQ= "1",
                                                        SEQ_RUN= "7",
                                                        WDEPT_ID= "7",
                                                        REVISION= "2",
                                                        ACT_DATE= "12/10/2020",
                                                        MACH_ID= "5016",
                                                        COMP_ID= "001",
                                                        PERIOD= "A",
                                                        PTC_TYPE= "PL",
                                                        PTC_ID= "D5206019-1",
                                                        DIECUT_SN= null,
                                                        WITHD_DATE= null,
                                                        WITHD_USER_ID= null,
                                                        RETURN_DATE= null,
                                                        RETURN_USER_ID= null,
                                                        PTC_SN= "D5206020-1",
                                                        LOC_ID= "$R70",
                                                        LOC_DESC= "ตู้ 05 ชั้น 01",
                                                        CHECK_SHOW= null }
                            };
                        decimal _numCount = (currentPlans as List<RequestCurrentPlans>).Count;
                        for (int i = 0; i < _numCount; i++)
                        {
                            _data = currentPlans as List<RequestCurrentPlans>;
                            var _jobID = _data[i].JOB_ID.ToString();
                            var _stepID = _data[i].STEP_ID.ToString();
                            string _queryCheckIDPlan = $"SELECT COUNT(1) AS COUN FROM KPDBA.PTC_JS_PLAN_DETAIL WHERE JOB_ID = '{currentPlans[i].JOB_ID}' AND STEP_ID = TO_CHAR('{currentPlans[i].STEP_ID}') AND SPLIT_SEQ = TO_NUMBER('{currentPlans[i].SPLIT_SEQ}') AND PLAN_SUB_SEQ = TO_NUMBER('{currentPlans[i].PLAN_SUB_SEQ}') AND SEQ_RUN = TO_NUMBER('{currentPlans[i].SEQ_RUN}') AND WDEPT_ID = TO_NUMBER('{currentPlans[i].WDEPT_ID}') AND REVISION = TO_NUMBER('{currentPlans[i].REVISION}') AND PTC_TYPE = '{currentPlans[i].PTC_TYPE}' AND PTC_ID = '{currentPlans[i].PTC_ID}'";
                            var _stateCheckPlan = await new DataContext().GetResultDapperAsyncObject(DataBaseHostEnum.KPR, _queryCheckIDPlan);
                            decimal _stateCheck = (_stateCheckPlan as List<dynamic>)[0].COUN;
                            if (_stateCheck == 0)
                            {
                                _data[i].CHECK_SHOW = "T";
                            }
                            else
                            {
                                _data[i].CHECK_SHOW = "F";
                            }
                        }
                        _ptcList = _mapper.Map<List<RequestCurrentPlans>, List<RequestCurrentPlansList>>(_data);
                        break;
                }
            }
            else
            {
                _returnFlag = "1";
                _returnText = "ไม่พบข้อมูลผู้ใช้ กรุณา Login อีกครั้ง หรือติดต่อฝ่าย IT ";
            }
            var returnResult = new ResponseCurrentPlans
            {
                ptcList = _ptcList,
                flag = _returnFlag,
                text = _returnText
            };
            return returnResult;
        }
        //* Pick up the tool
        public async Task<object> PickUpTooling(RequestAllModelsPTC model)
        {
            string _returnFlag = "0";
            string _returnText = "ผ่าน";
            GetLoc _dataLoc = new GetLoc();
            if (model.token != null)
            {
                UserProfile userProfile = _jwtGenerator.DecodeToken(model.token);
                var queryCheck = $"SELECT COUNT(1) AS COUN FROM KPDBA.DIECUT_SN WHERE DIECUT_SN ='{model.ptcID}'";
                var resultCheck = await new DataContext().GetResultDapperAsyncDynamic(DataBaseHostEnum.KPR, queryCheck);
                decimal toolValid = (resultCheck as List<dynamic>)[0].COUN;
                if (toolValid == 1)
                {
                    var query = $"SELECT SD.LOC_ID, LOC.LOC_DETAIL, SD.QTY FROM (SELECT SD.WAREHOUSE_ID, SD.LOC_ID, SUM (SD.QTY) QTY FROM KPDBA.PTC_STOCK_DETAIL SD WHERE SD.WAREHOUSE_ID = '{model.warehouseID}' AND SD.PTC_ID = '{model.ptcID}' GROUP BY SD.WAREHOUSE_ID, SD.LOC_ID HAVING SUM (SD.QTY) > 0) SD JOIN (SELECT WAREHOUSE_ID, LOC_ID, LOC_DETAIL FROM KPDBA.LOCATION_PTC) LOC ON (SD.WAREHOUSE_ID = LOC.WAREHOUSE_ID AND SD.LOC_ID = LOC.LOC_ID)";
                    var result = await new DataContext().GetResultDapperAsyncObject(DataBaseHostEnum.KPR, query);
                    decimal count = (result as List<object>).Count;
                    if (count != 0)
                    {

                        var results = _mapper.Map<IEnumerable<GetLoc>>(result);
                        var dataLoc = results.ElementAt(0);
                        var queryCompID = $"SELECT COMP_ID COMP FROM KPDBA.WAREHOUSE WHERE WAREHOUSE_ID ='{model.warehouseID}'";
                        var resultCompID = await new DataContext().GetResultDapperAsyncObject(DataBaseHostEnum.KPR, queryCompID);
                        string queryCheckIDPlan = $"SELECT COUNT(1) AS COUN FROM KPDBA.PTC_JS_PLAN_DETAIL WHERE JOB_ID = '{model.jobID}' AND STEP_ID = TO_CHAR('{model.stepID}') AND SPLIT_SEQ = TO_NUMBER('{model.splitSeq}') AND PLAN_SUB_SEQ = TO_NUMBER('{model.planSubSeq}') AND SEQ_RUN = TO_NUMBER('{model.seqRun}') AND WDEPT_ID = TO_NUMBER('{model.wdeptID}') AND REVISION = TO_NUMBER('{model.revision}') AND PTC_TYPE = '{model.ptcType}' AND PTC_ID = '{model.ptcID}'";
                        var stateCheckPlan = await new DataContext().GetResultDapperAsyncObject(DataBaseHostEnum.KPR, queryCheckIDPlan);
                        decimal stateCheck = (stateCheckPlan as List<dynamic>)[0].COUN;
                        if (stateCheck == 0)
                        {
                            string queryPlanDetail = $"INSERT INTO KPDBA.PTC_JS_PLAN_DETAIL (JOB_ID, MACH_ID, STEP_ID, SPLIT_SEQ, PLAN_SUB_SEQ, SEQ_RUN, WDEPT_ID, REVISION, ACT_DATE, PTC_TYPE, PTC_ID, WITHD_DATE, WITHD_USER_ID, DIECUT_SN) VALUES ('{model.jobID}', TO_CHAR ('{model.machID}'),  TO_CHAR ('{model.stepID}'), TO_NUMBER ('{model.splitSeq}'), TO_NUMBER ('{model.planSubSeq}'), TO_NUMBER ('{model.seqRun}'), TO_NUMBER ('{model.wdeptID}'), TO_NUMBER ('{model.revision}'), TO_DATE ('{model.actDate}', 'dd/mm/yyyy hh24:mi:ss'), '{model.ptcType}', '{model.ptcID}', TO_DATE(TO_CHAR(SYSDATE), 'dd/mm/yyyy'), TO_CHAR ('{userProfile.userID}'), '{model.ptcID}')";
                            var insertPlanDetail = await new DataContext().InsertResultDapperAsync(DataBaseHostEnum.KPR, queryPlanDetail);
                            var compID = (resultCompID as List<dynamic>)[0].COMP;
                            // var tranSEQ = 1;
                            // var tranType = "5"; // โอนย้ายเข้า
                            // var locID = dataLoc.LOC_ID; // old loc
                            string tran_id = await new StoreConnectionMethod(_mapper).PtcGetTranID(compID: model.warehouseID, tranType: compID);
                            // var tranDate = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss", new CultureInfo("en-US"));
                            // var insertGetQuery = $"INSERT INTO KPDBA.PTC_STOCK_DETAIL (TRAN_ID, TRAN_SEQ, TRAN_TYPE, TRAN_DATE,PTC_ID, QTY, COMP_ID, WAREHOUSE_ID, LOC_ID, STATUS, CR_DATE, CR_ORG_ID, CR_USER_ID) VALUES ('{tran_id}', TO_NUMBER('{tranSEQ}'), TO_NUMBER('{tranType}'), TO_DATE('{tranDate}', 'dd/mm/yyyy hh24:mi:ss'),'{model.ptcID}', TO_NUMBER('-1'), TO_CHAR('{compID}'),'{model.warehouseID}','{locID}', 'T', SYSDATE, '{userProfile.org}', '{userProfile.userID}')";
                            // var resultInsert = await new DataContext().InsertResultDapperAsync(DataBaseHostEnum.KPR, insertGetQuery);
                            // Console.WriteLine(Foo(resultInsert));
                            // string Foo<T>(T parameter) { return typeof(T).Name; }
                            var tranSEQ = 1;
                            var tranType = "2"; // โอนย้ายเข้า
                            var locID = "$W70"; // newLoc ย้ายไปพื้นที่เบิก
                            var tranDate = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss", new CultureInfo("en-US"));
                            var insertOutQuery = $"INSERT INTO KPDBA.PTC_STOCK_DETAIL (TRAN_ID, TRAN_SEQ, TRAN_TYPE, TRAN_DATE,PTC_ID, QTY, COMP_ID, WAREHOUSE_ID, LOC_ID, STATUS, CR_DATE, CR_ORG_ID, CR_USER_ID) VALUES ('{tran_id}', TO_NUMBER('{tranSEQ}'), TO_NUMBER('{tranType}'), TO_DATE('{tranDate}', 'dd/mm/yyyy hh24:mi:ss'),'{model.ptcID}', TO_NUMBER('-1'), TO_CHAR('{compID}'),'{model.warehouseID}','{locID}', 'T', SYSDATE, '{userProfile.org}', '{userProfile.userID}')";
                            var resultOutInsert = await new DataContext().InsertResultDapperAsync(DataBaseHostEnum.KPR, insertOutQuery);
                        }
                        else
                        {
                            _returnFlag = "1";
                            _returnText = "หมายเลข JOB ID นี้ถูกแล้ว";
                        }
                    }
                    else
                    {
                        _returnFlag = "1";
                        _returnText = "ไม่พบอุปกรณ์คงเหลือภายในคลัง";
                    }
                }
                else
                {
                    //* check QR code is Loccation right?
                    var queryCheckLoc = $"SELECT COUNT(1) AS COUN FROM KPDBA.LOCATION_PTC WHERE LOC_ID = '{model.ptcID}'";
                    var resultCheckLoc = await new DataContext().GetResultDapperAsyncDynamic(DataBaseHostEnum.KPR, queryCheckLoc);
                    decimal LocValid = (resultCheckLoc as List<dynamic>)[0].COUN;
                    if (LocValid == 1)
                    {
                        _returnFlag = "1";
                        _returnText = "QR code นี้คือ Location กรุณาสแกนใหม่อีกครั้ง";
                    }
                    else
                    {
                        _returnFlag = "1";
                        _returnText = "ไม่พบรหัส Tooling นี้ในฐานข้อมูล";
                    }
                }
            }
            else
            {
                _returnFlag = "1";
                _returnText = "ไม่พบข้อมูลผู้ใช้ กรุณา Login อีกครั้ง หรือติดต่อฝ่าย IT ";
            }
            var returnResult = new ReturnDataMoveLoc
            {
                flag = _returnFlag,
                text = _returnText
            };
            return returnResult;
        }
    }
}