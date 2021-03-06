﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.IO;
using cfcusaga.data;

namespace cfcusaga.domain.Events
{
    public class Item
    {
        public int Id { get; set; }

        public int CatagorieId { get; set; }

        [Required(ErrorMessage = "An Item Name is required")]
        [StringLength(160)]
        public string Name { get; set; }

        [Display(Name = "Price")]
        public decimal Price { get; set; }


        [Display(Name = "Picture")]
        public byte[] InternalImage { get; set; }

        [Display(Name = "Local file")]
        public HttpPostedFileBase File
        {
            get
            {
                return null;
            }

            set
            {
                try
                {
                    MemoryStream target = new MemoryStream();

                    if (value.InputStream == null)
                        return;

                    value.InputStream.CopyTo(target);
                    InternalImage = target.ToArray();
                }
                catch (Exception ex)
                {
                }
            }
        }

        public string ItemPictureUrl { get; set; }
        public int EventId { get; set; }
        public string EventName { get; set; }

        [DisplayName("Category")]
        public virtual Catagory Catagorie { get; set; }

        public bool IsShirtIncluded { get; set; }
        public bool IsRequireRegistrationInfo { get; set; }
        public bool IsRequireParentWaiver { get; set; }
        public bool IsRequireBirthDateInfo { get; set; }
        public bool IsRequireTshirtSize { get; set; }
        public string TshirtSize { get; set; }
        public string Description { get; set; }
        public ICollection<ItemImage> ItemImages { get; set; }
    }
}
