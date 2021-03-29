using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using twitch_alexa_skill.Utils;
using twitch_alexa_skill.Entities;
using twitch_alexa_skill.Services;

namespace twitch_alexa_skill
{
    public class AuthCallback
    {
        [FunctionName("AuthCallback")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var redemption = JsonConvert.DeserializeObject<TwitchRedemptionMessage>(requestBody);
            //var twitchId = redemption.channel_id;
            //var user =  await Tables.GetUserByTwitchId(twitchId);
            //var redemptionEntity = new RedemptionEntity(user.alexa_id, redemption.channel_id, redemption.reward.id, redemption.id, redemption.reward.title, redemption.user.display_name);
            //var inserted = await Tables.InsertRedemptionAsync(redemptionEntity);
            //if (inserted)
            //{ return new OkObjectResult(redemptionEntity); }
            //else
            //{
            //    var response = new OkObjectResult(new {Message = "An error occured inserting the redemption", RedemptionData = redemptionEntity, Request = req });
            //    response.StatusCode = 500;
            //    return response;
            //}


            return null;

        }
    }
    public class TwitchRedemptionMessage
    {
        public string id { get; set; }
        public User user { get; set; }
        public string channel_id { get; set; }
        public Reward reward { get; set; }
        public string user_input { get; set; }
        public string status { get; set; }
    }

    public class User
    {
        public string id { get; set; }
        public string login { get; set; }
        public string display_name { get; set; }
    }

    public class Reward
    {
        public string id { get; set; }
        public string channel_id { get; set; }
        public string title { get; set; }
        public string prompt { get; set; }
        public int cost { get; set; }
        public bool is_user_input_required { get; set; }
        public bool is_sub_only { get; set; }
        public string background_color { get; set; }
        public bool is_enabled { get; set; }
        public bool is_paused { get; set; }
        public bool is_in_stock { get; set; }
        public bool should_redemptions_skip_request_queue { get; set; }
        public object template_id { get; set; }
        public string updated_for_indicator_at { get; set; }
        public object redemptions_redeemed_current_stream { get; set; }
        public object cooldown_expires_at { get; set; }
    }
}
