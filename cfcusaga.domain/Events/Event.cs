using System;

namespace cfcusaga.domain.Events
{
    public class Event
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        /// <summary>
        /// CFC|YFC|KFC|SFC|HOLD|SOLD
        /// </summary>
        public string OrgId { get; set; }
    }
}