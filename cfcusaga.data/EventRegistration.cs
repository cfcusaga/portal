//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace cfcusaga.data
{
    using System;
    using System.Collections.Generic;
    
    public partial class EventRegistration
    {
        public int ID { get; set; }
        public int EventId { get; set; }
        public int OrderId { get; set; }
        public int MemberId { get; set; }
        public System.DateTime CreationDate { get; set; }
        public System.DateTime ModifiedDate { get; set; }
    
        public virtual Member Member { get; set; }
        public virtual Event Event { get; set; }
        public virtual Order Order { get; set; }
    }
}
