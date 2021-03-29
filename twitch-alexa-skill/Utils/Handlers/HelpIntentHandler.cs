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
    public class HelpIntentHandler : IAlexaRequestHandler
    {
        public bool CanHandle(AlexaRequestInformation<SkillRequest> information)
        {
            var intentRequest = (IntentRequest)information.SkillRequest.Request;
            return intentRequest.Intent.Name == "AMAZON.HelpIntent";
        }

        public async Task<SkillResponse> Handle(AlexaRequestInformation<SkillRequest> information)
        {
            var alexaId = information.SkillRequest.Session.User.UserId;
            StateCache.Cache[alexaId].UserContext.State["LastIntent"] = "AMAZON.HelpIntent";
            Session session = information.SkillRequest.Session;
            var speech = "Thanks for coming by, I can tell we're going to be good friends.  I'm here to help you manage all things channel points on twitch.  I can help you create rewards, delete rewards, and approve and reject redemptions. " +
                         "You can say things like, Alexa open channel points, to get the skill started. If the skill is open you can say things like," +
                         " I want to create a reward, or even, delete a reward. I can only delete rewards I created. so keep that in mind." +
                         "  You can also say manage my redemptions to approve or reject redemptions that are in your queue.   Again, This will only work for rewards I created." +
                         " I can also provide some suggestions for rewards if you are interested just say,  suggest reward ideas. " +
                         "One last thing I want you to know, if the skill isn't open already you can just say. Alexa tell channel points I want to manage my redemptions.  This will work for all my commands." +
                         " So what would you like to get started with?";
            Reprompt rp = new Reprompt("Lets get started, What would you like to do first?");
           return await ResponseBuilderWithState.Ask(speech, rp, session);
        }
    }
}