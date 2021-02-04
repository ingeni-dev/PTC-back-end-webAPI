using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using PTCwebApi;
using PTCwebApi.Models.PersistenceModels;
using webAPI.Models.Elearning;

namespace webAPI.Methods.Elearning
{
    public class StoreConnection
    {
        private readonly IMapper _mapper;
        public StoreConnection(IMapper mapper) => _mapper = mapper;
        public async Task<String> KmapGetIconMenu(AddCourseDetail model, String empType, String empID, String userLogin)
        {
            List<Param> param = new List<Param>() {
                new Param () { ParamName = "as_emp_type", ParamType = ParamMeterTypeEnum.STRING, ParamValue =empType },
                new Param () { ParamName = "as_id", ParamType = ParamMeterTypeEnum.STRING, ParamValue = empID },
                new Param () { ParamName = "as_course_name", ParamType = ParamMeterTypeEnum.STRING, ParamValue = model.courseName },
                new Param () { ParamName = "as_place", ParamType = ParamMeterTypeEnum.STRING, ParamValue = model.place },
                new Param () { ParamName = "ad_begin_date", ParamType = ParamMeterTypeEnum.STRING, ParamValue = model.startDate },
                new Param () { ParamName = "ad_end_date", ParamType = ParamMeterTypeEnum.STRING, ParamValue = model.endDate },
                new Param () { ParamName = "as_remark", ParamType = ParamMeterTypeEnum.STRING, ParamValue = model.remark },
                new Param () { ParamName = "as_user_login", ParamType = ParamMeterTypeEnum.STRING, ParamValue = userLogin },
            };
            var results = await new DataContext(_mapper).CallStoredProcudureElearn(DataBaseHostEnum.KPR, "KPDBA.SP_GET_APP_USER_TOOL", param);
            if (results == null)
                return null;
            // var result = _mapper.Map<IEnumerable<AppUserToolModel>>(results);
            // var resultReal = _mapper.Map<IEnumerable<AppUserToolModel>, IEnumerable<GetMenuModel>>(result).ToList();
            return null;
        }
    }
}