using AutoMapper;
using Microsoft.AspNetCore.Hosting;
using PTCwebApi.Interfaces;

namespace webAPI.Methods.Elearning
{
    public class OnlineCourse
    {
        private readonly IWebHostEnvironment _environment;
        private readonly IMapper _mapper;
        private readonly IJwtGenerator _jwtGenerator;
        public OnlineCourse(IMapper mapper, IWebHostEnvironment environment, IJwtGenerator jwtGenerator)
        {
            _mapper = mapper;
            _environment = environment;
            _jwtGenerator = jwtGenerator;
        }
    }
}