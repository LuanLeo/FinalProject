using AspNetCoreHero.ToastNotification.Abstractions;
using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MimeKit;
using TablesideOrdering.Areas.StoreOwner.Models;
using TablesideOrdering.Areas.StoreOwner.ViewModels;
using TablesideOrdering.Data;
using TablesideOrdering.Migrations;
using TablesideOrdering.Models;
using TablesideOrdering.Models.Order;
using TablesideOrdering.Services;
using TablesideOrdering.ViewModels;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;
using TopFoodSizePrice = TablesideOrdering.Models.TopFoodSizePrice;

namespace TablesideOrdering.Controllers
{
    public class HomeController : Controller
    {
        //Call Database and Relatives
        private readonly ApplicationDbContext _context;
        private readonly SMSMessage _SMSMessage;
        private readonly EmailReceiptOnline _EmailReceiptOnline;

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
        public static string EmailReceiptOnline;

        public HomeController(ApplicationDbContext context,
            INotyfService notyfService,
            IVnPayService vnPayService,
            IOptions<SMSMessage> SMSMessage,
            IOptions<EmailReceiptOnline> EmailReceiptOnline,
            IMomoService momoService)
        {
            _context = context;

            _SMSMessage = SMSMessage.Value;
            _EmailReceiptOnline = EmailReceiptOnline.Value;

            _notyfService = notyfService;
            _vnPayService = vnPayService;
            _momoService = momoService;
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
            HomeViewModel home = NavData();
            return View(home);
        }

        [HttpPost]
        public IActionResult PhoneValidation(HomeViewModel home)
        {
            if (home.PhoneValid.PhoneNumber == home.PhoneValid.PhoneConfirmed && (home.PhoneValid.PhoneNumber != null && home.PhoneValid.PhoneConfirmed != null))
            {
                PhoneNumber = home.PhoneValid.PhoneNumber;
                TableNo = home.PhoneValid.TableNo;
                CusName = home.PhoneValid.CusName;
                return RedirectToAction("Index", "Home");
            }
            return View();
        }
        public string ConvertToPhoneValid()
        {
            string number = PhoneNumber.Substring(1);
            string validnum = "+84" + number;
            return validnum;
        }





        //CONTROLLER FOR SENDING ONLINE RECEIPT
        public void SendMail(EmailReceiptOnline data)
        {
            data.Email = _EmailReceiptOnline.Email;
            data.Password = _EmailReceiptOnline.Password;

            data.Body = "";
            var email = new MimeMessage();
            {
                email.From.Add(MailboxAddress.Parse(data.Email));
                email.To.Add(MailboxAddress.Parse(EmailReceiptOnline));
                email.Subject = "L&L Coffee Online Receipt";
                email.Body = new TextPart(MimeKit.Text.TextFormat.Html) { Text = data.Body };
            }
            using var smtp = new SmtpClient();
            {
                smtp.Connect("smtp.gmail.com", 587, MailKit.Security.SecureSocketOptions.StartTls);
                smtp.Authenticate(data.Email, data.Password);
                smtp.Send(email);
                smtp.Disconnect(true);
            }
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

        //Add to Cart from Index
        public IActionResult IndexCart(int id)
        {
            AddToCart(id);
            return RedirectToAction("Index", "Home");
        }

        //Add to Cart from Menu
        public IActionResult MenuCart(int id)
        {
            AddToCart(id);
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

            NavData();
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
                    _notyfService.Information("Payment method can't be null", 5);
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
        public IActionResult PlaceOrder()
        {
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

            //Renew the cart and notify customer
            TotalPrice = 0;
            carts.Clear();
            _notyfService.Success("Your order has been received");
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

            EmailReceiptOnline = home.Payment.Email;

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

                //Renew the cart and notify customer
                TotalPrice = 0;
                carts.Clear();
                _notyfService.Success("Your order has been received");
                return RedirectToAction("Index");
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

            EmailReceiptOnline = home.Payment.Email;
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

                //Renew the cart and notify customer
                TotalPrice = 0;
                carts.Clear();
                _notyfService.Success("Your order has been received");
                return RedirectToAction("Index");
            }
            _notyfService.Error("Something went wrong, please try again!");
            return RedirectToAction("Thankyou");
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
    }
}
