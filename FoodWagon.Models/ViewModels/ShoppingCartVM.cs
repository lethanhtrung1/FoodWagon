﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodWagon.Models.ViewModels {
    public class ShoppingCartVM {
        public IEnumerable<ShoppingCart> ShoppingCarts { get; set; }
        public double OrderTotal { get; set; }
    }
}
