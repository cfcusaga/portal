﻿using System;
using cfcusaga.data;

namespace cfcusaga.domain.Orders
{
    public class Cart
    {
        public int Id { get; set; }
        public string CartId { get; set; }
        public int ItemId { get; set; }
        public int Count { get; set; }
        public System.DateTime DateCreated { get; set; }
        public decimal ItemPrice { get; set; }
        public string ItemName { get; set; }

        public decimal LineTotal => ItemPrice*Count;
        public virtual domain.Orders.ItemRegistration ItemRegistration { get; set; }


        // TODO
        public string Lastname { get; set; }
        public string Firstname { get; set; }
        public DateTime? BirthDate { get; internal set; }
        public string Gender { get; set; }
        public short RelationToMemberTypeId { get; set; }
        public int? MemberId { get; set; }
        public string Notes { get; set; }
        public string Allergies { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public int CategoryId { get; set; }
        public int? ItemRegistrationId { get; set; }
        public string TshirtSize { get; set; }

        public string DisplayName
        {
            get
            {

                if (ItemRegistrationId.HasValue)
                {
                    return $"{ItemName} ({Lastname},{Firstname})";
                }
                else if (CategoryId == (int) Enums.CategoryTypeEnum.Tshirt)
                {
                    return $"{ItemName} ({TshirtSize})";
                }
                else
                {
                    return ItemName;
                }
            } 
        }

        public int? EventId { get; set; }
        public string RegistrationTshirtSize { get; set; }
    }
}
