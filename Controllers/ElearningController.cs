using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PTCwebApi;
using webAPI.Models.Elearning;
using PTCwebApi.Models.Elearning;
using Microsoft.AspNetCore.Hosting;
using webAPI.Models.Elearning.configs;
using webAPI.Methods.Elearning;
using PTCwebApi.Interfaces;
using PTCwebApi.Models.ProfilesModels;
using System.IO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.StaticFiles;
using HeyRed.Mime;

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
            return new CheckUser(
                mapper: _mapper,
                environment: _environment,
                jwtGenerator: _jwtGenerator).GetDirectory();
        }

        [HttpPost("getCourses")]
        public async Task<dynamic> GetCourses(RequestCourses model)
        {
            return await new CheckUser(
                 mapper: _mapper,
                 environment: _environment,
                 jwtGenerator: _jwtGenerator).CheckGetCourses(model: model);
        }

        [HttpPost("getApplicants")]
        public async Task<List<ListApplicantResult>> GetApplicants(RequestApplicants model)
        {
            return await new CheckUser(
                 mapper: _mapper,
                 environment: _environment,
                 jwtGenerator: _jwtGenerator).CheckGetApplicants(model: model);
        }

        [HttpPost("getQrcode")]
        public SetQrCode GetQrcode(RequestApplicants model)
        {
            return new CheckUser(
                 mapper: _mapper,
                 environment: _environment,
                 jwtGenerator: _jwtGenerator).CheckGetQrcode(model: model);
        }

        [HttpPost("setCheck")]
        public async Task<dynamic> SetCheck(SetCheckName model)
        {
            return await new CheckUser(
                mapper: _mapper,
                environment: _environment,
                jwtGenerator: _jwtGenerator).CheckSetCheck(model: model);
        }

        [HttpGet("getAllLecturer")]
        public async Task<dynamic> GetAllLecture()
        {
            return await new CheckUser(
                mapper: _mapper,
                environment: _environment,
                jwtGenerator: _jwtGenerator).CheckGetAllLecture();
        }

        [HttpPost("getAllApplicant")]
        public async Task<dynamic> GetAllApplicant(PostModelAllApplcant model)
        {
            return await new CheckUser(
                mapper: _mapper,
                environment: _environment,
                jwtGenerator: _jwtGenerator).CheckGetAllApplicant(model: model);
        }
        [HttpPost("getCourseForm")]
        public async Task<dynamic> GetCourseForm(RequestCourses model)
        {
            return await new CheckUser(
                mapper: _mapper,
                environment: _environment,
                jwtGenerator: _jwtGenerator).CheckGetCourseForm(model: model);
        }

        [HttpPost("setNewCourse")]
        public async Task<StateLectError> SetNewCourse(LecturerForms model)
        {
            return await new CheckUser(
                mapper: _mapper,
                environment: _environment,
                jwtGenerator: _jwtGenerator).CheckSetNewCourse(model: model);
        }

        [HttpPost("getTrainType")]
        public async Task<dynamic> GetTrainType()
        {
            return await new CheckUser(
                 mapper: _mapper,
                 environment: _environment,
                 jwtGenerator: _jwtGenerator).CheckGetTrainType();
        }

        [HttpPost("getTitleCourses")]
        public async Task<dynamic> GetAllCourses()
        {
            return await new CreateDoc(
                 mapper: _mapper,
                 environment: _environment,
                 jwtGenerator: _jwtGenerator).CreateGetAllCourses();
        }
        [HttpPost("getTitleISOs")]
        public async Task<dynamic> GetAllISOs()
        {
            return await new CreateDoc(
               mapper: _mapper,
               environment: _environment,
               jwtGenerator: _jwtGenerator).CreateGetAllISOs();
        }
        [HttpPost("getTopicDetail")]
        async public Task<dynamic> GetTopicDetail(RequestTopicDetail model)
        {
            return await new CreateDoc(
               mapper: _mapper,
               environment: _environment,
               jwtGenerator: _jwtGenerator).CreateGetTopicDetail(model: model);
        }

        [HttpPost("upLoadGroup")]
        async public Task<StateUpload> UploadGroup(UpLoadGroup model)
        {
            return await new CreateDoc(
               mapper: _mapper,
               environment: _environment,
               jwtGenerator: _jwtGenerator).CreateUploadGroup(model: model);
        }

        [HttpPost("upLoadTopic")]
        async public Task<StateUpload> UpLoadTopic(UpLoadTopic model)
        {
            return await new CreateDoc(
               mapper: _mapper,
               environment: _environment,
               jwtGenerator: _jwtGenerator).CreateUpLoadTopic(model: model);
        }

        [HttpPost("upLoadDoc"), DisableRequestSizeLimit]
        async public Task<Boolean> UpLoadDoc([FromForm] UpLoadDoc model)
        {
            var formCollection = await Request.ReadFormAsync();

            return await new CreateDoc(
              mapper: _mapper,
              environment: _environment,
              jwtGenerator: _jwtGenerator).CreateUpLoadDoc(model: model);
        }

        // [HttpPost("upLoadDoc2"), DisableRequestSizeLimit]
        // async public Task<IActionResult> UpLoadDoc2([FromForm] UpLoadDoc model)
        // {
        //     var formCollection = await Request.ReadFormAsync();

        //     try
        //     {
        //         await new CreateDoc(
        //          mapper: _mapper,
        //          environment: _environment,
        //          jwtGenerator: _jwtGenerator).CreateUpLoadDoc(model: model);
        //         return Ok(true);
        //     }
        //     catch (Exception ex)
        //     {
        //         return Ok(ex.ToString());
        //     }
        // }

        // [Authorize]
        [HttpPost("getOrderLatest")]
        async public Task<dynamic> GetOrderLatest(OnlineGetOrderModel model)
        {
            ReturnOnlineGetOrderLatest dataReturn = new ReturnOnlineGetOrderLatest();
            if (model.token != null)
            {
                UserProfile userProfile = _jwtGenerator.DecodeToken(model.token);
                string userID = userProfile.userID;
                string org = userProfile.org;

                var queryQGOCL = new ElearnigQueryConfig().Q_GET_ONLINE_COURSE_LATEST;
                var queryQGOCLn = queryQGOCL.Replace(":as_emp_id", $"'{userID}'");
                var responseQGOCL = await new DataContext().GetResultDapperAsyncDynamic(DataBaseHostEnum.KPR, queryQGOCLn);
                var result = _mapper.Map<IEnumerable<GetALLOnlineCourseLatest>>(responseQGOCL);
                var results = result as List<GetALLOnlineCourseLatest>;
                var resultReal = _mapper.Map<List<GetALLOnlineCourseLatest>, List<SetALLOnlineCourseLatest>>(results);

                dataReturn.stateError = false;
                dataReturn.message = "success";
                dataReturn.returns = resultReal;
                return dataReturn;
            }
            else
            {
                dataReturn.stateError = true;
                dataReturn.message = "Token is Empty!!";
                return dataReturn;
            }
            // return await new OnlineCourse(
            //   mapper: _mapper,
            //   environment: _environment,
            //   jwtGenerator: _jwtGenerator).OnlineGetOrderLatest();
        }

        [HttpPost("getAllOnlineCourse")]
        async public Task<dynamic> GetAllOnlineCourse(OnlineGetOrderModel model)
        {
            ReturnGetALLOnlineCourse dataReturn = new ReturnGetALLOnlineCourse();
            if (model.token != null && model.token != "")
            {
                UserProfile userProfile = _jwtGenerator.DecodeToken(model.token);
                string userID = userProfile.userID;
                string org = userProfile.org;

                var queryQGAOC = new ElearnigQueryConfig().Q_GET_ALL_ONLINE_COURSE;
                var queryQGAOCn = queryQGAOC.Replace(":as_emp_id", $"'{userID}'");
                var responseQGAOC = await new DataContext().GetResultDapperAsyncDynamic(DataBaseHostEnum.KPR, queryQGAOCn);
                var result = _mapper.Map<IEnumerable<GetALLOnlineCourse>>(responseQGAOC);
                var results = result as List<GetALLOnlineCourse>;
                var resultReal = _mapper.Map<List<GetALLOnlineCourse>, List<SetALLOnlineCourse>>(results);

                dataReturn.stateError = false;
                dataReturn.message = "success";
                dataReturn.returns = resultReal;
                return dataReturn;
            }
            else
            {
                dataReturn.stateError = true;
                dataReturn.message = "Token is Empty!!";
                return dataReturn;
            }
        }

        [HttpPost("setSawvdo")]
        async public Task<ReturnSetSawVDO> SetSawvdo(RequestSetSawVDO model)
        {
            // try
            // {
            ReturnSetSawVDO returns = new ReturnSetSawVDO();
            if (model.token != null && model.token != "")
            {
                UserProfile userProfile = _jwtGenerator.DecodeToken(model.token);
                string userID = userProfile.userID;
                string org = userProfile.org;
                string finishFlag = "F";

                var queryCCQDW = new ElearnigQueryConfig().C_COURSE_QUERY_DOC_WHERE;
                var queryCCQDWn = queryCCQDW.Replace(":AS_COURSE_DOC_ID", $"'{model.courseDocID}'")
                                            .Replace(":AS_QUERY_ID", $"'{model.queryID}'")
                                            .Replace(":AS_APP_EMP_ID", $"'{userID}'");
                var responseCCQDW = await new DataContext().GetResultDapperAsyncDynamic(DataBaseHostEnum.KPR, queryCCQDWn);
                var result = _mapper.Map<IEnumerable<GetCourseQueryDoc>>(responseCCQDW);
                var results = result as List<GetCourseQueryDoc>;
                var resultReal = _mapper.Map<List<GetCourseQueryDoc>, List<SetCourseQueryDoc>>(results);

                decimal countCCQDW = resultReal.Count;
                if (countCCQDW != 0)
                {
                    List<string> queryAll = new List<string> { };
                    if (model.stateVDO == "P")
                    {
                        var resultStore = new StoreConnectionElearning(_mapper).ElearningViewVDO(queryID: model.queryID, appEmpID: userID, courseDocID: model.courseDocID);

                        var querySCQDD = new ElearnigQueryConfig().S_COURSE_QUERY_DOC_DETAIL;
                        var querySCQDDn = querySCQDD.Replace(":AS_QUERY_DOC_ID", $"'{resultReal[0].queryDocID}'");
                        var responseSCQDD = await new DataContext().GetResultDapperAsyncDynamic(DataBaseHostEnum.KPR, querySCQDDn);
                        var seq = (responseSCQDD as List<dynamic>).Count;

                        var queryICQDD = new ElearnigQueryConfig().I_COURSE_QUERY_DOC_DETAIL;
                        var queryICQDDn = queryICQDD.Replace(":AS_QUERY_DOC_ID", $"'{resultReal[0].queryDocID}'")
                                                    .Replace(":AS_SEQ", $"'{seq}'")
                                                    .Replace(":AS_START_TIME", $"'{model.openTime}'")
                                                    .Replace(":AS_END_TIME", $"'{model.currTime}'");
                        queryAll.Add(queryICQDDn);
                    }

                    if (model.stateVDO == "O") { model.currTime = resultReal[0].currTime; }
                    var queryUCQDW = new ElearnigQueryConfig().U_COURSE_QUERY_DOC;
                    var queryUCQDWn = queryUCQDW.Replace(":AS_COUNT", $"'{int.Parse(resultReal[0].count) + 1}'")
                                                .Replace(":AS_CURR_TIME", $"'{ model.currTime}'")
                                                .Replace(":AS_FINISH_FLAG", $"'{finishFlag}'")
                                                .Replace(":AS_QUERY_DOC_ID", $"'{resultReal[0].queryDocID}'");
                    queryAll.Add(queryUCQDWn);
                    var responseUCQDW = await new DataContext().ExecuteDapperMultiAsync(DataBaseHostEnum.KPR, queryAll);
                    // var responseUCQDW = await new DataContext().GetResultDapperAsyncDynamic(DataBaseHostEnum.KPR, queryUCQDWn);

                    returns.stateError = false;
                    returns.messageError = "Success to update data";
                    returns.currTime = int.Parse(model.currTime);
                    return returns;
                }
                else
                {
                    var date = DateTime.Now.ToString("yyMM");
                    var queryCCQD = new ElearnigQueryConfig().C_COURSE_QUERY_DOC;
                    var responseCCQD = await new DataContext().GetResultDapperAsyncDynamic(DataBaseHostEnum.KPR, queryCCQD);
                    var countSCTM = (responseCCQD as List<dynamic>)[0].COUN;
                    string queryDocID = date + countSCTM.ToString("0000");

                    var queryICQD = new ElearnigQueryConfig().I_COURSE_QUERY_DOC;
                    var queryICQDn = queryICQD.Replace(":AS_QUERY_DOC_ID", $"'{queryDocID}'")
                                                .Replace(":AS_COURSE_DOC_ID", $"'{model.courseDocID}'")
                                                .Replace(":AS_QUERY_ID", $"'{model.queryID}'")
                                                .Replace(":AS_APP_EMP_ID", $"'{userID}'");
                    var responseICQD = await new DataContext().GetResultDapperAsyncDynamic(DataBaseHostEnum.KPR, queryICQDn);

                    //TODO INSERT
                    returns.stateError = false;
                    returns.messageError = "Success to insert data";
                    return returns;

                }
                // await new CreateDoc(
                //  mapper: _mapper,
                //  environment: _environment,
                //  jwtGenerator: _jwtGenerator).CreateUpLoadDoc(model: model);
            }
            returns.stateError = true;
            returns.messageError = "Token's Empty !!";
            return returns;
            // }
            // catch (Exception ex)
            // {
            //     return Ok(ex.ToString());
            // }
        }
        [HttpPost("downloadDoc")]
        public async Task<FileContentResult> DownloadDoc(FileParam model)
        {
            return await new OnlineCourse(
                     mapper: _mapper,
                     environment: _environment,
                     jwtGenerator: _jwtGenerator).DownloadDoc(model: model);
        }
    }
}