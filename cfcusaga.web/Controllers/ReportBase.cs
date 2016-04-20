using System.ComponentModel;

namespace Cfcusaga.Web.Controllers
{
    public abstract class ReportBase
    {

        [Browsable(false)]
        public int? ItemId { get; set; }
        public string ItemType
        {
            get
            {
                switch (ItemId)
                {
                    case 3:
                        return "Kids";
                    case 4:
                        return "Youth";
                    case 5:
                        return "Parent";
                    case 6:
                        return "Room";
                    case 8:
                        return "BedSpace";
                    case 7:
                        return "TShirt";
                    case int.MaxValue:
                        return "Discount";
                    default:
                        return "";
                }
            }
        }
    }
}