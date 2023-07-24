﻿using System.ComponentModel.DataAnnotations;

namespace TablesideOrdering.Areas.StoreOwner.Models
{
    public class Orders
    {
        [Key]
        public int OrderId { get; set; }
        public DateTime OrderDate { get; set; }
        public float OrderPrice { get; set; }
        public int ProductQuantity { get; set; }
        public string PhoneNumber { get; set; }
        public string TableNo { get; set; }
        public string Status { get; set; }
        public string CusName { get; set; }
    }
}