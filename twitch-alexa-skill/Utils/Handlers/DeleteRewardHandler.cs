using System;
using System.Collections.Generic;
using System.Linq;
using Alexa.NET;
using Alexa.NET.Request;
using Alexa.NET.RequestHandlers;
using Alexa.NET.Response;
using System.Threading.Tasks;
using Microsoft.ApplicationInsights.WindowsServer;
using twitch_alexa_skill.Entities;
using twitch_alexa_skill.Services;
using twitch_alexa_skill.StateUtils;
using TwitchLib.Api.Core.Enums;

namespace twitch_alexa_skill.Utils.Handlers
{
    public class DeleteRewardHandler : IAlexaRequestHandler
    {

        public bool CanHandle(AlexaRequestInformation<SkillRequest> information)
        {
            var intentRequest = (Alexa.NET.Request.Type.IntentRequest)information.SkillRequest.Request;
            return intentRequest.Intent.Name == "DeleteReward";
        }

        public async Task<SkillResponse> Handle(AlexaRequestInformation<SkillRequest> information)
        {
            
            var alexaId = information.SkillRequest.Context.System.User.UserId;
            var accessToken = information.SkillRequest.Context.System.User.AccessToken;
            var twitchId = StateCache.Cache[alexaId].UserContext.UserTwitchId;
            var intentRequest = (Alexa.NET.Request.Type.IntentRequest)information.SkillRequest.Request;
            var title = intentRequest.Intent.Slots["title"]?.Value;
            var hasRewards = StateCache.Cache[alexaId].UserContext.State.TryGetValue("Rewards", out object reward); ;
            StateCache.Cache[alexaId].UserContext.State["LastIntent"] = "DeleteReward";

            if (!hasRewards)
            {
                return await ResponseBuilderWithState.Ask(
                    "You do not have any rewards I can manage. In order for me to be able to delete a reward, I must have created it. Maybe you want to ask me to create a reward for you, since you don't have any, my friend?",
                    new Reprompt("No Rewards I can manage would you like to do something else?"), information.SkillRequest.Session);
            }

            Reprompt rePrompt;
            var rewards = (List<RewardEntity>) StateCache.Cache[alexaId].UserContext.State["Rewards"];

            var matchResult = StringMatch.GetScore(title.ToLower(), rewards.Select(r => r.title.ToLower()).ToArray());
            var match = rewards.Where(r => r.title.ToLower() == title.ToLower()).Select(t => t.reward_id);
            
            if (match.Any() && match.Count() == 1)
            {
                return await SendMatchResponse(accessToken, twitchId, match.FirstOrDefault(), information.SkillRequest.Session);
            }

            var perfectScore = matchResult.Where(k => k.Value == 1).Select(kv => kv);
            var perfectScoreId = string.Empty;


            if (perfectScore.Any() && perfectScoreId.Length == 1)
            {
                perfectScoreId = rewards.Where(r => r.title.ToLower() == perfectScore.FirstOrDefault().Key.ToLower())
                    .Select(t => t.reward_id).FirstOrDefault();
                return await SendMatchResponse(accessToken, twitchId, perfectScoreId, information.SkillRequest.Session);
            }

            var topScore = matchResult.FirstOrDefault(v => v.Value == matchResult.Values.Max());
            if (!string.IsNullOrWhiteSpace(topScore.Key) && topScore.Value > 0.70)
            {
                StateCache.Cache[alexaId].UserContext.State["Match"] = topScore.Key;
                rePrompt = new Reprompt($"I found {topScore.Key} is this the correct reward?");
               return await ResponseBuilderWithState.Ask(
                    $"Well I didn't find a perfect match, but I really tried my best. the closest I could come up with is {topScore.Key}. Is that what you meant?",
                    rePrompt,
                    information.SkillRequest.Session);

            }


            return await ResponseBuilderWithState.Ask("My programmers are big dummys, and could not write a good enough algorithm to match that to a title of any of your rewards." +
                                                       " They are such nerds." +
                                                       " I know you would never tell me a wrong name. would you. Friend. Well you're going to have to start over.  Say delete reward to try again",
                new Reprompt("What would you like to do now?") ,
                information.SkillRequest.Session);
        }

        public async Task<SkillResponse> SendMatchResponse(string accessToken,string twitchId, string rewardId, Session session)
        {
            Reprompt rePrompt;
            StateCache.Cache[session.User.UserId].UserContext.State.Remove("Match", out object match);
            var deleted = await Twitch.DeleteCustomReward(accessToken, twitchId, rewardId);
            if (deleted)
            {
                rePrompt = new Reprompt("Are you interested in doing anything else, with me?");
               return await ResponseBuilderWithState.Ask($"Your reward was deleted successfully, of course it was, your are successful at everything." +
                                           $" so are you going to leave me again?  Or can I help you do something like maybe manage your redemptions or delete another reward?  Your call.", rePrompt,

                    session);
            }
            rePrompt = new Reprompt("Are you interested in doing anything else, with me?");
           return await ResponseBuilderWithState.Ask($"You know me, always causing trouble for you.  I was unable to delete that reward for you.", rePrompt,

                session);
        }

    }
}