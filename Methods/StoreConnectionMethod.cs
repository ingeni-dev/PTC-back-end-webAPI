using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using PTCwebApi.Models;
using PTCwebApi.Models.PersistenceModels;
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
        public async Task<string> PtcGetTranID (ModelTablePTCDetail model) {
            List<Param> param = new List<Param> () {
                new Param () { ParamName = "I_TRAN_TYPE", ParamType = ParamMeterTypeEnum.STRING, ParamValue = model.TRAN_TYPE },
                new Param () { ParamName = "I_COMP_ID", ParamType = ParamMeterTypeEnum.STRING, ParamValue = model.TRAN_ID },
            };
            var results = await new DataContext (_mapper).CallStoredProcudurePTC (DataBaseHostEnum.KPR, "KPDBA.PACK_PTC.SP_GET_PTC_TRAN_ID", param);
            if (results == null)
                return null;

            string result = (results as List<dynamic>) [0].TRAN_ID;
            return result;
        }
    }
}