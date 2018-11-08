using kl_eCom.Web.Entities;
using kl_eCom.Web.Infrastructure;
using kl_eCom.Web.Models;
using kl_eCom.Web.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace kl_eCom.Web.Areas.VendorStore.Models
{
    public class ProductsCategoriesViewModel
    {
        public Dictionary<string, int> Breadcrum { get; set; }
        public List<Category> Categories { get; set; }
        public Dictionary<Category, bool> IsLeafDict { get; set; }
    }

    public class ProductsListViewModel
    {
        public string CurrencySymbol { get; set; }
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
        public bool IsKlCat { get; set; }
        public List<KL_Category> KL_Categories { get; set; }
    }

    public class ProductsDetailsViewModel
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
}
