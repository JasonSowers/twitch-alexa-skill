using Microsoft.Azure.Cosmos.Table;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using twitch_alexa_skill.Utils.Id.Configuration;
using witch_alexa_skill.Utils.Id;

namespace twitch_alexa_skill.Entities
{
    public class ErrorEntity : TableEntity
    {
        public ErrorEntity(object e)
        {
            var options = new GenerationOptions
            {
                UseNumbers = true,
                UseSpecialCharacters = false,
                Length = 12
            };
            error = JsonConvert.SerializeObject(e);
            RowKey = ShortId.Generate(options);
            PartitionKey = DateTime.Now.Month.ToString();
        }

        public string error { get; set; }

    }
}
