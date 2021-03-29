using Microsoft.Azure.Cosmos.Table;
using System;
using System.Collections.Generic;
using System.Text;
using TwitchLib.Api.Helix.Models.ChannelPoints;

namespace twitch_alexa_skill.Entities
{
    public class RewardEntity : TableEntity
    {
        public RewardEntity() { }

        public RewardEntity(CustomReward reward, string alexaId)
        {
            alexa_id = alexaId;
            title = reward.Title;
            reward_id = reward.Id;
            cost = reward.Cost;
            twitch_id = reward.BroadcasterId;
            twitch_username = reward.BroadcasterName;
            prompt = reward.Prompt;
            user_input = reward.IsUserInputRequired;
            stream_max = reward.MaxPerStreamSetting.IsEnabled;
            stream_max_amount = reward.MaxPerStreamSetting.MaxPerStream;
            user_max = reward.MaxPerUserPerStreamSetting.IsEnabled;
            user_max_amount = reward.MaxPerUserPerStreamSetting.MaxPerStream;
            global_cooldown = reward.GlobalCooldownSetting.IsEnabled;
            global_cooldown_time = reward.GlobalCooldownSetting.GlobalCooldownSeconds;
            RowKey = reward.Id;
            PartitionKey = reward.BroadcasterId;
        }



        public string alexa_id { get; set; }
        public string twitch_id { get; set; }
        public string twitch_username { get; set; }
        public string reward_id { get; set; }
        public string title { get; set; }
        public int cost { get; set; }
        public string prompt { get; set; }
        public bool user_input { get; set; }
        public bool stream_max { get; set; }
        public int stream_max_amount { get; set; }
        public bool user_max { get; set; }
        public int user_max_amount { get; set; }
        public bool global_cooldown { get; set; }
        public int global_cooldown_time { get; set; }
    }

}
