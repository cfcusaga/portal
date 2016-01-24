using System.ComponentModel.DataAnnotations;

namespace cfcusaga.domain
{
    public static class Enums
    {
        public enum OrgTypeId
        {
            [Display(Name = "CFC")]
            CFC = 1,

            [Display(Name = "SFC")]
            SFC = 2,

            [Display(Name = "YFC")]
            YFC = 3,

            [Display(Name = "KFC")]
            KFC = 4,

            [Display(Name = "HOLD")]
            HOLD = 5,

            [Display(Name = "SOLD")]
            SOLD = 6,

            [Display(Name = "Not Applicable")]
            NA = 7
        }
    }
}