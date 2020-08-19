using System;
using Microsoft.AspNetCore.Mvc;
using PTCwebApi.Models;
using PTCwebApi.Security;

namespace PTCwebApi.Controllers {
    [Route ("[controller]")]
    [ApiController]
    public class UsersController : ControllerBase {
        public UsersController () { }

        [HttpPost ("login")]
        public string Login (UserLogin model) {
            Boolean status = AuthenticateUsers.AuthenticateUser (model);
            if (status != false) {
                return "ยินดีต้อนรับ";
            }
            return "เสียใจด้วย";
        }
    }
}