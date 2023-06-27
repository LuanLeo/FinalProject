﻿using System.ComponentModel.DataAnnotations;

namespace TablesideOrdering.Models
{
    public class CustomerEmail
    {
        [Key]
        public int EmailId { get; set; }
        public string Email { get; set; }
    }
}
