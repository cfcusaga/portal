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
    
    public partial class CartItemRegistration
    {
        public int ID { get; set; }
        public int CartID { get; set; }
        public Nullable<int> MemberId { get; set; }
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public Nullable<System.DateTime> BirthDate { get; set; }
        public Nullable<short> RelationToMemberTypeId { get; set; }
        public string Gender { get; set; }
        public string Notes { get; set; }
        public string Allergies { get; set; }
    
        public virtual Cart Cart { get; set; }
        public virtual RelationToMemberType RelationToMemberType { get; set; }
    }
}
