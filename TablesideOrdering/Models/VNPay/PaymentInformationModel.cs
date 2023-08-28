﻿using Microsoft.Build.Framework;

namespace TablesideOrdering.Models.VNPay
{
    public class PaymentInformationModel
    {
        [Required]
        public string OrderType { get; set; } = "Drinks";

        [Required]
        public double Amount { get; set; }

        [Required]
        public string OrderDescription { get; set; } = "Checkout at L&L coffee";

        [Required]
        public string Name { get; set; }

        [Required]
        public string Email { get; set; }
    }
}