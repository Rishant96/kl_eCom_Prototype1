using kl_eCom.Web.Entities;
using kl_eCom.Web.Infrastructure;
using kl_eCom.Web.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace kl_eCom.Web.Models
{
    public class ShopIndexViewModel
    {
        public Dictionary<string, int> Breadcrum { get; set; } 
        public List<Category> Categories { get; set; }
        public Dictionary<Category, bool> IsLeafDict { get; set; }
    }

    public class ShopProductsViewModel
    {
        public ShopFilteringOptions FilteringOptions { get; set; }
        public ShopFilterViewModel FilterViewModel { get; set; }
        public int? CategoryId { get; set; }
        public int? StoreId { get; set; }
        public Dictionary<string, int> Breadcrum { get; set; }
        public List<Stock> Stocks { get; set; }
        public int Qty { get; set; }
        public Dictionary<int, int> Max { get; set; }
        [Required]
        public SortOption SelectedOption { get; set; }
    }

    public class ShopProductDetailsViewModel
    {
        public string ReturnUrl { get; set; }
        public string Description { get; set; }
        public Stock Stock { get; set; }
        public int StockId { get; set; }
        public int AlreadyInCart { get; set; }
        public int Qty { get; set; }
        public float? NewPrice { get; set; }
        public List<DiscountConstraint> MinOrderDiscounts { get; set; }
        public List<DiscountConstraint> BundleDiscounts { get; set; }
        public Dictionary<DiscountConstraint, List<Stock>> BundleStocks { get; set; }
        public Dictionary<DiscountConstraint, string> BundleOldPrices { get; set; }
        public Dictionary<DiscountConstraint, string> BundleNewPrices { get; set; }
        public List<DiscountConstraint> MinQtyDiscounts { get; set; }
    }

    public class ShopBundleOrderViewModel
    {
        public int[] StockIds { get; set; }
    }

    // Helpers: Price Selection, Rating Selection, Newest Arrivals Selection
    //          Availability Selection

    public class SelectedFilters
    {
        public int PriceFilterSelected { get; set; }
        public int RatingFilterSelected { get; set; }
        public int NewArrivalFilterSelected { get; set; }
        public int AvailabilityFilterSelected { get; set; }
    }

    public class ShopFilteringOptions
    {
        public ShopFilteringOptions()
        {
            this.Price_MaxValue = int.MaxValue;
            this.Price_MinValue = 0;
            this.Rating_Min = 0;
            this.Allowed_Days = -1;
            this.Availability = true;
        }

        public int Price_MinValue { get; set; }
        public int Price_MaxValue { get; set; }
        public int Rating_Min { get; set; }
        public int Allowed_Days { get; set; }
        public bool Availability { get; set; }
    }

    public class ShopFilterViewModel
    {
        public PriceSelection PriceSelection { get; set; }
        public int MinValue { get; set; }
        public int MaxValue { get; set; }
        public RatingSelection RatingSelection { get; set; }
        public NewestArrivalSelection NewestArrivalSelection { get; set; }
        public AvailabilitySelection AvailabilitySelection { get; set; }
    }

    public class PriceSelection
    {
        public int PriceItemSelected { get; set; }
        public List<PriceSelectionItem> PriceSelectionItems { get; set; }
    }

    public class PriceSelectionItem
    {
        public int Id { get; set; }
        public string DisplayName { get; set; }
        public int MinPrice { get; set; }
        public int MaxPrice { get; set; }
    }

    public class RatingSelection
    {
        public int RatingItemSelected { get; set; }
        public List<RatingSelectionItem> RatingSelectionItems { get; set; }
    }

    public class RatingSelectionItem
    {
        public int Id { get; set; }
        public string DisplayName { get; set; }
        public int MinRating { get; set; }
    }

    public class NewestArrivalSelection
    {
        public int NewestArrivalItemSelected { get; set; }
        public List<NewestArrivalSelectionItem> NewestArrivalSelectionItems { get; set; }
    }

    public class NewestArrivalSelectionItem
    {
        public int Id { get; set; }
        public string DisplayName { get; set; }
        public int AllowedDays { get; set; }
    }

    public class AvailabilitySelection
    {
        public int AvailabilityItemSelected { get; set; }
        public List<AvailabilitySelectionItem> AvailabilitySelectionItems { get; set; }
    }

    public class AvailabilitySelectionItem
    {
        public int Id { get; set; }
        public string DisplayName { get; set; }
        public bool Value { get; set; }
    }
}