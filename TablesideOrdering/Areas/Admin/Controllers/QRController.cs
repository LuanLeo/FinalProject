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
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

namespace TablesideOrdering.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class QRController : Controller
    {
        private readonly ApplicationDbContext _context;
        public readonly IWebHostEnvironment webHostEnvironment;

        public INotyfService _notyfService { get; }

        public QRController(INotyfService notyfService, ApplicationDbContext context, IWebHostEnvironment _webHostEnvironment)
        {
            _notyfService = notyfService;
            _context = context;
            webHostEnvironment = _webHostEnvironment;

        }
        public IActionResult Index()
        {
            ViewBag.TableList = TableList();
            return View();
        }

        public List<SelectListItem> TableList()
        {
            var tableList = _context.Tables.ToList();
            List<SelectListItem> list = new List<SelectListItem>();
            foreach (var t in tableList)
            {
                list.Add(new SelectListItem { Text = t.IdTable.ToString(), Value = t.IdTable.ToString() });
            }
            return list;
        }

        public IActionResult QRCreate(QRCodeModel QR)
        {
            ViewBag.TableList = TableList();
            using (MemoryStream ms = new MemoryStream())
            {
                QRCodeGenerator QRCode = new QRCodeGenerator();
                string URL = $"{this.Request.Scheme}://{this.Request.Host}/Home/TableCheck/{QR.TableNo}";
                QRCodeData data = QRCode.CreateQrCode(URL, QRCodeGenerator.ECCLevel.Q);
                QRCode code = new QRCode(data);
                using (Bitmap obit = code.GetGraphic(20))
                {
                    obit.Save(ms, ImageFormat.Png);
                    Image img = Image.FromStream(ms);
                    string TargetPath = Path.Combine(webHostEnvironment.WebRootPath, "QR", $"QRCode-Table{QR.TableNo}.png");
                    img.Save(TargetPath, ImageFormat.Png);
                }
            }
            return View("Index");
        }

    }
}
