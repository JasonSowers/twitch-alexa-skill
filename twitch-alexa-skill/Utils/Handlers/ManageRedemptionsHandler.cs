using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Alexa.NET.RequestHandlers.Handlers;
using Alexa.NET.Request;
using Alexa.NET;
using Alexa.NET.Request.Type;
using Alexa.NET.RequestHandlers;
using Alexa.NET.Response;
using Alexa.NET.Profile;
using twitch_alexa_skill.Utils;
using twitch_alexa_skill.StateUtils;
using Alexa.NET.Conversations;
using Microsoft.AspNetCore.Http.Features;
using Newtonsoft.Json.Linq;
using twitch_alexa_skill.Entities;
using twitch_alexa_skill.Services;

namespace twitch_alexa_skill.Utils.Handlers
{
    public class ManageRedemptionsHandler : IAlexaRequestHandler
    {

        public bool CanHandle(AlexaRequestInformation<SkillRequest> information)
        {
            var intentRequest = (Alexa.NET.Request.Type.IntentRequest)information.SkillRequest.Request;
            return intentRequest.Intent.Name == "ManageRedemptions";
        }

        public async Task<SkillResponse> Handle(AlexaRequestInformation<SkillRequest> information)
        {

            var alexaId = information.SkillRequest.Context.System.User.UserId;
            var intentRequest = (Alexa.NET.Request.Type.IntentRequest)information.SkillRequest.Request;

            var hasLastIntent = StateCache.Cache[alexaId].UserContext.State.TryGetValue("LastIntent", out object lastIntentValue);
            var lastIntentName = new string(lastIntentValue?.ToString());
            StateCache.Cache[alexaId].UserContext.State["LastIntent"] = "ManageRedemptions";

            if (!StateCache.Cache[alexaId].UserContext.State.TryGetValue("Redemptions", out object redemptions1) || redemptions1.GetType() == typeof(JArray))
            {
                StateCache.Cache[alexaId].UserContext.State.Remove("CurrentRedemption", out object value);
                var rePrompt = new Reprompt($"What else would you like to do?");
               return await ResponseBuilderWithState.Ask($"You do not have any redemptions I can manage for you at this time. In order for me to manage redemptions for you I must have created the reward the redemption is for." +
                                                         $" To create a reward, just say, create a reward. If you need some ideas for rewards, just say i need reward ideas, and I'll hook you up", rePrompt, information.SkillRequest.Session);
            }


            var redemptions = (List<RedemptionEntity>)StateCache.Cache[alexaId].UserContext.State["Redemptions"];

            Slot allOrManyValue;

            if (intentRequest.Intent.Slots.TryGetValue("allorsingle", out allOrManyValue) && allOrManyValue != null)
            {
                var slot = intentRequest.Intent.Slots["allorsingle"];
                var single = "1";
                var one = "one";
                var all = "all";

                if (!slot.Value.ToLowerInvariant().Contains(single) && !slot.Value.ToLowerInvariant().Contains(one) &&
                    !slot.Value.ToLowerInvariant().Contains(all))
                {
                    var rePrompt = new Reprompt($"Ok I swear I'll pay attention this time. What can I do for you,");
                    return await ResponseBuilderWithState.Ask($"So sometimes I do this thing where you talk, and I don't listen. So you're going to have to start over, my friend.", rePrompt, information.SkillRequest.Session);

                }

                if (slot.Value.ToLowerInvariant().Contains(single) || slot.Value.ToLowerInvariant().Contains(one))
                {
                    StateCache.Cache[alexaId].UserContext.State["Manage"] = "single";
                    StateCache.Cache[alexaId].UserContext.State["CurrentRedemption"] = redemptions.FirstOrDefault();

                    var redemptionName = redemptions.FirstOrDefault().redemption_name;
                    var userName = redemptions.FirstOrDefault().twitch_username;

                    var userInputMessage = redemptions.FirstOrDefault().user_input;
                    if (userInputMessage != string.Empty)
                    {
                        userInputMessage = $"{userName} sent the message, {userInputMessage}, when they redeemed the reward";
                    }
                    var rePrompt = new Reprompt($"Accept redemption from {userName} for {redemptionName}");
                    return await ResponseBuilderWithState.Ask($"{userName} would like to redeem {redemptionName}. {userInputMessage},.  Do you want to accept this? Say yes to accept, no to reject, or cancel to do nothing", rePrompt, information.SkillRequest.Session);
                }

                if (slot.Value.ToLowerInvariant().Contains(all))
                {
                    StateCache.Cache[alexaId].UserContext.State["Manage"] = "all";
                    var rePrompt = new Reprompt("Accept all redemptions?");                    
                    return await ResponseBuilderWithState.Ask("Would you like to Accept all of them? Say yes to accept, no to reject, or cancel to do nothing", rePrompt, information.SkillRequest.Session);
                }
            }
            return await ResponseBuilderWithState.Tell("Sorry, I'm just a dumb bot and my coders are complete failures.  You are going to need to start over", information.SkillRequest.Session);
        }
    }
}