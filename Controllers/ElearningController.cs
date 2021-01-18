using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PTCwebApi;
using webAPI.Models.Elearning;
using PTCwebApi.Models.Elearning;
using webAPI.Security;

namespace webAPI.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ElearningController : ControllerBase
    {
        private readonly IMapper _mapper;
        public ElearningController(IMapper mapper)
        {
            _mapper = mapper;
        }

        [HttpGet("")]
        public ActionResult<String> GetTModels()
        {
            return "1";
        }

        [HttpPost("getCourses")]
        public async Task<List<ResultCourseDetails>> GetCourses(RequestCourses model)
        {
            string query = $"SELECT 'COURSE' DOC_TYPE, CM.COURSE_ID, CM.COURSE_DESC, CQ.QUERY_ID, PLACE, CQT.TIME_SEQ, LECT_NAME, APPLICANT_COUNT, COURSE_DATE QUERY_BEGIN, NVL ( COURSE_DATE + TO_NUMBER (DAY_HOUR) / 24 + TO_NUMBER (DAY_MIN) / 24 / 60, COURSE_DATE) QUERY_END, NVL (DAY_HOUR, 0) DAY_HOUR, NVL (DAY_MIN, 0) DAY_MIN, NVL (CQ.INSTANT_FLAG, 'F') INSTANT_FLAG FROM KPDBA.COURSE_MASTER CM, KPDBA.COURSE_QUERY CQ, KPDBA.COURSE_QUERY_TIME CQT, (  SELECT QUERY_ID, LISTAGG (THAI_FNAME || ' ' || THAI_SNAME, ', ') WITHIN GROUP (ORDER BY QUERY_ID, THAI_FNAME, THAI_SNAME) LECT_NAME FROM KPDBA.COURSE_LECTURER CL, KPDBA.LECTURER_MASTER LM WHERE CL.LECT_ID = LM.LECT_ID GROUP BY QUERY_ID) LECT, (  SELECT QUERY_ID, COUNT (APP_USER_ID) APPLICANT_COUNT FROM KPDBA.COURSE_APPLICANT CA GROUP BY QUERY_ID) APPLI WHERE     CLOSE_FLAG = 'F' AND CM.COURSE_ID = CQ.COURSE_ID AND CQ.QUERY_ID = CQT.QUERY_ID(+) AND ONLINE_CLASS_FLAG = 'T' AND CQ.QUERY_ID = LECT.QUERY_ID(+) AND CQ.QUERY_ID = APPLI.QUERY_ID(+) AND NVL (INSTANT_FLAG, 'F') LIKE '{model.instantFlag}' UNION ALL SELECT 'ISO' DOC_TYPE, IM.DOC_CODE || '/' || IM.DOC_REVISION DOC_CODE, DOC_NAME, ICQ.QUERY_ID, PLACE, ICT.TIME_SEQ, LECT_EMP_ID || ' ' || EMP_FNAME || ' ' || EMP_LNAME EMP_NAME, APPLICANT_COUNT, ICT.COURSE_DATE QUERY_BEGIN, NVL ( ICT.COURSE_DATE + TO_NUMBER (DAY_HOUR) / 24 + TO_NUMBER (DAY_MIN) / 24 / 60, ICT.COURSE_DATE) QUERY_END, NVL (DAY_HOUR, 0) DAY_HOUR, NVL (DAY_MIN, 0) DAY_MIN, NVL (ICQ.INSTANT_FLAG, 'F') INSTANT_FLAG FROM KPDBA.ISO_MASTER IM, KPDBA.ISO_COURSE_QUERY ICQ, KPDBA.ISO_COURSE_TIME ICT, KPDBA.EMPLOYEE EMP, (  SELECT QUERY_ID, COUNT (APP_EMP_ID) APPLICANT_COUNT FROM KPDBA.ISO_COURSE_APPLICANT CA GROUP BY QUERY_ID) APPLI WHERE     IM.DOC_CODE = ICQ.DOC_CODE AND IM.DOC_REVISION = ICQ.DOC_REVISION AND ICQ.QUERY_ID = ICT.QUERY_ID(+) AND DOC_STATUS <> 'C' AND ACCEPT_HIST_FLAG = 'T' AND EMP.EMP_ID = LECT_EMP_ID AND IM.ONLINE_CLASS_FLAG = 'T' AND ICQ.QUERY_ID = APPLI.QUERY_ID(+) AND NVL (INSTANT_FLAG, 'F') LIKE '{model.instantFlag}'";
            var response = await new DataContext().GetResultDapperAsyncDynamic(DataBaseHostEnum.KPR, query);
            var result = _mapper.Map<IEnumerable<ResponseCourseDetails>>(response);
            var results = result as List<ResponseCourseDetails>;
            var resultReal = _mapper.Map<List<ResponseCourseDetails>, List<ResultCourseDetails>>(results);
            return resultReal;
        }

        [HttpPost("getApplicants")]
        public async Task<List<ListApplicantResult>> GetApplicants(RequestApplicants model)
        {
            List<ListApplicantResult> listApplicantResul = new List<ListApplicantResult> { };
            string queryDepartment = $"SELECT COUNT(1) COUNT, UNIT_ID, UNIT_DESC FROM(SELECT DOC_TYPE, QUERY_ID, TIME_SEQ, APP_EMP_ID, EMP_NAME, TRAINING_FLAG, APPLICANT.UNIT_ID, UNIT_DESC FROM    (SELECT 'ISO' DOC_TYPE, ICT.QUERY_ID, ICT.TIME_SEQ, CA.APP_EMP_ID, EMP.UNIT_ID, EMP_FNAME || ' ' || EMP_LNAME EMP_NAME, NVL (CQA.TRAINING_FLAG, 'F') TRAINING_FLAG FROM KPDBA.ISO_COURSE_APPLICANT CA, KPDBA.ISO_COURSE_TIME ICT, KPDBA.COURSE_QUERY_ATTN CQA, KPDBA.EMPLOYEE EMP WHERE     CA.APP_EMP_ID = EMP.EMP_ID AND CA.QUERY_ID = ICT.QUERY_ID AND ICT.QUERY_ID = CQA.QUERY_ID(+) AND ICT.TIME_SEQ = CQA.TIME_SEQ(+) UNION ALL SELECT 'COURSE' DOC_TYPE, CA.QUERY_ID, CQT.TIME_SEQ, APP_USER_ID, EMP.UNIT_ID, EMP_FNAME || ' ' || EMP_LNAME EMP_NAME, NVL (CQA.TRAINING_FLAG, 'F') TRAINING_FLAG FROM KPDBA.COURSE_APPLICANT CA, KPDBA.COURSE_QUERY_TIME CQT, KPDBA.COURSE_QUERY_ATTN CQA, KPDBA.EMPLOYEE EMP WHERE     CA.APP_USER_ID = EMP.EMP_ID AND CA.QUERY_ID = CQT.QUERY_ID AND CQT.QUERY_ID = CQA.QUERY_ID(+) AND CQT.TIME_SEQ = CQA.TIME_SEQ(+)) APPLICANT JOIN (SELECT UNIT_ID, UNIT_DESC FROM KPDBA.UNIT) UNT ON (APPLICANT.UNIT_ID = UNT.UNIT_ID) WHERE QUERY_ID = '{model.queryID}' AND TIME_SEQ = '{ model.timeSeq}') GROUP BY UNIT_ID,UNIT_DESC";
            var responseDepartment = await new DataContext().GetResultDapperAsyncDynamic(DataBaseHostEnum.KPR, queryDepartment);
            var resultDepartment = _mapper.Map<IEnumerable<Department>>(responseDepartment);
            foreach (Department item in resultDepartment)
            {
                ListApplicantResult applicantResul = new ListApplicantResult { };
                applicantResul.departmentID = item.UNIT_ID;
                applicantResul.department = item.UNIT_DESC;
                string query = $"SELECT DOC_TYPE, QUERY_ID, TIME_SEQ, APP_EMP_ID, EMP_NAME, TRAINING_FLAG FROM    (SELECT 'ISO' DOC_TYPE, ICT.QUERY_ID, ICT.TIME_SEQ, CA.APP_EMP_ID, EMP.UNIT_ID, EMP_FNAME || ' ' || EMP_LNAME EMP_NAME, NVL (CQA.TRAINING_FLAG, 'F') TRAINING_FLAG FROM KPDBA.ISO_COURSE_APPLICANT CA, KPDBA.ISO_COURSE_TIME ICT, KPDBA.COURSE_QUERY_ATTN CQA, KPDBA.EMPLOYEE EMP WHERE     CA.APP_EMP_ID = EMP.EMP_ID AND CA.QUERY_ID = ICT.QUERY_ID AND ICT.QUERY_ID = CQA.QUERY_ID(+) AND ICT.TIME_SEQ = CQA.TIME_SEQ(+) UNION ALL SELECT 'COURSE' DOC_TYPE, CA.QUERY_ID, CQT.TIME_SEQ, APP_USER_ID, EMP.UNIT_ID, EMP_FNAME || ' ' || EMP_LNAME EMP_NAME, NVL (CQA.TRAINING_FLAG, 'F') TRAINING_FLAG FROM KPDBA.COURSE_APPLICANT CA, KPDBA.COURSE_QUERY_TIME CQT, KPDBA.COURSE_QUERY_ATTN CQA, KPDBA.EMPLOYEE EMP WHERE     CA.APP_USER_ID = EMP.EMP_ID AND CA.QUERY_ID = CQT.QUERY_ID AND CQT.QUERY_ID = CQA.QUERY_ID(+) AND CQT.TIME_SEQ = CQA.TIME_SEQ(+)) APPLICANT JOIN (SELECT UNIT_ID FROM KPDBA.UNIT) UNT ON (APPLICANT.UNIT_ID = UNT.UNIT_ID) WHERE QUERY_ID = '{model.queryID}' AND TIME_SEQ = '{model.timeSeq}' AND APPLICANT.UNIT_ID = '{item.UNIT_ID}'";
                var response = await new DataContext().GetResultDapperAsyncDynamic(DataBaseHostEnum.KPR, query);
                var result = _mapper.Map<IEnumerable<ApplicantDetailResponse>>(response);
                var results = result as List<ApplicantDetailResponse>;
                var resultReal = _mapper.Map<List<ApplicantDetailResponse>, List<ApplicantDetailResult>>(results);
                applicantResul.listApplicant = resultReal;
                listApplicantResul.Add(applicantResul);
            }
            return listApplicantResul;
        }
        [HttpPost("getQrcode")]
        public String PostTModel(RequestApplicants model)
        {
            if (model.queryID != string.Empty && model.queryID != null)
            {
                var stringBase = new GenerateQrcode().generateQrcode(model.queryID);
                return stringBase;
            }
            else
            {
                return "String is emty!";
            }
        }

    }
}