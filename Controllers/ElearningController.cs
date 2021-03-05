using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PTCwebApi;
using webAPI.Models.Elearning;
using PTCwebApi.Models.Elearning;
using webAPI.Security;
using Microsoft.AspNetCore.Hosting;
using webAPI.Models.Elearning.configs;
using webAPI.Methods.Elearning;
using System.Text;
using System.Linq;
using PTCwebApi.Interfaces;
using PTCwebApi.Models.ProfilesModels;
using System.IO;

namespace webAPI.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ElearningController : ControllerBase
    {
        private readonly IWebHostEnvironment _environment;
        private readonly IMapper _mapper;
        private readonly IJwtGenerator _jwtGenerator;
        public ElearningController(IMapper mapper, IWebHostEnvironment environment, IJwtGenerator jwtGenerator)
        {
            _mapper = mapper;
            _environment = environment;
            _jwtGenerator = jwtGenerator;
        }

        [HttpGet("")]
        public ActionResult<String> GetTModels()
        {
            string curr_dir = Directory.GetCurrentDirectory();
            return curr_dir;
        }

        [HttpPost("getCourses")]
        public async Task<dynamic> GetCourses(RequestCourses model)
        {
            string org;
            if (model.userID != "%")
            {
                UserProfile userProfile = _jwtGenerator.DecodeToken(model.token);
                org = userProfile.org;
                model.userID = userProfile.userID;
            }
            // string curr_dir = Directory.GetCurrentDirectory() + @"\configs";
            // string q = System.IO.File.ReadAllText(@$"{curr_dir}\Q_GET_COURE.txt");
            string q = new ElearnigQueryConfig().Q_GET_COURE;
            string que = q.Replace(":as_instant_flag", $"'{model.instantFlag}'");
            string query = que.Replace(":as_user_id", $"'{model.userID}'");
            var response = await new DataContext().GetResultDapperAsyncDynamic(DataBaseHostEnum.KPR, query);
            var result = _mapper.Map<IEnumerable<ResponseCourseDetails>>(response);
            var results = result as List<ResponseCourseDetails>;
            var resultReal = _mapper.Map<List<ResponseCourseDetails>, List<ResultCourseDetails>>(results);
            foreach (var item in resultReal)
            {
                item.day = item.queryBegin.Split(" ")[0].ToString();
            }
            IEnumerable<GroupCourseDetails> groupDay = resultReal
            .GroupBy(x => new { x.day })
            .Select(y => new GroupCourseDetails()
            {
                day = y.Key.day,
                items = y.ToList()
            }
            );

            return groupDay;
        }

        [HttpPost("getApplicants")]
        public async Task<List<ListApplicantResult>> GetApplicants(RequestApplicants model)
        {
            // byte[] data = Convert.FromBase64String(model.queryID);
            // model.queryID = Encoding.UTF8.GetString(data);
            List<ListApplicantResult> listApplicantResul = new List<ListApplicantResult> { };
            string qD = new ElearnigQueryConfig().Q_GET_DEPARTMENT;
            string queryDepartment = qD.Replace(":as_query_id", $"'{model.queryID}'").Replace(":ai_time_seq", $"'{model.timeSeq}'");
            var responseDepartment = await new DataContext().GetResultDapperAsyncDynamic(DataBaseHostEnum.KPR, queryDepartment);
            var resultDepartment = _mapper.Map<IEnumerable<Department>>(responseDepartment);
            foreach (Department item in resultDepartment)
            {
                ListApplicantResult applicantResul = new ListApplicantResult { };
                applicantResul.departmentID = item.UNIT_ID;
                applicantResul.department = item.UNIT_DESC;
                string q = new ElearnigQueryConfig().Q_GET_APPLICANT;
                string query = q.Replace(":as_query_id", $"'{model.queryID}'").Replace(":ai_time_seq", $"'{model.timeSeq}'").Replace(":unit_id", $"'{ item.UNIT_ID}'");
                // string query = $"SELECT DOC_TYPE, QUERY_ID, TIME_SEQ, APP_EMP_ID, EMP_NAME, TRAINING_FLAG FROM    (SELECT 'ISO' DOC_TYPE, ICT.QUERY_ID, ICT.TIME_SEQ, CA.APP_EMP_ID, EMP.UNIT_ID, EMP_FNAME || ' ' || EMP_LNAME EMP_NAME, NVL (CQA.TRAINING_FLAG, 'F') TRAINING_FLAG FROM KPDBA.ISO_COURSE_APPLICANT CA, KPDBA.ISO_COURSE_TIME ICT, KPDBA.COURSE_QUERY_ATTN CQA, KPDBA.EMPLOYEE EMP WHERE     CA.APP_EMP_ID = EMP.EMP_ID AND CA.QUERY_ID = ICT.QUERY_ID AND ICT.QUERY_ID = CQA.QUERY_ID(+) AND ICT.TIME_SEQ = CQA.TIME_SEQ(+) UNION ALL SELECT 'COURSE' DOC_TYPE, CA.QUERY_ID, CQT.TIME_SEQ, APP_USER_ID, EMP.UNIT_ID, EMP_FNAME || ' ' || EMP_LNAME EMP_NAME, NVL (CQA.TRAINING_FLAG, 'F') TRAINING_FLAG FROM KPDBA.COURSE_APPLICANT CA, KPDBA.COURSE_QUERY_TIME CQT, KPDBA.COURSE_QUERY_ATTN CQA, KPDBA.EMPLOYEE EMP WHERE     CA.APP_USER_ID = EMP.EMP_ID AND CA.QUERY_ID = CQT.QUERY_ID AND CQT.QUERY_ID = CQA.QUERY_ID(+) AND CQT.TIME_SEQ = CQA.TIME_SEQ(+)) APPLICANT JOIN (SELECT UNIT_ID FROM KPDBA.UNIT) UNT ON (APPLICANT.UNIT_ID = UNT.UNIT_ID) WHERE QUERY_ID = '{model.queryID}' AND TIME_SEQ = '{model.timeSeq}' AND APPLICANT.UNIT_ID = '{item.UNIT_ID}'";
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
        public SetQrCode GetQrcode(RequestApplicants model)
        {
            SetQrCode response = new SetQrCode();
            if (model.queryID != string.Empty && model.queryID != null)
            {
                var stringBase = new GenerateQrcode().generateQrcode(model.queryID);
                response.qrCode = stringBase;
                return response;
            }
            else
            {
                response.qrCode = "";
                return response;
            }
        }

        [HttpPost("setCheck")]
        public async Task<dynamic> SetCheck(SetCheckName model)
        {
            ResponeError error = new ResponeError { };
            if (model.token != null)
            {
                UserProfile userProfile = _jwtGenerator.DecodeToken(model.token);
                string org = userProfile.org;
                string userID = userProfile.userID;

                string qCheck = new ElearnigQueryConfig().Q_CHECK_APPLICANT_ID;
                string qCheckEmpID = qCheck.Replace(":AS_QUERY_ID", $"'{model.queryID}'")
                    .Replace(":AI_TIME_SEQ", $"'{model.timeSeq}'")
                    .Replace(":APP_EMP_ID", $"'{model.appEmpID}'");
                var resCheck = await new DataContext().GetResultDapperAsyncDynamic(DataBaseHostEnum.KPR, qCheckEmpID);
                decimal r = (resCheck as List<dynamic>).Count;
                if (r != 0)
                {
                    var rC = (resCheck as List<dynamic>)[0].TRAINING_FLAG;
                    var empID = (resCheck as List<dynamic>)[0].APP_EMP_ID;
                    var empName = (resCheck as List<dynamic>)[0].EMP_NAME;
                    var empDepartment = (resCheck as List<dynamic>)[0].UNIT_DESC;

                    if (model.appEmpID != null && rC == "F")
                    {
                        string q = new ElearnigQueryConfig().I_SET_CHECK;
                        string query = q.Replace(":as_query_id", $"'{model.queryID}'")
                        .Replace(":ai_time_seq", $"'{model.timeSeq}'")
                        .Replace(":app_emp_id", $"'{model.appEmpID}'")
                        .Replace(":training_flag", $"'{model.trainingFlag}'")
                        .Replace(":org", $"'{org}'")
                        .Replace(":user_id", $"'{userID}'");
                        var response = await new DataContext().GetResultDapperAsyncDynamic(DataBaseHostEnum.KPR, query);
                        error.stateError = false;
                        error.cuzError = "Success";
                        error.empID = empID;
                        error.empName = (empName as string).Split(' ')[1] + " " + (empName as string).Split(' ')[2];
                        error.empDepartment = empDepartment;
                        return error; ;
                    }
                    else
                    {
                        error.stateError = true;
                        error.cuzError = "รายชื่อนี้ลงทะเบียนแล้ว";
                        return error;
                    }
                }
                else
                {
                    error.stateError = true;
                    error.cuzError = "ไม่สามารถเช็คชื่อคอร์สนี้ได้ กรุณาติดต่อเจ้าหน้าที่";
                    return error;
                }
            }
            else
            {
                error.stateError = true;
                error.cuzError = "Token empty!!";
                return error;
            }
        }

        [HttpGet("getAllLecturer")]
        public async Task<dynamic> GetAllLecture()
        {
            string query = new ElearnigQueryConfig().Q_GET_ALL_LECTURE;
            var response = await new DataContext().GetResultDapperAsyncDynamic(DataBaseHostEnum.KPR, query);
            var result = _mapper.Map<IEnumerable<GetAllLectureMap>>(response);
            var results = result as List<GetAllLectureMap>;
            var resultReal = _mapper.Map<List<GetAllLectureMap>, List<GetAllLecture>>(results);

            return resultReal;
        }

        [HttpPost("getAllApplicant")]
        public async Task<dynamic> GetAllApplicant(PostModelAllApplcant model)
        {
            string query = new ElearnigQueryConfig().Q_GET_ALL_APPLICANT;
            string querys = query.Replace(":AS_QUERY_ID", $"'{model.queryID}'");
            var response = await new DataContext().GetResultDapperAsyncDynamic(DataBaseHostEnum.KPR, querys);
            var result = _mapper.Map<IEnumerable<GetAllApplicantMap>>(response);
            var results = result as List<GetAllApplicantMap>;
            var resultReal = _mapper.Map<List<GetAllApplicantMap>, List<GetAllApplicant>>(results);
            var groupPosID = resultReal
           .GroupBy(w => new { w.posID, w.posDESC })
           .Select(x => new
           {
               posID = x.Key.posID,
               posDESC = x.Key.posDESC,
               completed = x.Where(r => !r.selectedFlag).Count() <= 0,
               listRole = x.GroupBy(y => new { y.roleID, y.roleDESC })
               .Select(z => new
               {
                   roleID = z.Key.roleID,
                   roleDESC = z.Key.roleDESC,
                   completed = z.Where(r => !r.selectedFlag).Count() <= 0,
                   listApplicant = z.ToList()
               }
               ).ToList()
           }
           );


            return groupPosID;
        }
        [HttpPost("getCourseForm")]
        public async Task<dynamic> PostTModel(RequestCourses model)
        {
            string q = new ElearnigQueryConfig().Q_GET_COURE;
            string query = q.Replace(":as_instant_flag", $"'{model.instantFlag}'").Replace(":as_user_id", $"'{model.userID}'");
            var response = await new DataContext().GetResultDapperAsyncDynamic(DataBaseHostEnum.KPR, query);
            var result = _mapper.Map<IEnumerable<ResponseCourseDetails>>(response);
            var results = result as List<ResponseCourseDetails>;
            var resultReal = _mapper.Map<List<ResponseCourseDetails>, List<ResultCourseDetails>>(results);
            return resultReal;
        }

        [HttpPost("setNewCourse")]
        public async Task<StateLectError> SetLecturer(LecturerForms model)
        {
            StateLectError stateError = new StateLectError();
            List<string> insertQuery = new List<string>();
            if (model.token != null && model.token != "")
            {
                var setLecturer = model.setLecturer;
                UserProfile userProfile = _jwtGenerator.DecodeToken(model.token);
                var org = userProfile.org;
                var userID = userProfile.userID;
                if (setLecturer.queryID == "New")
                {
                    string query = new ElearnigQueryConfig().Q_GET_QUERY_ID;
                    var response = await new DataContext().GetResultDapperAsyncDynamic(DataBaseHostEnum.KPR, query);
                    setLecturer.queryID = (response as List<dynamic>)[0].NEW_QUERY_ID;
                }
                string queryCSEQ = new ElearnigQueryConfig().Q_CHECK_TIME_SEQ_BY_ID;
                string queryCSEQn = queryCSEQ.Replace(":AS_QUERY_ID", $"'{setLecturer.queryID}'");
                var responseCSEQ = await new DataContext().GetResultDapperAsyncDynamic(DataBaseHostEnum.KPR, queryCSEQn);
                decimal numTimeSeq = (responseCSEQ as List<dynamic>)[0].COUN;
                // setLecturer.timeSeq = numTimeSeq.ToString();
                var listDate = model.setLecturer.listDate;
                for (int i = 0; i < listDate.Count(); i++)
                {
                    //todo update
                    if (i == 0 && setLecturer.courseID != "New")
                    {
                        if (listDate[i].dayHr != "" && listDate[i].dayMin != "")
                        {
                            string queryUCQT = new ElearnigQueryConfig().U_COURSE_QUERY_TIME;
                            var _timeSeq = setLecturer.timeSeq;
                            var beginDate = $"{listDate[i].day} {listDate[i].startTime}:00";
                            string queryUCQTn = queryUCQT
                             .Replace(":AS_QUERY_ID", $"'{setLecturer.queryID}'")
                             .Replace(":AI_SEQ", $"'{_timeSeq.ToString()}'")
                             .Replace(":AD_BEGIN_DATE", $"'{beginDate}'")
                             .Replace(":AS_SUM_HOUR", $"'{listDate[i].dayHr}'")
                             .Replace(":AS_SUM_MIN", $"'{listDate[i].dayMin}'");
                            insertQuery.Add(queryUCQTn);
                            string queryUCQD = new ElearnigQueryConfig().U_COURSE_QUERY_DATE;
                            string queryUCQDn = queryUCQD
                                .Replace(":AS_UP_ORG_ID", $"'{org}'")
                                .Replace(":AS_UP_USER_ID", $"'{userID}'")
                                .Replace(":AD_BEGIN_DATE", $"'{listDate[i].day} {listDate[i].startTime}:00'")
                                .Replace(":AD_END_DATE", $"'{listDate[i].day} {listDate[i].endTime}:00'")
                                .Replace(":AS_QUERY_ID", $"'{setLecturer.queryID}'");
                            insertQuery.Add(queryUCQDn);
                        }
                        else
                        {
                            var _startDate = listDate[i].day + " " + listDate[i].startTime + ":00";
                            var _endDate = listDate[i].day + " " + listDate[i].endTime + ":00";
                            string queryUCQT = $"UPDATE KPDBA.COURSE_QUERY_TIME SET COURSE_DATE = TO_DATE ('{_startDate}', 'dd/mm/yyyy hh24:mi:ss') WHERE QUERY_ID = '{setLecturer.queryID}' AND TIME_SEQ = TO_NUMBER('{setLecturer.timeSeq}')";
                            insertQuery.Add(queryUCQT);

                            string queryUCQD = new ElearnigQueryConfig().U_COURSE_QUERY_DATE;
                            string queryUCQDn = queryUCQD
                                .Replace(":AS_UP_ORG_ID", $"'{org}'")
                                .Replace(":AS_UP_USER_ID", $"'{userID}'")
                                .Replace(":AD_BEGIN_DATE", $"'{_startDate}'")
                                .Replace(":AD_END_DATE", $"'{_endDate}'")
                                .Replace(":AS_QUERY_ID", $"'{setLecturer.queryID}'");
                            insertQuery.Add(queryUCQDn);
                        }
                    }
                    else
                    {
                        if (listDate[i].dayHr != "" && listDate[i].dayMin != "")
                        {
                            string queryIQT = new ElearnigQueryConfig().I_COURSE_QUERY_TIME;
                            var _timeSeq = numTimeSeq + i;
                            var beginDate = $"{listDate[i].day} {listDate[i].startTime}:00";
                            string queryIQTime = queryIQT
                             .Replace(":AS_QUERY_ID", $"'{setLecturer.queryID}'")
                             .Replace(":AI_SEQ", $"'{_timeSeq.ToString()}'")
                             .Replace(":AD_BEGIN_DATE", $"'{beginDate}'")
                             .Replace(":AS_SUM_HOUR", $"'{listDate[i].dayHr}'")
                             .Replace(":AS_SUM_MIN", $"'{listDate[i].dayMin}'");
                            insertQuery.Add(queryIQTime);
                        }
                    }
                }
                var courseId = "";
                if (setLecturer.lectList != null && setLecturer.lectList.Count != 0)
                {
                    foreach (var item in setLecturer.lectList)
                    {
                        var resultStore = new StoreConnectionElearning(_mapper).ElearningSetLecturer(empType: item.type, empID: item.lectID, courseDESC: setLecturer.courseDESC, userLogin: userID);
                        var results = _mapper.Map<IEnumerable<SetFormStoreLecture>>(resultStore.Result);
                        List<SetFormStoreLecture> resultReal = results as List<SetFormStoreLecture>;
                        SetFormStoreLecture resultLect = resultReal[0];
                        courseId = resultLect.COURSE_ID;
                        string queryICL = new ElearnigQueryConfig().I_COURSE_LECTURER;
                        string queryICLn = queryICL
                            .Replace(":AS_QUERY_ID", $"'{setLecturer.queryID}'")
                            .Replace(":AS_LECT_ID", $"'{resultLect.LECT_ID}'")
                            .Replace(":AS_USER_LOGIN", $"'{userID}'");
                        if (resultLect.RESULT == "F")
                        {
                            stateError.stateError = true;
                            stateError.messageError = resultLect.ERR_TEXT;
                            return stateError;
                        }
                        else
                        {
                            insertQuery.Add(queryICLn);
                        }
                    }
                }
                if (setLecturer.listApplicant != null)
                {
                    foreach (var applic in setLecturer.listApplicant)
                    {
                        string queryQAD = new ElearnigQueryConfig().Q_APPLICANT_DETAIL;
                        string queryQADs = queryQAD.Replace(":APP_USER_ID", $"'{applic.empID}'");
                        var responseQAD = await new DataContext().GetResultDapperAsyncDynamic(DataBaseHostEnum.KPR, queryQADs);
                        var resultQAD = _mapper.Map<IEnumerable<ApplicantDetail>>(responseQAD);
                        var resultQADs = resultQAD as List<ApplicantDetail>;

                        string queryICA = new ElearnigQueryConfig().I_COURSE_APPLICANT;
                        string queryICAs = queryICA
                            .Replace(":AS_QUERY_ID", $"'{setLecturer.queryID}'")
                            .Replace(":APP_USER_ID", $"'{applic.empID}'")
                            .Replace(":APP_POS_ID", $"'{resultQADs[0].POS_ID}'")
                            .Replace(":APP_LEVEL_ID", $"'{resultQADs[0].CUR_LEVEL}'")
                            .Replace(":APP_UNIT_ID", $"'{resultQADs[0].UNIT_ID}'")
                            .Replace(":APP_SECT_ID", $"'{resultQADs[0].SECT_ID}'")
                            .Replace(":APP_DEPT_ID", $"'{resultQADs[0].DEPT_ID}'")
                            .Replace(":ORG_USER_LOGIN", $"'{org}'")
                            .Replace(":AS_USER_LOGIN", $"'{userID}'")
                            .Replace(":APP_ROLE_ID", $"'{resultQADs[0].ROLE_ID}'");
                        //! INSERT COURSE APPLICANT
                        insertQuery.Add(queryICAs);
                    }
                }
                var resultInsert = await new DataContext().ExecuteDapperMultiAsync(DataBaseHostEnum.KPR, insertQuery);
                if (model.setLecturer.courseID == "New")
                {
                    string queryQSCD = new ElearnigQueryConfig().Q_SORT_COURSE_DATE;
                    string queryQSCDs = queryQSCD.Replace(":AS_QUERY_ID", $"'{setLecturer.queryID}'");
                    var responseQSCD = await new DataContext().GetResultDapperAsyncDynamic(DataBaseHostEnum.KPR, queryQSCDs);
                    int countTime = (responseQSCD as List<dynamic>).Count;
                    string _beginDate = (responseQSCD as List<dynamic>)[0].COURSE_DATE.ToString();
                    string _endDate = (responseQSCD as List<dynamic>)[countTime - 1].COURSE_DATE.ToString();

                    string _beginTime = _beginDate.Split(" ")[1].Substring(0, 5);
                    string _endTime = _endDate.Split(" ")[1].Substring(0, 5);

                    string queryICQ = new ElearnigQueryConfig().I_COURSE_QUERY;
                    string queryICQn = queryICQ
                        .Replace(":AS_QUERY_ID", $"'{setLecturer.queryID}'")
                        .Replace(":AS_COURSE_ID", $"'{courseId}'")
                        .Replace(":AD_BEGIN_DATE", $"'{_beginDate}'")
                        .Replace(":AD_END_DATE", $"'{_endDate}'")
                        .Replace(":AS_PLACE", $"'{setLecturer.place}'")
                        .Replace(":AS_REMARK", $"'{setLecturer.remark}'")
                        .Replace(":AS_USER_LOGIN", $"'{userID}'")
                        .Replace(":AS_BEGIN_TIME", $"'{_beginTime}'")
                        .Replace(":AS_END_TIME", $"'{_endTime}'")
                        .Replace(":AS_SUM_HOUR", $"'{setLecturer.dayHour}'")
                        .Replace(":AS_SUM_MIN", $"'{setLecturer.dayMin}'");
                    //! INSERT NEW COURSE
                    var responseICQ = await new DataContext().GetResultDapperAsyncDynamic(DataBaseHostEnum.KPR, queryICQn);

                    string queryUNQIR = new ElearnigQueryConfig().U_NEW_QUERY_ID_RUNNING;
                    string queryUNQIRn = queryUNQIR.Replace(":NEW_QUERY_ID", $"'{setLecturer.queryID}'");
                    var responseUNQIR = await new DataContext().GetResultDapperAsyncDynamic(DataBaseHostEnum.KPR, queryUNQIRn);

                }
                else
                {
                    List<string> updateQuery = new List<string>();
                    string _beginDate;
                    string _endDate;
                    // string _beginTime = setLecturer.queryBegin.Split(" ")[1].Substring(0, 5);
                    // string _endTime = setLecturer.queryEnd.Split(" ")[1].Substring(0, 5);
                    string queryUQM = new ElearnigQueryConfig().U_COURSE_MASTER;
                    string queryUQMn = queryUQM
                        .Replace(":AS_UP_ORG_ID", $"'{org}'")
                        .Replace(":AS_UP_USER_ID", $"'{userID}'")
                        .Replace(":AS_COURSE_DESC", $"'{setLecturer.courseDESC}'")
                        .Replace(":AS_COURSE_ID", $"'{setLecturer.courseID}'");
                    //! INSERT NEW COURSE
                    updateQuery.Add(queryUQMn);

                    if (setLecturer.listDate != null)
                    {
                        _beginDate = setLecturer.queryBegin.Split(" ")[1].Substring(0, 5);
                        _endDate = setLecturer.queryEnd.Split(" ")[1].Substring(0, 5);
                    }

                    string queryUCQ = new ElearnigQueryConfig().U_COURSE_QUERY;
                    string queryUCQn = queryUCQ
                        .Replace(":AS_UP_ORG_ID", $"'{org}'")
                        .Replace(":AS_UP_USER_ID", $"'{userID}'")
                        .Replace(":AS_PLACE", $"'{setLecturer.place}'")
                        .Replace(":AS_REMARK", $"'{setLecturer.remark}'")
                        .Replace(":AS_QUERY_ID", $"'{setLecturer.queryID}'");
                    //! INSERT NEW COURSE
                    updateQuery.Add(queryUCQn);

                    var resultUpdate = await new DataContext().ExecuteDapperMultiAsync(DataBaseHostEnum.KPR, updateQuery);

                    // var responseUCQ = await new DataContext().GetResultDapperAsyncDynamic(DataBaseHostEnum.KPR, queryUCQn);
                }

                stateError.stateError = false;
                stateError.messageError = "Success!!";
                return stateError;
            }
            else
            {
                stateError.stateError = true;
                stateError.messageError = "Token is empty!!";
                return stateError;
            }
        }


        [HttpPost("getTitleCourses")]
        public async Task<dynamic> GetAllCourses()
        {
            var query = new ElearnigQueryConfig().Q_ALL_COURSE;
            var response = await new DataContext().GetResultDapperAsyncDynamic(DataBaseHostEnum.KPR, query);
            var result = _mapper.Map<IEnumerable<GetAllCoursesDB>>(response);
            var results = result as List<GetAllCoursesDB>;
            var resultReal = _mapper.Map<List<GetAllCoursesDB>, List<SetAllCoursesDB>>(results);
            List<SetItemCourses> _items = new List<SetItemCourses> { };
            foreach (SetAllCoursesDB item in resultReal)
            {
                SetItemCourses _item = new SetItemCourses
                {
                    docID = item.docID,
                    docRev = item.docRev,
                    docDesc = item.docDesc,
                    remark = item.remark
                };
                _items.Add(_item);
            }
            SetAllCourses _allCourse = new SetAllCourses { docType = "COURSE", items = _items };
            return _allCourse;
        }
        [HttpPost("getTitleISOs")]
        public async Task<dynamic> GetAllISOs()
        {
            var query = new ElearnigQueryConfig().Q_ALL_ISO;
            var response = await new DataContext().GetResultDapperAsyncDynamic(DataBaseHostEnum.KPR, query);
            var result = _mapper.Map<IEnumerable<GetAllISOsDB>>(response);
            var results = result as List<GetAllISOsDB>;
            var resultReal = _mapper.Map<List<GetAllISOsDB>, List<SetAllISOsDB>>(results);
            List<SetItemISOs> _items = new List<SetItemISOs> { };
            foreach (SetAllISOsDB item in resultReal)
            {
                SetItemISOs _item = new SetItemISOs
                {
                    docCode = item.docCode,
                    docRevision = item.docRevision,
                    docName = item.docName,
                    isoSTD = item.isoSTD
                };
                _items.Add(_item);
            }
            SetAllISOs _allISO = new SetAllISOs { docType = "ISO", items = _items };
            // return _allISO;
            return _allISO;
        }
        [HttpPost("getTopicDetail")]
        async public Task<dynamic> GetTopicDetail(RequestTopicDetail model)
        {
            var querySCT = new ElearnigQueryConfig().S_COURSE_TOPIC;
            var querySCTn = querySCT.Replace(":AD_COURSE_ID", $"'{model.courseID}'").Replace(":AD_COURSE_REVSION", $"'{model.courseRevision}'");
            var responseSCT = await new DataContext().GetResultDapperAsyncDynamic(DataBaseHostEnum.KPR, querySCTn);
            var resultSCT = _mapper.Map<IEnumerable<GetTopicGroup>>(responseSCT);
            var resultSCTas = resultSCT as List<GetTopicGroup>;
            var resultSCTs = _mapper.Map<List<GetTopicGroup>, List<SetTopicGroup>>(resultSCTas);

            List<TopicGroupDetail> topicGroupDetails = new List<TopicGroupDetail> { };
            foreach (SetTopicGroup group in resultSCTs)
            {
                TopicGroupDetail topicGroupDetail = new TopicGroupDetail { };
                topicGroupDetail.courseType = group.courseType;
                topicGroupDetail.courseID = group.courseID;
                topicGroupDetail.courseRevision = group.courseRevision;
                topicGroupDetail.groupID = group.groupID;
                topicGroupDetail.groupOrder = group.groupOrder;
                topicGroupDetail.groupName = group.groupName;
                topicGroupDetail.topics = new List<TopicDetail> { };
                var querySTM = new ElearnigQueryConfig().S_TOPIC_MASTER;
                var querySTMn = querySTM.Replace(":AD_PARENT_TOPIC_ID", $"'{group.groupID}'");
                var responseSTM = await new DataContext().GetResultDapperAsyncDynamic(DataBaseHostEnum.KPR, querySTMn);
                var resultSTM = _mapper.Map<IEnumerable<GetTopic>>(responseSTM);
                var resultSTMas = resultSTM as List<GetTopic>;
                var resultSTMs = _mapper.Map<List<GetTopic>, List<SetTopic>>(resultSTMas);
                List<TopicDetail> topicDetails = new List<TopicDetail> { };
                foreach (SetTopic topic in resultSTMs)
                {
                    TopicDetail topicDetail = new TopicDetail { };
                    topicDetail.topicID = topic.topicID;
                    topicDetail.topicOrder = topic.topicOrder;
                    topicDetail.topicName = topic.topicName;
                    topicDetail.groupID = topic.groupID;
                    var querySDT = new ElearnigQueryConfig().S_DOC_TOPIC;
                    var querySDTn = querySDT.Replace(":AD_TOPIC_ID", $"'{topic.topicID}'");
                    var responseSDT = await new DataContext().GetResultDapperAsyncDynamic(DataBaseHostEnum.KPR, querySDTn);
                    var resultSDT = _mapper.Map<IEnumerable<GetDoc>>(responseSDT);
                    var resultSDTas = resultSDT as List<GetDoc>;
                    var resultSDTs = _mapper.Map<List<GetDoc>, List<SetDoc>>(resultSDTas);
                    topicDetail.docs = resultSDTs;
                    topicGroupDetail.topics.Add(topicDetail);
                }
                topicGroupDetails.Add(topicGroupDetail);
            }
            return topicGroupDetails;
        }

        [HttpPost("upLoadTopicMulti")]
        public Task<dynamic> PostTModel([FromForm] UpLoadGroupMultiple model)
        {
            List<string> insterQuerys = new List<string>();

            return null;
        }

        [HttpPost("upLoadGroup")]
        async public Task<StateUpload> uploadGroup(UpLoadGroup model)
        {
            StateUpload stateError = new StateUpload();
            if (model.token != null && model.token != "")
            {
                UserProfile userProfile = _jwtGenerator.DecodeToken(model.token);
                var org = userProfile.org;
                var userID = userProfile.userID;
                var cancelFlag = "F";
                if (model.groupID == "New")
                {
                    List<string> insertQroupNew = new List<string>();

                    var date = DateTime.Now.ToString("yyMM");
                    var querySCTM = new ElearnigQueryConfig().S_COUNT_TOPIC_MASTER;
                    var resultSCTM = await new DataContext().GetResultDapperAsyncDynamic(DataBaseHostEnum.KPR, querySCTM);
                    var countSCTM = (resultSCTM as List<dynamic>)[0].CON;
                    model.groupID = date + countSCTM.ToString("0000");

                    var parent = "";
                    var topicType = "C";

                    var queryITM = new ElearnigQueryConfig().I_TOPIC_MASTER;
                    var queryITMn = queryITM.Replace(":AD_TOPIC_ID", $"'{model.groupID}'")
                                            .Replace(":AD_TOPIC_NAME", $"'{model.groupName}'")
                                            .Replace(":AD_PARENT_TOPIC_ID", $"'{parent}'")
                                            .Replace(":AD_TOPIC_TYPE", $"'{topicType}'")
                                            .Replace(":AD_TOPIC_ORDER", $"'{model.groupOrder}'")
                                            .Replace(":AD_ORG", $"'{org}'")
                                            .Replace(":AD_USER_ID", $"'{userID}'")
                                            .Replace(":AD_CANCEL_FLAG", $"'{cancelFlag}'");
                    insertQroupNew.Add(queryITMn);

                    var courseType = "C";
                    var queryICT = new ElearnigQueryConfig().I_COURSE_TOPIC;
                    var queryICTn = queryICT.Replace(":AD_COURSE_ID", $"'{model.courseID}'")
                                            .Replace(":AD_COURSE_REVISION", $"'{model.courseRevision}'")
                                            .Replace(":AD_TOPIC_ID", $"'{model.groupID}'")
                                            .Replace(":AD_COURSE_TYPE", $"'{courseType}'")
                                            .Replace(":AD_TOPIC_ORDER", $"'{model.groupOrder}'")
                                            .Replace(":AD_ORG", $"'{org}'")
                                            .Replace(":AD_USER_ID", $"'{userID}'")
                                            .Replace(":AD_CANCEL_FLAG", $"'{cancelFlag}'");
                    insertQroupNew.Add(queryICTn);

                    var resultInsert = await new DataContext().ExecuteDapperMultiAsync(DataBaseHostEnum.KPR, insertQroupNew);

                    stateError.error = false;
                    stateError.messageError = "Upload group success.";
                    stateError.groupID = model.groupID;
                    return stateError;
                }
                else
                {
                    if (model.stateDelete) { cancelFlag = "T"; }
                    List<string> updateQroup = new List<string>();
                    var queryUGTM = new ElearnigQueryConfig().UG_TOPIC_MASTER;
                    var queryUGTMn = queryUGTM.Replace(":AD_TOPIC_NAME", $"'{model.groupName}'")
                                            .Replace(":AD_TOPIC_ORDER", $"'{model.groupOrder}'")
                                            .Replace(":AD_ORG_ID", $"'{org}'")
                                            .Replace(":AD_USER_ID", $"'{userID}'")
                                            .Replace(":AD_TOPIC_ID", $"'{model.groupID}'")
                                            .Replace(":AD_CANCEL_FLAG", $"'{cancelFlag}'");
                    updateQroup.Add(queryUGTMn);

                    var queryUGCT = new ElearnigQueryConfig().UG_COURSE_TOPIC;
                    var queryUGCTn = queryUGCT.Replace(":AD_TOPIC_ORDER", $"'{model.groupOrder}'")
                                           .Replace(":AD_ORG_ID", $"'{org}'")
                                           .Replace(":AD_USER_ID", $"'{userID}'")
                                           .Replace(":AD_TOPIC_ID", $"'{model.groupID}'")
                                           .Replace(":AD_COURSE_ID", $"'{model.courseID}'")
                                           .Replace(":AD_COURSE_REVSION", $"'{model.courseRevision}'")
                                           .Replace(":AD_CANCEL_FLAG", $"'{cancelFlag}'");
                    updateQroup.Add(queryUGCTn);

                    var resultUpdate = await new DataContext().ExecuteDapperMultiAsync(DataBaseHostEnum.KPR, updateQroup);

                    stateError.error = false;
                    stateError.messageError = "Update group success.";
                    stateError.groupID = model.groupID;
                    return stateError;
                }
            }
            else
            {
                stateError.error = true;
                stateError.messageError = "Token is empty!!!";
                return stateError;
            }
        }

        [HttpPost("upLoadTopic")]
        async public Task<StateUpload> upLoadTopic(UpLoadTopic model)
        {
            StateUpload stateError = new StateUpload();
            if (model.token != null && model.token != "")
            {
                UserProfile userProfile = _jwtGenerator.DecodeToken(model.token);
                var org = userProfile.org;
                var userID = userProfile.userID;
                var cancelFlag = "F";
                if (model.topicID == "New")
                {
                    var date = DateTime.Now.ToString("yyMM");
                    var querySCTM = new ElearnigQueryConfig().S_COUNT_TOPIC_MASTER;
                    var resultSCTM = await new DataContext().GetResultDapperAsyncDynamic(DataBaseHostEnum.KPR, querySCTM);
                    var countSCTM = (resultSCTM as List<dynamic>)[0].CON;
                    model.topicID = date + countSCTM.ToString("0000");

                    var topicType = "T";

                    var queryITM = new ElearnigQueryConfig().I_TOPIC_MASTER;
                    var queryITMn = queryITM.Replace(":AD_TOPIC_ID", $"'{model.topicID}'")
                                            .Replace(":AD_TOPIC_NAME", $"'{model.topicName}'")
                                            .Replace(":AD_PARENT_TOPIC_ID", $"'{model.groupID}'")
                                            .Replace(":AD_TOPIC_TYPE", $"'{topicType}'")
                                            .Replace(":AD_TOPIC_ORDER", $"'{model.topicOrder}'")
                                            .Replace(":AD_ORG", $"'{org}'")
                                            .Replace(":AD_USER_ID", $"'{userID}'")
                                            .Replace(":AD_CANCEL_FLAG", $"'{cancelFlag}'");

                    var result = await new DataContext().InsertResultDapperAsync(DataBaseHostEnum.KPR, queryITMn);
                    stateError.error = false;
                    stateError.messageError = "Upload topic success.";
                    stateError.topicID = model.topicID;
                    return stateError;
                }
                else
                {
                    if (model.stateDelete) { cancelFlag = "T"; }
                    var queryUGTM = new ElearnigQueryConfig().UT_TOPIC_MASTER;
                    var queryUGTMn = queryUGTM.Replace(":AD_TOPIC_NAME", $"'{model.topicName}'")
                                            .Replace(":AD_TOPIC_ORDER", $"'{model.topicOrder}'")
                                            .Replace(":AD_ORG_ID", $"'{org}'")
                                            .Replace(":AD_USER_ID", $"'{userID}'")
                                            .Replace(":AD_TOPIC_ID", $"'{model.topicID}'")
                                            .Replace(":AD_PARENT_TOPIC_ID", $"'{model.groupID}'")
                                            .Replace(":AD_CANCEL_FLAG", $"'{cancelFlag}'");

                    var result = await new DataContext().InsertResultDapperAsync(DataBaseHostEnum.KPR, queryUGTMn);
                    stateError.error = false;
                    stateError.messageError = "Update topic success.";
                    stateError.topicID = model.topicID;
                    return stateError;
                }
            }
            else
            {
                stateError.error = true;
                stateError.messageError = "Token is empty!!!";
                return stateError;
            }
        }

        [HttpPost("upLoadDoc"), DisableRequestSizeLimit]
        async public Task<Boolean> upLoadDoc([FromForm] UpLoadDoc model)
        {
            var formCollection = await Request.ReadFormAsync();

            StateUpload stateError = new StateUpload();
            if (model.token != null && model.token != "")
            {
                UserProfile userProfile = _jwtGenerator.DecodeToken(model.token);
                var org = userProfile.org;
                var userID = userProfile.userID;
                if (model.docID == "New")
                {
                    var date = DateTime.Now.ToString("yyMM");
                    var queryDID = new ElearnigQueryConfig().S_COUNT_COURSE_DOC_MASTER;
                    var resultDID = await new DataContext().GetResultDapperAsyncDynamic(DataBaseHostEnum.KPR, queryDID);
                    var countDID = (resultDID as List<dynamic>)[0].CON;
                    model.docID = date + countDID.ToString("00000000");
                    string cancelFlag = "F";
                    string emptyString = "";
                    switch (model.docType)
                    {
                        case "V":
                            List<string> insertQuery = new List<string>();
                            String rndStr = new UploadImageAndVideo(_environment).GetRandomCharacter();
                            string fileName = $"V{model.docID}-{rndStr}";
                            if (model.fileVideo != null)
                            {
                                string docPathOld = new UploadImageAndVideo(_environment).UploadFile
                                (
                                    queryID: model.courseID,
                                    folderType: "videos",
                                    fileName: fileName,
                                    file: model.fileVideo
                                );
                                model.docPath = docPathOld.Replace("\\", "/");
                            }
                            if (model.fileVideo != null)
                            {
                                string imgCoverPathOld = new UploadImageAndVideo(_environment).UploadFile
                                (
                                    queryID: model.courseID,
                                    folderType: "images",
                                    fileName: fileName,
                                    file: model.fileImg
                                );
                                model.videoCover = imgCoverPathOld.Replace("\\", "/");
                            }
                            string queryCDM = new ElearnigQueryConfig().I_COURSE_DOC_MASTER;
                            var queryCDMn = queryCDM.Replace(":AD_COURSE_DOC_ID", $"'{model.docID}'")
                                           .Replace(":AD_DOC_TYPE", $"'{model.docType}'")
                                           .Replace(":AD_DOC_NAME", $"'{model.docName}'")
                                           .Replace(":AD_DOC_PATH", $"'{model.docPath}'")
                                           .Replace(":AD_VIDEO_COVER", $"'{model.videoCover}'")
                                           .Replace(":AD_VIDEO_LENGTH", $"'{model.videoLength}'")
                                           .Replace(":AD_ORG_ID", $"'{org}'")
                                           .Replace(":AD_USER_ID", $"'{userID}'")
                                           .Replace(":AD_CANCEL_FLAG", $"'{cancelFlag}'");
                            insertQuery.Add(queryCDMn);
                            string queryCTD = new ElearnigQueryConfig().I_TOPIC_DOC;
                            var queryCTDn = queryCTD.Replace(":AD_TOPIC_ID", $"'{model.topicID}'")
                                           .Replace(":AD_COURSE_DOC_ID", $"'{model.docID}'")
                                           .Replace(":AD_DOC_ORDER", $"'{model.docOrder}'")
                                           .Replace(":AD_ORG_ID", $"'{org}'")
                                           .Replace(":AD_USER_ID", $"'{userID}'")
                                           .Replace(":AD_CANCEL_FLAG", $"'{cancelFlag}'");
                            insertQuery.Add(queryCTDn);
                            var resultInsertAll = await new DataContext().ExecuteDapperMultiAsync(DataBaseHostEnum.KPR, insertQuery);
                            return true;
                        case "T":
                            List<string> insertQueryT = new List<string>();
                            string queryCDMT = new ElearnigQueryConfig().I_COURSE_DOC_MASTER;
                            var queryCDMTn = queryCDMT.Replace(":AD_COURSE_DOC_ID", $"'{model.docID}'")
                                           .Replace(":AD_DOC_TYPE", $"'{model.docType}'")
                                           .Replace(":AD_DOC_NAME", $"'{model.docName}'")
                                           .Replace(":AD_DOC_PATH", $"'{model.docPath}'")
                                           .Replace(":AD_VIDEO_COVER", $"'{emptyString}'")
                                           .Replace(":AD_VIDEO_LENGTH", $"'{emptyString}'")
                                           .Replace(":AD_ORG_ID", $"'{org}'")
                                           .Replace(":AD_USER_ID", $"'{userID}'")
                                           .Replace(":AD_CANCEL_FLAG", $"'{cancelFlag}'");
                            insertQueryT.Add(queryCDMTn);
                            string queryCTDT = new ElearnigQueryConfig().I_TOPIC_DOC;
                            var queryCTDTn = queryCTDT.Replace(":AD_TOPIC_ID", $"'{model.topicID}'")
                                           .Replace(":AD_COURSE_DOC_ID", $"'{model.docID}'")
                                           .Replace(":AD_DOC_ORDER", $"'{model.docOrder}'")
                                           .Replace(":AD_ORG_ID", $"'{org}'")
                                           .Replace(":AD_USER_ID", $"'{userID}'")
                                           .Replace(":AD_CANCEL_FLAG", $"'{cancelFlag}'");
                            insertQueryT.Add(queryCTDTn);
                            var resultInsertAllT = await new DataContext().ExecuteDapperMultiAsync(DataBaseHostEnum.KPR, insertQueryT);
                            return true;
                        case "D":
                            List<string> insertQueryD = new List<string>();
                            String rndStrD = new UploadImageAndVideo(_environment).GetRandomCharacter();
                            string fileNameD = $"D{model.docID}-{rndStrD}";
                            if (model.fileDoc != null)
                            {
                                string docPathOldD = new UploadImageAndVideo(_environment).UploadFile
                                (
                                    queryID: model.courseID,
                                    folderType: "documents",
                                    fileName: fileNameD,
                                    file: model.fileDoc
                                );
                                model.docPath = docPathOldD.Replace("\\", "/");
                            }
                            string queryCDMD = new ElearnigQueryConfig().I_COURSE_DOC_MASTER;
                            var queryCDMDn = queryCDMD.Replace(":AD_COURSE_DOC_ID", $"'{model.docID}'")
                                           .Replace(":AD_DOC_TYPE", $"'{model.docType}'")
                                           .Replace(":AD_DOC_NAME", $"'{model.docName}'")
                                           .Replace(":AD_DOC_PATH", $"'{model.docPath}'")
                                           .Replace(":AD_VIDEO_COVER", $"'{emptyString}'")
                                           .Replace(":AD_VIDEO_LENGTH", $"'{emptyString}'")
                                           .Replace(":AD_ORG_ID", $"'{org}'")
                                           .Replace(":AD_USER_ID", $"'{userID}'")
                                           .Replace(":AD_CANCEL_FLAG", $"'{cancelFlag}'");
                            insertQueryD.Add(queryCDMDn);

                            string queryCTDD = new ElearnigQueryConfig().I_TOPIC_DOC;
                            var queryCTDDn = queryCTDD.Replace(":AD_TOPIC_ID", $"'{model.topicID}'")
                                           .Replace(":AD_COURSE_DOC_ID", $"'{model.docID}'")
                                           .Replace(":AD_DOC_ORDER", $"'{model.docOrder}'")
                                           .Replace(":AD_ORG_ID", $"'{org}'")
                                           .Replace(":AD_USER_ID", $"'{userID}'")
                                           .Replace(":AD_CANCEL_FLAG", $"'{cancelFlag}'");
                            insertQueryD.Add(queryCTDDn);
                            var resultInsertAllD = await new DataContext().ExecuteDapperMultiAsync(DataBaseHostEnum.KPR, insertQueryD);
                            return true;
                        default:
                            return false;
                    }
                }
                else
                {
                    string cancelFlag = "F";
                    switch (model.docType)
                    {
                        case "V":
                            if (model.stateDelete) { cancelFlag = "T"; }
                            String rndStr = new UploadImageAndVideo(_environment).GetRandomCharacter();
                            string fileName = $"V{model.docID}-{rndStr}";
                            if (model.fileVideo != null)
                            {
                                List<string> insertQuery = new List<string>();
                                string docPathOld = new UploadImageAndVideo(_environment).UploadFile
                                (
                                    queryID: model.courseID,
                                    folderType: "videos",
                                    fileName: fileName,
                                    file: model.fileVideo
                                );
                                model.docPath = docPathOld.Replace("\\", "/");
                            }
                            if (model.fileImg != null)
                            {
                                string imgCoverPathOld = new UploadImageAndVideo(_environment).UploadFile
                                (
                                    queryID: model.courseID,
                                    folderType: "images",
                                    fileName: fileName,
                                    file: model.fileImg
                                );
                                model.videoCover = imgCoverPathOld.Replace("\\", "/");
                            }
                            List<string> insertQueryV = new List<string>();

                            string queryCDMV = new ElearnigQueryConfig().U_COURSE_DOC_MASTER;
                            var queryCDMVn = queryCDMV.Replace(":AD_COURSE_DOC_ID", $"'{model.docID}'")
                                           .Replace(":AD_DOC_NAME", $"'{model.docName}'")
                                           .Replace(":AD_DOC_PATH", $"'{model.docPath}'")
                                           .Replace(":AD_VIDEO_COVER", $"'{model.videoCover}'")
                                           .Replace(":AD_VIDEO_LENGTH", $"'{model.videoLength}'")
                                           .Replace(":AD_ORG_ID", $"'{org}'")
                                           .Replace(":AD_USER_ID", $"'{userID}'")
                                           .Replace(":AD_CANCEL_FLAG", $"'{cancelFlag}'");
                            insertQueryV.Add(queryCDMVn);

                            string queryCTDV = new ElearnigQueryConfig().U_TOPIC_DOC;
                            var queryCTDVn = queryCTDV.Replace(":AD_COURSE_DOC_ID", $"'{model.docID}'")
                                           .Replace(":AD_TOPIC_ID", $"'{model.topicID}'")
                                           .Replace(":AD_DOC_ORDER", $"'{model.docOrder}'")
                                           .Replace(":AD_ORG_ID", $"'{org}'")
                                           .Replace(":AD_USER_ID", $"'{userID}'")
                                           .Replace(":AD_CANCEL_FLAG", $"'{cancelFlag}'");
                            insertQueryV.Add(queryCTDVn);
                            var resultInsertAllV = await new DataContext().ExecuteDapperMultiAsync(DataBaseHostEnum.KPR, insertQueryV);

                            // string queryCDMV = $"UPDATE KPDBA.COURSE_DOCUMENT_MASTER SET UPDATE_DATE = SYSDATE, UP_ORG_ID = TO_CHAR ('{org}'), UP_USER_ID = TO_CHAR ('{userID}'), DOC_NAME = '{model.docName}', DOC_PATH = '{model.docPath}', VIDEO_COVER = '{model.videoCover}', VIDEO_LENGTH = TO_NUMBER('{model.videoLength}') WHERE COURSE_DOC_ID = '{model.courseDocID}'";
                            // string queryCTDV = $"UPDATE KPDBA.COURSE_TOPIC_DOCUMENT SET UPDATE_DATE = SYSDATE, UP_ORG_ID = TO_CHAR ('{org}'), UP_USER_ID = TO_CHAR ('{userID}'), DOC_ORDER = TO_NUMBER('{model.docOrder}') WHERE COURSE_DOC_ID = '{model.courseDocID}' AND TOPIC_ID ='{model.topicID}' AND COURSE_ID = '{model.queryID}'";
                            return true;
                        case "T":
                            if (model.stateDelete) { cancelFlag = "T"; }
                            List<string> insertQueryT = new List<string>();
                            string queryCDMT = new ElearnigQueryConfig().U_COURSE_DOC_MASTER;
                            var queryCDMTn = queryCDMT.Replace(":AD_COURSE_DOC_ID", $"'{model.docID}'")
                                           .Replace(":AD_DOC_NAME", $"'{model.docName}'")
                                           .Replace(":AD_DOC_PATH", $"'{model.docPath}'")
                                           .Replace(":AD_VIDEO_COVER", $"''")
                                           .Replace(":AD_VIDEO_LENGTH", $"''")
                                           .Replace(":AD_ORG_ID", $"'{org}'")
                                           .Replace(":AD_USER_ID", $"'{userID}'")
                                           .Replace(":AD_CANCEL_FLAG", $"'{cancelFlag}'");
                            insertQueryT.Add(queryCDMTn);

                            string queryCTDT = new ElearnigQueryConfig().U_TOPIC_DOC;
                            var queryCTDTn = queryCTDT.Replace(":AD_COURSE_DOC_ID", $"'{model.docID}'")
                                           .Replace(":AD_TOPIC_ID", $"'{model.topicID}'")
                                           .Replace(":AD_DOC_ORDER", $"'{model.docOrder}'")
                                           .Replace(":AD_ORG_ID", $"'{org}'")
                                           .Replace(":AD_USER_ID", $"'{userID}'")
                                           .Replace(":AD_CANCEL_FLAG", $"'{cancelFlag}'");
                            insertQueryT.Add(queryCTDTn);
                            var resultInsertAllT = await new DataContext().ExecuteDapperMultiAsync(DataBaseHostEnum.KPR, insertQueryT);
                            // string queryCDMTo = $"UPDATE KPDBA.COURSE_DOCUMENT_MASTER SET UPDATE_DATE = SYSDATE, UP_ORG_ID = TO_CHAR ('{org}'), UP_USER_ID = TO_CHAR ('{userID}'), DOC_NAME = '{model.docName}', DOC_PATH = '{model.docPath}' WHERE COURSE_DOC_ID = '{model.courseDocID}'";
                            // string queryCTDTo = $"UPDATE KPDBA.COURSE_TOPIC_DOCUMENT SET UPDATE_DATE = SYSDATE, UP_ORG_ID = TO_CHAR ('{org}'), UP_USER_ID = TO_CHAR ('{userID}'), DOC_ORDER = TO_NUMBER('{model.docOrder}') WHERE COURSE_DOC_ID = '{model.courseDocID}' AND TOPIC_ID ='{model.topicID}' AND COURSE_ID = '{model.queryID}'";
                            return true;
                        case "D":
                            if (model.stateDelete) { cancelFlag = "T"; }
                            if (model.fileDoc != null)
                            {
                                String rndStrD = new UploadImageAndVideo(_environment).GetRandomCharacter();
                                string fileNameD = $"D{model.docID}-{rndStrD}";
                                string docPathOldD = new UploadImageAndVideo(_environment).UploadFile
                                (
                                    queryID: model.courseID,
                                    folderType: "documents",
                                    fileName: fileNameD,
                                    file: model.fileDoc
                                );
                                model.docPath = docPathOldD.Replace("\\", "/");

                            }
                            List<string> insertQueryD = new List<string>();

                            string queryCDMD = new ElearnigQueryConfig().U_COURSE_DOC_MASTER;
                            var queryCDMDn = queryCDMD.Replace(":AD_COURSE_DOC_ID", $"'{model.docID}'")
                                           .Replace(":AD_DOC_NAME", $"'{model.docName}'")
                                           .Replace(":AD_DOC_PATH", $"'{model.docPath}'")
                                           .Replace(":AD_VIDEO_COVER", $"''")
                                           .Replace(":AD_VIDEO_LENGTH", $"''")
                                           .Replace(":AD_ORG_ID", $"'{org}'")
                                           .Replace(":AD_USER_ID", $"'{userID}'")
                                           .Replace(":AD_CANCEL_FLAG", $"'{cancelFlag}'");
                            insertQueryD.Add(queryCDMDn);

                            string queryCTDD = new ElearnigQueryConfig().U_TOPIC_DOC;
                            var queryCTDDn = queryCTDD.Replace(":AD_COURSE_DOC_ID", $"'{model.docID}'")
                                           .Replace(":AD_TOPIC_ID", $"'{model.topicID}'")
                                           .Replace(":AD_DOC_ORDER", $"'{model.docOrder}'")
                                           .Replace(":AD_ORG_ID", $"'{org}'")
                                           .Replace(":AD_USER_ID", $"'{userID}'")
                                           .Replace(":AD_CANCEL_FLAG", $"'{cancelFlag}'");
                            insertQueryD.Add(queryCTDDn);
                            var resultInsertAllD = await new DataContext().ExecuteDapperMultiAsync(DataBaseHostEnum.KPR, insertQueryD);

                            // string queryCDMDo = $"UPDATE KPDBA.COURSE_DOCUMENT_MASTER SET UPDATE_DATE = SYSDATE, UP_ORG_ID = TO_CHAR ('{org}'), UP_USER_ID = TO_CHAR ('{userID}'), DOC_NAME = '{model.docName}', DOC_PATH = '{model.docPath}' WHERE COURSE_DOC_ID = '{model.courseDocID}'";
                            // string queryCTDDo = $"UPDATE KPDBA.COURSE_TOPIC_DOCUMENT SET UPDATE_DATE = SYSDATE, UP_ORG_ID = TO_CHAR ('{org}'), UP_USER_ID = TO_CHAR ('{userID}'), DOC_ORDER = TO_NUMBER('{model.docOrder}') WHERE COURSE_DOC_ID = '{model.courseDocID}' AND TOPIC_ID ='{model.topicID}' AND COURSE_ID = '{model.queryID}'";
                            return true;
                        default:
                            return false;
                    }
                }
            }
            else
            {
                return false;
            }
        }
    }
}