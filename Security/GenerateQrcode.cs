using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using Microsoft.AspNetCore.Mvc;
using QRCoder;
namespace webAPI.Security
{
    public class GenerateQrcode
    {
        public String generateQrcode(string txt)
        {
            // String qrText = EndoceBase64(txt);
            String qrcodeText = "http://localhost:4200/kmap-login-screen?queryID=" + txt;
            QRCodeGenerator qrGenerator = new QRCodeGenerator();
            QRCodeData qrCodeData = qrGenerator.CreateQrCode(qrcodeText,
            QRCodeGenerator.ECCLevel.Q);
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
        public String EndoceBase64(String txt)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(txt);
            return System.Convert.ToBase64String(plainTextBytes);

        }
        public String DecodeBase64(String base64String)
        {
            var base64EncodedBytes = System.Convert.FromBase64String(base64String);
            return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
        }

    }
}