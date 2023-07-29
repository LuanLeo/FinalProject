using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.Web.CodeGeneration;
using System.Drawing.Imaging;
using System.Drawing;
using TablesideOrdering.Data;
using QRCoder;
using TablesideOrdering.Areas.Admin.Models;

namespace TablesideOrdering.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class QRController : Controller
    {
        private readonly ApplicationDbContext _context;
        public INotyfService _notyfService { get; }

        public QRController(INotyfService notyfService, ApplicationDbContext context)
        {
            _notyfService = notyfService;
            _context = context;
        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult QRCreate(QRCodeModel QR)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                QRCodeGenerator QRCode = new QRCodeGenerator();
                QRCodeData data = QRCode.CreateQrCode(QR.Url, QRCodeGenerator.ECCLevel.Q);
                QRCode code = new QRCode(data);
                using (Bitmap obit = code.GetGraphic(20))
                {
                    obit.Save(ms, ImageFormat.Png);
                    ViewBag.QRCode = "data:image/png;base64," + Convert.ToBase64String(ms.ToArray());
                }
            }
            return View("Index");
        }
    }
}
