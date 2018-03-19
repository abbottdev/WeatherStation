using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using WeatherStation.HealthPortal.Models;
using WeatherStation.HealthPortal.Models.Conditions;
using WeatherStation.HealthPortal.Services;

namespace WeatherStation.HealthPortal.Controllers
{
    [RoutePrefix("conditions")]
    public class ConditionController : Controller
    {
        // GET: Condition
        [Route("")]
        public async Task<ActionResult> Index()
        {
            List<Condition> conditions = new List<Condition>();

            var query = @"
                g.V()
                    .hasLabel('condition')
                            .project('name', 'id')
                                .by('name')
                                .by('id')
            ";

            var results = await GremlinService.ExecuteGremlinQueryAsync(query);

            if (results.Any())
            {
                foreach (var result in results)
                {
                    string id = result.id;
                    string condition = result.name;

                    conditions.Add(new Condition() { Id = id, Name = condition });
                }
            }

            return View(conditions);
        }

        public async Task<ActionResult> BuildGraphAsync()
        {

            await Services.GremlinService.CreateConditionGraphAsync();

            return RedirectToAction(nameof(Index));
        }

        [Route("add")]
        public ActionResult Add()
        {
            return View();
        }

        [Route("edit/{id}")]
        public async Task<ActionResult> Edit(string id)
        {
            var query = $"g.V('{id}').project('name', 'id').by('name').by('id')";

            var results = await GremlinService.ExecuteGremlinQueryAsync(query);

            if (results.Any())
            {
                foreach (var result in results)
                {
                    Condition condition = new Condition()
                    {
                        Name = result.name,
                        Id = result.id
                    };

                    return View(condition);
                }
            }

            return HttpNotFound();
        }

        [HttpPost]
        public ActionResult AddCondition(Models.Conditions.Condition condition)
        {
            return View(condition);
        }

        [HttpPost()]
        [Route("triggered-by/weather/{weatherCondition}")]
        public async Task<ActionResult> GetConditionsAffectedByWeatherCondition(string weatherCondition, double temperature)
        {

            List<ConditionComplication> conditions = await GetConditionComplicationsByWeatherAsync(weatherCondition);

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

        [HttpGet()]
        [Route("{conditionId}/details")]
        public async Task<ActionResult> Details(string conditionId)
        {

            var query = @"
                g.V('{0}')
                    .project('name', 'id', 'symptoms', 'complications')
                        .by('name')
                        .by('id')
                        .by(inE('symptom').outV().project('name', 'id').by('name').by('id').fold())
                        .by(
                            union(
                                outE('complicates').inV().project('name').by('name').fold(),
                                inE('complicates').outV().project('name').by('name').fold()
                            )
                        )
            ";

            query = string.Format(query, conditionId);

            var results = await GremlinService.ExecuteGremlinQueryAsync(string.Format(query, conditionId));

            if (results.Any())
            {
                foreach (var result in results)
                {
                    string name = result.name;
                    string id = result.id;

                    IEnumerable<string> complications = ((IEnumerable<dynamic>)result.complications).Select(wb => (string)wb.name);
                    IEnumerable<Symptom> symptoms = ((IEnumerable<dynamic>)result.symptoms).Select(wb => new Symptom() { Name = wb.name, Id = wb.id });

                    ConditionDetails details = new ConditionDetails()
                    {
                        ConditionName = name,
                        Symptoms = symptoms,
                        Complications = complications
                    };

                    if (Request.AcceptTypes.Contains("application/json"))
                    {
                        return Json(details, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return View(details);
                    }

                }
            }

            return HttpNotFound();
        }


        [Route("triggered-by/weather/{weatherCondition}")]
        public async Task<ActionResult> ConditionsAffectedByWeather(string weatherCondition, double? temperature = null)
        {
            List<ConditionComplication> complications = await GetConditionComplicationsByWeatherAsync(weatherCondition);

            return View(new ComplicatedByWeatherModel() { Weather = weatherCondition, Complications = complications });
        }

        private static async Task<List<ConditionComplication>> GetConditionComplicationsByWeatherAsync(string weatherCondition)
        {
            List<ConditionComplication> conditions = new List<ConditionComplication>();

            //TODO: Use a graph Db to query all conditions which have specified weather as a symptom
            //Or cold weather as a symptom.

            var query = @"
                g.V()
                    .hasLabel('weather')
                    .has('name', '{0}')
                        .outE('complicates')
                        .inV()
                        .hasLabel('condition')
                            .project('condition', 'id', 'suggestions')
                                .by('name')
                                .by('id')
                                .by(inE().values('suggestions'))
            ";


            var results = await GremlinService.ExecuteGremlinQueryAsync(string.Format(query, weatherCondition));

            if (results.Any())
            {
                foreach (var result in results)
                {
                    string suggestions = result.suggestions;
                    string condition = result.condition;

                    conditions.Add(new ConditionComplication((string)result.id, condition, suggestions.Split(new string[] { "|" }, StringSplitOptions.RemoveEmptyEntries).AsEnumerable()));
                }
            }

            return conditions;
        }
    }
}