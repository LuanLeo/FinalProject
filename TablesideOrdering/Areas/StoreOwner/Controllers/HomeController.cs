using AspNetCoreHero.ToastNotification.Abstractions;
using ClosedXML.Excel;
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
using System.Text;
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
        private readonly ApplicationDbContext _context;
        public INotyfService _notyfService { get; }
        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context, INotyfService notyfService)
        {
            _logger = logger;
            _context = context;
            _notyfService = notyfService;
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
            List<OrderDetail> oList = new List<OrderDetail>();
            OrderData.Order = _context.Orders.Find(id);
            foreach (var order in _context.OrderDetails)
            {
                if (order.OrderId == id)
                {
                    oList.Add(order);
                }
            }
            OrderData.OrderDetail = oList;
            return View(OrderData);
        }
        public IActionResult AllOrders(DateTime date)
        {
            OrderViewModel OrderData = new OrderViewModel();
            List<Orders> list = new List<Orders>();
            foreach (var order in _context.Orders.ToList())
            {
                if (Convert.ToDateTime(order.OrderDate).ToShortDateString() == date.ToShortDateString() && order.Status == "Done" || date.ToShortDateString() == "01/01/0001" && order.Status == "Done")
                {
                    list.Add(order);
                }
            }
            OrderData.OrderList = list;
            return View(OrderData);
        }

        [HttpGet]
        public IActionResult Delete(int id)
        {
            OrderViewModel OrderData = new OrderViewModel();
            List<OrderDetail> oList = new List<OrderDetail>();
            OrderData.Order = _context.Orders.Find(id);
            foreach (var order in _context.OrderDetails)
            {
                if (order.OrderId == id)
                {
                    oList.Add(order);
                }
            }
            OrderData.OrderDetail = oList;
            return PartialView("Delete", OrderData);
        }

        [HttpPost]
        public IActionResult Delete(OrderViewModel model)
        {
            var order = _context.Orders.Find(model.Order.OrderId);

            foreach (var od in _context.OrderDetails)
            {
                if (od.OrderId == model.Order.OrderId)
                {
                    _context.OrderDetails.Remove(od);
                }
            }

            _context.Orders.Remove(order);
            _context.SaveChanges();

            _notyfService.Success("The order is deleted", 5);
            return RedirectToAction("AllOrders");
        }

        public IActionResult Csv()
        {
            if (_context.Orders.Count() != 0)
            {
                var builder = new StringBuilder();
                builder.AppendLine("Order Id, Order Date, Order Type, Status, Phone Number, Customer Name, Payment Type, Table No, Address, Pick Time, Product Quantity, Order Price\r\n");
                foreach (var order in _context.Orders)
                {
                    builder.AppendLine($"{order.OrderId},{order.OrderDate},{order.OrderType},{order.Status},{order.PhoneNumber},{order.CusName},{order.PaymentType},{order.TableNo},{order.Address},{order.PickTime},{order.ProductQuantity},{order.OrderPrice}");
                }

                return File(Encoding.UTF8.GetBytes(builder.ToString()), "text/csv", "OrdersList.csv");
            }
            _notyfService.Warning("Lack of orders to export.", 5);
            return RedirectToAction("Index");
        }

        public IActionResult Excel()
        {
            if (_context.Orders.Count() != 0)
            {
                using var workbook = new XLWorkbook();
                var worksheet = workbook.Worksheets.Add("Orders List");
                var currentRow = 1;

                worksheet.Cell(currentRow, 1).Value = "Order Id";
                worksheet.Cell(currentRow, 2).Value = "Order Date";
                worksheet.Cell(currentRow, 3).Value = "Order Type";
                worksheet.Cell(currentRow, 4).Value = "Status";
                worksheet.Cell(currentRow, 5).Value = "Phone Number";
                worksheet.Cell(currentRow, 6).Value = "Customer Name";
                worksheet.Cell(currentRow, 7).Value = "Payment Type";
                worksheet.Cell(currentRow, 8).Value = "Table No";
                worksheet.Cell(currentRow, 9).Value = "Address";
                worksheet.Cell(currentRow, 10).Value = "Pick Time";
                worksheet.Cell(currentRow, 11).Value = "Product Quantity";
                worksheet.Cell(currentRow, 12).Value = "Order Price";

                foreach (var order in _context.Orders)
                {
                    currentRow++;
                    worksheet.Cell(currentRow, 1).Value = order.OrderId;
                    worksheet.Cell(currentRow, 2).Value = order.OrderDate;
                    worksheet.Cell(currentRow, 3).Value = order.OrderType;
                    worksheet.Cell(currentRow, 4).Value = order.Status;
                    worksheet.Cell(currentRow, 5).Value = order.PhoneNumber;
                    worksheet.Cell(currentRow, 6).Value = order.CusName;
                    worksheet.Cell(currentRow, 7).Value = order.PaymentType;
                    worksheet.Cell(currentRow, 8).Value = order.TableNo;
                    worksheet.Cell(currentRow, 9).Value = order.Address;
                    worksheet.Cell(currentRow, 10).Value = order.PickTime;
                    worksheet.Cell(currentRow, 11).Value = order.ProductQuantity;
                    worksheet.Cell(currentRow, 12).Value = order.OrderPrice;
                }

                using var stream = new MemoryStream();
                workbook.SaveAs(stream);
                var content = stream.ToArray();
                return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "OrdersList.xlsx");
            }

            _notyfService.Warning("Lack of orders to export.", 5);
            return RedirectToAction("Index");
        }

        public void StatisticOrder(string time)
        {
            //Take list from database
            List<Orders> orders = new List<Orders>();
            foreach (var order in _context.Orders)
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

            foreach (var order in _context.Orders)
            {
                if (order.Status == "Done")
                {
                    orders.Add(order);
                }
            }

            foreach (var detail in _context.OrderDetails)
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
            else if (time == "Month")
            {
                ViewBag.Title2 = "this Month";
                comparedTime = DateTime.Now.Month.ToString();
            }
            else
            {
                ViewBag.Title2 = "this Year";
                comparedTime = DateTime.Now.Year.ToString();
            }

            foreach (var order in orders)
            {
                if (Convert.ToDateTime(order.OrderDate).ToShortDateString() == comparedTime)
                {
                    orderId.Add(order.OrderId);
                }
                if (Convert.ToDateTime(order.OrderDate).Month.ToString() == comparedTime)
                {
                    orderId.Add(order.OrderId);
                }
                if (Convert.ToDateTime(order.OrderDate).Year.ToString() == comparedTime)
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
            foreach (var t in top.OrderBy(i => i.Value))
            {
                FoodTopList.Add(new TopFoodLogicModel(t.FoodName, t.Value));
            }
            ViewBag.TopFood = JsonConvert.SerializeObject(FoodTopList);
        }
    }
}