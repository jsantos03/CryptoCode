using Common.Controller;
using Gma.QrCodeNet.Encoding;
using Gma.QrCodeNet.Encoding.Windows.Render;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
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
            try
            {
                AESController aes = new AESController();

                if (aes.HasError())
                    throw new Exception(aes.error);

                string textoEncriptado = aes.Encrypt(text);

                if(aes.HasError())
                    throw new Exception(aes.error);

                var path = @"C:\Temp\qrcode.png";
                var qrEncoder = new QrEncoder(ErrorCorrectionLevel.H);
                var qrCode = qrEncoder.Encode(textoEncriptado);

                var renderer = new GraphicsRenderer(new FixedModuleSize(5, QuietZoneModules.Two), Brushes.Black, Brushes.White);
                using (var stream = new FileStream(path, FileMode.Create))
                {
                    renderer.WriteToStream(qrCode.Matrix, ImageFormat.Png, stream);
                }

                var file = new FileStream(path, FileMode.Open, FileAccess.Read);

                HttpResponseMessage result = new HttpResponseMessage(HttpStatusCode.OK);
                result.Content = new StreamContent(file);
                result.Content.Headers.ContentType = new MediaTypeHeaderValue("image/jpeg");
                return result;
            }
            catch (Exception ex)
            {
                HttpResponseMessage result = new HttpResponseMessage(HttpStatusCode.BadRequest);
                result.Content = new StringContent(ex.Message);
                result.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                return result;
            }
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
            if (aes.HasError())
                return aes.error;

            return aes.Decrypt(text);
        }
    }
}