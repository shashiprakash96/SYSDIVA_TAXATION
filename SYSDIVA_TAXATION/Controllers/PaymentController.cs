using Microsoft.AspNetCore.Mvc;
using QRCoder;
using SkiaSharp;
using System;

using System.Collections.Generic;

using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace SYSDIVA_TAXATION.Controllers
{
    public class PaymentController : Controller
    {
        public IActionResult Index() => View();

        [HttpPost]
        public IActionResult Generate(string qrText)
        {
            using (QRCodeGenerator qrGenerator = new QRCodeGenerator())
            using (QRCodeData qrCodeData = qrGenerator.CreateQrCode(qrText, QRCodeGenerator.ECCLevel.Q))
            {
                var qrCode = new SkiaSharpQRCode(qrCodeData);

                using (SKImage image = qrCode.GetGraphic(20))
                {
                    if (image == null)
                    {
                        ViewBag.Error = "Failed to generate QR image.";
                        return View("Index");
                    }

                    using (SKData data = image.Encode(SKEncodedImageFormat.Png, 100))
                    using (var ms = new MemoryStream())
                    {
                        data.SaveTo(ms);
                        var base64 = Convert.ToBase64String(ms.ToArray());
                        ViewBag.QrCodeImage = "data:image/png;base64," + base64;
                    }
                }
            }

            return View("Index");
        }




    }

    internal class SkiaSharpQRCode
    {
        private QRCodeData qrCodeData;

        public SkiaSharpQRCode(QRCodeData qrCodeData)
        {
            this.qrCodeData = qrCodeData;
        }

        internal SKImage GetGraphic(int v)
        {
            throw new NotImplementedException();
        }
    }
}
