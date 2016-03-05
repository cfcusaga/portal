namespace cfcusaga.data
{
    public partial class CartItemRegistration
    {
        public string BirthDateDisplay
        {
            get
            {
                if (BirthDate.HasValue)
                {
                    return BirthDate.Value.ToString("MM/dd/yyyy");
                }
                return string.Empty;
            }
        }
    }

}
