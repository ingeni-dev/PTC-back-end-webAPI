using Newtonsoft.Json.Linq;
using PTCwebApi.Models;
using PTCwebApi.Models.ProfilesModels;
using PTCwebApi.Models.PTCModels;
using PTCwebApi.Models.PTCModels.Entities;
using PTCwebApi.Models.PTCModels.MethodModels;
using PTCwebApi.Models.PTCModels.MethodModels.CurrentPlans;
using PTCwebApi.Models.RequestModels;

namespace PTCwebApi.Profiles {
    public class MapperProfile : AutoMapper.Profile {
        public MapperProfile () {
            CreateMap<UserWareHouseID, WarehouseList> ()
                .ForMember (d => d.warehouseID, o => o.MapFrom (s => s.WAREHOUSE_ID))
                .ForMember (d => d.warehouseDESC, o => o.MapFrom (s => s.WAREHOUSE_DESC));

            CreateMap<AppUserToolModel, GetMenuModel> ()
                .ForMember (d => d.ID, o => o.MapFrom (s => s.TOOL_ID))
                .ForMember (d => d.appName, o => o.MapFrom (s => s.TOOL_NAME))
                .ForMember (d => d.URLPath, o => o.MapFrom (s => s.URL))
                .ForMember (d => d.img, o => o.MapFrom (s => s.ICON_SRC));

            CreateMap<UserProfileFromSQl, UserProfile> ()
                .ForMember (d => d.org, o => o.MapFrom (s => s.ORG_ID))
                .ForMember (d => d.userID, o => o.MapFrom (s => s.EMP_ID))
                .ForMember (d => d.userName, o => o.MapFrom (s => s.EMP_FNAME + ' ' + s.EMP_LNAME))
                // .ForMember (d => d.EMP_NAME_ENG, o => o.MapFrom (s => s.EMP_FNAME + ' ' + s.EMP_LNAME))
                .ForMember (d => d.posrole, o => o.MapFrom (s => s.POS_ID + '-' + s.ROLE_ID))
                .ForMember (d => d.email, o => o.MapFrom (s => s.E_MAIL))
                .ForMember (d => d.nickname, o => o.MapFrom (s => s.EMP_NICKNAME));

            CreateMap<RequestCurrentPlans, MappRequestCurrentPlans> ()
                .ForMember (d => d.actDate, o => o.MapFrom (s => s.ACT_DATE))
                .ForMember (d => d.compID, o => o.MapFrom (s => s.COMP_ID))
                .ForMember (d => d.diecutSN, o => o.MapFrom (s => s.DIECUT_SN))
                .ForMember (d => d.jobID, o => o.MapFrom (s => s.JOB_ID))
                .ForMember (d => d.locID, o => o.MapFrom (s => s.LOC_ID))
                .ForMember (d => d.locName, o => o.MapFrom (s => s.LOC_NAME))
                .ForMember (d => d.machID, o => o.MapFrom (s => s.MACH_ID))
                .ForMember (d => d.period, o => o.MapFrom (s => s.PERIOD))
                .ForMember (d => d.planSubSeq, o => o.MapFrom (s => s.PLAN_SUB_SEQ))
                .ForMember (d => d.ptcID, o => o.MapFrom (s => s.PTC_ID))
                .ForMember (d => d.ptcType, o => o.MapFrom (s => s.PTC_TYPE))
                .ForMember (d => d.returnDate, o => o.MapFrom (s => s.RETURN_DATE))
                .ForMember (d => d.returnUserID, o => o.MapFrom (s => s.RETURN_USER_ID))
                .ForMember (d => d.revision, o => o.MapFrom (s => s.REVISION))
                .ForMember (d => d.seqRun, o => o.MapFrom (s => s.SEQ_RUN))
                .ForMember (d => d.splitSeq, o => o.MapFrom (s => s.SPLIT_SEQ))
                .ForMember (d => d.stepID, o => o.MapFrom (s => s.STEP_ID))
                .ForMember (d => d.wdeptID, o => o.MapFrom (s => s.WDEPT_ID))
                .ForMember (d => d.withdDate, o => o.MapFrom (s => s.WITHD_DATE))
                .ForMember (d => d.withdUserID, o => o.MapFrom (s => s.WITHD_USER_ID));
        }
    }
}