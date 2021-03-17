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

    }
}