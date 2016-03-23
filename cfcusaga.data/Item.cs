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
    
    public partial class Item
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Item()
        {
            this.Carts = new HashSet<Cart>();
            this.OrderDetails = new HashSet<OrderDetail>();
            this.Discounts = new HashSet<Discount>();
            this.ItemImages = new HashSet<ItemImage>();
        }
    
        public int ID { get; set; }
        public int CatagoryID { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public string ItemPictureUrl { get; set; }
        public byte[] InternalImage { get; set; }
        public Nullable<int> EventId { get; set; }
        public Nullable<bool> IsRequireRegistrationInfo { get; set; }
        public Nullable<bool> IsRequireParentWaiver { get; set; }
        public Nullable<bool> IsRequireBirthDateInfo { get; set; }
        public Nullable<bool> IsShirtIncluded { get; set; }
        public string Description { get; set; }
        public Nullable<bool> IsRequireTshirtSize { get; set; }
        public string TshirtSize { get; set; }
        public Nullable<short> OrgTypeId { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Cart> Carts { get; set; }
        public virtual Catagory Catagory { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<OrderDetail> OrderDetails { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Discount> Discounts { get; set; }
        public virtual Event Event { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ItemImage> ItemImages { get; set; }
    }
}
