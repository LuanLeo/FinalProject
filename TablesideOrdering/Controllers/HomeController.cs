﻿using AspNetCoreHero.ToastNotification.Abstractions;
using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MimeKit;
using Microsoft.AspNetCore.Hosting;
using TablesideOrdering.Areas.StoreOwner.Models;
using TablesideOrdering.Areas.StoreOwner.ViewModels;
using TablesideOrdering.Data;
using TablesideOrdering.Migrations;
using TablesideOrdering.Models;
using TablesideOrdering.Models.Order;
using TablesideOrdering.PaymentServices.Momo;
using TablesideOrdering.PaymentServices.VNPay;
using TablesideOrdering.ViewModels;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;
using TopFoodSizePrice = TablesideOrdering.Models.TopFoodSizePrice;
using Twilio.Jwt.AccessToken;
using System.Text;
using System.IO;
using Twilio.TwiML.Messaging;
using System.Security.Cryptography.X509Certificates;
using Microsoft.AspNetCore.Identity.UI.V4.Pages.Account.Internal;
using IHostingEnvironment = Microsoft.AspNetCore.Hosting.IHostingEnvironment;
using Syncfusion.Pdf;
using Syncfusion.Pdf.Graphics;
using Syncfusion.Drawing;
using System.Net;
using Org.BouncyCastle.Utilities.Net;
using Syncfusion.Pdf.Grid;
using System.Net.Mime;
using Org.BouncyCastle.Utilities;
using TablesideOrdering.SignalR.Repositories;

namespace TablesideOrdering.Controllers
{
    public class HomeController : Controller
    {
        //Call Database and Relatives
        private readonly IHostingEnvironment _host;
        private readonly ApplicationDbContext _context;
        private readonly SMSMessage _SMSMessage;
        private readonly Email _email;

        //Call Additional Services
        public INotyfService _notyfService { get; }
        private readonly IVnPayService _vnPayService;
        private IMomoService _momoService;

        //Static variables before saving to database
        public static List<AddToCart> carts = new List<AddToCart>();
        public static float TotalPrice;
        public static string PhoneNumber;
        public static string TableNo;
        public static string CusName;
        public static string Message;
        public static string Subject;
        public static string Email;
        public static string file;

        public HomeController(ApplicationDbContext context,
            INotyfService notyfService,
            IVnPayService vnPayService,
            IOptions<SMSMessage> SMSMessage,
            IOptions<Email> email,
            IMomoService momoService,
            IHostingEnvironment host)
        {
            _context = context;

            _SMSMessage = SMSMessage.Value;
            _email = email.Value;

            _notyfService = notyfService;
            _vnPayService = vnPayService;
            _momoService = momoService;

            _host = host;

        }

        //CONTROLLER FOR HOME PAGE
        [HttpGet]
        public IActionResult Index()
        {

            HomeViewModel Homedata = new HomeViewModel();
            Homedata = NavData();
            Homedata.Product = (from ProSP in _context.ProductSizePrice
                                join Pro in _context.Products on ProSP.ProductId equals Pro.ProductId
                                select new ProductSizePriceViewModel
                                {
                                    SizePriceId = ProSP.Id,
                                    ProductId = Pro.ProductId,
                                    Name = Pro.Name,
                                    CategoryId = Pro.CategoryId,
                                    Description = Pro.Description,
                                    Pic = Pro.Pic,
                                    Size = ProSP.Size,
                                    Price = ProSP.Price
                                });
            Homedata.Category = _context.Categories.ToList();
            return View(Homedata);
        }






