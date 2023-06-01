using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using TablesideOrdering.Models;

namespace TablesideOrdering.ViewModels
{
    public class CartList
    {
        public string PhoneNumber { get; set; }
        public List<AddToCart> CartLists { get; set; }
        public float CartAmount { get; set; }
    }
}
