using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Diagnostics;
using System.Linq;
using TablesideOrdering.Data;
using TablesideOrdering.Models;
using TablesideOrdering.ViewModels;

namespace TablesideOrdering.Controllers
{
    public class CartsController : Controller
    {
        private readonly ApplicationDbContext _context;
        public static List<AddToCart> carts = new List<AddToCart>();
        public static float TotalPrice;
        public CartsController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            AddToCartViewModel cartlist = new AddToCartViewModel();
            cartlist.CartList = carts;
            cartlist.CartAmount = TotalPrice;
            return View(cartlist);
        }

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

            AddToCartViewModel cartlist = new AddToCartViewModel();
            cartlist.CartList = carts;
            cartlist.CartAmount = TotalPrice;

            return RedirectToAction("Index", "Home");
        }

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

            AddToCartViewModel cartlist = new AddToCartViewModel();
            cartlist.CartList = carts;
            cartlist.CartAmount = TotalPrice;

            return RedirectToAction("Index", "Carts");
        }
    }
}
