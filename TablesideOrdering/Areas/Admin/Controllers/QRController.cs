using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.Web.CodeGeneration;
using System.Drawing.Imaging;
using System.Drawing;
using TablesideOrdering.Data;
using QRCoder;
using TablesideOrdering.Areas.Admin.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

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

        public List<SelectListItem> TableList()
        {
            var tableList = _context.Tables.ToList();
            List<SelectListItem> list = new List<SelectListItem>();
            foreach (var t in tableList)
            {
                list.Add(new SelectListItem{ Text= t.IdTable, Value=t.IdTable});
            }
            return list;
        }

        public IActionResult QRCreate(QRCodeModel QR)
        {
            ViewBag.TableList = TableList();
            using (MemoryStream ms = new MemoryStream())
            {
                QRCodeGenerator QRCode = new QRCodeGenerator();
                QRCodeData data = QRCode.CreateQrCode(QR.Url+"/"+QR.TableNo, QRCodeGenerator.ECCLevel.Q);
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
