using System;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PTCwebApi.Interfaces;
using PTCwebApi.Methods;
using PTCwebApi.Models;
using PTCwebApi.Models.AuthenticateModels;
using PTCwebApi.Models.RequestModels;
using PTCwebApi.Security;

namespace PTCwebApi.Controllers {
    [Route ("[controller]")]
    [ApiController]
    public class UsersController : ControllerBase {
        private readonly IMapper _mapper;
        private readonly IJwtGenerator _jwtGenerator;
        private object _results = null;
        public UsersController (IMapper mapper, IJwtGenerator jwtGenerator) {
            _jwtGenerator = jwtGenerator;
            _mapper = mapper;
        }

        [HttpPost ("login")]
        public ActionResult<AuthenticateResponse> Login (UserLogin model) {
            Boolean status = AuthenticateUsers.AuthenticateUser (model);
            if (status != false) {
                var response = new MapUserProfile (_mapper).UserProfileMapped (model);
                if (response != null) {
                    var token = _jwtGenerator.generateJwtToken (response, model.Username);
                    return new AuthenticateResponse (response, token);
                }
                return Unauthorized (new { message = "This user doesn't register in the system yet" });
            }
            return Unauthorized (new { message = "Username or password is incorrect" });
        }

        [Authorize]
        [HttpPost ("getProflie")]
        public IActionResult GetAllProfile (UserRequestUserProfile model) {
            var userProfile = _jwtGenerator.DecodeToken (model.token);
            return Ok (userProfile);
        }

        [Authorize]
        [HttpPost ("getMenu")]
        public async Task<ActionResult<AppUserToolModel>> PostToGetIcon (UserRequestMenu model) {
            try {
                switch (model.fn) {
                    case "fn1":
                        _results = await new StoreConnectionMethod (_mapper).KmapGetIconMenu (model);
                        break;
                    default: //ยังไม่สนใจ fn
                        _results = await new StoreConnectionMethod (_mapper).KmapGetIconMenu (model);
                        break;
                }
                if (_results != null)
                    return Ok (_results);
                return BadRequest (new { message = "Information from Oracle be null!!" });
            } catch (Exception e) {
                return BadRequest (new { message = e.Message });
            }
        }
    }
}