        //CONTROLLER FOR MENU PAGE
        public IActionResult Menu(string term = "", string orderBy = "")
        {
            term = string.IsNullOrEmpty(term) ? "" : term.ToLower();
            var productList = (from ProSP in _context.ProductSizePrice
                               join Pro in _context.Products on ProSP.ProductId equals Pro.ProductId
                               join Cat in _context.Categories on Pro.CategoryId equals Cat.CategoryId
                               where (term == "" || Pro.Name.Contains(term) || Cat.CategoryName.Contains(term) || ProSP.Size.Contains(term))
                               select new ProductSizePriceViewModel
                               {
                                   SizePriceId = ProSP.Id,
                                   ProductId = Pro.ProductId,
                                   Name = Pro.Name,
                                   CategoryId = Pro.CategoryId,
                                   Description = Pro.Description,
                                   Pic = Pro.Pic,
                                   Size = ProSP.Size,
                                   Price = ProSP.Price
                               });

            HomeViewModel Homedata = new HomeViewModel();
            Homedata.NameSort = string.IsNullOrEmpty(orderBy) ? "NameDesc" : "";
            switch (orderBy)
            {
                case "PriceAsc":
                    productList = productList.OrderBy(a => a.Price);
                    break;
                case "PriceDesc":
                    productList = productList.OrderByDescending(a => a.Price);
                    break;
                case "NameDesc":
                    productList = productList.OrderByDescending(a => a.Name).ThenByDescending(a => a.Size);
                    break;
                default:
                    productList = productList.OrderBy(a => a.Name).ThenBy(a => a.Size);
                    break;
            }
            Homedata = NavData();
            Homedata.Category = _context.Categories.ToList();
            Homedata.ProductSizes = _context.ProductSize.ToList();
            Homedata.Product = productList;
            Homedata.TopProduct = GetTopFood();
            Homedata.Term = term;
            return View(Homedata);
        }

        public List<TopFood> GetTopFood()
        {
            //Take infor from orders
            List<TopFoodSizePrice> toplist = (from p in _context.OrderDetails
                                              select new TopFoodSizePrice
                                              {
                                                  Price = p.Price,
                                                  Size = p.Size,
                                                  Name = p.ProductName,
                                              }).ToList();

            //Join table Product and Product Size Price
            var FoodDis = toplist.GroupBy(i => new { i.Name, i.Size, i.Price }).Select(i => i.FirstOrDefault()).ToList();
            var product = (from Pro in _context.ProductSizePrice
                           join Prod in _context.Products on Pro.ProductId equals Prod.ProductId
                           join Cat in _context.Categories on Prod.CategoryId equals Cat.CategoryId
                           select new ProductFull
                           {
                               ProductId = Prod.ProductId,
                               Category = Cat.CategoryName,
                               Name = Prod.Name,
                               Description = Prod.Description,
                               Pic = Prod.Pic,
                               Size = Pro.Size,
                               Price = Pro.Price
                           }).ToList();

            List<ProductFull> productfull = new List<ProductFull>();
            foreach (var food in FoodDis)
            {
                foreach (var prod in product)
                {
                    if (food.Name == prod.Name && food.Price == prod.Price && food.Size == prod.Size)
                    {
                        productfull.Add(prod);
                    }
                }
            }

            List<ProductFull> productList = new List<ProductFull>();
            foreach (var pro in productfull)
            {
                foreach (var p in _context.ProductSizePrice)
                {
                    if (pro.ProductId == p.ProductId && pro.Size == p.Size)
                    {
                        ProductFull prod = new ProductFull();
                        prod = pro;
                        prod.ProSizePriceId = p.Id;

                        productList.Add(prod);
                    }
                }
            }

            List<TopFood> topFood = new List<TopFood>();
            foreach (var item in productList)
            {
                float Price = 0;
                foreach (var food in _context.OrderDetails)
                {
                    if (item.Name == food.ProductName && item.Size == food.Size)
                    {
                        Price += item.Price;
                    }
                }

                TopFood top = new TopFood();
                top.ProductFull = item;
                top.TotalPrice = Price;
                topFood.Add(top);
            }

            List<TopFood> FoodList = topFood.OrderByDescending(i => i.TotalPrice).Take(6).ToList();
            return FoodList;
        }





