using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cfcusaga.domain.Events
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
