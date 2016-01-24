using System;
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

        public string Name { get; set; }

        public decimal Price { get; set; }

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

        public virtual Catagory Catagorie { get; set; }
    }
}
