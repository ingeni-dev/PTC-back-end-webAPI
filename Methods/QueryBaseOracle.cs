using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using PTCwebApi.Models.PTCModels.Entities;
using PTCwebApi.Models.RequestModels;

namespace PTCwebApi.Methods {
    public class QueryBaseOracle {
        private readonly IMapper _mapper;
        public QueryBaseOracle (IMapper mapper) {
            _mapper = mapper;
        }
        public async Task<bool> checkToolingInWare (SerialNumber model) {
            var query = $"SELECT COUNT(1) FROM KPDBA.DIECUT_SN WHERE DIECUT_SN ='{model.PTC_ID}'";
            var results = await new DataContext ().GetResultDapperAsyncNew (DataBaseHostEnum.KPR, query);
            var result = _mapper.Map<IEnumerable<CheckToolingInWare>> (results);
            CheckToolingInWare count = result.ElementAt (0);
            // try {
            //     if (result != null)
            //         return true;
                return false;
            // } catch (Exception e) {
            //     return false;
            // }
        }
    }
}