using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Web;

namespace kl_eCom.Web.Infrastructure
{
    public class QueryOptions
    {
        public QueryOptions()
        {
            SortOption = SortOption.Relevance;

            CurrentPage = 1;
            PageSize = 10;
        }
        
        public SortOption SortOption { get; set; }

        public string Sort
        {
            get
            {
                var result = "";
                switch (SortOption)
                {
                    case SortOption.Relevance:
                    {
                        result = "Id ASC";
                        break;
                    }
                    case SortOption.NewestArrivals:
                    {
                        result = "StockingDate ASC";
                        break;
                    }
                    case SortOption.Price_HighToLow:
                    {
                        result = "Price DESC";
                        break;
                    }
                    case SortOption.Price_LowToHigh:
                    {
                        result = "Price ASC";
                        break;
                    }
                    case SortOption.AverageRating:
                    {
                        result = "Product.Rating DESC";
                        break;

                    }
                    default:
                        result = "Id ASC";
                        break;
                }
                return result;
            }
        }

        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
    }

    public enum SortOption
    {
        [Display(Name = "Relevance")]
        Relevance,
        [Display(Name = "Newest Arrival")]
        NewestArrivals,
        [Display(Name = "Price (High to Low)")]
        Price_HighToLow,
        [Display(Name = "Price (Low to High")]
        Price_LowToHigh,
        [Display(Name = "Average Rating")]
        AverageRating
    }

    public static class EnumExtensions
    {
        public static string GetDisplayName(this Enum enumValue)
        {
            return enumValue.GetType()
                            .GetMember(enumValue.ToString())
                            .First()
                            .GetCustomAttribute<DisplayAttribute>()
                            .GetName();
        }
    }
}