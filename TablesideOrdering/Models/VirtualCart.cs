using MailKit.Search;
using System.ComponentModel.DataAnnotations;

namespace TablesideOrdering.Models
{
    public class VirtualCart
    {
        [Key]
        public int CartId { get; set; }
        public string TableId { get; set; }
        public float CartAmount { get; set; }//cart total
        public string CusPhoneNum { get; set; } = "";
        public string CusName { get; set; } = "";
        public string CusEmail { get; set; } = "";
        public string PaymentType { get; set; } = "";
        public string OrderType { get; set; } = "";
        public string Address { get; set; } = "";
        public string Coupon { get; set; } = "";

        public string file { get; set; } = "";
        public string Subject { get; set; } = "";
        public string EmailMessage { get; set; } = "";

    }
}
