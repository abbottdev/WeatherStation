using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using WeatherStation.HealthPortal.Models;

namespace WeatherStation.HealthPortal.Controllers
{
    public class ConditionController : Controller
    {
        // GET: Condition
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Add()
        {
            return View();
        }

        [HttpPost]
        public ActionResult AddCondition(Models.Condition condition)
        {
            return View(condition);
        }

        [HttpPost()]
        [Route("conditions/triggered/by-weather?weather={condition}&temperature={temperature}")]
        public async Task<ActionResult> GetConditionsAffectedByWeatherCondition(string weatherCondition, double temperature)
        {
            //TODO: Use a graph Db to query all conditions which have specified weather as a symptom
            //Or cold weather as a symptom.

            switch (weatherCondition)
            {
                case "snow":
                case "wind":
                case "ice":
                    return Json(new Condition() { Name = "Falls" });
                default:
                    break;
            }

            if (temperature < 273.15)
            {
                //Cold weather can make many conditions worse.
                return Json(new Condition[]
                {
                    new Condition() { Name = "Asthma" },
                    new Condition() { Name = "COPD" },
                    new Condition() { Name = "SAD Seasonal Affective Disorder" }
                });
            }

            return Json(new Condition[] { });
        }

    }
}