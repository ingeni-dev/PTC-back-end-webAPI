using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using PTCwebApi;
using PTCwebApi.Interfaces;
using PTCwebApi.Models.ProfilesModels;
using webAPI.Models.Elearning;
using webAPI.Models.Elearning.configs;

namespace webAPI.Methods.Elearning
{
    public class CreateDoc
    {
        private readonly IWebHostEnvironment _environment;
        private readonly IMapper _mapper;
        private readonly IJwtGenerator _jwtGenerator;
        public CreateDoc(IMapper mapper, IWebHostEnvironment environment, IJwtGenerator jwtGenerator)
        {
            _mapper = mapper;
            _environment = environment;
            _jwtGenerator = jwtGenerator;
        }
        public async Task<dynamic> CreateGetAllCourses()
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
        public async Task<dynamic> CreateGetAllISOs()
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
        async public Task<dynamic> CreateGetTopicDetail(RequestTopicDetail model)
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
        async public Task<StateUpload> CreateUploadGroup(UpLoadGroup model)
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
        async public Task<StateUpload> CreateUpLoadTopic(UpLoadTopic model)
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
        async public Task<Boolean> CreateUpLoadDoc([FromForm] UpLoadDoc model)
        {
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