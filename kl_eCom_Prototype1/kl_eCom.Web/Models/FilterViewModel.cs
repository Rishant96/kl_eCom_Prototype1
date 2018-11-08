using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace kl_eCom.Web.Models
{
    public class FilterViewModel
    {
        public int MinPrice { get; set; }

        public int MaxPrice { get; set; }

        public int PriceRange
        {
            get
            {
                return MaxPrice - MinPrice;
            }
        }

        public int MinRating { get; set; } 

        public int MaxDays { get; set; }

        public bool ListInStock { get; set; }
    }
}