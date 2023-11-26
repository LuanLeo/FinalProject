using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Threading.Tasks;
using AspNetCoreHero.ToastNotification.Abstractions;
using DocumentFormat.OpenXml.InkML;
using DocumentFormat.OpenXml.Spreadsheet;
using ICSharpCode.SharpZipLib.Zip;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Utilities.Zlib;
using QRCoder;
using TablesideOrdering.Areas.Admin.Models;
using TablesideOrdering.Areas.StoreOwner.Models;
using TablesideOrdering.Data;
using TablesideOrdering.Models;
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
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInPass _signInPass;

        public INotyfService _notyfService { get; }

        public TablesController(ApplicationDbContext context, INotyfService notyfService, IOptions<SignInPass> signInPass, IWebHostEnvironment webHostEnvironment, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _notyfService = notyfService;
            this.webHostEnvironment = webHostEnvironment;
            _userManager = userManager;
            _signInPass = signInPass.Value;
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
        public async Task<IActionResult> Create(Table table)
        {
            var existTable = _context.Tables.FirstOrDefault(x => x.IdTable == table.IdTable);
            if (existTable == null)
            {
                table.QRImg = QRCreate(table);
                _context.Tables.Add(table);
                _context.SaveChanges();

                Chat chat = new Chat();
                chat.TableId = table.Id.ToString();
                chat.ChatRoomID = table.IdTable;
                _context.Chat.Add(chat);

                var user = CreateUser();
                user.TableId = table.Id;
                user.Firstname = table.IdTable.ToString();
                user.Lastname = table.IdTable.ToString();
                user.UserName =$"{table.IdTable}@gmail.com";
                user.Email = $"{table.IdTable}@gmail.com";

                var result = await _userManager.CreateAsync(user, _signInPass.AccPass);
                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user,"Customer");
                }

                _context.SaveChanges();

                _notyfService.Success("Table has been created!", 5);
                return RedirectToAction(nameof(Index));
            }
            _notyfService.Error("Table has already existed!", 5);
            return View(table);
        }
        private ApplicationUser CreateUser()
        {
            return Activator.CreateInstance<ApplicationUser>();
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
            return PartialView("Edit", model);
        }


        // POST: StoreOwner/Tables/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Table table)
        {
            if (ModelState.IsValid)
            {
                _context.Tables.Update(table);
                _context.SaveChanges();

                Chat chat = _context.Chat.FirstOrDefault(x => x.TableId == table.Id.ToString());
                chat.TableId = table.Id.ToString();
                chat.ChatRoomID = table.IdTable;
                _context.Chat.Update(chat);

                var user = _context.ApplicationUsers.FirstOrDefault(i=>i.TableId == table.Id);
                user.Firstname = table.IdTable.ToString();
                user.Lastname = table.IdTable.ToString();
                user.Email = $"{table.IdTable}@gmail.com";
                user.UserName = $"{table.IdTable}@gmail.com";

                await _userManager.UpdateAsync(user);
                _context.SaveChanges();
                _notyfService.Success("The info has been updated!", 5);
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
        public async Task<IActionResult> Delete(Table model)
        {
            var table = _context.Tables.Find(model.Id);
            if (table.Status == "Available")
            {
                string ExitingFile = Path.Combine(webHostEnvironment.WebRootPath, "QR", table.QRImg);
                System.IO.File.Delete(ExitingFile);
                _context.Tables.Remove(table);

                Chat chat = _context.Chat.FirstOrDefault(x => x.TableId == table.Id.ToString());
                _context.Chat.Remove(chat);

                var user = _context.ApplicationUsers.FirstOrDefault(i=>i.TableId == table.Id);
                _context.ApplicationUsers.Remove(user);

                await _context.SaveChangesAsync();
                _notyfService.Success("The table is deleted!", 5);
                return RedirectToAction("Index");
            }
            _notyfService.Warning("Table is being used!", 5);
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
