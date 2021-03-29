using Alexa.NET;
using Alexa.NET.Request;
using Alexa.NET.RequestHandlers;
using Alexa.NET.RequestHandlers.Handlers;
using Alexa.NET.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using twitch_alexa_skill.StateUtils;

namespace twitch_alexa_skill.Utils
{
    public class ErrorHandler : IAlexaErrorHandler
    {
        public bool CanHandle(AlexaRequestInformation<SkillRequest> information, Exception exception)
        {
            return true;
        }

        public async Task<SkillResponse> Handle(AlexaRequestInformation<SkillRequest> information, Exception exception)
        {
            var alexaId = new string(information.SkillRequest.Session.User.UserId);
            var twitchId = new string(StateCache.Cache[alexaId].UserContext.UserTwitchId);

            StateCache.Cache.Remove(alexaId, out State value);
            StateCache.Cache.TryAdd(alexaId, new State(alexaId, twitchId, information.SkillRequest.Session, information.SkillRequest));
            Session session = information.SkillRequest.Session;
            string speech = "You know tha thing I do where I completely mess up everything and make you start over?  Well, right now, I just did that.";
            
           return await ResponseBuilderWithState.Tell(speech, session);
        }
    }
}
