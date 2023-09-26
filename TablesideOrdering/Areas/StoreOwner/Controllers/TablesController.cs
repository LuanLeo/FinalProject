using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Threading.Tasks;
using AspNetCoreHero.ToastNotification.Abstractions;
using DocumentFormat.OpenXml.Drawing;
using DocumentFormat.OpenXml.InkML;
using DocumentFormat.OpenXml.Office2010.Excel;
using ICSharpCode.SharpZipLib.Zip;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Utilities.Zlib;
using QRCoder;
using TablesideOrdering.Areas.Admin.Models;
using TablesideOrdering.Areas.StoreOwner.Models;
using TablesideOrdering.Data;
using Twilio.Rest.Preview.Marketplace.AvailableAddOn;
using Path = System.IO.Path;
using Table = TablesideOrdering.Areas.StoreOwner.Models.Table;

namespace TablesideOrdering.Areas.StoreOwner.Controllers
{
    [Area("StoreOwner")]
    [Authorize(Roles = "Store Owner, Admin")]

    public class TablesController : Controller
    {
        private readonly ApplicationDbContext _context;
        public readonly IWebHostEnvironment webHostEnvironment;

        public INotyfService _notyfService { get; }

        public static string TableId;
        public TablesController(ApplicationDbContext context, INotyfService notyfService, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _notyfService = notyfService;
            this.webHostEnvironment = webHostEnvironment;
        }

        // GET: StoreOwner/Tables
        public async Task<IActionResult> Index()
        {
            var TableList = await _context.Tables.ToListAsync();
            return View(TableList);
        }

        // GET: StoreOwner/Tables/Create
        public IActionResult Create()
        {
            Table model = new Table();
            return PartialView("Create", model);
        }

        // POST: StoreOwner/Tables/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Table table)
        {
            var existTable = _context.Tables.FirstOrDefault(x => x.IdTable == table.IdTable);
            if (existTable == null)
            {
                table.QRImg = QRCreate(table);
                _context.Tables.Add(table);
                _context.SaveChanges();

                Chat chat = new Chat();
                chat.TableId = table.IdTable.ToString();
                chat.ChatRoomID = table.IdTable.ToString();
                _context.Chat.Add(chat);
                _context.SaveChanges();

                _notyfService.Success("Table is created successfully", 5);
                return RedirectToAction(nameof(Index));
            }
            _notyfService.Error("Table has already existed ", 5);
            return View(table);
        }

        // GET: StoreOwner/Tables/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            if (id == null || _context.Tables == null)
            {
                return NotFound();
            }

            var table = await _context.Tables.FindAsync(id);
            Table model = new Table()
            {
                IdTable = table.IdTable,
                Status = table.Status,
                PeopleCap = table.PeopleCap,
            };

            TableId = model.IdTable.ToString();
            return PartialView("Edit", model);
        }


        // POST: StoreOwner/Tables/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Table table)
        {
            if (ModelState.IsValid)
            {
                _context.Tables.Update(table);

                Chat chat = _context.Chat.FirstOrDefault(x => x.TableId == table.Id.ToString());
                chat.TableId = table.IdTable.ToString();
                chat.ChatRoomID = table.IdTable.ToString();
                _context.Chat.Update(chat);

                _context.SaveChanges();
                _notyfService.Success("The info is edited succeesfully", 5);
                return RedirectToAction("Index");
            }

            _notyfService.Error("Something went wrong, try again!", 5);
            return View(table);
        }

        // GET: StoreOwner/Tables/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            if (id == null || _context.ProductSizePrice == null)
            {
                return NotFound();
            }

            var model = await _context.Tables.FirstOrDefaultAsync(m => m.Id == id);
            if (model == null)
            {
                return NotFound();
            }

