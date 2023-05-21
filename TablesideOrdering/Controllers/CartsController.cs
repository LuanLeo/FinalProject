using Microsoft.AspNetCore.Mvc;
using TablesideOrdering.Data;
using TablesideOrdering.Models;
using TablesideOrdering.ViewModels;

namespace TablesideOrdering.Controllers
{
    public class CartsController : Controller
    {
        private readonly ApplicationDbContext _context;
        public static List<AddToCart> carts = new List<AddToCart>();
        public static float TotalPrice = 0;
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

        /*public IActionResult Index(int id)
        {
            if (ModelState.IsValid)
                {
                AddToCart model = new AddToCart();
                Product product = _context.Products.Find(id);

                if (carts.Count() == 0)
                {
                    model.Product = product;
                    model.Quantity += 1;
                    model.Total = model.Quantity * model.Product.Price;
                    carts.Add(model);
                }
                else
                {
                    if (carts.Any(x => x.Product.ProductId == id))
                    {
                        model = carts.Single(x => x.Product.ProductId == id);
                        model.Quantity += 1;
                        model.Total = model.Quantity * model.Product.Price;
                    }
                    else
                    {
                        model.Product = product;
                        model.Quantity = 1;
                        model.Total = product.Price;
                        carts.Add(model);
                    }
                }
                foreach (var item in carts)
                {
                    TotalPrice += item.Total;
                }
                ViewBag.CartQuantity = carts.Count();
                ViewBag.CartPrice = TotalPrice;

                AddToCartViewModel cartlist = new AddToCartViewModel();
                cartlist.CartList = carts;
                cartlist.CartAmount = TotalPrice;
                return View(cartlist);
            }
            return View("Index");
        }*/
    }
}
