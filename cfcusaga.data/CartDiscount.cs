//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace cfcusaga.data
{
    using System;
    using System.Collections.Generic;
    
    public partial class CartDiscount
    {
        public int Id { get; set; }
        public int DiscountId { get; set; }
        public decimal Discount { get; set; }
        public string CartId { get; set; }
        public int Quantity { get; set; }
    
        public virtual Discount Discount1 { get; set; }
    }
}
