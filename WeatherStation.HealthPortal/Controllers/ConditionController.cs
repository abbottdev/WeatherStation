using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using WeatherStation.HealthPortal.Models;
using WeatherStation.HealthPortal.Services;

namespace WeatherStation.HealthPortal.Controllers
{
    [RoutePrefix("conditions")]
    public class ConditionController : Controller
    {
        // GET: Condition
        public ActionResult Index()
        {
            return View();
        }

        public async Task<ActionResult> BuildGraphAsync()
        {

            await Services.GremlinService.CreateConditionGraphAsync();

            return RedirectToAction(nameof(Index));
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
        [Route("triggered-by/weather/{weatherCondition}")]
        public async Task<ActionResult> GetConditionsAffectedByWeatherCondition(string weatherCondition, double temperature)
        {
            List<WorsenedCondition> conditions = new List<WorsenedCondition>();

            //TODO: Use a graph Db to query all conditions which have specified weather as a symptom
            //Or cold weather as a symptom.

            var query = @"
                g.V()
                    .hasLabel('weather')
                    .has('name', '{0}')
                        .outE('worsened by')
                        .inV()
                        .hasLabel('condition')
                            .project('condition', 'suggestions')
                                .by('name')
                                .by(inE().values('suggestions'))
            ";


            var results = await GremlinService.ExecuteGremlinQueryAsync(string.Format(query, weatherCondition));

            if (results.Any())
            {
                foreach (var result in results)
                {
                    string suggestions = result.suggestions;
                    string condition = result.condition;

                    conditions.Add(new WorsenedCondition(condition, suggestions.Split(new string[] { "|" }, StringSplitOptions.RemoveEmptyEntries).AsEnumerable()));
                }
            }

            switch (weatherCondition)
            {
                case "snow":
                case "wind":
                case "ice":
                    return Json(new Condition() { Name = "Falls" });
                default:
                    break;
            }

            return Json(conditions);
        }

    }
}