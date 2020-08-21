using Newtonsoft.Json.Linq;
using PTCwebApi.Models;
using PTCwebApi.Models.ProfilesModels;
using PTCwebApi.Models.PTCModels.Entities;
using PTCwebApi.Models.RequestModels;

namespace PTCwebApi.Profiles {
    public class MapperProfile : AutoMapper.Profile {
        public MapperProfile () {
            // CreateMap<JObject, CheckToolingInWare> ()
            //     .ForMember (d => d.Count, o => { o.MapFrom (s => s["COUNT(1)"]); });

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
        }
    }
}