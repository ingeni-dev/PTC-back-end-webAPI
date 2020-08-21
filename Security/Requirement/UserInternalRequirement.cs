using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;

namespace PTCwebApi.Security.Requirement {
    public class UserInternalRequirement : IAuthorizationRequirement {

    }
    public class UserInternalRequirementHandler : AuthorizationHandler<UserInternalRequirement> {

        private readonly IHttpContextAccessor _httpContextAccessor;
        public UserInternalRequirementHandler (IHttpContextAccessor httpContextAccessor) {
            _httpContextAccessor = httpContextAccessor;
        }

        protected override Task HandleRequirementAsync (AuthorizationHandlerContext context, UserInternalRequirement requirement) {
            var currentUserName = _httpContextAccessor.HttpContext.User?.Claims?.SingleOrDefault (x => x.Type == ClaimTypes.NameIdentifier)?.Value;
            var userID = _httpContextAccessor.HttpContext.User?.Claims?.SingleOrDefault (x => x.Type == "userID")?.Value;
            var user = _httpContextAccessor.HttpContext.User?.Claims?.SingleOrDefault (x => x.Type == "aduserID")?.Value;
            var ORG = _httpContextAccessor.HttpContext.User?.Claims?.SingleOrDefault (x => x.Type == "org")?.Value;
            if (ORG == "KPR" || ORG =="LAP" || ORG =="KPP" || ORG =="OPPN")
                context.Succeed (requirement);
            return Task.CompletedTask;
        }
    }

}