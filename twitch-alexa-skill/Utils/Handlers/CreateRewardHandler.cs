using Alexa.NET;
using Alexa.NET.Request;
using Alexa.NET.RequestHandlers;
using Alexa.NET.Response;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Routing.Template;
using twitch_alexa_skill.Entities;
using twitch_alexa_skill.Services;
using twitch_alexa_skill.StateUtils;

namespace twitch_alexa_skill.Utils.Handlers
{
    public class CreateRewardHandler : IAlexaRequestHandler
    {

        public bool CanHandle(AlexaRequestInformation<SkillRequest> information)
        {
            var intentRequest = (Alexa.NET.Request.Type.IntentRequest)information.SkillRequest.Request;
            return intentRequest.Intent.Name == "CreateReward";
        }

        public async Task<SkillResponse> Handle(AlexaRequestInformation<SkillRequest> information)
        {
            var alexaId = information.SkillRequest.Context.System.User.UserId;
            var intentRequest = (Alexa.NET.Request.Type.IntentRequest)information.SkillRequest.Request;

            var twitchId = new string(StateCache.Cache[alexaId].UserContext.UserTwitchId);
            StateCache.Cache[alexaId].UserContext.State["LastIntent"] = "CreateReward";



            StateCache.Cache[alexaId].UserContext.State["Title"] = intentRequest.Intent.Slots["title"].Value;
            StateCache.Cache[alexaId].UserContext.State["Cost"] = intentRequest.Intent.Slots["cost"].Value;


            Reprompt rePrompt = new Reprompt("Manually approve yourself? Say yes to accept, no to reject, or cancel to do nothing");
            StateCache.Cache[alexaId].UserContext.State["Approval"] = string.Empty;
            return await ResponseBuilderWithState.Ask($"Got It, " + $"Would you like to approve the redemptions manually?" +
                                        $" Say yes to accept, no to approve automatically, or cancel to do nothing",
                                        rePrompt,
                                        information.SkillRequest.Session);
        }
    }
}