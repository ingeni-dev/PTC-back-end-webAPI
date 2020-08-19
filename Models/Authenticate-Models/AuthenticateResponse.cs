using PTCwebApi.Models.Profiles_Models;

namespace PTCwebApi.Models.Authenticate_Models {
    public class AuthenticateResponse {
        public string org { get; set; }
        public string userID { get; set; }
        public string userName { get; set; }
        public string userNameENG { get; set; }
        public string nickname { get; set; }
        public string email { get; set; }
        public string posrole { get; set; }
        public string Token { get; set; }

        public AuthenticateResponse (UserProfile user, string token) {
            org = user.org;
            userID = user.userID;
            userName = user.userName;
            userNameENG = user.userNameENG;
            nickname = user.nickname;
            email = user.email;
            posrole = user.posrole;
            Token = token;
        }
    }
}