namespace Cfcusaga.Web.ViewModels
{
    public class CartItemViewModel
    {
        public string Name { get; set; }
        public string ItemPictureUrl { get; set; }
        public int Id { get; set; }
        public int ItemId { get; set; }
        public decimal Price { get; set; }
        public bool? IsRequireTshirtSize { get; set; }
        public string TshirtSize { get; set; }
        public int CategoryId { get; set; }
    }
}