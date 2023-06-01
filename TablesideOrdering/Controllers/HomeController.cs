using AspNetCore;
using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Text.RegularExpressions;
using TablesideOrdering.Data;
using TablesideOrdering.Models;
using TablesideOrdering.ViewModels;
namespace TablesideOrdering.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;
        public INotyfService _notyfService { get; }

        public static List<AddToCart> carts = new List<AddToCart>();
        public static float TotalPrice;
        public static string PhoneNumber = "";
        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context, INotyfService notyfService)
        {
            _logger = logger;
            _context = context;
            _notyfService = notyfService;
        }

        //HOME page
        public IActionResult Index()
        {
            var Homedata = new HomeViewModel();
            List<Product> products = _context.Products.ToList();
            List<ProductSizePriceViewModel> productlist = new List<ProductSizePriceViewModel>();
            ProductSizePriceViewModel product = new ProductSizePriceViewModel();

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

            CartList cartlist = new CartList();
            cartlist.CartLists = carts;
            cartlist.CartAmount = TotalPrice;
            cartlist.PhoneNumber = PhoneNumber;

            Homedata.Cart = cartlist;
            Homedata.Category = cat;
            Homedata.Product = productList;

            //Homedata.Cart
            return View(Homedata);
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
        public IActionResult PhoneValidation(HomeViewModel phone)
        {
                PhoneNumber = phone.PhoneNumber;
                return RedirectToAction("Index", "Home");

        }

        //CART page
        public IActionResult Cart()
        {
            HomeViewModel home = NavData();
            return View(home);
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

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}