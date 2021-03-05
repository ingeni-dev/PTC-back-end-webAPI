using System.Collections;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;

namespace webAPI.Models.Elearning
{

    public class StateUpload
    {
        public bool error { get; set; }
        public string messageError { get; set; }
        public string groupID { get; set; }
        public string topicID { get; set; }

    }
    public class ReturnTopicID
    {
        public string topicID { get; set; }
    }
    public class UpLoadGroup
    {
        public string token { get; set; }
        public bool stateDelete { get; set; }
        public string courseType { get; set; }
        public string courseID { get; set; }
        public string courseRevision { get; set; }
        public string groupID { get; set; }
        public string groupOrder { get; set; }
        public string groupName { get; set; }


    }
    public class UpLoadTopic
    {
        public string token { get; set; }
        public bool stateDelete { get; set; }
        public string topicID { get; set; }
        public string topicOrder { get; set; }
        public string topicName { get; set; }
        public string groupID { get; set; }

    }

    public class UpLoadDoc
    {
        public string token { get; set; }
        public bool stateDelete { get; set; }
        public string courseID { get; set; }
        public string topicID { get; set; }
        public string docID { get; set; }
        public string docOrder { get; set; }
        public string docType { get; set; }
        public string docName { get; set; }
        public string docPath { get; set; }
        public string videoCover { get; set; }
        public string videoLength { get; set; }
        public IFormFile fileVideo { get; set; }
        public IFormFile fileImg { get; set; }
        public IFormFile fileDoc { get; set; }
    }
    // public class UpLoadDoc
    // {
    //     public string token { get; set; }
    //     public string queryID { get; set; }
    //     public string docOrder { get; set; }
    //     public string topicID { get; set; }
    //     public string courseDocID { get; set; }
    //     public string docName { get; set; }
    //     public string docType { get; set; }
    //     public string docPath { get; set; }
    //     public string videoCover { get; set; }
    //     public string videoLength { get; set; }
    //     public string revision { get; set; }
    //     public IFormFile fileVideo { get; set; }
    //     public IFormFile fileImg { get; set; }
    //     public IFormFile fileDoc { get; set; }
    // }

    public class UpLoadGroupMultiple
    {
        public string courseType { get; set; }
        public string courseID { get; set; }
        public string courseRevision { get; set; }
        public string groupID { get; set; }
        public string groupOrder { get; set; }
        public string groupName { get; set; }
        public List<UpLoadTopicMultiple> topics { get; set; }
    }
    public class UpLoadTopicMultiple
    {
        public string topicID { get; set; }
        public string topicOrder { get; set; }
        public string topicName { get; set; }
        public string groupID { get; set; }
        public UpLoadDocMultiple docs { get; set; }
        public IFormCollection flies { get; set; }

    }
    public class UpLoadDocMultiple
    {
        public string topicID { get; set; }
        public string docID { get; set; }
        public string docOrder { get; set; }
        public string docType { get; set; }
        public string docName { get; set; }
        public string docPath { get; set; }
        public string videoCover { get; set; }
        public string videoLength { get; set; }
    }
}