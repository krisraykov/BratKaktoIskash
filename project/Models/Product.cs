﻿using System.ComponentModel.DataAnnotations;

namespace WebApplication6.Models
{
    public class Product
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Description { get;set; }
        [Required]
        public double Price { get; set; }
    }
}
