using Alexa.NET.ProactiveEvents;
using Alexa.NET.ProactiveEvents.MessageReminders;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using twitch_alexa_skill.Entities;
using twitch_alexa_skill.Services;
using twitch_alexa_skill.Utils;

namespace twitch_alexa_skill
{
    public class ProactiveMessage
    {
        [FunctionName("ProactiveMessage")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {

            //return new OkObjectResult(StringMatch.GetScore("super awesome reward", new string[] {"awesome"}));

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var redemption = JsonConvert.DeserializeObject<TwitchRedemptionMessage>(requestBody);

            var twitchId = redemption.twitch_id;
            var user = await Tables.GetUserByTwitchId(twitchId);


            var client = new AccessTokenClient(AccessTokenClient.ApiDomainBaseAddress);
            var tokenResponse = await client.Send(Environment.GetEnvironmentVariable("ALEXA_CLIENT_ID"), Environment.GetEnvironmentVariable("ALEXA_CLIENT_SECRET"));
            var token = tokenResponse.Token;

            var proactiveMessage = new MessageReminder();
            proactiveMessage.Payload = new MessageReminderPayload()
            {
                State = new MessageReminderState(MessageReminderStatus.Unread, MessageReminderFreshness.New),
                MessageGroup = new MessageReminderGroup(redemption.viewer_name, 1, MessageReminderUrgency.Urgent)                
            };           

            var request = new UserEventRequest(user.alexa_id, proactiveMessage)
            {
                ExpiryTime = DateTimeOffset.UtcNow.AddMinutes(10),
                ReferenceId = Guid.NewGuid().ToString("N"),
                TimeStamp = DateTimeOffset.UtcNow
            };

            var client2 = new ProactiveEventsClient(ProactiveEventsClient.NorthAmericaEndpoint, token, new HttpClient(), true);           
            var res =  await client2.Send(request);


            if ((int)res.StatusCode >= 400) 
            {
                var responseObject = new
                {
                    Message = "An error occured sending the notification to the user. The notification was not sent to the user.",
                    ProactiveMessage = proactiveMessage,
                    Request = req,
                    NotificationRequest = request,
                    NotificationResponse = res
                };
                var response = new OkObjectResult(responseObject);
                await Tables.InsertErrorAsync(new ErrorEntity(responseObject));
                response.StatusCode = 500;
                return response;
            }
            return new OkObjectResult(new 
            { 
                Message = "Notification was sent",                
                ProactiveMessage = proactiveMessage,
            });
        }

        public class TwitchRedemptionMessage
        {
            public string twitch_id { get; set; }
            public string viewer_name { get; set; }
        }
    }

}
