using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cfcusaga.domain.Orders
{
    public class ItemRegistration
    {
        [Key]
        [ScaffoldColumn(false)]
        public int ID { get; set; }

        [ScaffoldColumn(false)]
        public int CartID { get; set; }

        [ScaffoldColumn(false)]
        public int? MemberId { get; set; }
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public DateTime? BirthDate { get; set; }
        public short? RelationToMemberType { get; set; }
        public string Gender { get; set; }
        public string Notes { get; set; }
        public string Allergies { get; set; }
    }
}
