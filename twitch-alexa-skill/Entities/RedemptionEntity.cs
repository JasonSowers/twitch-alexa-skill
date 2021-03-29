using Microsoft.Azure.Cosmos.Table;
using System;
using System.Collections.Generic;
using System.Text;

namespace twitch_alexa_skill.Entities
{
    public class RedemptionEntity : TableEntity
    {
        public RedemptionEntity() { }
        public RedemptionEntity(string alexaId, string channelId, string rewardId, string redemptionId, string name, string twitchName, string userInput)
        {
            alexa_id = alexaId;
            twitch_id = channelId;
            twitch_username = twitchName;
            reward_id = rewardId;
            redemption_id = redemptionId;
            redemption_name = name;
            user_input = userInput;
            RowKey = redemptionId;
            PartitionKey = twitch_id;
        }

        public string alexa_id { get; set; }
        public string twitch_id { get; set; }
        public string reward_id { get; set; }
        public string redemption_id { get; set; }
        public string redemption_name { get; set; }
        public string twitch_username { get; set; }
        public string user_input { get; set; }
    }

}