            return PartialView("Delete", model);
        }

        // POST: StoreOwner/Tables/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(Table model)
        {
            var table = _context.Tables.Find(model.Id);
            if (table.Status == "Available")
            {
                string ExitingFile = Path.Combine(webHostEnvironment.WebRootPath, "QR", table.QRImg);
                System.IO.File.Delete(ExitingFile);
                _context.Tables.Remove(table);

                Chat chat = _context.Chat.FirstOrDefault(x => x.TableId == table.Id.ToString());
                _context.Chat.Remove(chat);

                _context.SaveChanges();

                _notyfService.Success("Table is deleted successfully", 5);
                return RedirectToAction("Index");
            }
            _notyfService.Warning("Table is being used", 5);
            return View("Index");
        }

        public string QRCreate(Table table)
        {
            string qrfilename = $"QRCode-Table{table.IdTable}.png";
            using (MemoryStream ms = new MemoryStream())
            {

                QRCodeGenerator QRCode = new QRCodeGenerator();
                string URL = $"{this.Request.Scheme}://{this.Request.Host}/Home/TableCheck/{table.IdTable}";
                QRCodeData data = QRCode.CreateQrCode(URL, QRCodeGenerator.ECCLevel.Q);
                QRCode code = new QRCode(data);
                using (Bitmap obit = code.GetGraphic(20))
                {
                    obit.Save(ms, ImageFormat.Png);
                    Image img = Image.FromStream(ms);
                    string TargetPath = Path.Combine(webHostEnvironment.WebRootPath, "QR", qrfilename);
                    img.Save(TargetPath, ImageFormat.Png);
                }
            }
            return qrfilename;
        }

        public IActionResult DownloadQR(int id)
        {
            Table tab = _context.Tables.FirstOrDefault(x => x.Id == id);
            string TargetPath = Path.Combine(webHostEnvironment.WebRootPath, "QR", tab.QRImg);
            MemoryStream memory = new MemoryStream();
            using (FileStream stream = new FileStream(TargetPath, FileMode.Open))
            {
                stream.CopyToAsync(memory);
            }
            memory.Position = 0;
            return File(memory, "image/png", Path.GetFileName(TargetPath));
        }

        public ActionResult DownloadAllQR()
        {
            var webRoot = webHostEnvironment.WebRootPath;
            var FileList = new List<string>();

            List<Table> tableList = _context.Tables.ToList();
            foreach (var tab in tableList)
            {
                FileList.Add(webRoot + "/QR/" + tab.QRImg);
            }

            var Name = "QR Code Images";
            var fileName = $"{Name}.zip";
            var tempOutput = webRoot + "QR" + fileName;

            using (ZipOutputStream oZipOutputStream = new ZipOutputStream(System.IO.File.Create(tempOutput)))
            {
                oZipOutputStream.SetLevel(9);
                byte[] buffer = new byte[4096];
                for (int i = 0; i < FileList.Count; i++)
                {
                    ZipEntry entry = new ZipEntry(Path.GetFileName(FileList[i]));
                    entry.DateTime = DateTime.Now;
                    entry.IsUnicodeText = true;
                    oZipOutputStream.PutNextEntry(entry);
                    using (FileStream oFileStream = System.IO.File.OpenRead(FileList[i]))
                    {
                        int sourceBytes;
                        do
                        {
                            sourceBytes = oFileStream.Read(buffer, 0, buffer.Length);
                            oZipOutputStream.Write(buffer, 0, sourceBytes);
                        } while (sourceBytes > 0);
                    }
                }
                oZipOutputStream.Finish();
                oZipOutputStream.Flush();
                oZipOutputStream.Close();
            }

            byte[] finalResult = System.IO.File.ReadAllBytes(tempOutput);
            if (System.IO.File.Exists(tempOutput))
            {
                System.IO.File.Delete(tempOutput);
            }
            if (finalResult == null || !finalResult.Any())
            {
                throw new Exception(String.Format("Nothing found"));
            }
            return File(finalResult, "application/zip", fileName);
        }
    }
}
