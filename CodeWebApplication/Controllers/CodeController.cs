using Common.Controller;
using Gma.QrCodeNet.Encoding;
using Gma.QrCodeNet.Encoding.Windows.Render;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http;

namespace CodeApp.Controllers
{
    public class CodeController : ApiController
    {
        /// <summary>
        /// GET api/Code/GetQR?texto=hello
        /// Retonar el codigo QR encriptado 
        /// Debe existir la carpeta C:\Temp
        /// </summary>
        /// <param name="texto"></param>
        /// <returns>image/jpeg (QR Code)</returns>
        [HttpGet]
        public HttpResponseMessage GetQr(string text)
        {
            AESController aes = new AESController();
            string textoEncriptado = aes.Encrypt(text);

            var qrEncoder = new QrEncoder(ErrorCorrectionLevel.H);
            var qrCode = qrEncoder.Encode(textoEncriptado);

            var renderer = new GraphicsRenderer(new FixedModuleSize(5, QuietZoneModules.Two), Brushes.Black, Brushes.White);
            using (var stream = new FileStream(@"C:\Temp\qrcode.png", FileMode.Create))
            {
                renderer.WriteToStream(qrCode.Matrix, ImageFormat.Png, stream);
            }

            var path = @"C:\Temp\qrcode.png";
            var file = new FileStream(path, FileMode.Open, FileAccess.Read);

            HttpResponseMessage result = new HttpResponseMessage(HttpStatusCode.OK);
            result.Content = new StreamContent(file);
            result.Content.Headers.ContentType = new MediaTypeHeaderValue("image/jpeg");
            return result;
        }

        /// <summary>
        /// Desencripta el texto recibido
        /// </summary>
        /// <param name="text"></param>
        /// <returns>Texto desencriptado</returns>
        [HttpGet]
        public string Decrypt(string text)
        {
            AESController aes = new AESController();
            return aes.Decrypt(text);
        }
    }
}
