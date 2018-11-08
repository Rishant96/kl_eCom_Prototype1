using kl_eCom.Web.Models;
using kl_eCom.Web.Areas.KL_Admin.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
using kl_eCom.Web.Utilities;

namespace kl_eCom.Web.Areas.KL_Admin.Controllers
{
    public class Markets_PlacesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        // GET: KL_Admin/Markets_Places
        public ActionResult Index()
        {
            var model = new AdminMarketsPlaces_IndexViewModel {
                Countries = db.Countries.ToList(),
                States = new Dictionary<Country, List<State>>(),
                Places = new Dictionary<State, List<Place>>(),
                Markets = new Dictionary<Place, List<Market>>()
            };

            foreach (var country in model.Countries)
            {
                model.States[country] = db.States
                    .Where(m => m.CountryId == country.Id)
                    .ToList();

                foreach (var state in model.States[country])
                {
                    model.Places[state] = db.Places
                        .Where(m => m.StateId == state.Id)
                        .ToList();

                    foreach (var place in model.Places[state])
                    {
                        model.Markets[place] = db.Markets
                            .Where(m => m.PlaceId == place.Id)
                            .ToList();
                    }
                }
            }

            return View(model);
        }

        public ActionResult CreateCountry()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateCountry(AdminMarketsPlaces_CreateCountryViewModel model)
        {
            if (ModelState.IsValid && db.Countries.FirstOrDefault(
                  m => string.Compare(model.Name.Trim(), m.Name.Trim(), 
                    true) == 0) == null)
            {
                db.Countries.Add(new Country {
                    Name = model.Name
                });

                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ModelState.AddModelError("Name", new Exception());
            return View(model);
        }

        public ActionResult CreateMultipleCountries()
        {
            var model = new AdminMarketsPlaces_MultipleCreateViewModel {
                Delimiters = new Dictionary<string, int>()
            };

            model.Delimiters.Add("Comma ','", 1);
            model.Delimiters.Add("Colon ';'", 2);
            model.Delimiters.Add("Space ' '", 3);
            model.Delimiters.Add("Newline '\n'", 4);

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateMultipleCountries(AdminMarketsPlaces_MultipleCreateViewModel model)
        {
            if (ModelState.IsValid)
            {
                char delimiter = '\0';

                switch (model.Delimiter)
                {
                    case 1:
                        {
                            delimiter = ',';
                            break;
                        }
                    case 2:
                        {
                            delimiter = ';';
                            break;
                        }
                    case 3:
                        {
                            delimiter = ' ';
                            break;
                        }
                    case 4:
                        {
                            delimiter = '\n';
                            break;
                        }
                }

                var countries = model.Countries.Split(delimiter).Select(s => s.Trim()).ToArray();

                foreach(var country in countries)
                {
                    if (db.Countries.FirstOrDefault(m => m.Name == country) is null)
                        db.Countries.Add(new Country { Name = country });
                }

                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return RedirectToAction("CreateMultipleCountries");
        }

        public ActionResult CountryDetails(int? id)
        {
            if (id is null) return View("Error");

            var country = db.Countries
                .Include(m => m.States)
                .FirstOrDefault(m => m.Id == id);
            if (country is null) return View("Error");

            var model = new AdminMarketPlaces_CountryDetailsViewModel {
                 Id = country.Id,
                 Name = country.Name,
                 States = new Dictionary<string, int>()
            };

            foreach (var state in country.States)
            {
                model.States.Add(state.Name, state.Id);
            }

            return View(model);
        }

        public ActionResult EditCountry(int? id)
        {
            if (id is null) return View("Error");

            var country = db.Countries.FirstOrDefault(m => m.Id == id);
            if (country is null) return View("Error");
            
            return View(new AdminMarketsPlaces_EditCountryViewModel {
                Id = country.Id,
                Name = country.Name
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditCountry(AdminMarketsPlaces_EditCountryViewModel model)
        {
            if (ModelState.IsValid)
            {
                var country = db.Countries.FirstOrDefault(m => m.Id == model.Id);
                if (country is null) return View("Error");

                country.Name = model.Name;

                db.Entry(country).State = EntityState.Modified;
                db.SaveChanges();

                return RedirectToAction("CountryDetails", new { id = model.Id });
            }

            return View(model);
        }

        public ActionResult DeleteCountry(int? id)
        {
            if (id == null) return View("Error");

            var country = db.Countries.FirstOrDefault(m => m.Id == id);
            if (country is null) return View("Error");
            
            return View(country);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteCountry(Country model)
        {
            if (ModelState.IsValid)
            {
                var country = db.Countries.Include(m => m.States).FirstOrDefault(m => m.Id == model.Id);

                var states = new List<State>(country.States);
                foreach (var state in states)
                {
                    DeleteState(new State { CountryId = model.Id, Id = state.Id, Name = state.Name });
                }

                db.Entry(country).State = EntityState.Deleted;
                db.SaveChanges();

                return RedirectToAction("Index");
            }

            return View(model);
        }

        public ActionResult CreateState(int? id)
        {
            if (id is null) return View("Error");

            var country = db.Countries.FirstOrDefault(m => m.Id == id);
            if (country is null) return View("Error");
            
            return View(new AdminMarketsPlaces_CreateStateViewModel {
                CountryName = country.Name,
                CountryId = country.Id
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateState(AdminMarketsPlaces_CreateStateViewModel model)
        {
            if (ModelState.IsValid)
            {
                db.States.Add(new State { CountryId = model.CountryId, Name = model.Name });
                db.SaveChanges();

                return RedirectToAction("CountryDetails", new { id = model.CountryId });
            }
            return View(model);
        }

        public ActionResult CreateMultipleStates(int? id)
        {
            if (id is null) return View("Error");

            var country = db.Countries.FirstOrDefault(m => m.Id == id);
            if (country is null) return View("Error");

            var model = new AdminMarketsPlaces_CreateMultipleStatesViewModel {
                CountryName = country.Name,
                Delimiters = new Dictionary<string, int>(),
                CountryId = country.Id
            };

            model.Delimiters.Add("Comma ','", 1);
            model.Delimiters.Add("Colon ';'", 2);
            model.Delimiters.Add("Space ' '", 3);
            model.Delimiters.Add("Newline '\n'", 4);

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateMultipleStates(AdminMarketsPlaces_CreateMultipleStatesViewModel model)
        {
            if (ModelState.IsValid)
            {
                char delimiter = '\0';

                switch (model.Delimiter)
                {
                    case 1:
                        {
                            delimiter = ',';
                            break;
                        }
                    case 2:
                        {
                            delimiter = ';';
                            break;
                        }
                    case 3:
                        {
                            delimiter = ' ';
                            break;
                        }
                    case 4:
                        {
                            delimiter = '\n';
                            break;
                        }
                }

                var states = model.States.Split(delimiter).Select(s => s.Trim()).ToArray();

                foreach (var state in states)
                {
                    if (db.States.FirstOrDefault(m => m.Name == state
                            && m.CountryId == model.CountryId) is null) 
                        db.States.Add(new State { CountryId = model.CountryId, Name = state });
                }

                db.SaveChanges();
                return RedirectToAction("CountryDetails", new { id = model.CountryId });
            }
            return RedirectToAction("CreateMultipleStates");
        }

        public ActionResult CreateMultipleCities(int? id)
        {
            if (id is null) return View("Error");

            var state = db.States.FirstOrDefault(m => m.Id == id);
            if (state is null) return View("Error");

            var model = new AdminMarketsPlaces_CreateMultipleCitiesViewModel {
                Delimiters = new Dictionary<string, int>(),
                StateId = state.Id,
                StateName = state.Name
            };

            model.Delimiters.Add("Comma ','", 1);
            model.Delimiters.Add("Colon ';'", 2);
            model.Delimiters.Add("Space ' '", 3);
            model.Delimiters.Add("Newline '\n'", 4);

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateMultipleCities(AdminMarketsPlaces_CreateMultipleCitiesViewModel model)
        {
            if (ModelState.IsValid)
            {
                char delimiter = '\0';

                switch (model.Delimiter)
                {
                    case 1:
                        {
                            delimiter = ',';
                            break;
                        }
                    case 2:
                        {
                            delimiter = ';';
                            break;
                        }
                    case 3:
                        {
                            delimiter = ' ';
                            break;
                        }
                    case 4:
                        {
                            delimiter = '\n';
                            break;
                        }
                }

                var cities = model.Cities.Split(delimiter).Select(s => s.Trim()).ToArray();

                foreach (var city in cities)
                {
                    if (db.Places.FirstOrDefault(m => m.Name == city 
                            && m.StateId == model.StateId) is null)
                        db.Places.Add(new Place { Name = city, StateId = model.StateId });
                }

                db.SaveChanges();
                return RedirectToAction("StateDetails", new { id = model.StateId });
            }
            return RedirectToAction("CreateMultipleCities", new { id = model.StateId });
        }

        public ActionResult StateDetails(int? id)
        {
            if (id is null) return View("Error");

            var state = db.States
                          .Include(m => m.Country)
                          .FirstOrDefault(m => m.Id == id);
            if (state is null) return View("Error");

            var citiesDict = new Dictionary<string, int>();

            foreach (var city in state.Places)
            {
                citiesDict.Add(city.Name, city.Id);
            }

            return View(new AdminMarketsPlaces_IndexStateViewModel {
                StateId = state.Id,
                StateName = state.Name,
                CountryId = state.CountryId,
                CountryName = state.Country.Name,
                Citites = citiesDict
            });
        }

        public ActionResult EditState(int? id)
        {
            if (id is null) return View("Error");

            var state = db.States
                .Include(m => m.Country)
                .FirstOrDefault(m => m.Id == id);

            if (state is null) return View("Error");

            var model = new AdminMarketsPlaces_EditStateViewModel {
                Id = state.Id,
                StateName = state.Name,
                CountryName = state.Country.Name
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditState(AdminMarketsPlaces_EditStateViewModel model)
        {
            if (ModelState.IsValid)
            {
                var state = db.States.FirstOrDefault(m => m.Id == model.Id);

                state.Name = model.StateName;
                db.Entry(state).State = EntityState.Modified;
                db.SaveChanges();

                return RedirectToAction("StateDetails", new { id = model.Id });
            }

            return View("Error");
        }

        public ActionResult DeleteState(int? id)
        {
            if (id is null) return View("Error");

            var state = db.States.FirstOrDefault(m => m.Id == id);
            if (state is null) return View("Error");

            return View(state);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteState(State model)
        {
            if (ModelState.IsValid)
            {
                var state = db.States.Include(m => m.Places).FirstOrDefault(m => m.Id == model.Id);
                
                var places = new List<Place>(state.Places);
                foreach (var city in places)
                {
                    DeleteCity(new Place { Id = city.Id, StateId = model.Id, Name = city.Name });
                }

                db.Entry(state).State = EntityState.Deleted;

                db.SaveChanges();
                return RedirectToAction("CountryDetails", new { id = model.CountryId });
            }

            return View(model);
        }

        public ActionResult CreateCity(int? id)
        {
            if (id is null) return View("Error");

            var state = db.States.FirstOrDefault(m => m.Id == id);
            if (state is null) return View("Error");

            var model = new AdminMarketsPlaces_CreateCityViewModel {
                StateId = state.Id,
                StateName = state.Name
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateCity(AdminMarketsPlaces_CreateCityViewModel model)
        {
            if (ModelState.IsValid)
            {
                db.Places.Add(new Place {
                    Name = model.Name,
                    StateId = model.StateId
                });

                db.SaveChanges();
                return RedirectToAction("StateDetails", new { id = model.StateId });
            }
            return View(model);            
        }

        public ActionResult CityDetails(int? id)
        {
            if (id is null) return View("Error");

            var city = db.Places
                .Include(m => m.State)
                .Include(m => m.Markets)
                .FirstOrDefault(m => m.Id == id);

            if (city is null) return View("Error");

            var model = new AdminMarketsPlaces_CityDetailsViewModel {
                CityId = city.Id,
                CityName = city.Name,
                StateId = city.StateId,
                StateName = city.State.Name,
                Markets = new Dictionary<string, int>()
            };

            foreach (var market in city.Markets)
            {
                model.Markets.Add(market.Name, market.Id);
            }

            return View(model);
        }

        public ActionResult EditCity(int? id)
        {
            if (id is null) return View("Error");

            var city = db.Places.FirstOrDefault(m => m.Id == id);
            if (city is null) return View("Error");

            return View(new AdminMarketsPlaces_EditCityViewModel {
                CityId = city.Id,
                CityName = city.Name,
                StateId = city.StateId
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditCity(AdminMarketsPlaces_EditCityViewModel model)
        {
            if (ModelState.IsValid)
            {
                var city = db.Places.FirstOrDefault(m => m.Id == model.CityId);
                city.Name = model.CityName;

                db.SaveChanges();
                return RedirectToAction("CityDetails", new { id = model.CityId });
            }
            return View(model);
        }

        public ActionResult DeleteCity(int? id)
        {
            if (id is null) return View("Error");

            var city = db.Places.FirstOrDefault(m => m.Id == id);
            if (city is null) return View("Error");

            return View(city);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteCity(Place model)
        {
            if (ModelState.IsValid)
            {
                var city = db.Places.Include(m => m.Markets).FirstOrDefault(m => m.Id == model.Id);

                var markets = new List<Market>(city.Markets);
                foreach (var market in markets)
                {
                    DeleteMarket(new Market { Id = market.Id, PlaceId = city.Id, Name = market.Name,
                        Latitude = market.Latitude, Longitude = market.Longitude });
                }

                db.Entry(city).State = EntityState.Deleted;

                db.SaveChanges();
                return RedirectToAction("StateDetails", new { id = city.StateId });
            }
            return View(model);
        }

        public ActionResult CreateMarket(int? id)
        {
            if (id is null) return View("Error");

            var city = db.Places.FirstOrDefault(m => m.Id == id);
            if (city is null) return View("Error");

            var model = new AdminMarketsPlaces_CreateMarketViewModel {
                CityId = city.Id,
                CityName = city.Name
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateMarket(AdminMarketsPlaces_CreateMarketViewModel model)
        {
            if (ModelState.IsValid)
            {
                db.Markets.Add(new Market {
                    PlaceId = model.CityId,
                    Name = model.Name,
                    Latitude = model.Latitude,
                    Longitude = model.Longitude
                });

                db.SaveChanges();
                return RedirectToAction("CityDetails", new { id = model.CityId });
            }

            return View(model);
        }

        public ActionResult CreateMultipleMarkets(int? id)
        {
            if (id is null) return View("Errors");

            var city = db.Places.FirstOrDefault(m => m.Id == id);
            if (city is null) return View("Errors");

            var model = new AdminMarketsPlaces_CreateMultipleMarketsViewModel {
                Delimiters = new Dictionary<string, int>(),
                PlaceId = city.Id,
                PlaceName = city.Name
            };

            model.Delimiters.Add("Comma ','", 1);
            model.Delimiters.Add("Colon ';'", 2);
            model.Delimiters.Add("Space ' '", 3);
            model.Delimiters.Add("Newline '\n'", 4);

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateMultipleMarkets(AdminMarketsPlaces_CreateMultipleMarketsViewModel model)
        {
            if (ModelState.IsValid)
            {
                char delimiter = '\0';

                switch (model.Delimiter)
                {
                    case 1:
                        {
                            delimiter = ',';
                            break;
                        }
                    case 2:
                        {
                            delimiter = ';';
                            break;
                        }
                    case 3:
                        {
                            delimiter = ' ';
                            break;
                        }
                    case 4:
                        {
                            delimiter = '\n';
                            break;
                        }
                }

                var markets = model.Markets.Split(delimiter).Select(s => s.Trim()).ToArray();

                foreach (var market in markets)
                {
                    if (db.Markets.FirstOrDefault(m => m.Name == market
                            && m.PlaceId == model.PlaceId) is null)
                        db.Markets.Add(new Market { Name = market, PlaceId = model.PlaceId });
                }

                db.SaveChanges();
                return RedirectToAction("CityDetails", new { id = model.PlaceId });
            }
            return RedirectToAction("CreateMultipleMarkets", new { id = model.PlaceId });
        }

        public ActionResult MarketDetails(int? id)
        {
            if (id is null) return View("Error");

            var market = db.Markets
                .Include(m => m.Place)
                .FirstOrDefault(m => m.Id == id);

            if (market is null) return View("Error");

            var model = new AdminMarketsPlaces_MarketDetailsViewModel {
                CityId = market.PlaceId,
                CityName = market.Place.Name,
                MarketId = market.Id,
                MarketName = market.Name,
                Latitude = market.Latitude,
                Longitude = market.Longitude
            };

            return View(model);
        }

        public ActionResult EditMarket(int? id)
        {
            if (id is null) return View("Error");

            var market = db.Markets.FirstOrDefault(m => m.Id == id);
            if (market is null) return View("Error");

            var model = new AdminMarketsPlaces_EditMarketViewModel {
                MarketId = market.Id,
                MarketName = market.Name,
                Latitude = market.Latitude,
                Longitude = market.Longitude
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditMarket(AdminMarketsPlaces_EditMarketViewModel model)
        {
            if (ModelState.IsValid)
            {
                var market = db.Markets.FirstOrDefault(m => m.Id == model.MarketId);
                market.Name = model.MarketName;
                market.Latitude = model.Latitude;
                market.Longitude = model.Longitude;

                db.Entry(market).State = EntityState.Modified;
                db.SaveChanges();

                return RedirectToAction("MarketDetails", new { id = model.MarketId });
            }
            return View(model);
        }

        public ActionResult DeleteMarket(int? id)
        {
            if (id is null) return View("Error");

            var market = db.Markets
                .Include(m => m.Place)
                .FirstOrDefault(m => m.Id == id);
            if (market is null) return View("Error");

            return View(market);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteMarket(Market model)
        {
            if (ModelState.IsValid)
            {
                var market = db.Markets.FirstOrDefault(m => m.Id == model.Id);
                if (market is null) return View("Error");

                try
                {
                    db.Entry(market).State = EntityState.Deleted;
                }
                catch (Exception ex)
                {

                }
                db.SaveChanges();

                return RedirectToAction("CityDetails", new { id = market.PlaceId });
            }
            return View(model);
        }
    }
}