using System.Collections.Generic;

namespace webAPI.Models.Elearning
{
    public class OnlineGetOrderLatestModel
    {
        public string token { get; set; }
    }
    public class ReturnOnlineGetOrderLatest
    {
        public bool stateError { get; set; }
        public string message { get; set; }
        public List<string> returns { get; set; }

    }
}