﻿using System;
namespace PestKit.ViewModels
{
	public class BasketItemVM
	{
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public int Count { get; set; }
        public decimal Subtotal { get; set; }
    }
}

