using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using PTCwebApi.DataService;
using PTCwebApi.Models;
using PTCwebApi.Models.ProfilesModels;

namespace PTCwebApi.Methods {
    public class MapUserProfile {
        private readonly IMapper _mapper;
        public MapUserProfile (IMapper mapper) => _mapper = mapper;
        public UserProfile UserProfileMapped (UserLogin model) {
            var results = ConnectionProfile.SetProfile (model.Username);
            var result = _mapper.Map<IEnumerable<UserProfile>> (results);
            UserProfile user = result.ElementAt (0);
            return user;
        }
    }
}