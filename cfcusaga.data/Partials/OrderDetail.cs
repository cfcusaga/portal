using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cfcusaga.data
{
    public partial class OrderDetail
    {
        public string ItemDisplayNameFor
        {
            get
            {
                switch (Item.CatagoryID)
                {
                    case (int) Enums.CategoryTypeEnum.Registration:
                        var sb = new StringBuilder();
                        sb.Append($"{Item.Name} (Name:{Firstname})");
                        if (!string.IsNullOrEmpty(TshirtSize))
                        {
                            sb.Append($" (Tshirt:{TshirtSize})");
                        }
                        if (!string.IsNullOrEmpty(Allergies))
                        {
                            sb.Append($" (Allergies: {Allergies})");
                        }
                        return sb.ToString();
                    case (int) Enums.CategoryTypeEnum.Tshirt:
                        return $"{Item.Name} (Size:{TshirtSize})";
                    default:
                        return Item.Name;
                }
            }
        }

        //public string TshirtDisplayName => $"{Item.Name} ({TshirtSize})";
    }
}
