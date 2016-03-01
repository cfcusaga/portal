using System.ComponentModel.DataAnnotations;

namespace cfcusaga.data
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