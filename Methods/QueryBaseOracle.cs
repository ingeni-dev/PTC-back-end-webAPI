using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;

namespace PTCwebApi.Methods {
    public class QueryBaseOracle {
        public QueryBaseOracle () {

        }
        public async Task<Boolean> checkToolingInWare (string ptc_id) {
            var queryCheck = $"SELECT COUNT(1) AS COUN FROM KPDBA.DIECUT_SN WHERE DIECUT_SN ='{ptc_id}'";
            var resultCheck = await new DataContext ().GetResultDapperAsyncDynamic (DataBaseHostEnum.KPR, queryCheck);
            decimal result = (resultCheck as List<dynamic>) [0].COUN;
            if (result == 0)
                return false;
            return true;

        }
    }
}