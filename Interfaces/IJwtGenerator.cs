using PTCwebApi.Models.ProfilesModels;

namespace PTCwebApi.Interfaces {

    public interface IJwtGenerator {
        string generateJwtToken (UserProfile user, string userName);
        bool ValidateCurrentToken (string token);
        UserProfile DecodeToken (string token);
    }
}