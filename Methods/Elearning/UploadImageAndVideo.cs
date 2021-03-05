using System;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using webAPI.Models.Elearning;

namespace webAPI.Methods.Elearning
{
    public class UploadImageAndVideo
    {
        private readonly IWebHostEnvironment _environment;
        public UploadImageAndVideo(IWebHostEnvironment environment) => _environment = environment;

        public string UploadFile(String queryID, String folderType, String fileName, IFormFile file)
        {
            // string pathRoot = "\\Users\\parnu\\Documents\\GitHub\\WEB-elearning\\src\\";
            // string pathRoot = "\\DEPLOY\\elearning";
            string pathRoot = "\\Users\\Public\\elearning";

            // string pathRoot= Directory.GetCurrentDirectory();
            // string pathImg = $"assets\\upload\\{queryID}\\{folderType}\\";
            string pathImg = $"\\upload\\{queryID}\\{folderType}\\";
            string path = pathRoot + pathImg;
            try
            {
                if (file.Length > 0)
                {
                    if (!Directory.Exists(_environment.WebRootPath + path))
                    {
                        Directory.CreateDirectory(_environment.WebRootPath + path);
                    }
                    string fileType = file.FileName.Split('.')[file.FileName.Split('.').Length - 1];
                    if (fileType == "blob") { fileType = "png"; }
                    using (FileStream fileStream = System.IO.File.Create(_environment.WebRootPath + path + fileName + "." + fileType))
                    {
                        file.CopyTo(fileStream);
                        fileStream.Flush();
                        return pathImg + fileName + "." + fileType;
                    }
                }
                else
                {
                    return "Failed";
                }
            }
            catch (Exception ex)
            {
                return ex.Message.ToString();
            }
        }
        public String GetRandomCharacter()
        {
            String randomStr = "";
            const string chars = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";
            Random rng = new Random();
            for (int i = 0; i < 4; i++)
            {
                int index = rng.Next(chars.Length);
                randomStr = randomStr + chars[index];
            }
            return randomStr;
        }
    }
}