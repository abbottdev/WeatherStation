using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Linq;
using Microsoft.Azure.Graphs;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace WeatherStation.HealthPortal.Services
{
    public class GremlinService
    {
        private static string hostname = "https://healthweatherlinkdb.gremlin.cosmosdb.azure.com:443";
        private static int port = 443;
        private static string authKey = "sQKSFxRIQ9RdRVzqkNFGfYehzgeBtuEPd1FhjOtEHlqLJ5PCc51xvioGmdnbYBDXaZVH8rOy8h0C7crGbTQ9UA==";
        private static string database_name = "conditions";
        private static string collection = "conditionscol";

        private static Dictionary<string, string> gremlinQueries = new Dictionary<string, string>
{
        { "Cleanup",        "g.V().drop()" },
        { "AddCondition 1",    "g.addV('person').property('id', 'thomas').property('firstName', 'Thomas').property('age', 44)" },
        { "AddVertex 2",    "g.addV('person').property('id', 'mary').property('firstName', 'Mary').property('lastName', 'Andersen').property('age', 39)" },
        { "AddVertex 3",    "g.addV('person').property('id', 'ben').property('firstName', 'Ben').property('lastName', 'Miller')" },
        { "AddVertex 4",    "g.addV('person').property('id', 'robin').property('firstName', 'Robin').property('lastName', 'Wakefield')" },
        { "AddEdge 1",      "g.V('thomas').addE('knows').to(g.V('mary'))" },
        { "AddEdge 2",      "g.V('thomas').addE('knows').to(g.V('ben'))" },
        { "AddEdge 3",      "g.V('ben').addE('knows').to(g.V('robin'))" },
        { "UpdateVertex",   "g.V('thomas').property('age', 44)" },
        { "CountVertices",  "g.V().count()" },
        { "Filter Range",   "g.V().hasLabel('person').has('age', gt(40))" },
        { "Project",        "g.V().hasLabel('person').values('firstName')" },
        { "Sort",           "g.V().hasLabel('person').order().by('firstName', decr)" },
        { "Traverse",       "g.V('thomas').out('knows').hasLabel('person')" },
        { "Traverse 2x",    "g.V('thomas').out('knows').hasLabel('person').out('knows').hasLabel('person')" },
        { "Loop",           "g.V('thomas').repeat(out()).until(has('id', 'robin')).path()" },
        { "DropEdge",       "g.V('thomas').outE('knows').where(inV().has('id', 'mary')).drop()" },
        { "CountEdges",     "g.E().count()" },
        { "DropVertex",     "g.V('thomas').drop()" },
};

        public static async Task<IEnumerable<dynamic>> ExecuteGremlinQueryAsync(string gremlinQuery)
        {
            List<dynamic> results = new List<dynamic>();

            string endpoint = "https://healthweatherlinkdb.documents.azure.com:443/";

            using (DocumentClient client = new DocumentClient(
                new Uri(endpoint),
                authKey))
            {
                Database database = await client.CreateDatabaseIfNotExistsAsync(new Database { Id = database_name });

                Uri databaseUri = UriFactory.CreateDatabaseUri(database_name);

                DocumentCollection graph =
                    await client.CreateDocumentCollectionIfNotExistsAsync(
                        databaseUri, new DocumentCollection { Id = collection }, new RequestOptions { OfferThroughput = 400 });

                var query = client.CreateGremlinQuery<dynamic>(graph, gremlinQuery);

                while (query.HasMoreResults)
                {
                    foreach (dynamic result in await query.ExecuteNextAsync())
                    {
                        results.Add(result);
                    }
                }
            }

            return results;
        }

        public static async Task CreateConditionGraphAsync()
        {
            string endpoint = "https://healthweatherlinkdb.documents.azure.com:443/";

            using (DocumentClient client = new DocumentClient(
                new Uri(endpoint),
                authKey))
            {
                Database database = await client.CreateDatabaseIfNotExistsAsync(new Database { Id = database_name });

                Uri databaseUri = UriFactory.CreateDatabaseUri(database_name);

                DocumentCollection graph =
                    await client.CreateDocumentCollectionIfNotExistsAsync(
                        databaseUri, new DocumentCollection { Id = collection }, new RequestOptions { OfferThroughput = 400 });

                var query = client.CreateGremlinQuery<dynamic>(graph, gremlinQueries["Cleanup"]);

                while (query.HasMoreResults)
                {
                    foreach (dynamic result in await query.ExecuteNextAsync())
                    {
                        Console.WriteLine($"\t {JsonConvert.SerializeObject(result)}");
                    }
                }

                await AddWeatherConditionsAsync(client, graph);

                await AddCommonColdAsync(client, graph);

                await AddAsthmaAsync(client, graph);
                await AddDyspneaAsync(client, graph);

                await AddWorseningAffectsAsync(client, graph);

            }

        }

        private static async Task AddAsthmaAsync(DocumentClient client, DocumentCollection graph)
        {
            Guid asthma = Guid.NewGuid();

            await ExecuteGremlinQueryAsync(client, graph, "g" + CreateConditionWithGremlinQuery(asthma.ToString(), "Asthma"));

        }
        private static async Task AddDyspneaAsync(DocumentClient client, DocumentCollection graph)
        {
            Guid dyspnea = Guid.NewGuid();

            var query = "g" + CreateConditionWithGremlinQuery(dyspnea.ToString(), "Dyspnea");

            query += $".addE('complicates').to(g.V().hasLabel('condition').has('name', 'Asthma'))";
            query += ".property('suggestions', 'Increase medication dosage|Reduce phsyical activity').outV()";

            query += $".addE('symptom').to(g.V().hasLabel('condition').has('name', 'Asthma')).outV()";
            query += $".addE('symptom').to(g.V().hasLabel('condition').has('name', 'Common Cold')).outV()";

            await ExecuteGremlinQueryAsync(client, graph, query);

            //Asthma is made worse by a cold
            // command = "g.V().hasLabel('condition').has('name', 'Asthma').as('source').addE('complicates').to('source').from(g.V().hasLabel('condition').has('name', 'Common Cold'))";



        }

        private static async Task AddWeatherConditionsAsync(DocumentClient client, DocumentCollection graph)
        {
            await AddRainWeatherAsync(client, graph);
            await AddSnowWeatherAsync(client, graph);
            await AddColdWeatherAsync(client, graph);
        }

        private static async Task AddRainWeatherAsync(DocumentClient client, DocumentCollection graph)
        {
            var query = $"g.addV('weather').property('name', 'rain')";

            //query += $".property('weather code', '{WeatherStation.Core.Forecasts.WeatherCodes.drizzle_rain}')";
            //query += $".property('weather code', '{WeatherStation.Core.Forecasts.WeatherCodes.extreme_rain}')";
            //query += $".property('weather code', '{WeatherStation.Core.Forecasts.WeatherCodes.freezing_rain}')";
            //query += $".property('weather code', '{WeatherStation.Core.Forecasts.WeatherCodes.heavy_intensity_drizzle_rain}')";
            //query += $".property('weather code', '{WeatherStation.Core.Forecasts.WeatherCodes.heavy_intensity_rain}')";
            //query += $".property('weather code', '{WeatherStation.Core.Forecasts.WeatherCodes.heavy_intensity_shower_rain}')";
            //query += $".property('weather code', '{WeatherStation.Core.Forecasts.WeatherCodes.heavy_shower_rain_and_drizzle}')";
            //query += $".property('weather code', '{WeatherStation.Core.Forecasts.WeatherCodes.light_intensity_drizzle}')";
            //query += $".property('weather code', '{WeatherStation.Core.Forecasts.WeatherCodes.light_intensity_drizzle_rain}')";
            //query += $".property('weather code', '{WeatherStation.Core.Forecasts.WeatherCodes.light_intensity_shower_rain}')";
            //query += $".property('weather code', '{WeatherStation.Core.Forecasts.WeatherCodes.light_rain}')";
            //query += $".property('weather code', '{WeatherStation.Core.Forecasts.WeatherCodes.light_rain_and_snow}')";
            //query += $".property('weather code', '{WeatherStation.Core.Forecasts.WeatherCodes.moderate_rain}')";
            //query += $".property('weather code', '{WeatherStation.Core.Forecasts.WeatherCodes.ragged_shower_rain}')";
            //query += $".property('weather code', '{WeatherStation.Core.Forecasts.WeatherCodes.ragged_thunderstorm}')";
            //query += $".property('weather code', '{WeatherStation.Core.Forecasts.WeatherCodes.shower_drizzle}')";
            //query += $".property('weather code', '{WeatherStation.Core.Forecasts.WeatherCodes.shower_rain}')";
            //query += $".property('weather code', '{WeatherStation.Core.Forecasts.WeatherCodes.thunderstorm_with_heavy_rain}')";
            //query += $".property('weather code', '{WeatherStation.Core.Forecasts.WeatherCodes.thunderstorm_with_heavy_drizzle}')";
            //query += $".property('weather code', '{WeatherStation.Core.Forecasts.WeatherCodes.thunderstorm_with_rain}')";
            //query += $".property('weather code', '{WeatherStation.Core.Forecasts.WeatherCodes.thunderstorm_with_light_rain}')";
            //query += $".property('weather code', '{WeatherStation.Core.Forecasts.WeatherCodes.thunderstorm_with_light_drizzle}')";
            //query += $".property('weather code', '{WeatherStation.Core.Forecasts.WeatherCodes.very_heavy_rain}')";
            //query += $".property('weather code', '{WeatherStation.Core.Forecasts.WeatherCodes.shower_rain_and_drizzle}')";

            await ExecuteGremlinQueryAsync(client, graph, query);
        }

        private static async Task AddColdWeatherAsync(DocumentClient client, DocumentCollection graph)
        {
            var query = $"g.addV('weather').property('name', 'cold')";

            await ExecuteGremlinQueryAsync(client, graph, query);
        }

        private static async Task AddSnowWeatherAsync(DocumentClient client, DocumentCollection graph)
        {
            var query = $"g.addV('weather').property('name', 'snow')";

            //query += $".property('weather code', '{WeatherStation.Core.Forecasts.WeatherCodes.heavy_shower_snow}')";
            //query += $".property('weather code', '{WeatherStation.Core.Forecasts.WeatherCodes.heavy_snow}')";
            //query += $".property('weather code', '{WeatherStation.Core.Forecasts.WeatherCodes.light_rain_and_snow}')";
            //query += $".property('weather code', '{WeatherStation.Core.Forecasts.WeatherCodes.light_shower_snow}')";
            //query += $".property('weather code', '{WeatherStation.Core.Forecasts.WeatherCodes.light_snow}')";
            //query += $".property('weather code', '{WeatherStation.Core.Forecasts.WeatherCodes.rain_and_snow}')";
            //query += $".property('weather code', '{WeatherStation.Core.Forecasts.WeatherCodes.shower_snow}')";
            //query += $".property('weather code', '{WeatherStation.Core.Forecasts.WeatherCodes.snow}')";

            await ExecuteGremlinQueryAsync(client, graph, query);
        }

        private static async Task AddWorseningAffectsAsync(DocumentClient client, DocumentCollection graph)
        {
            //Colds are made worse by cold weather.
            var command = "g.V().hasLabel('condition').has('name', 'Common Cold').as('source').addE('complicates').to('source').from(g.V().hasLabel('weather').has('name', 'cold'))";

            command += ".property('suggestions', 'Reduce phsyical activity')";

            await ExecuteGremlinQueryAsync(client, graph, command);

            //Runny noses make colds worse
            command = "g.V().hasLabel('condition').has('name', 'Rhinorrhea').as('source').addE('complicates').to('source').from(g.V().hasLabel('weather').has('name', 'cold'))";

            //Suggestions to alleviate worsening.
            command += ".property('suggestions', 'Consider taking decongestants')";

            await ExecuteGremlinQueryAsync(client, graph, command);

            //Asthma is made worse by a cold
            command = "g.V().hasLabel('condition').has('name', 'Asthma').as('source').addE('complicates').to('source').from(g.V().hasLabel('condition').has('name', 'Common Cold'))";

            //Suggestions to alleviate worsening.
            command += ".property('suggestions', 'Increase dosage|Reduce phsyical activity')";

            await ExecuteGremlinQueryAsync(client, graph, command);

            //Asthma is made worse by the cold
            command = "g.V().hasLabel('condition').has('name', 'Asthma').as('source').addE('complicates').to('source').from(g.V().hasLabel('weather').has('name', 'cold'))";

            //Suggestions to alleviate worsening.
            command += ".property('suggestions', 'Increase dosage|Reduce phsyical activity')";

            await ExecuteGremlinQueryAsync(client, graph, command);
        }

        private static async Task AddCommonColdAsync(DocumentClient client, DocumentCollection graph)
        {
            string commonColdId = Guid.NewGuid().ToString();

            var gremlinCommand = "g" + CreateConditionWithGremlinQuery(commonColdId, "Common Cold");
            await ExecuteGremlinQueryAsync(client, graph, gremlinCommand);

            //Add symptoms of common cold.
            string runnyNoseId = Guid.NewGuid().ToString();
            gremlinCommand = "g" + CreateConditionWithGremlinQuery(runnyNoseId, "Rhinorrhea");

            await ExecuteGremlinQueryAsync(client, graph, gremlinCommand);

            //Link as a symptom
            gremlinCommand = "g" + LinkConditionAsSymptom(commonColdId, runnyNoseId);

            await ExecuteGremlinQueryAsync(client, graph, gremlinCommand);

            gremlinCommand = "g" + AddPropertyToSymptom(commonColdId, runnyNoseId, "common", "true");

            await ExecuteGremlinQueryAsync(client, graph, gremlinCommand);


            //gremlinCommand = "g.addV('condition').property('id', 'weather').property('name', 'weather')";

            //await ExecuteGremlinQueryAsync(client, graph, gremlinCommand);

            //gremlinCommand = "g" + LinkConditionAsSymptom(commonColdId, "weather");

            //await ExecuteGremlinQueryAsync(client, graph, gremlinCommand);

        }

        private static async Task ExecuteGremlinQueryAsync(DocumentClient client, DocumentCollection graph, string gremlinCommand)
        {
            IDocumentQuery<dynamic> query = client.CreateGremlinQuery<dynamic>(graph, gremlinCommand);
            while (query.HasMoreResults)
            {
                foreach (dynamic result in await query.ExecuteNextAsync())
                {
                    Console.WriteLine($"\t {JsonConvert.SerializeObject(result)}");
                }
            }

        }

        private static string LinkConditionAsSymptom(string condition, string symptom)
        {
            return $".V('{symptom}').as('symptomcondition').V('{condition}').addE('symptom').from('symptomcondition')";
        }

        private static string AddPropertyToSymptom(string condition, string symptom, string propertyName, string propertyValue)
        {
            return $".V('{condition}').outE('symptom').as('e').inV().has('id', '{symptom}').select('e').property('{propertyName}', '{propertyValue}')";
        }


        private static string CreateConditionWithGremlinQuery(string id, string condition)
        {
            return $".addV('condition').property('id', '{id}').property('name', '{condition}')";
        }
    }
}