using AspNetCore;
using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.Options;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Data.Common;
using System.Diagnostics;
using System.Security.Cryptography.Pkcs;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using TablesideOrdering.Areas.Admin.Models;
using TablesideOrdering.Areas.Admin.StatisticModels;
using TablesideOrdering.Areas.Admin.ViewModels;
using TablesideOrdering.Data;
using TablesideOrdering.Migrations;
using TablesideOrdering.Models;
using TablesideOrdering.Services;
using TablesideOrdering.ViewModels;
using Twilio;
using Twilio.Clients;
using Twilio.Rest.Api.V2010.Account;
using Twilio.TwiML.Voice;
using Twilio.Types;
using TopFoodSizePrice = TablesideOrdering.Models.TopFoodSizePrice;

namespace TablesideOrdering.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;
        private readonly IOptions<SMSMessage> _SMSMessage;
        private readonly IVnPayService _vnPayService;
        public INotyfService _notyfService { get; }

        public static List<AddToCart> carts = new List<AddToCart>();
        public static float TotalPrice;
        public static string PhoneNumber;
        public static string Message;
        public static string TableNo;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context, INotyfService notyfService, IOptions<SMSMessage> SMSMessage)
        {
            _logger = logger;
            _context = context;
            _notyfService = notyfService;
            _SMSMessage = SMSMessage;
        }

        //HOME page
        [HttpGet]
        public IActionResult Index()
        {
            var productList = (from ProSP in _context.ProductSizePrice
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

            HomeViewModel Homedata = new HomeViewModel();
            Homedata = NavData();
            Homedata.Category = _context.Categories.ToList();
            Homedata.Product = productList;

            return View(Homedata);
        }
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

        [HttpPost]
        public IActionResult GetMail(HomeViewModel home)
        {
            CustomerEmail Email = new CustomerEmail();
            Email.Email = home.CusMail;

            var emailList = _context.CustomerEmails.Select(i => i.Email).ToList();
            if (emailList.Contains(Email.Email) != true)
            {
                _context.CustomerEmails.Add(Email);
                _context.SaveChanges();
                _notyfService.Success("Your email is subcribed success", 5);
            }
            else
            {
                _notyfService.Error("Your email has been subcribed", 5);
            };
            return RedirectToAction("Index");
        }

        //GET take phone number
        [HttpGet]
        public IActionResult PhoneValidation()
        {
            HomeViewModel home = NavData();
            return View(home);
        }

        //POST take phone number
        [HttpPost]
        public IActionResult PhoneValidation(HomeViewModel home)
        {
            if (home.PhoneValid.PhoneNumber == home.PhoneValid.PhoneConfirmed && (home.PhoneValid.PhoneNumber != null && home.PhoneValid.PhoneConfirmed != null))
            {
                PhoneNumber = home.PhoneValid.PhoneNumber;
                TableNo = home.PhoneValid.TableNo;
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        //CART page
        public IActionResult Cart()
        {
            HomeViewModel home = NavData();
            if (home.Cart.PhoneNumber != null)
            {
                return View(home);
            }
            _notyfService.Error("The cart hasn't signed yet, please try again");
            return RedirectToAction("Index");
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

        public IActionResult PlaceOrder()
        {
            //Save order to database
            Orders order = new Orders();
            order.OrderDate = DateTime.Now;
            order.OrderPrice = TotalPrice;
            order.ProductQuantity = carts.Count();
            order.PhoneNumber = PhoneNumber;
            order.TableNo = TableNo;
            order.Status = "Processing";

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
        public void SendSMS()
        {
            string number = ConvertToPhoneValid();

            TwilioClient.Init(_SMSMessage.Value.AccountSid, _SMSMessage.Value.AuthToken);
            var message = MessageResource.Create(
                to: new PhoneNumber(number),
                from: new PhoneNumber(_SMSMessage.Value.PhoneFrom),
                body: Message);
        }

        public string ConvertToPhoneValid()
        {
            string number = PhoneNumber.Substring(1);
            string validnum = "+84" + number;
            return validnum;
        }

        public HomeViewModel NavData()
        {
            CartList cartlist = new CartList();
            cartlist.CartLists = carts;
            cartlist.CartAmount = TotalPrice;
            cartlist.PhoneNumber = PhoneNumber;

            HomeViewModel home = new HomeViewModel();
            home.Cart = cartlist;

            return home;
        }
        public IActionResult VNPayCheckout(PaymentInformationModel model)
        {
            
            HomeViewModel home = NavData();
            return View(home);
        }
        public IActionResult CreatePaymentUrl(PaymentInformationModel model)
        {
            var url = _vnPayService.CreatePaymentUrl(model, HttpContext);

            return Redirect(url);
        }

        public IActionResult PaymentCallback()
        {
            var response = _vnPayService.PaymentExecute(Request.Query);

            return Json(response);
        }
    }
}