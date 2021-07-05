using System;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using HeyRed.Mime;
using Microsoft.AspNetCore.Mvc;
using QRCoder;

namespace PTC_back_end_webAPI.Methods.ColorFolder
{
    public class CFGenerateQrcode
    {
        public String generateQrcode(string txt)
        {
            String qrcodeText = txt;
            QRCodeGenerator qrGenerator = new QRCodeGenerator();
            QRCodeData qrCodeData = qrGenerator.CreateQrCode(qrcodeText,
            QRCodeGenerator.ECCLevel.Q, true);
            QRCode qrCode = new QRCode(qrCodeData);
            Bitmap qrCodeImage = qrCode.GetGraphic(20);
            Byte[] imageBytes = BitmapToBytes(qrCodeImage);
            string base64String = Convert.ToBase64String(imageBytes);
            return base64String;
        }
        private static Byte[] BitmapToBytes(Bitmap img)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                img.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
                return stream.ToArray();
            }
        }
        public async Task<FileContentResult> GetFilePDF(String _name)
        {
            var fileStream = await File.ReadAllBytesAsync(_name);
            return new FileContentResult(fileStream, MimeTypesMap.GetMimeType(_name));
        }
    }
}