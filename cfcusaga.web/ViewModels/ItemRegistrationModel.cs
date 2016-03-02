using System;
using System.Web.Mvc;

namespace Cfcusaga.Web.ViewModels
{
    public class ItemRegistrationModel
    {
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public DateTime? BirthDate { get; set; }
        public short? RelationToMemberTypeId { get; set; }
        public string Gender { get; set; }
        public string TshirtSize { get; set; }
        public string Allergies { get; set; }
        public int ID { get; set; }
        public int CartID { get; set; }
        public int? MemberId { get; set; }
        public SelectList RelationToMemberTypes { get; set; }

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