using System.IO;
using System.Threading.Tasks;
using AutoMapper;
using HeyRed.Mime;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using PTCwebApi.Interfaces;
using webAPI.Models.Elearning;

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
        public async Task<FileContentResult> DownloadDoc(FileParam model)
        {
            model.filePath = model.filePath.Replace("/", "\\");
            var ServerResourceUrl = "\\\\192.168.55.92\\Users\\Public\\elearning" + model.filePath;
            var fileStream = await File.ReadAllBytesAsync(ServerResourceUrl);
            return new FileContentResult(fileStream, MimeTypesMap.GetMimeType(ServerResourceUrl));
        }

    }
}