using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Collections;
using System.Diagnostics;
using System.Linq;
using TablesideOrdering.Areas.Admin.Models;
using TablesideOrdering.Areas.Admin.StatisticModels;
using TablesideOrdering.Areas.Admin.ViewModels;
using TablesideOrdering.Data;
using TablesideOrdering.ViewModels;

namespace TablesideOrdering.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext context;
        public HomeController(ILogger<HomeController> logger, ApplicationDbContext _context)
        {
            _logger = logger;
            context = _context;
        }

        public IActionResult Index(string? time, HomeAdminViewModel home)
        {
            time = home.Time;
            StatisticOrder(time);
            return View(home);
        }

        public void StatisticOrder(string time)
        {
            //Take list from database
            List<Order> orders = new List<Order>();
            foreach (var order in context.Orders)
            {
                orders.Add(order);
            }

            //Take distinct by date (Can't do directly at database)
            List<OrderStatisticModel> modelList = new List<OrderStatisticModel>();
            List<string> Date = new List<string>();

            if (time == null || time == "Day")
            {
                ViewBag.Title1 = "Revenues by Day";
                List<DateTime> Time = new List<DateTime>();
                Time = orders.DistinctBy(i => i.OrderDate.Date).Select(h => h.OrderDate.Date).ToList();
                foreach (var da in Time)
                {
                    Date.Add(da.ToShortDateString());
                }
            } else
            {
                List<int> Time = new List<int>();
                if (time == "Month")
                {
                    ViewBag.Title1 = "Revenues by Month";
                    Time = orders.DistinctBy(i => i.OrderDate.Month).Select(h => h.OrderDate.Month).ToList();
                }
                else
                {
                    ViewBag.Title1 = "Revenues by Year";
                    Time = orders.DistinctBy(i => i.OrderDate.Year).Select(h => h.OrderDate.Year).ToList();
                }
                foreach (var da in Time)
                {
                    Date.Add(da.ToString());
                }
            }

            //Count quantity order and Price
            List<int> Quantities = new List<int>();
            List<float> Prices = new List<float>();
            foreach (var da in Date)
            {
                int Quantity = 0;
                float Price = 0;
                foreach (var order in orders)
                {
                    if (da == order.OrderDate.ToShortDateString() && (time == null || time == "Day"))
                    {
                        Quantity += 1;
                        Price += order.OrderPrice;
                    } else if (da == order.OrderDate.Month.ToString() && (time == null || time == "Month"))
                    {
                        Quantity += 1;
                        Price += order.OrderPrice;
                    }
                    else if (da == order.OrderDate.Year.ToString() && (time == null || time == "Year"))
                    {
                        Quantity += 1;
                        Price += order.OrderPrice;
                    }
                }

                OrderStatisticModel model = new OrderStatisticModel();
                model.Date = da;
                model.Quantity = Quantity;
                model.Price = Price;
                modelList.Add(model);
            }            

            //Save to model
            List<OrderLogicModel> OrderPrice = new List<OrderLogicModel>();
            List<OrderLogicModel> OrderQuantity = new List<OrderLogicModel>();

            foreach (var models in modelList)
            {
                OrderPrice.Add(new OrderLogicModel(models.Date, models.Price));
                OrderQuantity.Add(new OrderLogicModel(models.Date, models.Quantity));

            }
            ViewBag.OrderPrice = JsonConvert.SerializeObject(OrderPrice);
            ViewBag.OrderQuantity = JsonConvert.SerializeObject(OrderQuantity);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}