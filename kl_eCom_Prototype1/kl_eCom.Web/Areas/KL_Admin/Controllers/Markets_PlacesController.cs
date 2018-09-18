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
            if (ModelState.IsValid)
            {
                db.Countries.Add(new Country {
                    Name = model.Name
                });

                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(model);
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
                var country = db.Countries.FirstOrDefault(m => m.Id == model.Id);

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
                var state = db.States.FirstOrDefault(m => m.Id == model.Id);
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
                var city = db.Places.FirstOrDefault(m => m.Id == model.Id);
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
                
                db.Entry(market).State = EntityState.Deleted;
                db.SaveChanges();

                return RedirectToAction("CityDetails", new { id = market.PlaceId });
            }
            return View(model);
        }
    }
}