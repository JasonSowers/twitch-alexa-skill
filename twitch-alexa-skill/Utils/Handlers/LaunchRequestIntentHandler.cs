using Alexa.NET.Response;
using Alexa.NET.RequestHandlers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Alexa.NET.RequestHandlers.Handlers;
using Alexa.NET.Request;
using Alexa.NET;
using twitch_alexa_skill.Services;
using Alexa.NET.Response.Ssml;
using twitch_alexa_skill.StateUtils;

namespace twitch_alexa_skill.Utils.Handlers
{
    public class LaunchRequestIntentHandler : LaunchRequestHandler
    {

        public override  async  Task<SkillResponse> Handle(AlexaRequestInformation<SkillRequest> information)
        {
            var alexaId = information.SkillRequest.Session.User.UserId;

            StateCache.Cache[alexaId].UserContext.State["LastIntent"] = "LaunchRequest";
            Session session = information.SkillRequest.Session;
            Context context = information.SkillRequest.Context;
            var speech = "Hi there welcome to the channel points skill. how can I help you today?";

            var reprompt = new Reprompt("If you are new to the skill say help and find out what I can do. I can add custom rewards, delete custom reward, and will send you notifications to approve channel point redemptions");
           return await ResponseBuilderWithState.Ask(speech, reprompt, session);
        }
    }
}
 