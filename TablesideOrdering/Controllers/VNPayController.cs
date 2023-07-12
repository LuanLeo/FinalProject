using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TablesideOrdering.Data;
using TablesideOrdering.Models;
using TablesideOrdering.Services;
using TablesideOrdering.ViewModels;
using Twilio.Types;

namespace TablesideOrdering.Controllers
{
    public class VNPayController : Controller
    {
        private readonly IVnPayService _vnPayService;
        public static List<AddToCart> carts = new List<AddToCart>();
        public static float TotalPrice;
        public static string PhoneNumber;
        public static string Message;
        public static string TableNo;
        private readonly ApplicationDbContext _context;


        public VNPayController(ApplicationDbContext context, INotyfService notyfService, IVnPayService vnPayService)
        {
            _context = context;
            _vnPayService = vnPayService;
        }   
        public IActionResult Index()
        {
            HomeViewModel home = new HomeViewModel();
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
