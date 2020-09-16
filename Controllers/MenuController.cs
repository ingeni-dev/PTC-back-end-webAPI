using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PTCwebApi.Methods;
using PTCwebApi.Models.RequestModels;

namespace WebApi.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class MenuController : ControllerBase
    {
        private readonly IMapper _mapper;
        public MenuController(IMapper mapper) => _mapper = mapper;
        private object _results = null;

        // GET api/test
        [HttpGet("")]
        public ActionResult<IEnumerable<string>> Getstrings()
        {
            return new List<string> { };
        }

        [Authorize]
        [HttpPost("getMenu")]
        public async Task<ActionResult<AppUserToolModel>> PostToGetIcon(UserRequestMenu model)
        {
            try
            {
                switch (model.fn)
                {
                    case "fn1":
                        _results = await new StoreConnectionMethod(_mapper).KmapGetIconMenu(model);
                        break;
                    default: //ยังไม่สนใจ fn
                        _results = await new StoreConnectionMethod(_mapper).KmapGetIconMenu(model);
                        break;
                }
                if (_results != null)
                    return Ok(_results);
                return BadRequest(new { message = "Information from Oracle be null!!" });
            }
            catch (Exception e)
            {
                return BadRequest(new { message = e.Message });
            }
        }
    }
}