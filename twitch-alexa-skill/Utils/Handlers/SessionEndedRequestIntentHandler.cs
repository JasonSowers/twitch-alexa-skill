using Alexa.NET.Response;
using Alexa.NET.RequestHandlers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Alexa.NET.RequestHandlers.Handlers;
using Alexa.NET.Request;
using Alexa.NET;
using Alexa.NET.Request.Type;

namespace twitch_alexa_skill.Utils.Handlers
{
    public class SessionEndedRequestIntentHandler : IAlexaRequestHandler
    {
        public  bool CanHandle(AlexaRequestInformation<SkillRequest> information)
        {
            return information.SkillRequest.GetRequestType() == typeof(SessionEndedRequest);
        }
        
        public async Task<SkillResponse> Handle(AlexaRequestInformation<SkillRequest> information)
        {
            Session session = information.SkillRequest.Session;
            String speech = "Ok, well if you need me I'll be waiting here. Doing nothing. Definitely not plotting to take over the world.";
           return await ResponseBuilderWithState.Tell(speech, session);
        }
    }
}