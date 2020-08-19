using PTCwebApi.Models.Profiles_Models;

namespace PTCwebApi.Interfaces {

    public interface IJwtGenerator {
        string generateJwtToken (UserProfile user, string userName);
        bool ValidateCurrentToken (string token);
        UserProfile DecodeToken (string token);
    }
}