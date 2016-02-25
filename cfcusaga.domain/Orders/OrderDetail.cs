using System;
using cfcusaga.domain.Events;

namespace cfcusaga.domain.Orders
{
    public class OrderDetail
    {
        public int OrderDetailId { get; set; }
        public int OrderId { get; set; }
        public int ItemId { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public virtual Item Item { get; set; }
        public virtual Order Order { get; set; }
        public int CartId { get; set; }
        public string RegistrationDetail { get; set; }
        public string Lastname { get; set; }
        public string Firstname { get; set; }
        public string Gender { get; set; }
        public DateTime? BirthDate { get; set; }
        public string Allergies { get; set; }
        public string TshirtSize { get; set; }
        public int CategoryId { get; set; }
    }
}
