using kl_eCom.Web.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace kl_eCom.Web.Areas.KL_Admin.Models
{
    public class AdminMarketsPlaces_IndexViewModel
    {
        public List<Country> Countries { get; set; }
        public Dictionary<Country, List<State>> States { get; set; }
        public Dictionary<State, List<Place>> Places { get; set; }
        public Dictionary<Place, List<Market>> Markets { get; set; }
    }

    public class AdminMarketsPlaces_CreateCountryViewModel
    {
        [Required]
        public string Name { get; set; }
    }

    public class AdminMarketPlaces_CountryDetailsViewModel
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public Dictionary<string, int> States { get; set; }
    }

    public class AdminMarketsPlaces_EditCountryViewModel
    {
        [Required]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }
    }

    public class AdminMarketsPlaces_CreateStateViewModel
    {
        [Required]
        public string Name { get; set; }
        
        [Required]
        public int CountryId { get; set; }

        public string CountryName { get; set; }
    }

    public class AdminMarketsPlaces_IndexStateViewModel
    {
        public int StateId { get; set; }

        public string StateName { get; set; }

        public int CountryId { get; set; }

        public string CountryName { get; set; }

        public Dictionary<string, int> Citites { get; set; }
    }

    public class AdminMarketsPlaces_EditStateViewModel
    {
        [Required]
        public int Id { get; set; }

        [Required]
        public string StateName { get; set; }
        
        public string CountryName { get; set; }
    }

    public class AdminMarketsPlaces_CreateCityViewModel
    {
        [Required]
        public int StateId { get; set; }

        public string StateName { get; set; }

        [Required]
        public string Name { get; set; }
    }

    public class AdminMarketsPlaces_CityDetailsViewModel
    {
        public int CityId { get; set; }

        public int StateId { get; set; }

        public string CityName { get; set; }

        public string StateName { get; set; }

        public Dictionary<string, int> Markets { get; set; }
    }

    public class AdminMarketsPlaces_EditCityViewModel
    {
        [Required]
        public int CityId { get; set; }

        [Required]
        public string CityName { get; set; }

        [Required]
        public int StateId { get; set; }
    }

    public class AdminMarketsPlaces_CreateMarketViewModel
    {
        [Required]
        public string Name { get; set; }

        [Required]
        public int CityId { get; set; }

        public string CityName { get; set; }

        public float? Latitude { get; set; }

        public float? Longitude { get; set; } 
    }

    public class AdminMarketsPlaces_MarketDetailsViewModel
    {
        public int CityId { get; set; }

        public int MarketId { get; set; }
        
        public string CityName { get; set; }

        public string MarketName { get; set; }

        public float? Latitude { get; set; }

        public float? Longitude { get; set; }
    }

    public class AdminMarketsPlaces_EditMarketViewModel
    {
        [Required]
        public int MarketId { get; set; }

        [Required]
        public string MarketName { get; set; }
        
        public float? Latitude { get; set; }
        
        public float? Longitude { get; set; }
    }
}
