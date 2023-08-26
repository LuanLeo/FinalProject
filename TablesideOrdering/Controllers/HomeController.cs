using AspNetCoreHero.ToastNotification.Abstractions;
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
using Microsoft.CodeAnalysis;
using TablesideOrdering.Areas.Staff.Models;
using Reservation = TablesideOrdering.Areas.Staff.Models.Reservation;
using System;
using TablesideOrdering.Models.Momo;
using TablesideOrdering.Models.VNPay;

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
        public static string PhoneMessage;
        public static string TableNo;
        public static string CusName;

        public static string EmailMessage;
        public static string Subject;
        public static string Email;
        public static string file;

        public static string PaymentType;
        public static string OrderType;
        public static string Address;
        public static int CheckNotify = 0;
        public static Reservation ReserModel = new Reservation();

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

        public void Type(string term)
        {
            if (term == "Reservation")
            {
                OrderType = "Reservation";
            }
            else if (term == "Delivery")
            {
                OrderType = "Delivery";
            }
            else if (term == "Carry out")
            {
                OrderType = "Carry out";
            }
            else
            {
                OrderType = "Eat in";
            }
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

            if (CheckNotify > 0)
            {
                _notyfService.Success("Add to cart succeed", 5);
                CheckNotify = 0;
            }
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





        //INPUTTING TABLE NUMBER BY LINK FUNCTION
        [HttpGet]
        public IActionResult TableCheck(string id)
        {
            if (id != null)
            {
                TableNo = id;
                OrderType = "Eat in";
                NavData();
                return RedirectToAction("Index");
            }
            else
            {
                return RedirectToRoute(new { controller = "Account", action = "AccessDenied", area = "Identity" }); ;
            }
        }

        //MODIFY PHONE NUMBER FUNCTION
        public string ConvertToPhoneValid()
        {
            string validnum = "";
            if (PhoneNumber.Substring(1) != "0")
            {
                string number = PhoneNumber.Substring(1);
                validnum = "+84" + number;
            }
            else
            {
                validnum = PhoneNumber;
            }
            return validnum;
        }





        //SENDING MAIL FUCNTION
        public void SendMail(Email data)
        {
            data.EmailFrom = _email.EmailFrom;
            data.Password = _email.Password;
            data.Body = EmailMessage;
            data.Subject = Subject;

            var builder = new BodyBuilder();
            builder.HtmlBody = EmailMessage.ToString();
            builder.Attachments.Add(file);

            var email = new MimeMessage();
            email.From.Add(MailboxAddress.Parse(data.EmailFrom));
            email.To.Add(MailboxAddress.Parse(data.EmailTo));
            email.Subject = data.Subject;
            email.Body = builder.ToMessageBody();

            using var smtp = new SmtpClient();
            {
                smtp.Connect("smtp.gmail.com", 587, MailKit.Security.SecureSocketOptions.StartTls);
                smtp.Authenticate(data.EmailFrom, data.Password);
                smtp.Send(email);
                smtp.Disconnect(true);
            }

        }

        //CREATE INVOICE FUCNTION
        public void Invoice(Orders order, List<OrderDetail> orderDetailList, Email data)
        {
            StringBuilder subject = new StringBuilder();
            subject.Append("E-Invoice order ").Append(order.OrderId).Append(" at L&L coffee shop ");

            StringBuilder invoiceHtml = new StringBuilder();
            invoiceHtml.Append("<b >E-Invoice at L&L coffee shop ").Append("</b><br />");
            invoiceHtml.Append("<br /><b>Date : </b>").Append(DateTime.Now.ToShortDateString()).Append("<br />");
            if (order.OrderType == "Eat in")
            {
                invoiceHtml.Append("<b>Table : </b>").Append(order.TableNo).Append("<br />");
            }
            else if (order.OrderType == "Carry out")
            {
                invoiceHtml.Append("<b>Time to delivery: </b>").Append(order.PickTime).Append("<br />");
            }
            invoiceHtml.Append("<b>Invoice Total :</b> ").Append(order.OrderPrice.ToString()).Append(" VND<br />");
            invoiceHtml.Append("<br /><b>CUSTOMER CONTACT INFO:</b><br />");
            invoiceHtml.Append("<b>Name : </b>").Append(order.CusName).Append("<br />");
            invoiceHtml.Append("<b>Phone : </b>").Append(order.PhoneNumber).Append("<br />");
            if (order.OrderType == "Delivery")
            {
                invoiceHtml.Append("<b>Address : </b>").Append(order.Address).Append("<br />");

            }
            invoiceHtml.Append("<b>Email : </b>").Append(data.EmailTo).Append("<br />");

            Subject = subject.ToString();
            EmailMessage = invoiceHtml.ToString();
        }

        //CONVERT INVOICE TO PDF FUNCTION
        public void PdfGen(Orders order, List<OrderDetail> orderDetailList, Email data)
        {
            Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("Ngo9BigBOggjHTQxAR8/V1NGaF1cWGhIfEx1RHxQdld5ZFRHallYTnNWUj0eQnxTdEZjUX9YcH1WTmRYWUJ1Xw==");
            //Add image
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

        }





        //CART PAGE FUCNTION
        public IActionResult Cart()
        {
            HomeViewModel home = NavData();
            return View(home);
        }

        //Increase Quantity
        public void IncQty(int id)
        {
            AddToCart cart = new AddToCart();
            ProductSizePrice productprice = _context.ProductSizePrice.Find(id);
            if (carts.Find(x => x.SizePriceId == productprice.Id) != null)
            {
                cart = carts.Single(x => x.SizePriceId == productprice.Id);
                cart.Quantity += 1;
                cart.TotalProPrice = productprice.Price * cart.Quantity;
            }
            TotalPrice = 0;
            foreach (var item in carts)
            {
                float Total = item.Quantity * item.Price;
                TotalPrice += Total;
            }
            ViewBag.CartQuantity = carts.Count();
            ViewBag.CartPrice = TotalPrice;
        }

        //Decrease Quantity
        public void DecQty(int id)
        {
            AddToCart cart = new AddToCart();
            ProductSizePrice productprice = _context.ProductSizePrice.Find(id);
            if (carts.Find(x => x.SizePriceId == productprice.Id) != null)
            {
                cart = carts.Single(x => x.SizePriceId == productprice.Id);
                cart.Quantity -= 1;
                cart.TotalProPrice = productprice.Price * cart.Quantity;
            }
            TotalPrice = 0;
            foreach (var item in carts)
            {
                float Total = item.Quantity * item.Price;
                TotalPrice += Total;
            }

            ViewBag.CartQuantity = carts.Count();
            ViewBag.CartPrice = TotalPrice;
            if (cart.Quantity == 0)
            {
                DeleteFromCart(id);
            }
        }
        //ADD TO CART FUCNTION
        public void AddToCart(int id)
        {
            CheckNotify = id;

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

        //DELETE FROM CART FUNCTION
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





        //SENDING SMS TO CUSTOMER FUCNTION
        public void SendSMS()
        {
            string number = ConvertToPhoneValid();
            TwilioClient.Init(_SMSMessage.AccountSid, _SMSMessage.AuthToken);
            var message = MessageResource.Create(
                to: new PhoneNumber(number),
                from: new PhoneNumber(_SMSMessage.PhoneFrom),
                body: PhoneMessage);
        }






        //SELECTING PAYMENT TYPE FUCNTION
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
                        PaymentType = "VNPay";
                        return RedirectToAction("VNPayCheckout");
                    }
                    if (home.PaymentType == "Momo")
                    {
                        PaymentType = "Momo";
                        return RedirectToAction("MomoCheckout");
                    }
                    if (home.PaymentType == "Cash")
                    {
                        PaymentType = "Cash";
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

        //DELIVERY CHECK METHOD PAGE FUCNTION
        public Boolean DeliveryCheck(HomeViewModel home)
        {
            if (PaymentType == "Cash")
            {
                if (home.Payment.Name != null && home.Address != null && home.Cart.PhoneNumber != null)
                {
                    return true;
                }
            }
            if (PaymentType == "VNPay")
            {
                if (home.Payment.Name != null && home.Address != null && home.Cart.PhoneNumber != null)
                {
                    return true;
                }
            }

            if (PaymentType == "Momo")
            {
                if (home.MoMoPay.FullName != null && home.Address != null && home.Cart.PhoneNumber != null)
                {
                    return true;
                }
            }

            return false;
        }

        //CARRY OUT CHECK METHOD PAGE FUCNTION
        public Boolean CarryoutCheck(HomeViewModel home)
        {
            if (PaymentType == "Cash")
            {
                if (home.Payment.Name != null && home.Cart.PhoneNumber != null)
                {
                    return true;
                }
            }
            if (PaymentType == "VNPay")
            {
                if (home.Payment.Name != null && home.Cart.PhoneNumber != null)
                {
                    return true;
                }
            }

            if (PaymentType == "Momo")
            {
                if (home.MoMoPay.FullName != null && home.Cart.PhoneNumber != null)
                {
                    return true;
                }
            }

            return false;
        }

        //CASH PAYMENT METHOD PAGE FUCNTION
        public IActionResult CashCheckout()
        {
            HomeViewModel home = NavData();
            home.OrderType = OrderType;
            home.Reser = ReserModel;
            return View(home);
        }

        //CASH PAYMENT METHOD PAGE FUCNTION
        public IActionResult PlaceOrder(HomeViewModel home)
        {
            if ((OrderType == "Delivery" && DeliveryCheck(home) == true) || (OrderType == "Carry out" && CarryoutCheck(home) == true) || OrderType == "Eat in" || OrderType == "Reservation")
            {
                Email = home.Email.EmailTo;
                Email data = new Email();
                data.EmailTo = Email;

                //Save order to database
                Orders order = new Orders();
                order.OrderDate = DateTime.Now.ToString();
                order.OrderPrice = TotalPrice;
                order.ProductQuantity = carts.Count();
                order.PhoneNumber = home.Cart.PhoneNumber;
                order.CusName = home.Payment.Name;
                order.OrderType = OrderType;
                order.PaymentType = PaymentType;
                order.Status = "Processing";
                if (OrderType == "Carry out")
                {
                    order.PickTime = home.PickTime;
                    order.Address = "";
                    order.TableNo = "";
                }

                if (OrderType == "Delivery")
                {
                    order.Address = home.Address;
                    order.TableNo = "";
                }
                if (OrderType == "Eat in")
                {
                    order.Address = "";
                    order.TableNo = TableNo;
                }
                if (OrderType == "Reservation")
                {
                    order.Address = "";
                    order.TableNo = "";


                }

                _context.Orders.Add(order);
                _context.SaveChanges();
                if (OrderType == "Reservation")
                {
                    ReserModel.OrderId = order.OrderId;
                    _context.Reservations.Add(ReserModel);
                    _context.SaveChanges();
                }
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
                    System.IO.File.Delete(file);
                }

                //Renew the cart and notify customer
                TotalPrice = 0;
                carts.Clear();
                return RedirectToAction("ThankYou");
            }
            else
            {
                _notyfService.Warning("Please fill all needed info", 5);
                return RedirectToAction("CashCheckout");
            }
        }

        //CASH CHECKOUT PAGE FUCNTION
        public IActionResult ThankYou()
        {
            return View();
        }

        //VNPAY PAYMENT METHOD PAGE FUCNTION
        public IActionResult VNPayCheckout()
        {
            HomeViewModel home = NavData();
            home.OrderType = OrderType;
            return View(home);
        }

        //VNPAY URL PAYMENT FUNCTION
        public IActionResult CreatePaymentUrl(HomeViewModel home)
        {
            if ((OrderType == "Delivery" && DeliveryCheck(home) == true) || (OrderType == "Carry out" && CarryoutCheck(home) == true) || OrderType == "Eat in")
            {
                PaymentInformationModel model = new PaymentInformationModel();
                model = home.Payment;
                model.Amount = TotalPrice;
                Email = home.Email.EmailTo;
                PhoneNumber = home.Cart.PhoneNumber;
                CusName = home.Payment.Name;
                Address = home.Address;
                var url = _vnPayService.CreatePaymentUrl(model, HttpContext);
                return Redirect(url);
            }
            else
            {
                _notyfService.Warning("Please fill all needed info", 5);
                return RedirectToAction("VNPayCheckout");
            }
        }

        //VNPAY PAYMENT FUNCTION
        public IActionResult PaymentCallback()
        {
            var response = _vnPayService.PaymentExecute(Request.Query);
            if (response.VnPayResponseCode == "00")
            {
                //Save order to database
                Orders order = new Orders();
                order.OrderDate = DateTime.Now.ToString();
                order.OrderPrice = TotalPrice;
                order.ProductQuantity = carts.Count();
                order.PhoneNumber = PhoneNumber;
                order.Status = "Processing";
                order.CusName = CusName;
                order.OrderType = OrderType;
                order.PaymentType = PaymentType;

                if (OrderType == "Eat in" || OrderType == "Carry out")
                {
                    order.Address = "";
                    order.TableNo = TableNo;
                }
                else
                {
                    order.Address = Address;
                    order.TableNo = "";
                }
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
                    System.IO.File.Delete(file);
                }

                //Renew the cart and notify customer
                TotalPrice = 0;
                carts.Clear();
                return RedirectToAction("ThankYou");
            }
            _notyfService.Error("Something went wrong, please try again!");
            return RedirectToAction("Index");
        }

        //MOMO PAYMENT METHOD PAGE FUCNTION
        public IActionResult MomoCheckout()
        {
            HomeViewModel home = NavData();
            home.OrderType = OrderType;
            return View(home);

        }
        [HttpPost]
        public async Task<RedirectResult> CreateMomoPaymentUrl(HomeViewModel home)
        {
            if ((OrderType == "Delivery" && DeliveryCheck(home) == true) || (OrderType == "Carry out" && CarryoutCheck(home) == true) || OrderType == "Eat in")
            {
                OrderInfoModel model = new OrderInfoModel();
                model = home.MoMoPay;
                model.Amount = TotalPrice;
                Email = home.Email.EmailTo;
                PhoneNumber = home.Cart.PhoneNumber;
                CusName = home.MoMoPay.FullName;
                Address = home.Address;
                var response = await _momoService.CreatePaymentAsync(model);
                return Redirect(response.PayUrl);
            }
            else
            {
                _notyfService.Warning("Please fill all needed info", 5);
                return Redirect("/Home/MomoCheckout");
            }

        }


        //MOMO PAYMENT FUNCTION 
        [HttpGet]
        public IActionResult PaymentMomoCallBack()
        {
            var response = _momoService.PaymentExecuteAsync(HttpContext.Request.Query);
            if (response.ErrorCode == "0")
            {
                //Save order to database
                Orders order = new Orders();
                order.OrderDate = DateTime.Now.ToString();
                order.OrderPrice = TotalPrice;
                order.ProductQuantity = carts.Count();
                order.PhoneNumber = PhoneNumber;
                order.Status = "Processing";
                order.CusName = CusName;
                order.OrderType = OrderType;
                order.PaymentType = PaymentType;


                if (OrderType == "Eat in" || OrderType == "Carry out")
                {
                    order.Address = "";
                    order.TableNo = TableNo;
                }
                else
                {
                    order.Address = Address;
                    order.TableNo = "";
                }

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
                    System.IO.File.Delete(file);
                }

                //Renew the cart and notify customer
                TotalPrice = 0;
                carts.Clear();
                return RedirectToAction("ThankYou");
            }
            _notyfService.Error("Something went wrong, please try again!");
            return RedirectToAction("Index");
        }





        //TABLE RESERVATION FUNCTION
        public IActionResult Reservation()
        {
            HomeViewModel home = NavData();
            return View(home);
        }

        public IActionResult ReservationConfirm(HomeViewModel home)
        {


            Reservation book = new Reservation();
            book.CusName = home.Reservation.CusName;
            book.PhoneNumber = home.Reservation.PhoneNumber;

            if (home.Reservation.Email != null)
            {
                book.Email = home.Reservation.Email;
            }
            else
            {
                book.Email = "None";
            }

            DateTime d = home.Reservation.Date;
            DateTime t = home.Reservation.Time;
            DateTime dtCombined = new DateTime(d.Year, d.Month, d.Day, t.Hour, t.Minute, t.Second);
            book.Datetime = dtCombined;

            book.People = home.Reservation.People;
            if (home.Reservation.Notes != null)
            {
                book.Notes = home.Reservation.Notes;
            }
            else
            {
                book.Notes = "None";
            }

            book.OrderId = 0;
            if (home.Reservation.ReserveType == "ReservationOnly")
            {
                _context.Reservations.Add(book);
                _context.SaveChanges();
                return RedirectToAction("ThankYou");
            }
            else
            {
                ReserModel = book;
                return RedirectToAction("Menu");
            }

        }


        //ORDER HISTORY FUCNTION
        public IActionResult History(HomeViewModel model)
        {
            HomeViewModel home = NavData();
            if (model.OrderId != null && model.PhoneNumber != null)
            {
                OrderTracking(home, model);
            }
            return View(home);
        }

        public void OrderTracking(HomeViewModel home, HomeViewModel model)
        {
            List<Orders> olist = new List<Orders>();
            Orders order = _context.Orders.Where(o => o.OrderId == model.OrderId && o.PhoneNumber == model.PhoneNumber).FirstOrDefault();
            // home.Orders = olist.OrderByDescending(x => x.OrderDate).ToList();
            olist.Add(order);
            home.Orders = olist;
        }

        //ORDER DETAILS
        public IActionResult OrderDetails(int id)
        {
            HomeViewModel home = NavData();
            var checkorder = _context.Orders.FirstOrDefault(o => o.OrderId == id);

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
                              CusName = o.CusName,
                              PaymentType = o.PaymentType,
                              OrderType = o.OrderType
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

        //RETURN FROM THANK YOU PAGE FUNCTION
        public IActionResult Return()
        {
            return RedirectToAction("Index", "Home");
        }



        //NAVIGATION FUCNTION
        public HomeViewModel NavData()
        {
            CartList cartlist = new CartList();
            cartlist.CartLists = carts;
            cartlist.TableNo = TableNo;
            cartlist.CartAmount = TotalPrice;
            cartlist.PhoneNumber = PhoneNumber;
            cartlist.CusName = CusName;

            HomeViewModel home = new HomeViewModel();
            home.Cart = cartlist;

            return home;
        }
    }
}
