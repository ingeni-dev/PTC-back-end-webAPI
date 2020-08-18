using System;
using Microsoft.AspNetCore.Mvc;
using PTCwebApi.Models;
using WebApi.Services.DataServices;

namespace PTCwebApi.Controllers {
    [Route ("[controller]")]
    [ApiController]
    public class UserController : ControllerBase {
        public UserController () { }

        [HttpPost ("login")]
        public string Login (UserLogin model) {
            Boolean status = AuthenticateUsers.AuthenticateUser (model);
            if (status != false) {
                return "เสียใจด้วย";
            }
            return "ยินดีต้อนรับ";
        }
    }
}