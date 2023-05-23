﻿using System.ComponentModel.DataAnnotations;
using System.Drawing;

namespace TablesideOrdering.Models
{
    public class AddToCart
    {
        public int Id { get; set; }
        public Product Product { get; set; }
        public string Size { get; set; }
        public int Quantity { get; set; }
        public float Price { get; set; }
    }
}
