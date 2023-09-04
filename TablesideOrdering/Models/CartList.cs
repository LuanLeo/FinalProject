using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace TablesideOrdering.Models
{
    public class CartList
    {
        public string TableNo { get; set; }
        public string PhoneNumber { get; set; }
        public string CusName { get; set; }
        public List<AddToCart> CartLists { get; set; }
        public float CartAmount { get; set; }
        public float DicountAmount { get; set; }
        public float MustPaid { get; set; }

    }
}
