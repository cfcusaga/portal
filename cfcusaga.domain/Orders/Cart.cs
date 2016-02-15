namespace cfcusaga.domain.Orders
{
    public class Cart
    {
        public int ID { get; set; }
        public string CartId { get; set; }
        public int ItemId { get; set; }
        public int Count { get; set; }
        public System.DateTime DateCreated { get; set; }
        public decimal ItemPrice { get; set; }
        public string ItemName { get; set; }

        //public virtual domain.Events.Item Item { get; set; }
    }
}
