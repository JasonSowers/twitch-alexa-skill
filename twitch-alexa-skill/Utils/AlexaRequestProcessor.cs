using Alexa.NET;
using Alexa.NET.Request;
using Alexa.NET.RequestHandlers;
using Alexa.NET.RequestHandlers.Handlers;
using Alexa.NET.Response;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using twitch_alexa_skill.Entities;
using twitch_alexa_skill.Services;
using twitch_alexa_skill.StateUtils;
using twitch_alexa_skill.Utils.Handlers;

namespace twitch_alexa_skill.Utils
{
    public class AlexaRequestProcessor
    {
        public async Task<SkillResponse> ProcessAsync(SkillRequest input)
        {           

            await StateCache.CacheStateForUser(input);
            var requestHandlers = new List<IAlexaRequestHandler<SkillRequest>>()
            {
                new LaunchRequestIntentHandler(),
                new SessionEndedRequestIntentHandler(),
                new CancelIntentHandler(),
                new HelpIntentHandler(),
                new ManageRedemptionsHandler(),
                new CreateRewardHandler(),
                new DeleteRewardHandler(),
                new NoIntentHandler(),
                new YesIntentHandler(),
                new RewardIdeasHandler(),
                new FallbackIntentHandler(),
                new PromptMessageHandler()
            };

            var errorHandlers = new List<IAlexaErrorHandler<SkillRequest>>()
            {
                new ErrorHandler()
            };

            var request = new AlexaRequestPipeline(requestHandlers, errorHandlers);

            
            return await request.Process(input);

        }
    }
}
