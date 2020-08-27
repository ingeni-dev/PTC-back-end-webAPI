using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using PTCwebApi.Models;
using PTCwebApi.Models.PersistenceModels;
using PTCwebApi.Models.ProfilesModels;
using PTCwebApi.Models.PTCModels;
using PTCwebApi.Models.RequestModels;

namespace PTCwebApi.Methods {
    public class StoreConnectionMethod {
        private readonly IMapper _mapper;
        public StoreConnectionMethod (IMapper mapper) => _mapper = mapper;

        // Connect Oracle by store procedure 
        public async Task<IEnumerable<GetMenuModel>> KmapGetIconMenu (UserRequestMenu model) {
            List<Param> param = new List<Param> () {
                new Param () { ParamName = "AS_USER_ID", ParamType = ParamMeterTypeEnum.STRING, ParamValue = model.user_id },
                new Param () { ParamName = "AS_MENU_GRP", ParamType = ParamMeterTypeEnum.STRING, ParamValue = model.group_id },
            };
            var results = await new DataContext (_mapper).CallStoredProcudure (DataBaseHostEnum.KPR, "KPDBA.SP_GET_APP_USER_TOOL", param);
            if (results == null)
                return null;
            var result = _mapper.Map<IEnumerable<AppUserToolModel>> (results);
            var resultReal = _mapper.Map<IEnumerable<AppUserToolModel>, IEnumerable<GetMenuModel>> (result).ToList ();
            return resultReal;
        }
        public async Task<string> PtcGetTranID (string tranType, string compID) {
            List<Param> param = new List<Param> () {
                new Param () { ParamName = "I_TRAN_TYPE", ParamType = ParamMeterTypeEnum.STRING, ParamValue = tranType },
                new Param () { ParamName = "I_COMP_ID", ParamType = ParamMeterTypeEnum.STRING, ParamValue = compID },
            };
            var results = await new DataContext (_mapper).CallStoredProcudurePTC (DataBaseHostEnum.KPR, "KPDBA.PACK_PTC.SP_GET_PTC_TRAN_ID", param);
            if (results == null)
                return null;
            string result = (results as List<dynamic>) [0].TRAN_ID;
            return result;
        }

        public async Task<IEnumerable<dynamic>> PtcGetWareHouse (string user) {
            List<Param> param = new List<Param> () {
                new Param () { ParamName = "I_USER_ID", ParamType = ParamMeterTypeEnum.STRING, ParamValue = user },
            };
            var results = await new DataContext (_mapper).CallStoredProcudurePTC (DataBaseHostEnum.KPR, "KPDBA.PACK_PTC.SP_GET_USER_WAREHOUSE", param);
            if (results == null)
                return null;
            return results;
        }
        public async Task<IEnumerable<dynamic>> PtcGetWareHouseTool (string user) {
            List<Param> param = new List<Param> () {
                new Param () { ParamName = "I_USER_ID", ParamType = ParamMeterTypeEnum.STRING, ParamValue = user },
            };
            var results = await new DataContext (_mapper).CallStoredProcudurePTC (DataBaseHostEnum.KPR, "KPDBA.PACK_PTC.SP_GET_USER_TOOLING", param);
            if (results == null)
                return null;
            // var result = _mapper.Map<IEnumerable<UserWareHouseTool>> (results).ToList ();
            return results;
        }

        public async Task<IEnumerable<dynamic>> PtcGetCurrentPlans (string compID, string toolType, string startDay, string endDay) {
            List<Param> param = new List<Param> () {
                new Param () { ParamName = "I_COMP_ID", ParamType = ParamMeterTypeEnum.STRING, ParamValue = compID },
                new Param () { ParamName = "I_PTC_TYPE", ParamType = ParamMeterTypeEnum.STRING, ParamValue = toolType },
                new Param () { ParamName = "I_DATE_START", ParamType = ParamMeterTypeEnum.STRING, ParamValue = startDay },
                new Param () { ParamName = "I_DATE_END", ParamType = ParamMeterTypeEnum.STRING, ParamValue = endDay },
            };
            var results = await new DataContext (_mapper).CallStoredProcudurePTC (DataBaseHostEnum.KPR, "KPDBA.PACK_PTC.SP_GET_JS_PLAN", param);
            if (results == null)
                return null;
            return results;
        }
    }
}