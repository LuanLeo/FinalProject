﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.CodeAnalysis.VisualBasic.Syntax;
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

        public IActionResult Index(string? time)
        {
            StatisticOrder(time);
            FoodTopPercentage(time);
            return View();
        }

        public void StatisticOrder(string time)
        {
            //Take list from database
            List<Orders> orders = new List<Orders>();
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
            }
            else
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

            //Handle the data
            List<float> Prices = new List<float>();
            foreach (var da in Date)
            {
                float Price = 0;
                foreach (var order in orders)
                {
                    if (da == order.OrderDate.ToShortDateString() && (time == null || time == "Day"))
                    {
                        Price += order.OrderPrice;
                    }
                    else if (da == order.OrderDate.Month.ToString() && (time == null || time == "Month"))
                    {
                        Price += order.OrderPrice;
                    }
                    else if (da == order.OrderDate.Year.ToString() && (time == null || time == "Year"))
                    {
                        Price += order.OrderPrice;
                    }
                }

                OrderStatisticModel model = new OrderStatisticModel();
                model.Date = da;
                model.Price = Price;
                modelList.Add(model);
            }

            //Save to model
            List<OrderLogicModel> OrderPrice = new List<OrderLogicModel>();
            foreach (var models in modelList)
            {
                OrderPrice.Add(new OrderLogicModel(models.Date, models.Price));
            }
            ViewBag.OrderPrice = JsonConvert.SerializeObject(OrderPrice);
        }

        public void FoodTopPercentage(string time)
        {
            //Take list from database
            List<Orders> orders = new List<Orders>();
            List<OrderDetail> ordersDetails = new List<OrderDetail>();

            foreach (var order in context.Orders)
            {
                orders.Add(order);
            }

            foreach (var detail in context.OrderDetails)
            {
                ordersDetails.Add(detail);
            }

            //Handle the data
            List<OrderDetail> orderDetails = new List<OrderDetail>();
            List<string> FoodDistinct = new List<string>();
            List<int> orderId = new List<int>();
            string comparedTime = "";

            if (time == null || time == "Day")
            {
                ViewBag.Title2 = "Today";
                comparedTime = DateTime.Now.ToShortDateString();
            }
            else
            {
                if (time == "Month")
                {
                    ViewBag.Title2 = "this Month";
                    comparedTime = DateTime.Now.Month.ToString();
                }
                else
                {
                    ViewBag.Title2 = "this Year";
                    comparedTime = DateTime.Now.Year.ToString();
                }
            }

            foreach (var order in orders)
            {
                if (order.OrderDate.ToShortDateString() == comparedTime)
                {
                    orderId.Add(order.OrderId);
                }
                else if (order.OrderDate.Month.ToString() == comparedTime)
                {
                    orderId.Add(order.OrderId);
                }
                else if (order.OrderDate.Year.ToString() == comparedTime)
                {
                    orderId.Add(order.OrderId);
                }
            }

            float TotalPrice = 0;
            foreach (var id in orderId)
            {
                foreach (var order in ordersDetails)
                {
                    if (order.OrderId == id)
                    {
                        TotalPrice += order.Price;
                        orderDetails.Add(order);
                    }
                }
            }

            var FoodDis = ordersDetails.DistinctBy(i => i.ProductName).Select(i => i.ProductName).ToList();
            foreach (var f in FoodDis)
            {
                FoodDistinct.Add(f);
            }

            List<FoodStatisticModel> FoodPercentage = new List<FoodStatisticModel>();
            List<FoodStatisticModel> TopFood = new List<FoodStatisticModel>();
            foreach (var food in FoodDistinct)
            {
                float Price = 0;
                foreach (var item in orderDetails)
                {
                    if (item.ProductName == food)
                    {
                        Price += item.Price;
                    }
                }
                float Percentage = (Price / TotalPrice) * 100;

                FoodStatisticModel foodPercent = new FoodStatisticModel();
                foodPercent.FoodName = food;
                foodPercent.Value = Percentage;
                FoodPercentage.Add(foodPercent);

                FoodStatisticModel topFood = new FoodStatisticModel();
                topFood.FoodName = food;
                topFood.Value = Price;
                TopFood.Add(topFood);
            }

            //Save to model
            List<TopFoodLogicModel> FoodPercentageList = new List<TopFoodLogicModel>();
            foreach (var per in FoodPercentage)
            {
                FoodPercentageList.Add(new TopFoodLogicModel(per.FoodName, per.Value));
            }
            ViewBag.FoodList = JsonConvert.SerializeObject(FoodPercentageList);

            List<TopFoodLogicModel> FoodTopList = new List<TopFoodLogicModel>();
            var top = TopFood.OrderByDescending(i => i.Value).Take(3).ToList();
            foreach (var t in top.OrderBy(i => i.Value)
)
            {
                FoodTopList.Add(new TopFoodLogicModel(t.FoodName, t.Value));
            }
            ViewBag.TopFood = JsonConvert.SerializeObject(FoodTopList);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}