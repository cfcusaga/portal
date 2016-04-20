using System;
using System.ComponentModel.DataAnnotations;
using System.Text;
using cfcusaga.domain;

namespace Cfcusaga.Web.Models
{
    public class OrderItems
    {
        public int OrderId { get; set; }
        public int? OrderDetailId { get; set; }
        public int? ItemId { get; set; }

        public string ItemType
        {
            get
            {
                switch (ItemId)
                {
                    case 3:
                        return "Kids";
                    case 4:
                        return "Youth";
                    case 5:
                        return "Parent";
                    case 6:
                        return "Room";
                    case 8:
                        return "BedSpace";
                    case 7:
                        return "TShirt";
                    default:
                        return "";
                }
            }
        }
        public string ItemName { get; set; }

        public string ItemDisplayNameFor
        {
            get
            {
                switch (CategoryId)
                {
                    case (int)Enums.CategoryTypeEnum.Registration:
                        var sb = new StringBuilder();
                        //sb.Append($"{ItemName} (Name:{Firstname})");
                        sb.Append($"{ItemName}");
                        if (!string.IsNullOrEmpty(TshirtSize))
                        {
                            sb.Append($" (Tshirt:{TshirtSize})");
                        }
                        if (!string.IsNullOrEmpty(Allergies))
                        {
                            sb.Append($" (Allergies: {Allergies})");
                        }
                        return sb.ToString();
                    case (int)Enums.CategoryTypeEnum.Tshirt:
                        return $"{ItemName} (Size:{TshirtSize})";
                    default:
                        return ItemName;
                }
            }
        }

        public int? CategoryId { get; set; }
        public string Firstname { get; set; }

        public string Lastname { get; set; }

        

        public string TshirtSize { get; set; }
        public string Allergies { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string ZipCode { get; set; }
        public string Phone { get; set; }

        public string FullName => CategoryId == (int)Enums.CategoryTypeEnum.Registration ? $"{Lastname},{Firstname}" : string.Empty;

        public string OrderDateDisplay => OrderDate.ToString("MM/dd/yyyy");

        [Key]
        public string Id { get; set; }

        public DateTime OrderDate { get; set; }
        public string OrderdBy => $"{OrderByLastname},{OrderByFirstname}";
        public decimal Price { get; set; }
        public string OrderByLastname { get; set; }
        public string OrderByFirstname { get; set; }

        public DateTime? BirthDate { get; set; }

        public int? AgeOnEventDate => BirthDate?.Age(LaterDate);

        public DateTime LaterDate => new DateTime(2016, 6, 18);
        public decimal? OrderTotal { get; set; }
        public string Gender { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public string OrderNotes { get; set; }
        public decimal Discount { get; set; }
        public string Notes { get; set; }
    }

    public static class DateTimeExtensions
    {
         public static int Age(this DateTime birthDate, DateTime laterDate)
        {
            int age;
            age = laterDate.Year - birthDate.Year;

            if (age > 0)
            {
                age -= Convert.ToInt32(laterDate.Date < birthDate.Date.AddYears(age));
            }
            else
            {
                age = 0;
            }

            return age;
        }
    }
}