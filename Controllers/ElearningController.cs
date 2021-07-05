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

        [Authorize]
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
        [Authorize]
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
        [RequestFormLimits(MultipartBodyLengthLimit = 1073741824)]
        async public Task<ActionResult<Boolean>> UpLoadDoc([FromForm] UpLoadDoc model)
        {
            var formCollection = await Request.ReadFormAsync();

            try
            {
                return await new CreateDoc(
                         mapper: _mapper,
                         environment: _environment,
                         jwtGenerator: _jwtGenerator).CreateUpLoadDoc(model: model);
            }
            catch (Exception e)
            {

                return Ok(e);
            }

        }

        // [Authorize]
        [HttpPost("getOrderLatest")]
        async public Task<dynamic> GetOrderLatest(OnlineGetOrderModel model)
        {
            return await new OnlineCourse(
              mapper: _mapper,
              environment: _environment,
              jwtGenerator: _jwtGenerator).OnlineCourseGetOrderLatest(model);
        }

        [HttpPost("getAllOnlineCourse")]
        async public Task<dynamic> GetAllOnlineCourse(OnlineGetOrderModel model)
        {
            return await new OnlineCourse(
             mapper: _mapper,
             environment: _environment,
             jwtGenerator: _jwtGenerator).OnineCorseGetAllOnlineCourse(model);
        }

        [HttpPost("setSawvdo")]
        async public Task<ReturnSetSawVDO> SetSawvdo(RequestSetSawVDO model)
        {
            return await new OnlineCourse(
             mapper: _mapper,
             environment: _environment,
             jwtGenerator: _jwtGenerator).OnlineCourseSetSawvdo(model);
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