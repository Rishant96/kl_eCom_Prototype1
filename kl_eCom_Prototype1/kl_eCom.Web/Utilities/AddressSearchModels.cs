using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace kl_eCom.Web.Utilities
{
    public class Country
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public virtual ICollection<State> States { get; set; }
    }

    public class State
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public int CountryId { get; set; }
        public Country Country { get; set; }

        public virtual ICollection<Place> Places { get; set; }
    }

    public class Place
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        public string Name { get; set; }

        [Required]
        public int StateId { get; set; }
        public virtual State State { get; set; }

        public ICollection<Market> Markets { get; set; }
    }

    public class Market
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public int PlaceId { get; set; }
        public Place Place { get; set; }
        
        public float? Latitude { get; set; }
        public float? Longitude { get; set; }
    }
}