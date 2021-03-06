﻿using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using cfcusaga.domain.Orders;
using Cart = cfcusaga.domain.Orders.Cart;

namespace Cfcusaga.Web.ViewModels
{
    public class ShoppingCartViewModel
    {
        [Key]
        public List<Cart> CartItems { get; set; }
        public decimal CartTotal { get; set; }
        public List<CartDiscount> CartDiscounts { get; set; }
    }
}