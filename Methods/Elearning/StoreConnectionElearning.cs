using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using PTCwebApi;
using PTCwebApi.Models.PersistenceModels;
using webAPI.Models.Elearning;

namespace webAPI.Methods.Elearning
{
    public class StoreConnectionElearning
    {
        private readonly IMapper _mapper;
        public StoreConnectionElearning(IMapper mapper) => _mapper = mapper;
        public async Task<IEnumerable<dynamic>> ElearningSetLecturer(String empType, String empID, String courseDESC, String userLogin)
        {
            List<Param> param = new List<Param>() {
                new Param () { ParamName = "as_emp_type", ParamType = ParamMeterTypeEnum.STRING, ParamValue =empType },
                new Param () { ParamName = "as_id", ParamType = ParamMeterTypeEnum.STRING, ParamValue = empID },
                new Param () { ParamName = "as_course_name", ParamType = ParamMeterTypeEnum.STRING, ParamValue = courseDESC },
                new Param () { ParamName = "as_user_login", ParamType = ParamMeterTypeEnum.STRING, ParamValue = userLogin },
            };
            var result = await new DataContext(_mapper).CallStoredProcudureElearn(DataBaseHostEnum.KPR, "KPDBA.SP_ADD_LECTURER", param);
            if (result == null)
                return null;
            return result;
        }
    }
}