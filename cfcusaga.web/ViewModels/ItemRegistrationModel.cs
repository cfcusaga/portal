using System;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using Cfcusaga.Web.Attributes;

namespace Cfcusaga.Web.ViewModels
{
    public class ItemRegistrationModel
    {
        [Required]
        public string LastName { get; set; }
        [Required]
        public string FirstName { get; set; }

        public bool IsBirthDateRequired { get; set; }
        //IsRequireBirthDateInfo
        [RequiredIf("IsBirthDateRequired", true, "Birth Date is required")]
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}", ApplyFormatInEditMode = true)]
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