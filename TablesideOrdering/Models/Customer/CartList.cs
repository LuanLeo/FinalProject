using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace TablesideOrdering.Models.Customer
{
    public class CartList
    {
        public string PhoneNumber { get; set; }
        public List<AddToCart> CartLists { get; set; }
        public float CartAmount { get; set; }
    }
}
