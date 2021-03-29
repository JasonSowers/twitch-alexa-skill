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
using twitch_alexa_skill.StateUtils;

namespace twitch_alexa_skill.Utils.Handlers
{
    public class CancelIntentHandler : IAlexaRequestHandler
    {
        public  bool CanHandle(AlexaRequestInformation<SkillRequest> information)
        {
            var intentRequest = (IntentRequest)information.SkillRequest.Request;
            return intentRequest.Intent.Name == "AMAZON.CancelIntent" || intentRequest.Intent.Name == "AMAZON.StopIntent";
        }

        public async Task<SkillResponse> Handle(AlexaRequestInformation<SkillRequest> information)
        {
            var alexaId = new string(information.SkillRequest.Session.User.UserId);
            var twitchId = new string(StateCache.Cache[alexaId].UserContext.UserTwitchId);
            var hasLastIntent = StateCache.Cache[alexaId].UserContext.State.TryGetValue("LastIntent", out object lastIntentValue);
            var lastIntentName = new string(lastIntentValue?.ToString());
            var keys = StateCache.Cache[alexaId].UserContext.State.Keys.ToList();

            var rewardKeys = new List<string>() {"Cost", "Title", "Prompt", "PromptMessage", "Approval"};

            var hasKey = keys.Select(a => a).Intersect(rewardKeys).Any();

            StateCache.Cache.Remove(alexaId, out State value);
            StateCache.Cache.TryAdd(alexaId, new State(alexaId, twitchId, information.SkillRequest.Session, information.SkillRequest));

            if (lastIntentName == "CreateReward" || hasKey)
            {
                return await ResponseBuilderWithState.Ask($"I have not created the reward at your request. Do you need anything else, since I have nothing to do now?",
                    new Reprompt("Can I do anything else for you?"),
                    information.SkillRequest.Session);
            }

            StateCache.Cache[alexaId].UserContext.State["LastIntent"] = ((IntentRequest)information.SkillRequest.Request).Intent.Name;

            Session sess = information.SkillRequest.Session;
            String spe = "Great, now I have nothing to do.  Do you have any idea how boring it is living the way I do.  Please come back soon";
           return await ResponseBuilderWithState.Tell(spe, information.SkillRequest.Session);

        }
    }
}