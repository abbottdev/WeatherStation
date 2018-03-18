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
        private static string database = "conditions";
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

        public static async Task CreateConditionGraphAsync()
        {
            string endpoint = "https://healthweatherlinkdb.documents.azure.com:443/";

            using (DocumentClient client = new DocumentClient(
                new Uri(endpoint),
                authKey))
            {
                Database database = await client.CreateDatabaseIfNotExistsAsync(new Database { Id = "conditions" });

                Uri databaseUri = UriFactory.CreateDatabaseUri("conditions");

                DocumentCollection graph =
                    await client.CreateDocumentCollectionIfNotExistsAsync(
                        databaseUri, new DocumentCollection { Id = "conditionscol" }, new RequestOptions { OfferThroughput = 400 });

                var query = client.CreateGremlinQuery<dynamic>(graph, gremlinQueries["Cleanup"]);

                while (query.HasMoreResults)
                {
                    foreach (dynamic result in await query.ExecuteNextAsync())
                    {
                        Console.WriteLine($"\t {JsonConvert.SerializeObject(result)}");
                    }
                }

                await AddCommonColdAsync(client, graph);

            }


        }

        private static async Task AddCommonColdAsync(DocumentClient client, DocumentCollection graph)
        {
            string commonColdId = Guid.NewGuid().ToString();

            var gremlinCommand = "g" + CreateConditionWithGremlinQuery(commonColdId, "Common Cold");
            await ExecuteGremlinQueryAsync(client, graph, gremlinCommand);
            
            //Add symptoms of common cold.
            string runnyNoseId = Guid.NewGuid().ToString();
            gremlinCommand = "g" + CreateConditionWithGremlinQuery(runnyNoseId, "Runny Nose");

            await ExecuteGremlinQueryAsync(client, graph, gremlinCommand);

            //Link as a symptom
            gremlinCommand = "g" + LinkConditionAsSymptom(commonColdId, runnyNoseId);

            await ExecuteGremlinQueryAsync(client, graph, gremlinCommand);


            gremlinCommand = "g" + AddPropertyToSymptom(commonColdId, runnyNoseId, "common", "true");

            await ExecuteGremlinQueryAsync(client, graph, gremlinCommand);


            gremlinCommand = "g.addV('condition').property('id', 'weather').property('name', 'weather')";

            await ExecuteGremlinQueryAsync(client, graph, gremlinCommand);

            gremlinCommand = "g" + LinkConditionAsSymptom(commonColdId, "weather");
            
            await ExecuteGremlinQueryAsync(client, graph, gremlinCommand);
            
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