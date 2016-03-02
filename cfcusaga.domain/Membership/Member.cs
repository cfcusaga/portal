using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cfcusaga.domain.Membership
{
   public class Member
    {
       public string LastName { get; set; }
       public string Firstname { get; set; }
       public DateTime? BirthDate { get; set; }
       public string Gender { get; set; }
       public string Phone { get; set; }
       public string Email { get; set; }
       public int Id { get; set; }
       public string AspNetUserId { get; set; }
    }
}
