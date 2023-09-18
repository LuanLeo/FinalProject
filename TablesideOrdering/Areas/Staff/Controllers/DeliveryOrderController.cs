using DocumentFormat.OpenXml.Drawing.Charts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System;
using System.Data;
using TablesideOrdering.Areas.StoreOwner.Models;
using TablesideOrdering.Data;
using TablesideOrdering.Models;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;

namespace TablesideOrdering.Areas.Staff.Controllers
{
    [Area("Staff")]
    [Authorize(Roles = "Staff, Admin")]
    public class DeliveryOrderController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly SMSMessage _SMSMessage;

        public DeliveryOrderController(ApplicationDbContext context, IOptions<SMSMessage> SMSMessage)
        {
            _context = context;
            _SMSMessage = SMSMessage.Value;
        }
        public IActionResult Index()
        {
            ViewBag.Message = "New order has been updated";
            return View();
        }
        public void Delivery(int id)
        {
            Orders order = _context.Orders.Where(o => o.OrderId == id).FirstOrDefault();
            order.Status = "Delivering";
            _context.SaveChanges();


            SendSMS(order.PhoneNumber);
        }

        //SENDING SMS TO CUSTOMER FUCNTION
        public void SendSMS(string phone)
        {
            var PhoneMessage = "Your order is ready, please wait a minute!";
            string number = ConvertToPhoneValid(phone);
            TwilioClient.Init(_SMSMessage.AccountSid, _SMSMessage.AuthToken);
            var message = MessageResource.Create(
                to: new PhoneNumber(number),
                from: new PhoneNumber(_SMSMessage.PhoneFrom),
                body: PhoneMessage);
        }

        //MODIFY PHONE NUMBER FUNCTION
        public string ConvertToPhoneValid(string PhoneNumber)
        {
            string validnum = "";
            if (PhoneNumber.Substring(1) != "0")
            {
                string number = PhoneNumber.Substring(1);
                validnum = "+84" + number;
            }
            return validnum;
        }


        public IActionResult DoneDeliveryOrders()
        {
            var processOrder = _context.Orders.Where(i => i.OrderType == "Delivery" && i.Status == "Done");
            return View(processOrder);
        }
        public IActionResult Delivering()
        {
            var delivering = _context.Orders.Where(i => i.OrderType == "Delivery" && i.Status == "delivering");
            return View(delivering);

        }
    }
}
