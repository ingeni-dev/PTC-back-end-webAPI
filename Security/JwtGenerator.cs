using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using PTCwebApi.Interfaces;
using PTCwebApi.Models.Authenticate_Models;
using PTCwebApi.Models.Profiles_Models;

namespace PTCwebApi.Security {

    public class JwtGenerator : IJwtGenerator {
        private readonly SymmetricSecurityKey _key;
        public JwtGenerator (IConfiguration config) {
            _key = new SymmetricSecurityKey (Encoding.UTF8.GetBytes (config.GetSection ("AppSettings:Secret").Value));
        }
        // public AuthenticateResponse Authenticate (UserProfile model, string userName) { }
        public string generateJwtToken (UserProfile user, string userName) {

            var claims = new List<Claim> {
                new Claim ("org", user.org),
                new Claim ("userID", user.userID),
                new Claim ("userName", user.userName),
                // new Claim ("EMP_NAME_ENG", user.EMP_NAME_ENG),
                new Claim ("aduserID", userName),
                new Claim ("nickname", user.nickname),
                new Claim ("email", user.email),
                new Claim ("posrole", user.posrole)
            };

            //generate signing credentials
            var creds = new SigningCredentials (_key, SecurityAlgorithms.HmacSha512Signature);
            var tokenDescriptor = new SecurityTokenDescriptor {
                Subject = new ClaimsIdentity (claims),
                // generate token that is valid for 1 days
                Expires = DateTime.UtcNow.AddDays (1),
                SigningCredentials = creds
            };

            var tokenHandler = new JwtSecurityTokenHandler ();
            var token = tokenHandler.CreateToken (tokenDescriptor);
            return tokenHandler.WriteToken (token);
        }
        public bool ValidateCurrentToken (string token) {
            var tokenHandler = new JwtSecurityTokenHandler ();
            try {
                tokenHandler.ValidateToken (token, new TokenValidationParameters {
                    ValidateIssuerSigningKey = true,
                        IssuerSigningKey = _key,
                        ValidateAudience = false,
                        ValidateIssuer = false
                }, out SecurityToken validatedToken);
            } catch {
                return false;
            }
            return true;
        }
        public UserProfile DecodeToken (string token) {
            var jwtHandler = new JwtSecurityTokenHandler ();
            if (!jwtHandler.CanReadToken (token))
                return null;

            var tokenJwt = jwtHandler.ReadJwtToken (token);
            var jwtPayload = JsonConvert.SerializeObject (tokenJwt.Claims.Select (c => new { c.Type, c.Value }));

            List<JWTDecode> jwtDecode = JsonConvert.DeserializeObject<List<JWTDecode>> (jwtPayload);

            UserProfile user = new UserProfile ();
            foreach (JWTDecode j in jwtDecode) {
                switch (j.Type) {
                    case "userName":
                        user.userName = j.Value;
                        break;
                    case "userID":
                        user.userID = j.Value;
                        break;
                    case "aduserID":
                        user.aduserID = j.Value;
                        break;
                    case "nickname":
                        user.nickname = j.Value;
                        break;
                    case "email":
                        user.email = j.Value;
                        break;
                    case "org":
                        user.org = j.Value;
                        break;
                    case "posrole":
                        user.posrole = j.Value;
                        break;
                    default:
                        break;
                }
            }

            return user;
        }
    }
}