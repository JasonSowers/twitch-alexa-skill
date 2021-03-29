using Microsoft.Azure.Cosmos.Table;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using twitch_alexa_skill.StateUtils;
using twitch_alexa_skill.Utils.Id.Configuration;
using witch_alexa_skill.Utils.Id;

namespace twitch_alexa_skill.Entities
{
    public class UserEntity : TableEntity
    {
        public UserEntity() { }

        public UserEntity(string alexaId, string channelId, string accessToken, string twitch_name) 
        {
            twitch_usename = twitch_name;
            twitch_id = channelId;
            alexa_id = alexaId;
            access_token = accessToken;
            created = DateTime.Now.Ticks;
            RowKey = alexa_id;
            PartitionKey = channelId;            
        }

        public string twitch_usename { get; set; }
        public string twitch_id { get; set; } 
        public string alexa_id { get; set; }
        public string access_token { get; set; }
        public long created { get; set; }


    }

}
