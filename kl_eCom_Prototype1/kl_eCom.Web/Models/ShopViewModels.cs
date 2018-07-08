using kl_eCom.Web.Entities;
using kl_eCom.Web.Infrastructure;
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
        public int CategoryId { get; set; }
        public Dictionary<string, int> Breadcrum { get; set; }
        public List<Stock> Stocks { get; set; }
        public int Qty { get; set; }
        public Dictionary<int, int> Max { get; set; }
        [Required]
        public SortOption SelectedOption { get; set; }
    }

    // Helpers: Price Selection, Rating Selection, Newest Arrivals Selection
    //          Availability Selection

    public class ShopFilterViewModel
    {
        public PriceSelection PriceSelection { get; set; }
        public RatingSelection RatingSelection { get; set; }
        public NewestArrivalSelection NewestArrivalSelection { get; set; }
        public AvailabilitySelection AvailabilitySelection { get; set; }
    }

    public class PriceSelection
    {
        public PriceSelectionItem PriceItemSelected { get; set; }
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
        public RatingSelectionItem RatingItemSelected { get; set; }
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
        public NewestArrivalSelectionItem NewestArrivalItemSelected { get; set; }
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
        public AvailabilitySelectionItem AvailabilityItemSelected { get; set; }
        public List<AvailabilitySelectionItem> AvailabilitySelectionItems { get; set; }
    }

    public class AvailabilitySelectionItem
    {
        public int Id { get; set; }
        public string DisplayName { get; set; }
        public bool Value { get; set; }
    }
}