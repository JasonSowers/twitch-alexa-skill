using System;
using Alexa.NET;
using Alexa.NET.Request;
using Alexa.NET.RequestHandlers;
using Alexa.NET.Response;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using twitch_alexa_skill.Entities;
using twitch_alexa_skill.Services;
using twitch_alexa_skill.StateUtils;

namespace twitch_alexa_skill.Utils.Handlers
{
    public class PromptMessageHandler : IAlexaRequestHandler
    {

        public bool CanHandle(AlexaRequestInformation<SkillRequest> information)
        {
            var intentRequest = (Alexa.NET.Request.Type.IntentRequest)information.SkillRequest.Request;
            return intentRequest.Intent.Name == "PromptMessage";
        }

        public async Task<SkillResponse> Handle(AlexaRequestInformation<SkillRequest> information)
        {
            var intentRequest = (Alexa.NET.Request.Type.IntentRequest)information.SkillRequest.Request;
            var alexaId = information.SkillRequest.Session.User.UserId;
            var accessToken = information.SkillRequest.Session.User.AccessToken;
            var title = (string)StateCache.Cache[alexaId].UserContext.State["Title"];
            var cost = Convert.ToInt32((string)StateCache.Cache[alexaId].UserContext.State["Cost"]);
            var promptMessage = intentRequest.Intent.Slots["message"].Value;



            bool needsApproval = StateCache.Cache[alexaId].UserContext.State["Approval"].ToString() != "yes";
 

            var twichReward = await Twitch.CreateCustomReward(accessToken,
                StateCache.Cache[alexaId].UserContext.UserTwitchId,
                title,
                cost,
                needsApproval,
                true,
                promptMessage);

            await Tables.InsertRewardsAsync(new List<RewardEntity>() { new RewardEntity(twichReward.FirstOrDefault(), alexaId) });
            StateCache.Cache[alexaId].UserContext.State.Remove("Cost", out object value);
            StateCache.Cache[alexaId].UserContext.State.Remove("Title", out object value2);
            StateCache.Cache[alexaId].UserContext.State.Remove("Approval", out object value3);
            StateCache.Cache[alexaId].UserContext.State.Remove("UserInput", out object value4);
            StateCache.Cache[alexaId].UserContext.State.Remove("CustomMessage", out object value5);
            StateCache.Cache[alexaId].UserContext.State.Remove("UserInput", out object value6);
            StateCache.Cache[alexaId].UserContext.State.Remove("PromptMessage", out object value7);


            StateCache.Cache[alexaId].UserContext.State["LastIntent"] = "PromptMessage";

            return await ResponseBuilderWithState.Ask($"I saved your rewards with the title {title} and a cost of {cost} channel points." +
                                                      $" The user will see the message {promptMessage} when they redeem this reward. " +
                                                      $" You will now get notifications for redemptions if you have granted me permission to notify you in the " +
                                                      $"alexa app on your phone.  What else can i help you with?",
                new Reprompt("Can I do anything else for you?"),
                information.SkillRequest.Session);
        }
    }
}