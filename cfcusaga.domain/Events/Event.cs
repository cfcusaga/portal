using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace cfcusaga.domain.Events
{
    public class Event
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Event Name is required")]
        [DisplayName("Event Name")]
        public string Name { get; set; }
        public string Description { get; set; }

        [Required(ErrorMessage = "Start Date is required")]
        [Display(Name = "Start Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime StartDate { get; set; }

        [Display(Name = "End Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime EndDate { get; set; }
        /// <summary>
        /// CFC|YFC|KFC|SFC|HOLD|SOLD
        /// </summary>
        [Display(Name = "Org ID")]
        public string OrgId { get; set; }
    }
}