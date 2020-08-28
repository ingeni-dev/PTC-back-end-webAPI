using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using PTCwebApi.Interfaces;
using PTCwebApi.Models.ProfilesModels;
using PTCwebApi.Models.PTCModels.MethodModels;
using PTCwebApi.Models.PTCModels.MethodModels.MoveLoc;
using PTCwebApi.Models.PTCModels.MethodModels.ReturnTooling;

namespace PTCwebApi.Methods.PTCMethods {
    public class PTCMethodsReturnTool {
        private readonly IJwtGenerator _jwtGenerator;
        private readonly IMapper _mapper;
        public PTCMethodsReturnTool (IMapper mapper, IJwtGenerator jwtGenerator) {
            _jwtGenerator = jwtGenerator;
            _mapper = mapper;
        }
        public async Task<object> getWithdrawalHistory (RequestAllModelsPTC model) {
            string _returnFlag = "0";
            string _returnText = "ผ่าน";
            List<ResponseWthdralwalHistoryList> _resultHistory = null;
            if (model.token != null) {
                UserProfile userProfile = _jwtGenerator.DecodeToken (model.token);
                var query = $"SELECT * FROM KPDBA.PTC_JS_PLAN_DETAIL WHERE RETURN_DATE IS NULL AND WITHD_USER_ID = '{userProfile.userID}'";
                var result = await new DataContext ().GetResultDapperAsyncObject (DataBaseHostEnum.KPR, query);
                // return result;
                if ((result as List<object>).Count != 0) {
                    var results = _mapper.Map<IEnumerable<RequestWthdralwalHistoryList>> (result);
                    _resultHistory = _mapper.Map<IEnumerable<RequestWthdralwalHistoryList>, IEnumerable<ResponseWthdralwalHistoryList>> (results).ToList ();
                } else {
                    _returnFlag = "1";
                    _returnText = "\"error: ไม่พบประวัติการเบิกอุปกรณ์ของคุณ\"";
                }
            } else {
                _returnFlag = "1";
                _returnText = "\"error: ระบบไม่ได้รับ Token กรุณา Login ใหม่อีกครั้งหรือ ติดต่อแผนก IT\"";
            }
            var returnResult = new ResponseWthdralwalHistory {
                historyList = _resultHistory,
                flag = _returnFlag,
                text = _returnText
            };
            return returnResult;
        }
        public async Task<object> moveToKeep (RequestAllModelsPTC model) {
            ReturnDataMoveLoc result = new ReturnDataMoveLoc ();

            if (model.token != null) {
                UserProfile userProfile = _jwtGenerator.DecodeToken (model.token);
                result = await new PTCMethods (_mapper, _jwtGenerator).MoveTooling (model);
                if (result != null) {
                    if (result.flag == "0") {
                        //TODO update
                        string DateNow = DateTime.Today.ToString ("dd/MM/yyyy", new CultureInfo ("en-US"));
                        var query = $"UPDATE KPDBA.PTC_JS_PLAN_DETAIL SET RETURN_DATE = TO_DATE ('{DateNow}', 'dd/mm/yyyy'), RETURN_USER_ID = TO_CHAR ('{userProfile.userID}') WHERE JOB_ID = '{model.jobID}'";
                        var upDateResult = await new DataContext ().GetResultDapperAsyncDynamic (DataBaseHostEnum.KPR, query);
                    } else {
                        return result;
                    }
                }
            } else {
                result = new ReturnDataMoveLoc {
                    flag = "1",
                    text = "\"error: ระบบไม่ได้รับ Token กรุณา Login ใหม่อีกครั้งหรือ ติดต่อแผนก IT\""
                };
            }

            return result;
        }
    }
}