using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using TablesideOrdering.Areas.StoreOwner.Models;
using TablesideOrdering.Areas.StoreOwner.StatisticModels;
using TablesideOrdering.Areas.StoreOwner.ViewModels;
using TablesideOrdering.Data;
using TablesideOrdering.ViewModels;
using OrderViewModel = TablesideOrdering.Areas.StoreOwner.ViewModels.OrderViewModel;

namespace TablesideOrdering.Areas.StoreOwner.Controllers
{
    [Area("StoreOwner")]
    [Authorize(Roles = "Store Owner, Admin")]

    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext context;
        public INotyfService _notyfService { get; set; }
        public HomeController(ILogger<HomeController> logger, ApplicationDbContext _context, INotyfService notyfServic)
        {
            _logger = logger;
            context = _context;
            _notyfService = notyfServic;
        }

        public IActionResult Index(string? time)
        {
            StatisticOrder(time);
            FoodTopPercentage(time);
            return View();
        }
        public IActionResult OrderDetails(int id)
        {
            OrderViewModel OrderData = new OrderViewModel();
            List<Orders> o = new List<Orders>();
            foreach (var order in context.Orders)
            {
                if (order.OrderId == id)
                {
                    o.Add(order);
                }
            }
            OrderData.Order = o;
            List<OrderDetail> od = new List<OrderDetail>();
            foreach (var order in context.OrderDetails)
            {
                if (order.OrderId == id)
                {
                    od.Add(order);
                }
            }
            OrderData.OrderDetail = od;
            return View(OrderData);
        }
        public IActionResult AllOrders(DateTime date)
        {
            OrderViewModel OrderData = new OrderViewModel();
            List<Orders> list = new List<Orders>();
            foreach (var order in context.Orders)
            {
                if (Convert.ToDateTime(order.OrderDate).ToShortDateString() == date.ToShortDateString() && order.Status == "Done" || date.ToShortDateString() == "01/01/0001" && order.Status == "Done")
                {
                    list.Add(order);
                }
            }
            OrderData.Order = list;
            return View(OrderData);
        }

        public void StatisticOrder(string time)
        {
            //Take list from database
            List<Orders> orders = new List<Orders>();
            foreach (var order in context.Orders)
            {
                if (order.Status == "Done")
                {
                    orders.Add(order);
                }
            }

            //Take distinct by date (Can't do directly at database)
            List<OrderStatisticModel> modelList = new List<OrderStatisticModel>();
            List<string> Date = new List<string>();

            if (time == null || time == "Day")
            {
                ViewBag.Title1 = "Revenues by Day";
                List<DateTime> Time = new List<DateTime>();
                Time = orders.DistinctBy(i => Convert.ToDateTime(i.OrderDate).Date).Select(h => Convert.ToDateTime(h.OrderDate).Date).ToList();
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
                    Time = orders.DistinctBy(i => Convert.ToDateTime(i.OrderDate).Month).Select(h => Convert.ToDateTime(h.OrderDate).Month).ToList();
                }
                else
                {
                    ViewBag.Title1 = "Revenues by Year";
                    Time = orders.DistinctBy(i => Convert.ToDateTime(i.OrderDate).Year).Select(h => Convert.ToDateTime(h.OrderDate).Year).ToList();
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
                    if (da == Convert.ToDateTime(order.OrderDate).ToShortDateString() && (time == null || time == "Day"))
                    {
                        Price += order.OrderPrice;
                    }
                    else if (da == Convert.ToDateTime(order.OrderDate).Month.ToString() && (time == null || time == "Month"))
                    {
                        Price += order.OrderPrice;
                    }
                    else if (da == Convert.ToDateTime(order.OrderDate).Year.ToString() && (time == null || time == "Year"))
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
                if (order.Status == "Done")
                {
                    orders.Add(order);
                }
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
                if (Convert.ToDateTime(order.OrderDate).ToShortDateString() == comparedTime)
                {
                    orderId.Add(order.OrderId);
                }
                else if (Convert.ToDateTime(order.OrderDate).Month.ToString() == comparedTime)
                {
                    orderId.Add(order.OrderId);
                }
                else if (Convert.ToDateTime(order.OrderDate).Year.ToString() == comparedTime)
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
    }
}