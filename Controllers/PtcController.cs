using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PTCwebApi.Interfaces;
using PTCwebApi.Methods;
using PTCwebApi.Models.ProfilesModels;
using PTCwebApi.Models.PTCModels;
using PTCwebApi.Models.PTCModels.Entities;
using PTCwebApi.Models.PTCModels.MethodModels.MoveLoc;
using PTCwebApi.Models.RequestModels;
//using PTC-back-end-webAPI.Models;

namespace PTCwebApi.Controllers {
    [Route ("[controller]")]
    [ApiController]
    public class PtcController : ControllerBase {
        private readonly IJwtGenerator _jwtGenerator;
        private readonly IMapper _mapper;
        public PtcController (IMapper mapper, IJwtGenerator jwtGenerator) {
            _jwtGenerator = jwtGenerator;
            _mapper = mapper;
        }

        [HttpPost ("countToolingInWare")]
        public async Task<object> Post (SerialNumber model) {
            // !Task<bool> countToolingInWare = new QueryBaseOracle (_mapper).checkToolingInWare (model);
            // bool Item = countToolingInWare.Result;
            var query = $"SELECT COUNT(1) FROM KPDBA.DIECUT_SN WHERE DIECUT_SN ='{model.PTC_ID}'";
            // var _connectionString = $"select * from kpdba.ptc_stock_detail where ptc_id = '{model.PTC_ID}'";

            var result = await new DataContext ().GetResultDapperAsyncNew (DataBaseHostEnum.KPR, query);
            var results = _mapper.Map<IEnumerable<CheckToolingInWare>> (result);
            CheckToolingInWare count = results.ElementAt (0);
            return result;
        }

        [HttpPost ("importtooling")]
        public async Task<ActionResult<ModelTablePTCDetail>> ImportTooling (SerialNumber model) {
            UserProfile userProfile = _jwtGenerator.DecodeToken (model.token);
            // !Task<bool> countToolingInWare = new QueryBaseOracle (_mapper).checkToolingInWare (model);
            var query = $"SELECT * FROM KPDBA.PTC_STOCK_DETAIL WHERE PTC_ID ='{model.PTC_ID}'";
            var result = await new DataContext ().GetResultDapperAsyncNew (DataBaseHostEnum.KPR, query);
            var results = _mapper.Map<IEnumerable<ModelTablePTCDetail>> (result);
            ModelTablePTCDetail count = results.ElementAt (0);
            //Get tran ID by store
            var tran_id = await new StoreConnectionMethod (_mapper).PtcGetTranID (count);

            var insertQuery = $@"INSERT INTO KPDBA.PTC_STOCK_DETAIL 
            (TRAN_ID, TRAN_SEQ, TRAN_TYPE, TRAN_DATE,PTC_ID, QTY, COMP_ID, WAREHOUSE_ID, 
            LOC_ID, STATUS, CR_DATE, CR_ORG_ID, CR_USER_ID) 
            VALUES ({count.TRAN_ID}, TO_NUMBER({count.TRAN_SEQ}), TO_NUMBER({count.TRAN_TYPE}), 
            TO_DATE({count.TRAN_DATE}, 'dd/mm/yyyy hh24:mi:ss'),{count.PTC_ID}, 
            TO_NUMBER('{count.QTY-1}'),  {count.COMP_ID},{count.WAREHOUSE_ID}, {count.LOC_ID}, 'F', 
            {count.CR_DATE}, {userProfile.org}, {userProfile.userID});";

            return Ok (tran_id);
        }

        [HttpPost ("movetooling")]
        public object MoveTooling (SerialNumber modelFT,RequestDataMoveLoc modelML) {

            return null;
        }

    }
}