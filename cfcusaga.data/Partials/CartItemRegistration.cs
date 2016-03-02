using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Common.CommandTrees;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
