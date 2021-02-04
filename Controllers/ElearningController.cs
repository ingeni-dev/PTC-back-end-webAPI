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

namespace webAPI.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ElearningController : ControllerBase
    {
        private readonly IWebHostEnvironment _environment;
        private readonly IMapper _mapper;
        public ElearningController(IMapper mapper, IWebHostEnvironment environment)
        {
            _mapper = mapper;
            _environment = environment;
        }

        [HttpGet("")]
        public ActionResult<String> GetTModels()
        {
            return "1";
        }

        [HttpPost("getCourses")]
        public async Task<dynamic> GetCourses(RequestCourses model)
        {
            string q = new ElearnigQueryConfig().Q_GET_COURE;
            string query = q.Replace(":INSTANT_FLAG", $"'{model.instantFlag}'");
            var response = await new DataContext().GetResultDapperAsyncDynamic(DataBaseHostEnum.KPR, query);
            var result = _mapper.Map<IEnumerable<ResponseCourseDetails>>(response);
            var results = result as List<ResponseCourseDetails>;
            var resultReal = _mapper.Map<List<ResponseCourseDetails>, List<ResultCourseDetails>>(results);
            foreach (var item in resultReal)
            {
                item.day=item.queryBegin.Split(" ")[0].ToString();
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
            byte[] data = Convert.FromBase64String(model.queryID);
            model.queryID = Encoding.UTF8.GetString(data);
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
        public SetQrCode PostTModel(RequestApplicants model)
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

        [HttpPost("addCourse")]
        public async Task<Boolean> PostTModel(AddCourseDetail model)
        {

            await Task.Yield();

            return false;
        }

        [HttpPost("getTitleCourses")]
        public async Task<dynamic> GetAllCourses()
        {
            var query = new ElearnigQueryConfig().queryAllcourse;
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
            var query = new ElearnigQueryConfig().queryAllISO;
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
        async public Task<List<ResponseTopicDetail>> GetTopicDetail(RequestTopicDetail model)
        {
            List<ResponseTopicDetail> listTopicDetail = new List<ResponseTopicDetail> { };
            var query = $"SELECT TOPIC_ID, TOPIC_NAME, TOPIC_ORDER, COURSE_REVISION FROM KPDBA.COURSE_TOPIC WHERE COURSE_ID = '{model.courseID}'";
            var response = await new DataContext().GetResultDapperAsyncDynamic(DataBaseHostEnum.KPR, query);
            var result = _mapper.Map<IEnumerable<GetTopicDetail>>(response);
            var results = result as List<GetTopicDetail>;
            var resultReal = _mapper.Map<List<GetTopicDetail>, List<SetTopicDetail>>(results);
            foreach (SetTopicDetail item in resultReal)
            {
                ResponseTopicDetail topicDetail = new ResponseTopicDetail { };
                topicDetail.topicID = item.topicID;
                topicDetail.topicName = item.topicName;
                topicDetail.topicOrder = item.topicOrder;
                topicDetail.courseRevision = item.courseRevision;
                var queryDoc = $"SELECT CTD.COURSE_DOC_ID, DOC_ORDER, CDM.DOC_TYPE, CDM.DOC_NAME, CDM.DOC_PATH, CDM.VIDEO_COVER, CDM.VIDEO_LENGTH FROM KPDBA.COURSE_TOPIC_DOCUMENT CTD, KPDBA.COURSE_DOCUMENT_MASTER CDM WHERE CTD.COURSE_DOC_ID = CDM.COURSE_DOC_ID AND COURSE_ID = '{model.courseID}' AND TOPIC_ID = '{item.topicID}' AND CTD.COURSE_REVISION = '{item.courseRevision}'";
                var responseDoc = await new DataContext().GetResultDapperAsyncDynamic(DataBaseHostEnum.KPR, queryDoc);
                var resultDoc = _mapper.Map<IEnumerable<GetDocDetail>>(responseDoc);
                var resultsDoc = resultDoc as List<GetDocDetail>;
                List<SetDocDetail> resultRealDoc = _mapper.Map<List<GetDocDetail>, List<SetDocDetail>>(resultsDoc);
                topicDetail.items = resultRealDoc;
                listTopicDetail.Add(topicDetail);
            }
            return listTopicDetail;
        }

        [HttpPost("upLoadTopic")]
        async public Task<ReturnTopicID> upLoadTopic([FromForm] UpLoadTopic model)
        {
            string org = "KPR";
            string userID = "630054";
            string topicId = "";
            if (model.topicID == "New")
            {
                var date = DateTime.Now.ToString("yyMM");
                string queryTID = $"SELECT COUNT(1) CON FROM KPDBA.COURSE_TOPIC WHERE COURSE_ID='{model.queryID}'";
                var resultTID = await new DataContext().GetResultDapperAsyncDynamic(DataBaseHostEnum.KPR, queryTID);
                var countTID = (resultTID as List<dynamic>)[0].CON;
                topicId = date + (countTID + 1).ToString("0000");

                string topicType;
                string queryC = $"SELECT COUNT(1) CON FROM KPDBA.COURSE_MASTER WHERE COURSE_ID='{model.queryID}'";
                var resultC = await new DataContext().GetResultDapperAsyncDynamic(DataBaseHostEnum.KPR, queryC);
                var countC = (resultC as List<dynamic>)[0].CON;
                string queryI = $"SELECT COUNT(1) CON FROM KPDBA.ISO_MASTER WHERE DOC_CODE='{model.queryID}'";
                var resultI = await new DataContext().GetResultDapperAsyncDynamic(DataBaseHostEnum.KPR, queryI);
                var countI = (resultI as List<dynamic>)[0].CON;
                if (countC != 0)
                {
                    topicType = "C";
                }
                else if (countI != 0)
                {
                    topicType = "I";
                }
                else
                {
                    return null;
                }
                string query = $"INSERT INTO KPDBA.COURSE_TOPIC (COURSE_ID, TOPIC_ID, DOC_TYPE, TOPIC_NAME, TOPIC_ORDER, COURSE_REVISION, CR_DATE, CR_ORG_ID, CR_USER_ID) VALUES ('{model.queryID}', '{topicId}', '{topicType}', '{model.topicName}', TO_NUMBER ('{countTID + 1}'), TO_NUMBER ('{model.revision}'), TO_DATE (TO_CHAR (SYSDATE), 'dd/mm/yyyy'), TO_CHAR ('{org}'), TO_CHAR ('{userID}'))";
                var result = await new DataContext().InsertResultDapperAsync(DataBaseHostEnum.KPR, query);
                ReturnTopicID topicIDs = new ReturnTopicID();
                topicIDs.topicID = topicId;
                return topicIDs;
            }
            else if (model.topicID != "New" && model.topicID != null)
            {
                string queryUP = $"UPDATE KPDBA.COURSE_TOPIC SET UPDATE_DATE = SYSDATE, UP_ORG_ID = TO_CHAR ('{org}'), UP_USER_ID = TO_CHAR ('{userID}'), TOPIC_NAME = '{model.topicName}', TOPIC_ORDER = '{model.topicOrder}' WHERE     COURSE_ID = '{model.queryID}' AND TOPIC_ID = '{model.topicID}' AND COURSE_REVISION = '{model.revision}'";
                var resultUP = await new DataContext().InsertResultDapperAsync(DataBaseHostEnum.KPR, queryUP);
                ReturnTopicID topicIDs = new ReturnTopicID();
                topicIDs.topicID = model.topicID;
                return topicIDs;
            }
            else { return null; }

        }

        [HttpPost("upLoadDoc"), DisableRequestSizeLimit]
        async public Task<Boolean> upLoadDoc([FromForm] UpLoadDoc model)
        {
            var formCollection = await Request.ReadFormAsync();
            string org = "KPR";
            string userID = "630054";
            string courseDocID;
            if (model.courseDocID == "New")
            {
                var date = DateTime.Now.ToString("yyMM");
                string queryDID = "SELECT COUNT(1) CON FROM KPDBA.COURSE_DOCUMENT_MASTER";
                var resultDID = await new DataContext().GetResultDapperAsyncDynamic(DataBaseHostEnum.KPR, queryDID);
                var countDID = (resultDID as List<dynamic>)[0].CON;
                courseDocID = date + (countDID + 1).ToString("00000000");
                string cancelFlag = "F";
                switch (model.docType)
                {
                    case "V":
                        List<string> insertQuery = new List<string>();
                        String rndStr = new UploadImageAndVideo(_environment).GetRandomCharacter();
                        string fileName = $"V{courseDocID}-{rndStr}";
                        string docPathOld = new UploadImageAndVideo(_environment).UploadFile
                        (
                            queryID: model.queryID,
                            folderType: "videos",
                            fileName: fileName,
                            file: model.fileVideo
                        );
                        string docPath = docPathOld.Replace("\\", "/");
                        string imgCoverPathOld = new UploadImageAndVideo(_environment).UploadFile
                        (
                            queryID: model.queryID,
                            folderType: "images",
                            fileName: fileName,
                            file: model.fileImg
                        );
                        string imgCoverPath = imgCoverPathOld.Replace("\\", "/");
                        string queryCDM = $"INSERT INTO KPDBA.COURSE_DOCUMENT_MASTER (COURSE_DOC_ID, DOC_TYPE, DOC_NAME, DOC_PATH, VIDEO_COVER, VIDEO_LENGTH, CANCEL_FLAG, CR_DATE, CR_ORG_ID, CR_USER_ID) VALUES ('{courseDocID}', '{model.docType}', '{model.docName}', '{docPath}', '{imgCoverPath}', TO_NUMBER('{model.videoLength}'), TO_CHAR('{cancelFlag}'), TO_DATE (TO_CHAR (SYSDATE), 'dd/mm/yyyy'), TO_CHAR('{org}'), TO_CHAR('{userID}'))";
                        insertQuery.Add(queryCDM);
                        string queryCTD = $"INSERT INTO KPDBA.COURSE_TOPIC_DOCUMENT (COURSE_ID, TOPIC_ID, COURSE_DOC_ID, DOC_ORDER, COURSE_REVISION, CANCEL_FLAG, CR_DATE, CR_ORG_ID, CR_USER_ID) VALUES ('{model.queryID}', '{model.topicID}', '{courseDocID}', TO_NUMBER ('{model.docOrder}'), TO_NUMBER ('{model.revision}'), TO_CHAR('{cancelFlag}'), TO_DATE (TO_CHAR (SYSDATE), 'dd/mm/yyyy'), TO_CHAR('{org}'), TO_CHAR('{userID}'))";
                        insertQuery.Add(queryCTD);
                        var resultInsertAll = await new DataContext().ExecuteDapperMultiAsync(DataBaseHostEnum.KPR, insertQuery);
                        // var result = await new DataContext().InsertResultDapperAsync(DataBaseHostEnum.KPR, query);
                        return true;
                    case "T":
                        List<string> insertQueryT = new List<string>();
                        string queryCDMT = $"INSERT INTO KPDBA.COURSE_DOCUMENT_MASTER (COURSE_DOC_ID, DOC_TYPE, DOC_NAME, DOC_PATH, CANCEL_FLAG, CR_DATE, CR_ORG_ID, CR_USER_ID) VALUES ('{courseDocID}', '{model.docType}', '{model.docName}', '{model.docPath}', TO_CHAR('{cancelFlag}'),  TO_DATE (TO_CHAR (SYSDATE), 'dd/mm/yyyy'), TO_CHAR('{org}'), TO_CHAR('{userID}'))";
                        insertQueryT.Add(queryCDMT);
                        string queryCTDT = $"INSERT INTO KPDBA.COURSE_TOPIC_DOCUMENT (COURSE_ID, TOPIC_ID, COURSE_DOC_ID, DOC_ORDER, COURSE_REVISION, CANCEL_FLAG, CR_DATE, CR_ORG_ID, CR_USER_ID) VALUES ('{model.queryID}', '{model.topicID}', '{courseDocID}', TO_NUMBER ('{model.docOrder}'), TO_NUMBER ('{model.revision}'), TO_CHAR('{cancelFlag}'),  TO_DATE (TO_CHAR (SYSDATE), 'dd/mm/yyyy'), TO_CHAR('{org}'), TO_CHAR('{userID}'))";
                        insertQueryT.Add(queryCTDT);
                        var resultInsertAllT = await new DataContext().ExecuteDapperMultiAsync(DataBaseHostEnum.KPR, insertQueryT);
                        return true;
                    case "D":
                        String rndStrD = new UploadImageAndVideo(_environment).GetRandomCharacter();
                        string fileNameD = $"D{courseDocID}-{rndStrD}";
                        string docPathOldD = new UploadImageAndVideo(_environment).UploadFile
                        (
                            queryID: model.queryID,
                            folderType: "documents",
                            fileName: fileNameD,
                            file: model.fileDoc
                        );
                        string docPathD = docPathOldD.Replace("\\", "/");
                        List<string> insertQueryD = new List<string>();
                        string queryCDMD = $"INSERT INTO KPDBA.COURSE_DOCUMENT_MASTER (COURSE_DOC_ID, DOC_TYPE, DOC_NAME, DOC_PATH, CANCEL_FLAG, CR_DATE, CR_ORG_ID, CR_USER_ID) VALUES ('{courseDocID}', '{model.docType}', '{model.docName}', '{docPathD}', TO_CHAR('{cancelFlag}'),  TO_DATE (TO_CHAR (SYSDATE), 'dd/mm/yyyy'), TO_CHAR('{org}'), TO_CHAR('{userID}'))";
                        insertQueryD.Add(queryCDMD);
                        string queryCTDD = $"INSERT INTO KPDBA.COURSE_TOPIC_DOCUMENT (COURSE_ID, TOPIC_ID, COURSE_DOC_ID, DOC_ORDER, COURSE_REVISION, CANCEL_FLAG, CR_DATE, CR_ORG_ID, CR_USER_ID) VALUES ('{model.queryID}', '{model.topicID}', '{courseDocID}', TO_NUMBER ('{model.docOrder}'), TO_NUMBER ('{model.revision}'), TO_CHAR('{cancelFlag}'),  TO_DATE (TO_CHAR (SYSDATE), 'dd/mm/yyyy'), TO_CHAR('{org}'), TO_CHAR('{userID}'))";
                        insertQueryD.Add(queryCTDD);
                        var resultInsertAllD = await new DataContext().ExecuteDapperMultiAsync(DataBaseHostEnum.KPR, insertQueryD);

                        return true;
                    default:
                        return false;
                }
            }
            else if (model.courseDocID != "New" && model.courseDocID != null)
            {
                switch (model.docType)
                {
                    case "V":
                        String rndStr = new UploadImageAndVideo(_environment).GetRandomCharacter();
                        string fileName = $"V{model.courseDocID}-{rndStr}";
                        if (model.fileVideo != null)
                        {
                            List<string> insertQuery = new List<string>();
                            string docPathOld = new UploadImageAndVideo(_environment).UploadFile
                            (
                                queryID: model.queryID,
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
                                queryID: model.queryID,
                                folderType: "images",
                                fileName: fileName,
                                file: model.fileImg
                            );
                            model.videoCover = imgCoverPathOld.Replace("\\", "/");
                        }
                        List<string> insertQueryV = new List<string>();
                        string queryCDMV = $"UPDATE KPDBA.COURSE_DOCUMENT_MASTER SET UPDATE_DATE = SYSDATE, UP_ORG_ID = TO_CHAR ('{org}'), UP_USER_ID = TO_CHAR ('{userID}'), DOC_NAME = '{model.docName}', DOC_PATH = '{model.docPath}', VIDEO_COVER = '{model.videoCover}', VIDEO_LENGTH = TO_NUMBER('{model.videoLength}') WHERE COURSE_DOC_ID = '{model.courseDocID}'";
                        insertQueryV.Add(queryCDMV);
                        string queryCTDV = $"UPDATE KPDBA.COURSE_TOPIC_DOCUMENT SET UPDATE_DATE = SYSDATE, UP_ORG_ID = TO_CHAR ('{org}'), UP_USER_ID = TO_CHAR ('{userID}'), DOC_ORDER = TO_NUMBER('{model.docOrder}') WHERE COURSE_DOC_ID = '{model.courseDocID}' AND TOPIC_ID ='{model.topicID}' AND COURSE_ID = '{model.queryID}'";
                        insertQueryV.Add(queryCTDV);
                        var resultInsertAllV = await new DataContext().ExecuteDapperMultiAsync(DataBaseHostEnum.KPR, insertQueryV);
                        return true;
                    case "T":
                        List<string> insertQueryT = new List<string>();
                        string queryCDMT = $"UPDATE KPDBA.COURSE_DOCUMENT_MASTER SET UPDATE_DATE = SYSDATE, UP_ORG_ID = TO_CHAR ('{org}'), UP_USER_ID = TO_CHAR ('{userID}'), DOC_NAME = '{model.docName}', DOC_PATH = '{model.docPath}' WHERE COURSE_DOC_ID = '{model.courseDocID}'";
                        insertQueryT.Add(queryCDMT);
                        string queryCTDT = $"UPDATE KPDBA.COURSE_TOPIC_DOCUMENT SET UPDATE_DATE = SYSDATE, UP_ORG_ID = TO_CHAR ('{org}'), UP_USER_ID = TO_CHAR ('{userID}'), DOC_ORDER = TO_NUMBER('{model.docOrder}') WHERE COURSE_DOC_ID = '{model.courseDocID}' AND TOPIC_ID ='{model.topicID}' AND COURSE_ID = '{model.queryID}'";
                        insertQueryT.Add(queryCTDT);
                        var resultInsertAllT = await new DataContext().ExecuteDapperMultiAsync(DataBaseHostEnum.KPR, insertQueryT);
                        return true;
                    case "D":
                        if (model.fileDoc != null)
                        {
                            String rndStrD = new UploadImageAndVideo(_environment).GetRandomCharacter();
                            string fileNameD = $"D{model.courseDocID}-{rndStrD}";
                            string docPathOldD = new UploadImageAndVideo(_environment).UploadFile
                            (
                                queryID: model.queryID,
                                folderType: "documents",
                                fileName: fileNameD,
                                file: model.fileDoc
                            );
                            model.docPath = docPathOldD.Replace("\\", "/");

                        }
                        List<string> insertQueryD = new List<string>();
                        string queryCDMD = $"UPDATE KPDBA.COURSE_DOCUMENT_MASTER SET UPDATE_DATE = SYSDATE, UP_ORG_ID = TO_CHAR ('{org}'), UP_USER_ID = TO_CHAR ('{userID}'), DOC_NAME = '{model.docName}', DOC_PATH = '{model.docPath}' WHERE COURSE_DOC_ID = '{model.courseDocID}'";
                        insertQueryD.Add(queryCDMD);
                        string queryCTDD = $"UPDATE KPDBA.COURSE_TOPIC_DOCUMENT SET UPDATE_DATE = SYSDATE, UP_ORG_ID = TO_CHAR ('{org}'), UP_USER_ID = TO_CHAR ('{userID}'), DOC_ORDER = TO_NUMBER('{model.docOrder}') WHERE COURSE_DOC_ID = '{model.courseDocID}' AND TOPIC_ID ='{model.topicID}' AND COURSE_ID = '{model.queryID}'";
                        insertQueryD.Add(queryCTDD);
                        var resultInsertAllD = await new DataContext().ExecuteDapperMultiAsync(DataBaseHostEnum.KPR, insertQueryD);
                        return true;
                    default:
                        return false;

                }
            }
            return false;
        }

    }
}