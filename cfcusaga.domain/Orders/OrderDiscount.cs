namespace cfcusaga.domain.Orders
{
    public class OrderDiscount
    {
        public int Id { get; set; }
        public int DiscountId { get; set; }
        public decimal Discount { get; set; }
        public int Quantity { get; set; }
        public string Name { get; set; }

        public decimal LineTotal => Discount * Quantity;
        public int OrderId { get; set; }
    }
}