        //CONTROLLER FOR RECEIVING ADS MAIL
        [HttpPost]
        public IActionResult MailPR(HomeViewModel home)
        {
            EmailPR Email = new EmailPR();
            Email.Email = home.MailPR;

            var emailList = _context.EmailPRs.Select(i => i.Email).ToList();
            if (emailList != null)
            {
                if (emailList.Contains(Email.Email) != true)
                {
                    _context.EmailPRs.Add(Email);
                    _context.SaveChanges();
                    _notyfService.Success("Your email is subcribed success", 5);
                }
                else
                {
                    _notyfService.Error("Your email has been subcribed", 5);
                };
            }
            else
            {
                _context.EmailPRs.Add(Email);
                _context.SaveChanges();
                _notyfService.Success("Your email is subcribed success", 5);
            }
            return RedirectToAction("Index");
        }





        //CONTROLLER FOR INPUTTING PHONE PAGE
        [HttpGet]
        public IActionResult PhoneValidation()
        {
            return View();
        }

        [HttpPost]
        public IActionResult PhoneValidation(HomeViewModel home)
        {
            if (home.PhoneValid.PhoneNumber == home.PhoneValid.PhoneConfirmed && CheckValid(home) == true)
            {
                PhoneNumber = home.PhoneValid.PhoneNumber;
                TableNo = home.PhoneValid.TableNo;
                CusName = home.PhoneValid.CusName;
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        public Boolean CheckValid(HomeViewModel home)
        {
            if ((home.PhoneValid.PhoneNumber != null && home.PhoneValid.PhoneConfirmed != null && home.PhoneValid.CusName != null && home.PhoneValid.TableNo != null) == true)
            {
                return true;
            };
            return false;
        }

        public string ConvertToPhoneValid()
        {
            string number = PhoneNumber.Substring(1);
            string validnum = "+84" + number;
            return validnum;
        }

        public IActionResult Return()
        {
            return RedirectToAction("Index", "Home");
        }
        public void PdfGen(Orders order, List<OrderDetail> orderDetailList, Email data)
        {
            //add image
            PdfDocument pdfDocument = new PdfDocument();
            PdfPage currentPage = pdfDocument.Pages.Add();
            SizeF clientSize = currentPage.GetClientSize();
            string logoRootPath = _host.WebRootPath + "/Logo/horizontal logo.png";
            FileStream imageStream = new FileStream(logoRootPath, FileMode.Open, FileAccess.Read);
            PdfImage icon = new PdfBitmap(imageStream);
            SizeF iconSize = new SizeF(200, 40);
            PointF iconLocation = new PointF(14, 13);
            PdfGraphics graphics = currentPage.Graphics;
            graphics.DrawImage(icon, iconLocation, iconSize);
            PdfFont font = new PdfStandardFont(PdfFontFamily.Helvetica, 20, PdfFontStyle.Bold);
            var text = new PdfTextElement("Invoice", font, new PdfSolidBrush(Color.FromArgb(1, 53, 67, 168)));
            text.StringFormat = new PdfStringFormat(PdfTextAlignment.Right);
            PdfLayoutResult result = text.Draw(currentPage, new PointF(clientSize.Width - 25, iconLocation.Y + 10));

            font = new PdfStandardFont(PdfFontFamily.Helvetica, 10);
            text = new PdfTextElement("Customer:", font);
            result = text.Draw(currentPage, new PointF(14, result.Bounds.Bottom + 30));
            font = new PdfStandardFont(PdfFontFamily.Helvetica, 14, PdfFontStyle.Bold);
            text = new PdfTextElement($"{order.CusName}", font);
            result = text.Draw(currentPage, new PointF(14, result.Bounds.Bottom + 3));

            font = new PdfStandardFont(PdfFontFamily.Helvetica, 10, PdfFontStyle.Bold);
            text = new PdfTextElement($"Invoice No.#{order.OrderId}", font);
            text.StringFormat = new PdfStringFormat(PdfTextAlignment.Right);
            text.Draw(currentPage, new PointF(clientSize.Width - 25, result.Bounds.Y - 20));
            font = new PdfStandardFont(PdfFontFamily.Helvetica, 10, PdfFontStyle.Bold);


            PdfGrid grid = new PdfGrid();
            font = new PdfStandardFont(PdfFontFamily.Helvetica, 10, PdfFontStyle.Regular);
            grid.Style.Font = font;
            grid.Columns.Add(4);
            grid.Columns[1].Width = 70;
            grid.Columns[2].Width = 70;
            grid.Columns[3].Width = 70;

            grid.Headers.Add(1);
            PdfStringFormat stringFormat = new PdfStringFormat(PdfTextAlignment.Right, PdfVerticalAlignment.Middle);
            PdfGridRow header = grid.Headers[0];
            header.Cells[0].Value = " Product Name";
            header.Cells[0].StringFormat.LineAlignment = PdfVerticalAlignment.Middle;
            header.Cells[1].Value = " Size ";
            header.Cells[1].StringFormat = stringFormat;
            header.Cells[2].Value = " Quantity ";
            header.Cells[2].StringFormat = stringFormat;
            header.Cells[3].Value = " Price (VND) ";
            header.Cells[3].StringFormat = stringFormat;
            PdfGridRow row;
            foreach (var item in orderDetailList)
            {
                row = grid.Rows.Add();
                row.Cells[0].Value = $"{item.ProductName}";
                row.Cells[0].StringFormat.LineAlignment = PdfVerticalAlignment.Middle;

                row.Cells[1].Value = $"{item.Size}";
                row.Cells[1].StringFormat = stringFormat;

                row.Cells[2].Value = $"{item.ProQuantity}";
                row.Cells[2].StringFormat = stringFormat;

                row.Cells[3].Value = $"{item.Price}";
                row.Cells[3].StringFormat = stringFormat;
            }




            grid.ApplyBuiltinStyle(PdfGridBuiltinStyle.GridTable4Accent5);
            PdfGridStyle gridStyle = new PdfGridStyle();
            gridStyle.CellPadding = new PdfPaddings(5, 5, 5, 5);
            PdfGridLayoutFormat layoutFormat = new PdfGridLayoutFormat();
            layoutFormat.Layout = PdfLayoutType.Paginate;
            result = grid.Draw(currentPage, 14, result.Bounds.Bottom + 30, clientSize.Width - 35, layoutFormat);
            currentPage.Graphics.DrawRectangle(new PdfSolidBrush(Color.FromArgb(255, 239, 242, 255)),
                new RectangleF(result.Bounds.Right - 100, result.Bounds.Bottom + 20, 100, 25));

            PdfTextElement element = new PdfTextElement("Total", font);
            element.Draw(currentPage, new RectangleF(result.Bounds.Right - 100, result.Bounds.Bottom + 22, result.Bounds.Width, result.Bounds.Height));
            var totalPrice = $"{order.OrderPrice} VND";
            element = new PdfTextElement(totalPrice, font);
            element.StringFormat = new PdfStringFormat(PdfTextAlignment.Right);
            element.Draw(currentPage, new RectangleF(15, result.Bounds.Bottom + 22, result.Bounds.Width, result.Bounds.Height));



            //Saving the PDF to the MemoryStream/
            MemoryStream stream = new MemoryStream();

            pdfDocument.Save(stream);
            pdfDocument.Close(true);
            stream.Position = 0;
            file = $"Invoice-{order.OrderId}.pdf";
            System.IO.File.WriteAllBytes(file, stream.ToArray());


            /*//Set the position as '0'.
            stream.Position = 0;

            //Download the PDF document in the browser.
            FileStreamResult fileStreamResult = new FileStreamResult(stream, "application/pdf");

            fileStreamResult.FileDownloadName = "Sample.pdf";*/
        }



        //CONTROLLER FOR SENDING MAIL
        public void SendMail(Email data)
        {
            data.EmailFrom = _email.EmailFrom;
            data.Password = _email.Password;
            data.Body = Message;
            data.Subject = Subject;

            /*_host.WebRootPath = System.IO.Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
            string contentRootPath = _host.WebRootPath + "/Logo/Logo.png";*/
            var builder = new BodyBuilder();
            builder.HtmlBody = Message.ToString();
            builder.Attachments.Add(file);

            var email = new MimeMessage();
            email.From.Add(MailboxAddress.Parse(data.EmailFrom));
            email.To.Add(MailboxAddress.Parse(data.EmailTo));
            email.Subject = data.Subject;
            //email.Body = new TextPart(MimeKit.Text.TextFormat.Html) { Text = data.Body };
            email.Body = builder.ToMessageBody();
            //email.Attachments = new Attachment(file, "Test.pdf");


            using var smtp = new SmtpClient();
            {
                smtp.Connect("smtp.gmail.com", 587, MailKit.Security.SecureSocketOptions.StartTls);
                smtp.Authenticate(data.EmailFrom, data.Password);
                smtp.Send(email);
                smtp.Disconnect(true);
            }
        }

        //CONTROLLER FOR SENDING RECEIPT    
        public void Invoice(Orders order, List<OrderDetail> orderDetailList, Email data)
        {
            StringBuilder subject = new StringBuilder();
            subject.Append("E-Invoice order ").Append(order.OrderId).Append(" at L&L coffee shop ");
            StringBuilder invoiceHtml = new StringBuilder();
            invoiceHtml.Append("<b >E-Invoice at L&L coffee shop ").Append("</b><br />");
            invoiceHtml.Append("<br /><b>Date : </b>").Append(DateTime.Now.ToShortDateString()).Append("<br />");
            invoiceHtml.Append("<b>Table : </b>").Append(order.TableNo).Append("<br />");
            invoiceHtml.Append("<b>Invoice Total :</b> ").Append(order.OrderPrice.ToString()).Append(" VND<br />");
            invoiceHtml.Append("<br /><b>CUSTOMER CONTACT INFO:</b><br />");
            invoiceHtml.Append("<b>Name : </b>").Append(order.CusName).Append("<br />");
            invoiceHtml.Append("<b>Phone : </b>").Append(order.PhoneNumber).Append("<br />");
            invoiceHtml.Append("<b>Email : </b>").Append(data.EmailTo).Append("<br />");

            invoiceHtml.Append("<br /><b>PRODUCTS:</b><br /><table><tr><th>Product Name  </th><th>Size  </th><th>Quantity  </th><th>Total</th></tr>");
            // InvoiceItem should be a collection property which contains list of invoice lines
            foreach (var product in orderDetailList)
            {
                invoiceHtml.Append("<tr><td>").Append(product.ProductName).Append("</td><td>").Append(product.Size).Append(@"</td><td style = ""text-align: center;"">").Append(product.ProQuantity.ToString()).Append("</td><td>").Append(product.Price.ToString()).Append(" VND</td></tr>");
            }
            invoiceHtml.Append("</table>");
            invoiceHtml.Append("</div>");
            Subject = subject.ToString();
            Message = invoiceHtml.ToString();
        }





        //CONTROLLER FOR CART PAGE
        public IActionResult Cart()
        {
            HomeViewModel home = NavData();
            if (home.Cart.PhoneNumber != null)
            {
                return View(home);
            }
            return RedirectToAction("PhoneValidation");
        }

        //Add to Cart from Menu
        public IActionResult MenuCart(int id)
        {
            AddToCart(id);
            _notyfService.Success("Add to cart success", 5);
            return RedirectToAction("Menu", "Home");
        }

        public void AddToCart(int id)
        {
            AddToCart cart = new AddToCart();
            ProductSizePrice productprice = _context.ProductSizePrice.Find(id);
            ProductSizePriceViewModel model = new ProductSizePriceViewModel();

            if (carts.Count() == 0)
            {
                cart.SizePriceId = productprice.Id;
                cart.Product = _context.Products.Find(productprice.ProductId);
                cart.Quantity = 1;
                cart.Size = productprice.Size;
                cart.Price = productprice.Price;
                cart.TotalProPrice = productprice.Price * cart.Quantity;
                carts.Add(cart);
            }
            else
            {
                if (carts.Find(x => x.SizePriceId == productprice.Id) != null)
                {
                    cart = carts.Single(x => x.SizePriceId == productprice.Id);
                    cart.Quantity += 1;
                    cart.TotalProPrice = productprice.Price * cart.Quantity;
                }
                else
                {
                    cart.SizePriceId = productprice.Id;
                    cart.Product = _context.Products.Find(productprice.ProductId);
                    cart.Quantity = 1;
                    cart.Size = productprice.Size;
                    cart.Price = productprice.Price;
                    cart.TotalProPrice = productprice.Price * cart.Quantity;

                    carts.Add(cart);
                }
            }

            TotalPrice = 0;
            foreach (var item in carts)
            {
                float Total = item.Quantity * item.Price;
                TotalPrice += Total;
            }
            ViewBag.CartQuantity = carts.Count();
            ViewBag.CartPrice = TotalPrice;
            NavData();
        }

        //DELETE from cart
        public IActionResult DeleteFromCart(int id)
        {
            AddToCart cart = new AddToCart();
            cart = carts.Find(x => x.SizePriceId == id);
            if (cart != null)
            {
                carts.Remove(cart);
            }

            TotalPrice = 0;
            foreach (var item in carts)
            {
                float Total = item.Quantity * item.Price;
                TotalPrice += Total;
            }

            _notyfService.Success("The product is deleted", 5);
            return RedirectToAction("Cart", "Home");
        }





        //CONTROLLER FOR SENDING SMS TO CUSTOMER
        public void SendSMS()
        {
            string number = ConvertToPhoneValid();
            TwilioClient.Init(_SMSMessage.AccountSid, _SMSMessage.AuthToken);
            var message = MessageResource.Create(
                to: new PhoneNumber(number),
                from: new PhoneNumber(_SMSMessage.PhoneFrom),
                body: Message);
        }






        //CONTROLLER FOR SELECTING PAYMENT TYPE
        public IActionResult PaymentMethod(HomeViewModel home)
        {
            if (carts.Count != 0)
            {
                if (home.PaymentType == null)
                {
                    _notyfService.Information("Please select one Payment Method!", 5);
                    return RedirectToAction("Cart");
                }
                else
                {
                    if (home.PaymentType == "VNPay")
                    {
                        return RedirectToAction("VNPayCheckout");
                    }
                    if (home.PaymentType == "Momo")
                    {
                        return RedirectToAction("MomoCheckout");
                    }
                    if (home.PaymentType == "Cash")
                    {
                        return RedirectToAction("CashCheckout");
                    }
                }
            }
            else
            {
                _notyfService.Warning("The cart is emtpy", 5);
                return RedirectToAction("Cart");
            }
            _notyfService.Error("Something went wrong, please try again!", 5);
            return RedirectToAction("Cart");
        }

        //CONTROLLER FOR CASH PAYMENT METHOD PAGE
        public IActionResult CashCheckout()
        {
            HomeViewModel home = NavData();
            return View(home);
        }

        //CONTROLLER FOR CASH PAYMENT METHOD PAGE
        public IActionResult PlaceOrder(HomeViewModel home)
        {
            Email = home.Email.EmailTo;
            Email data = new Email();
            data.EmailTo = Email;

            //Save order to database
            Orders order = new Orders();
            order.OrderDate = DateTime.Now;
            order.OrderPrice = TotalPrice;
            order.ProductQuantity = carts.Count();
            order.PhoneNumber = PhoneNumber;
            order.TableNo = TableNo;
            order.CusName = CusName;
            order.Status = "Not Paid";

            _context.Orders.Add(order);
            _context.SaveChanges();

            //Save order list to database
            List<OrderDetail> orderDetailList = new List<OrderDetail>();
            foreach (var item in carts)
            {
                OrderDetail orderDetail = new OrderDetail();
                orderDetail.OrderId = order.OrderId;
                orderDetail.ProductName = item.Product.Name;
                orderDetail.Size = item.Size;
                orderDetail.ProQuantity = item.Quantity;
                orderDetail.Price = item.Quantity * item.Price;

                orderDetailList.Add(orderDetail);
            }

            foreach (var orderDt in orderDetailList)
            {
                _context.OrderDetails.Add(orderDt);
            }
            _context.SaveChanges();
            if (data.EmailTo != null)
            {
                PdfGen(order, orderDetailList, data);
                Invoice(order, orderDetailList, data);
                SendMail(data);
            }

            //Renew the cart and notify customer
            TotalPrice = 0;
            carts.Clear();
            return RedirectToAction("ThankYou");
        }

        //CONTROLLER FOR CASH CHECKOUT PAGE
        public IActionResult ThankYou()
        {
            return View();
        }

        //CONTROLLER FOR VNPAY PAYMENT METHOD PAGE
        public IActionResult VNPayCheckout()
        {
            HomeViewModel home = NavData();
            return View(home);
        }

        public IActionResult CreatePaymentUrl(HomeViewModel home)
        {
            PaymentInformationModel model = new PaymentInformationModel();
            model = home.Payment;
            model.Amount = TotalPrice;

            Email = home.Email.EmailTo;

            var url = _vnPayService.CreatePaymentUrl(model, HttpContext);
            return Redirect(url);
        }

        public IActionResult PaymentCallback()
        {
            var response = _vnPayService.PaymentExecute(Request.Query);
            if (response.VnPayResponseCode == "00")
            {
                //Save order to database
                Orders order = new Orders();
                order.OrderDate = DateTime.Now;
                order.OrderPrice = TotalPrice;
                order.ProductQuantity = carts.Count();
                order.PhoneNumber = PhoneNumber;
                order.TableNo = TableNo;
                order.Status = "Processing";
                order.CusName = CusName;

                _context.Orders.Add(order);
                _context.SaveChanges();

                //Save order list to database
                List<OrderDetail> orderDetailList = new List<OrderDetail>();
                foreach (var item in carts)
                {
                    OrderDetail orderDetail = new OrderDetail();
                    orderDetail.OrderId = order.OrderId;
                    orderDetail.ProductName = item.Product.Name;
                    orderDetail.Size = item.Size;
                    orderDetail.ProQuantity = item.Quantity;
                    orderDetail.Price = item.Quantity * item.Price;

                    orderDetailList.Add(orderDetail);
                }

                foreach (var orderDt in orderDetailList)
                {
                    _context.OrderDetails.Add(orderDt);
                }
                _context.SaveChanges();

                Email data = new Email();
                data.EmailTo = Email;
                if (data.EmailTo != null)
                {
                    PdfGen(order, orderDetailList, data);
                    Invoice(order, orderDetailList, data);
                    SendMail(data);
                }

                //Renew the cart and notify customer
                TotalPrice = 0;
                carts.Clear();
                return RedirectToAction("ThankYou");
            }
            _notyfService.Error("Something went wrong, please try again!");
            return RedirectToAction("Index");
        }

        //CONTROLLER FOR Momo PAYMENT METHOD PAGE
        public IActionResult MomoCheckout()
        {
            HomeViewModel home = NavData();
            return View(home);
        }
        [HttpPost]
        public async Task<RedirectResult> CreateMomoPaymentUrl(HomeViewModel home)
        {
            OrderInfoModel model = new OrderInfoModel();
            model = home.MoMoPay;
            model.Amount = TotalPrice;

            Email = home.Email.EmailTo;
            var response = await _momoService.CreatePaymentAsync(model);
            return Redirect(response.PayUrl);
        }

        [HttpGet]
        public IActionResult PaymentMomoCallBack()
        {
            var response = _momoService.PaymentExecuteAsync(HttpContext.Request.Query);
            if (response.ErrorCode == "0")
            {
                //Save order to database
                Orders order = new Orders();
                order.OrderDate = DateTime.Now;
                order.OrderPrice = TotalPrice;
                order.ProductQuantity = carts.Count();
                order.PhoneNumber = PhoneNumber;
                order.TableNo = TableNo;
                order.Status = "Processing";
                order.CusName = CusName;

                _context.Orders.Add(order);
                _context.SaveChanges();

                //Save order list to database
                List<OrderDetail> orderDetailList = new List<OrderDetail>();
                foreach (var item in carts)
                {
                    OrderDetail orderDetail = new OrderDetail();
                    orderDetail.OrderId = order.OrderId;
                    orderDetail.ProductName = item.Product.Name;
                    orderDetail.Size = item.Size;
                    orderDetail.ProQuantity = item.Quantity;
                    orderDetail.Price = item.Quantity * item.Price;

                    orderDetailList.Add(orderDetail);
                }

                foreach (var orderDt in orderDetailList)
                {
                    _context.OrderDetails.Add(orderDt);
                }
                _context.SaveChanges();

                Email data = new Email();
                data.EmailTo = Email;
                if (data.EmailTo != null)
                {
                    PdfGen(order, orderDetailList, data);
                    Invoice(order, orderDetailList, data);
                    SendMail(data);
                }

                //Renew the cart and notify customer
                TotalPrice = 0;
                carts.Clear();
                return RedirectToAction("ThankYou");
            }
            _notyfService.Error("Something went wrong, please try again!");
            return RedirectToAction("Index");
        }




        //CONTROLLER FOR NAVIGATION
        public HomeViewModel NavData()
        {

            CartList cartlist = new CartList();
            cartlist.CartLists = carts;

            cartlist.CartAmount = TotalPrice;
            cartlist.PhoneNumber = PhoneNumber;
            cartlist.CusName = CusName;

            HomeViewModel home = new HomeViewModel();
            home.Cart = cartlist;

            return home;
        }

        //History orders of customer
        public IActionResult History()
        {
            HomeViewModel home = NavData();
            if (home.Cart.PhoneNumber != null)
            {
                List<Orders> olist = new List<Orders>();
                foreach (var o in _context.Orders)
                {
                    if (o.PhoneNumber == home.Cart.PhoneNumber)
                    {

                        olist.Add(o);
                    }
                }
                home.Orders = olist;
                return View(home);
            }
            return RedirectToAction("PhoneValidation");
        }
        public IActionResult OrderDetails(int id)
        {
            HomeViewModel home = NavData();
            if (home.Cart.PhoneNumber != null)
            {
                var checkorder = _context.Orders.FirstOrDefault(o => o.OrderId == id);
                if (home.Cart.PhoneNumber == checkorder.PhoneNumber)
                {
                    home.Order = (from o in _context.Orders
                                  where id == o.OrderId
                                  select new HomeViewModel
                                  {
                                      OrderId = o.OrderId,
                                      OrderDate = o.OrderDate,
                                      OrderPrice = o.OrderPrice,
                                      PhoneNumber = o.PhoneNumber,
                                      ProductQuantity = o.ProductQuantity,
                                      TableNo = o.TableNo,
                                      Status = o.Status,
                                      CusName = o.CusName
                                  });

                    home.OrderDetail = (from o in _context.OrderDetails
                                        where id == o.OrderId
                                        select new HomeViewModel
                                        {
                                            OrderId = o.OrderId,
                                            OrderDetailId = o.OrderDetailId,
                                            ProductName = o.ProductName,
                                            Size = o.Size,
                                            ProQuantity = o.ProQuantity,
                                            Price = o.Price,
                                        });

                    return View(home);
                }
                return RedirectToAction("Index");
            }
            return RedirectToAction("PhoneValidation");
        }
    }
}
