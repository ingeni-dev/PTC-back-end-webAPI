using System;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PTCwebApi.Interfaces;
using PTCwebApi.Methods.PTCMethods;
using PTCwebApi.Models.PTCModels.MethodModels;
using PTCwebApi.Models.PTCModels.MethodModels.MoveLoc;

namespace PTCwebApi.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class PtcController : ControllerBase
    {
        private readonly IJwtGenerator _jwtGenerator;
        private readonly IMapper _mapper;
        public PtcController(IMapper mapper, IJwtGenerator jwtGenerator)
        {
            _jwtGenerator = jwtGenerator;
            _mapper = mapper;
        }

        // [Authorize]
        [HttpPost("")]
        public async Task<ActionResult<Object>> ApiAllPTC(RequestAllModelsPTC model)
        {
            switch (model.fn)
            {

                case "FIND TOOL":
                    var _findToolResult = await new PTCMethods(_mapper, _jwtGenerator).FindLocOfTooling(model);
                    return Ok(_findToolResult);
                case "SCAN LOCATION TO MOVE":
                    var _moveToolResult = await new PTCMethods(_mapper, _jwtGenerator).MoveTooling(model, "4");
                    return Ok(_moveToolResult);
                case "CHECK USER WAREHOUSE":
                    var _WareHouseResult = await new PTCMethods(_mapper, _jwtGenerator).checkWareHouse(model);
                    return Ok(_WareHouseResult);
                case "CURRENT PLANS":
                    var _currentPlansResult = await new PTCMethodsPickUp(_mapper, _jwtGenerator).GetCurrentPlans(model);
                    return Ok(_currentPlansResult);
                case "PICK UP TO MOVE":
                    var _pickUpResult = await new PTCMethodsPickUp(_mapper, _jwtGenerator).PickUpTooling(model);
                    return Ok(_pickUpResult);
                case "GET WITHDRAWAL HISTORY":
                    var _historyResult = await new PTCMethodsReturnTool(_mapper, _jwtGenerator).getWithdrawalHistory(model);
                    return Ok(_historyResult);
                case "MOVE TO KEEP":
                    var _keepResult = await new PTCMethodsReturnTool(_mapper, _jwtGenerator).moveToKeep(model);
                    return Ok(_keepResult);

                //!Function DEV test, Don't forget to delete this function!!
                case "CHECK LOCATION OF TOOL":
                    var _findLocOfTool = await new PTCMethods(_mapper, _jwtGenerator).FindLocOfTooling(model);
                    return Ok(_findLocOfTool);
                default:
                    var res = new ReturnDataMoveLoc
                    {
                        flag = "1",
                        text = "\"error: กรุณากำหนด Function ให้ถูกต้อง\""
                    };
                    return res;
            }
        }
    }
}