using PTCwebApi.Models.Elearning;
using PTCwebApi.Models.PTCModels.MethodModels;
using webAPI.Models.Elearning;

namespace webAPI.MapperProfile
{
    public class ElearnMapper : AutoMapper.Profile
    {
        public ElearnMapper()
        {
            CreateMap<ResponseCourseDetails, ResultCourseDetails>()
               .ForMember(d => d.docType, o => o.MapFrom(s => s.DOC_TYPE))
               .ForMember(d => d.courseID, o => o.MapFrom(s => s.COURSE_ID))
               .ForMember(d => d.courseDESC, o => o.MapFrom(s => s.COURSE_DESC))
               .ForMember(d => d.queryID, o => o.MapFrom(s => s.QUERY_IDS))
               .ForMember(d => d.place, o => o.MapFrom(s => s.PLACE))
               .ForMember(d => d.timeSeq, o => o.MapFrom(s => s.TIME_SEQ))
               .ForMember(d => d.lectName, o => o.MapFrom(s => s.LECT_NAME))
               .ForMember(d => d.applicantCount, o => o.MapFrom(s => s.APPLICANT_COUNT))
               .ForMember(d => d.queryBegin, o => o.MapFrom(s => s.QUERY_BEGIN))
               .ForMember(d => d.queryEnd, o => o.MapFrom(s => s.QUERY_END))
               .ForMember(d => d.dayHour, o => o.MapFrom(s => s.DAY_HOUR))
               .ForMember(d => d.dayMin, o => o.MapFrom(s => s.DAY_MIN))
               .ForMember(d => d.instantFlag, o => o.MapFrom(s => s.INSTANT_FLAG));

            CreateMap<ApplicantDetailResponse, ApplicantDetailResult>()
               .ForMember(d => d.docType, o => o.MapFrom(s => s.DOC_TYPE))
               .ForMember(d => d.queryID, o => o.MapFrom(s => s.QUERY_IDS))
               .ForMember(d => d.timeSeq, o => o.MapFrom(s => s.TIME_SEQ))
               .ForMember(d => d.appEmpID, o => o.MapFrom(s => s.APP_EMP_ID))
               .ForMember(d => d.empName, o => o.MapFrom(s => s.EMP_NAME))
               .ForMember(d => d.trainingFlag, o => o.MapFrom(s => s.TRAINING_FLAG));
            //    .ForMember(d => d.unitID, o => o.MapFrom(s => s.UNIT_ID))
            //    .ForMember(d => d.unitDesc, o => o.MapFrom(s => s.UNIT_DESC));
        }
    }
}