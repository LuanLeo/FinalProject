using AspNetCore;
using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.Options;
using Microsoft.Extensions.Options;
using System.Diagnostics;
using System.Security.Cryptography.Pkcs;
using System.Text.RegularExpressions;
using TablesideOrdering.Areas.Admin.Models;
using TablesideOrdering.Areas.Admin.ViewModels;
using TablesideOrdering.Data;
using TablesideOrdering.Migrations;
using TablesideOrdering.Models;
using TablesideOrdering.ViewModels;
using Twilio;
using Twilio.Clients;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;

namespace TablesideOrdering.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;
        private readonly IOptions<SMSMessage> _SMSMessage;
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

            var cat = (from categories in _context.Categories
                       select new Category
                       {
                           CategoryId = categories.CategoryId,
                           CategoryName = categories.CategoryName,
                       });

            HomeViewModel Homedata = new HomeViewModel();
            Homedata = NavData();
            Homedata.Category = cat;
            Homedata.Product = productList;
            return View(Homedata);
        }
        public IActionResult Menu(string term="", string orderBy = "")
        {
            term = string.IsNullOrEmpty(term) ? "" : term.ToLower();
            HomeViewModel Homedata = new HomeViewModel();
            List<ProductSizePriceViewModel> productlist = new List<ProductSizePriceViewModel>();                         
            var productList = (from ProSP in _context.ProductSizePrice
                               join Pro in _context.Products on ProSP.ProductId equals Pro.ProductId
                               where (term == "" || Pro.Name.Contains(term))
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
            
            var cat = (from categories in _context.Categories
                       select new Category
                       {
                           CategoryId = categories.CategoryId,
                           CategoryName = categories.CategoryName,
                       });
            var size = (from s in _context.ProductSize
                       select new ProductSize
                       {
                           SizeName = s.SizeName,
                           SizeId = s.SizeId,
                       });
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
            Homedata.Category = cat;
            Homedata.ProductSizes = size;
            Homedata.Product = productList;
            Homedata.Term = term;
            //Homedata.Cart
            return View(Homedata);
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

        //ADD to cart
        public IActionResult AddToCart(int id)
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

            Message = "Hello";

            //SendSMS();
            NavData();
            return RedirectToAction("Index", "Home");
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
    }
}