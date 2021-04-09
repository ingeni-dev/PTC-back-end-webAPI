using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using AutoMapper;
using HeyRed.Mime;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using PTCwebApi;
using PTCwebApi.Interfaces;
using PTCwebApi.Models.ProfilesModels;
using webAPI.Models.Elearning;
using webAPI.Models.Elearning.configs;

namespace webAPI.Methods.Elearning
{
    public class OnlineCourse
    {
        private readonly IWebHostEnvironment _environment;
        private readonly IMapper _mapper;
        private readonly IJwtGenerator _jwtGenerator;
        public OnlineCourse(IMapper mapper, IWebHostEnvironment environment, IJwtGenerator jwtGenerator)
        {
            _mapper = mapper;
            _environment = environment;
            _jwtGenerator = jwtGenerator;
        }
        public async Task<FileContentResult> DownloadDoc(FileParam model)
        {
            model.filePath = model.filePath.Replace("/", "\\");
            var ServerResourceUrl = "\\\\192.168.55.92\\Users\\Public\\elearning" + model.filePath;
            var fileStream = await File.ReadAllBytesAsync(ServerResourceUrl);
            return new FileContentResult(fileStream, MimeTypesMap.GetMimeType(ServerResourceUrl));
        }
        async public Task<dynamic> OnlineCourseGetOrderLatest(OnlineGetOrderModel model)
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
        }
        async public Task<dynamic> OnineCorseGetAllOnlineCourse(OnlineGetOrderModel model)
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
        async public Task<ReturnSetSawVDO> OnlineCourseSetSawvdo(RequestSetSawVDO model)
        {
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
            }
            returns.stateError = true;
            returns.messageError = "Token's Empty !!";
            return returns;
        }

    }
}