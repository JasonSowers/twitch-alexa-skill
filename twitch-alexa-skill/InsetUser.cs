using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace twitch_alexa_skill
{
    public class InsertUser
    {
        [FunctionName("InsertUser")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get","post", Route = null)] HttpRequest req,
            ILogger log)
        {

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();            
            var queryData = req.GetQueryParameterDictionary();
            var data = new List<Value>();
            var val1 = new Value();
            val1.name = "John Smith";
            val1.zip = "111111";
            val1.social = "1111";
            data.Add(val1);

            var val2 = new Value();
            val2.name = "Julie Jones";
            val2.zip = "222222";
            val2.social = "2222";
            data.Add(val2);

            var val3 = new Value();
            val3.name = "Scott Awesome";
            val3.zip = "333333";
            val3.social = "3333";
            data.Add(val3);



            var match = data.Where(d => d.name == queryData["name"] && d.zip == queryData["zip"] && d.social == queryData["social"]).Select(e => e).ToList().Any();



            if (match) { 
                var responseObject = new { isMatch = match };
                return new OkObjectResult(responseObject);
            }

            var r = new OkObjectResult("fail");
            r.StatusCode = 500;
            return r;


            //return new OkObjectResult(phone);
        }
    }


    public class Data
    {
        public Value[] value { get; set; }
    }

    public class Value
    {
        public string PartitionKey { get; set; }
        public string RowKey { get; set; }
        public DateTime Timestamp { get; set; }
        public string name { get; set; }
        public string zip { get; set; }
        public string social { get; set; }
    }

}
