using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using PTCwebApi;
using PTCwebApi.Interfaces;
using PTCwebApi.Models.Elearning;
using PTCwebApi.Models.ProfilesModels;
using webAPI.Models.Elearning;
using webAPI.Models.Elearning.configs;
using webAPI.Security;

namespace webAPI.Methods.Elearning
{
    public class CheckUser
    {
        private readonly IWebHostEnvironment _environment;
        private readonly IMapper _mapper;
        private readonly IJwtGenerator _jwtGenerator;
        public CheckUser(IMapper mapper, IWebHostEnvironment environment, IJwtGenerator jwtGenerator)
        {
            _mapper = mapper;
            _environment = environment;
            _jwtGenerator = jwtGenerator;
        }

        public ActionResult<String> GetDirectory()
        {
            string curr_dir = Directory.GetCurrentDirectory();
            return curr_dir.ToString();
        }
        public async Task<dynamic> CheckGetCourses(RequestCourses model)
        {
            string org;
            if (model.userID != "%")
            {
                UserProfile userProfile = _jwtGenerator.DecodeToken(model.token);
                org = userProfile.org;
                model.userID = userProfile.userID;
            }
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
        public async Task<List<ListApplicantResult>> CheckGetApplicants(RequestApplicants model)
        {
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
        public SetQrCode CheckGetQrcode(RequestApplicants model)
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
        public async Task<dynamic> CheckSetCheck(SetCheckName model)
        {
            ResponeError error = new ResponeError { };
            if (model.token != null && model.token != "")
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
        public async Task<dynamic> CheckGetAllLecture()
        {
            string query = new ElearnigQueryConfig().Q_GET_ALL_LECTURE;
            var response = await new DataContext().GetResultDapperAsyncDynamic(DataBaseHostEnum.KPR, query);
            var result = _mapper.Map<IEnumerable<GetAllLectureMap>>(response);
            var results = result as List<GetAllLectureMap>;
            var resultReal = _mapper.Map<List<GetAllLectureMap>, List<GetAllLecture>>(results);

            return resultReal;
        }
        public async Task<dynamic> CheckGetAllApplicant(PostModelAllApplcant model)
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
        public async Task<dynamic> CheckGetCourseForm(RequestCourses model)
        {
            string q = new ElearnigQueryConfig().Q_GET_COURE;
            string query = q.Replace(":as_instant_flag", $"'{model.instantFlag}'").Replace(":as_user_id", $"'{model.userID}'");
            var response = await new DataContext().GetResultDapperAsyncDynamic(DataBaseHostEnum.KPR, query);
            var result = _mapper.Map<IEnumerable<ResponseCourseDetails>>(response);
            var results = result as List<ResponseCourseDetails>;
            var resultReal = _mapper.Map<List<ResponseCourseDetails>, List<ResultCourseDetails>>(results);
            return resultReal;
        }
        public async Task<StateLectError> CheckSetNewCourse(LecturerForms model)
        {
            StateLectError stateError = new StateLectError();
            List<string> insertQuery = new List<string>();
            if (model.token != null && model.token != "")
            {
                var setLecturer = model.setLecturer;
                UserProfile userProfile = _jwtGenerator.DecodeToken(model.token);
                var org = userProfile.org;
                var userID = userProfile.userID;
                if (setLecturer.docType == "COURSE" || setLecturer.docType == "ISO")
                {
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
                            .Replace(":AS_SUM_MIN", $"'{setLecturer.dayMin}'")
                            .Replace(":AS_TRAIN_TYPE_ID", $"'{setLecturer.trainTypeID}'");
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
                            .Replace(":AS_QUERY_ID", $"'{setLecturer.queryID}'")
                            .Replace(":AS_TRAIN_TYPE_ID", $"'{setLecturer.trainTypeID}'");
                        //! INSERT NEW COURSE
                        updateQuery.Add(queryUCQn);

                        var resultUpdate = await new DataContext().ExecuteDapperMultiAsync(DataBaseHostEnum.KPR, updateQuery);
                    }

                    stateError.stateError = false;
                    stateError.messageError = "Course Success!!";
                    return stateError;
                }
                // else if (setLecturer.docType == "ISO")
                // {
                //     stateError.stateError = true;
                //     stateError.messageError = "ระบบยังไม่รองรับการสร้าง ISO!!";
                //     return stateError;
                // }
                else
                {
                    stateError.stateError = true;
                    stateError.messageError = "Don't have doc type!!";
                    return stateError;
                }
            }
            else
            {
                stateError.stateError = true;
                stateError.messageError = "Token is empty!!";
                return stateError;
            }
        }
        public async Task<dynamic> CheckGetTrainType()
        {
            string query = new ElearnigQueryConfig().S_TRAIN_TYPE;
            var response = await new DataContext().GetResultDapperAsyncDynamic(DataBaseHostEnum.KPR, query);
            var result = _mapper.Map<IEnumerable<GetTrainType>>(response);
            var results = result as List<GetTrainType>;
            var resultReal = _mapper.Map<List<GetTrainType>, List<SetTrainType>>(results);
            return resultReal;
        }
    }
}