using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using PTCwebApi.Models.PTCModels;

namespace PTCwebApi.Security {
    public class JwtMiddleware {
        private readonly RequestDelegate _next;
        private readonly SymmetricSecurityKey _key;
        private List<DataORG> _users = new List<DataORG> {
            new DataORG {
            org = "OPPN"
            },
            new DataORG {
            org = "LAP"
            },
            new DataORG {
            org = "KPP"
            },
            new DataORG {
            org = "KPR"
            },
            new DataORG {
            org = "IGS"
            }
        };
        public JwtMiddleware (RequestDelegate next, IConfiguration config) {
            _next = next;
            _key = new SymmetricSecurityKey (Encoding.UTF8.GetBytes (config.GetSection ("AppSettings:Secret").Value));
        }

        public async Task Invoke (HttpContext context) {
            var token = context.Request.Headers["Authorization"].FirstOrDefault ()?.Split (" ").Last ();

            if (token != null)
                attachUserToContext (context, token);

            await _next (context);
        }

        private void attachUserToContext (HttpContext context, string token) {
            try {
                var tokenHandler = new JwtSecurityTokenHandler ();
                tokenHandler.ValidateToken (token, new TokenValidationParameters {
                    ValidateIssuerSigningKey = true,
                        IssuerSigningKey = _key,
                        ValidateIssuer = false,
                        ValidateAudience = false,
                        // set clockskew to zero so tokens expire exactly at token expiration time (instead of 5 minutes later)
                        ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);

                var jwtToken = (JwtSecurityToken) validatedToken;
                var org = jwtToken.Claims.First (x => x.Type == "org").Value;
                var userID = jwtToken.Claims.First (x => x.Type == "userID").Value;
                var userName = jwtToken.Claims.First (x => x.Type == "userName").Value;
                var nickname = jwtToken.Claims.First (x => x.Type == "nickname").Value;
                var email = jwtToken.Claims.First (x => x.Type == "email").Value;
                var posrole = jwtToken.Claims.First (x => x.Type == "posrole").Value;

                // attach user to context on successful jwt validation
                context.Items["UserProfile"] = GetByOrg (org);
            } catch {
                // do nothing if jwt validation fails
                // user is not attached to context so request won't have access to secure routes
            }
        }
         public DataORG GetByOrg (string org) {
            return _users.FirstOrDefault (x => x.org == org);
        }
    }
}