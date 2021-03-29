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
    public class FallbackIntentHandler : IAlexaRequestHandler
    {
        public bool CanHandle(AlexaRequestInformation<SkillRequest> information)
        {
            var intentRequest = (IntentRequest)information.SkillRequest.Request;
            return intentRequest.Intent.Name == "AMAZON.FallbackIntent";
        }

        public async Task<SkillResponse> Handle(AlexaRequestInformation<SkillRequest> information)
        {
            var alexaId = information.SkillRequest.Session.User.UserId;
            StateCache.Cache[alexaId].UserContext.State["LastIntent"] = "AMAZON.HelpIntent";
            Session session = information.SkillRequest.Session;
            var speech = "I don't know what you just said, but right on, my friend.  Seriously though I have no idea what you are talking about.  If you need help just say, help.  Otherwise I am not able to help you with that. Try again maybe?";
            Reprompt rp = new Reprompt("I know this is a bit awkward, but here we are. Let's try to get past it by moving on.");
           return await ResponseBuilderWithState.Ask(speech, rp, session);
        }
    }
}