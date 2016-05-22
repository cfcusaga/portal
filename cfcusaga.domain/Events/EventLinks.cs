using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cfcusaga.domain.Events
{
    public class EventInfo
    {
        public List<EventLink> Links { get; set; }
    }

    public class EventLink
    {
        public string Name { get; set; }
        public string Url { get; set; }
    }
}
