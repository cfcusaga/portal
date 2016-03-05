using System.ComponentModel.DataAnnotations;

namespace cfcusaga.domain
{
    public static class Enums
    {
        public enum OrgTypeId
        {
            [Display(Name = "CFC")]
            CFC = 0,

            [Display(Name = "SFC")]
            SFC = 1,

            [Display(Name = "YFC")]
            YFC = 2,

            [Display(Name = "KFC")]
            KFC = 3,

            [Display(Name = "HOLD")]
            HOLD = 4,

            [Display(Name = "SOLD")]
            SOLD = 5,

            [Display(Name = "Not Applicable")]
            NA = -1
        }
        public enum CategoryTypeEnum
        {
            Registration = 2,
            Tshirt = 3,
            Room = 4,
            Default = 1 //does not have item attributes/details
        }

        public enum RelationToMe
        {
            Self = 0,
            Spouse = 1,
            Child = 2,
            Parent = 3,
            Other = 9 //not related
        }
    }


}