using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Alexa.NET.RequestHandlers.Handlers;
using Alexa.NET.Request;
using Alexa.NET;
using Alexa.NET.Request.Type;
using Alexa.NET.RequestHandlers;
using Alexa.NET.Response;
using Alexa.NET.Profile;
using twitch_alexa_skill.StateUtils;
using twitch_alexa_skill.Entities;
using twitch_alexa_skill.Services;
using TwitchLib.Api.Core.Enums;
using Newtonsoft.Json.Linq;

namespace twitch_alexa_skill.Utils.Handlers
{
    public class YesIntentHandler : IAlexaRequestHandler
    {
        public bool CanHandle(AlexaRequestInformation<SkillRequest> information)
        {
            var intentRequest = (IntentRequest)information.SkillRequest.Request;
            return intentRequest.Intent.Name == "AMAZON.YesIntent";

        }

        public async Task<SkillResponse> Handle(AlexaRequestInformation<SkillRequest> information)
        {
            var alexaId = new string(information.SkillRequest.Session.User.UserId);
            var accessToken = new string(information.SkillRequest.Session.User.AccessToken);
            var twitchId = new string(StateCache.Cache[alexaId].UserContext.UserTwitchId);
            var hasLastIntent = StateCache.Cache[alexaId].UserContext.State.TryGetValue("LastIntent", out object lastIntentValue);
            var lastIntentName = new string(lastIntentValue?.ToString());
            StateCache.Cache[alexaId].UserContext.State["LastIntent"] = "AMAZON.YesIntent";

            if (lastIntentName == "DeleteReward")
            {
                var rewards = (List<RewardEntity>) StateCache.Cache[alexaId].UserContext.State["Rewards"];
                var rewardNAme = StateCache.Cache[alexaId].UserContext.State["Match"].ToString();
                var reward = rewards.FirstOrDefault(a => a.title == rewardNAme);
                StateCache.Cache[alexaId].UserContext.State.Remove("Match", out object match);
                var success = await Twitch.DeleteCustomReward(accessToken, twitchId, reward.reward_id);

                StateCache.Cache[alexaId].UserContext.State.Remove("Match", out object valueMatch);
                if (success)
                {
                    return await ResponseBuilderWithState.Tell("We did it! We deleted the reward! We're the best.", information.SkillRequest.Session);
                }

                return await ResponseBuilderWithState.Tell("Well, it's confirmed I'm a failure.  I guess you have to start over.", information.SkillRequest.Session);
            }

            var userInput = StateCache.Cache[alexaId].UserContext.State.TryGetValue("UserInput", out object inputValue);
            if (userInput && inputValue.ToString() == string.Empty)
            {
                StateCache.Cache[alexaId].UserContext.State["UserInput"] = "yes";
            }

            var customMessage = StateCache.Cache[alexaId].UserContext.State.TryGetValue("CustomMessage", out object customMessageValue);
            if (customMessage && customMessageValue.ToString() == string.Empty)
            {
                StateCache.Cache[alexaId].UserContext.State["CustomMessage"] = "yes";
            }



            if (!StateCache.Cache[alexaId].UserContext.State.TryGetValue("Redemptions", out object redeemCheck)
                && !StateCache.Cache[alexaId].UserContext.State.TryGetValue("CurrentRedemption", out object current)
                && StateCache.Cache[alexaId].UserContext.State.TryGetValue("Manage", out object manage1))
            {
                StateCache.Cache[alexaId].UserContext.State.Remove("Manage", out object value2);
                var rePrompt = new Reprompt($"Is there anything else?");
               return await ResponseBuilderWithState.Ask($"You do not have any redemptions at this time, friend. Maybe you could ask me to create a reward? Or you can just say cancel.", rePrompt, information.SkillRequest.Session);

            }


            if (StateCache.Cache[alexaId].UserContext.State.TryGetValue("Redemptions", out object redeem)
                && redeem.GetType() != typeof(JArray)
                && (lastIntentName == "ManageRedemptions" || lastIntentName == "AMAZON.YesIntent" || lastIntentName == "AMAZON.NoIntent")
                && !userInput
                && !customMessage)
            {

                try
                {
                    return await HandlerUtils.HandleManageRedemptions(alexaId, accessToken, information.SkillRequest.Session);
                }
                catch (Exception e )
                {

                    throw e;
                }
            }

            if ((lastIntentName == "CreateReward") 
                || userInput
                || customMessage)
            {

                if (StateCache.Cache[alexaId].UserContext.State["Approval"].ToString() == string.Empty)
                {
                    StateCache.Cache[alexaId].UserContext.State["Approval"] = "yes";
                }

                
 

                if (!userInput)
                {
                        StateCache.Cache[alexaId].UserContext.State["UserInput"] = string.Empty;
                    return await ResponseBuilderWithState.Ask($"Would you like to require the user to enter a message when they redeem this reward?  Say yes to require it, or no to not require it",
                        new Reprompt("Require user to enter a message on redemption?"),
                        information.SkillRequest.Session);
                }

                if (userInput && StateCache.Cache[alexaId].UserContext.State["UserInput"].ToString() == "yes" && !customMessage)
                {
                    StateCache.Cache[alexaId].UserContext.State["CustomMessage"] = string.Empty;
                    return await ResponseBuilderWithState.Ask($"Do you want to have a custom message display when the user redeems this reward and enters their message?",
                        new Reprompt("Display a custom message upon redemption?"),
                        information.SkillRequest.Session);
                }

                if (customMessage && StateCache.Cache[alexaId].UserContext.State["CustomMessage"].ToString() == "yes")
                {
                    return await ResponseBuilderWithState.Ask($"What would you like that custom message to say?  When you respond please begin you response with the word. Message.  Followed by the message you would like to display.",
                        new Reprompt("What should the custom message say?"),
                        information.SkillRequest.Session);
                }



                var title = (string)StateCache.Cache[alexaId].UserContext.State["Title"]; 
                var cost = Convert.ToInt32((string)StateCache.Cache[alexaId].UserContext.State["Cost"]);

                var twichReward = await Twitch.CreateCustomReward(accessToken,
                    StateCache.Cache[alexaId].UserContext.UserTwitchId,
                    title,
                    cost,
                    true);                 

                await  Tables.InsertRewardsAsync(new List<RewardEntity>() { new RewardEntity(twichReward.FirstOrDefault(), alexaId) });
                StateCache.Cache[alexaId].UserContext.State.Remove("Cost", out object value);
                StateCache.Cache[alexaId].UserContext.State.Remove("Title", out object value2);
                StateCache.Cache[alexaId].UserContext.State.Remove("Approval", out object value3);
                StateCache.Cache[alexaId].UserContext.State.Remove("UserInput", out object value4);
                StateCache.Cache[alexaId].UserContext.State.Remove("CustomMessage", out object value5);
                StateCache.Cache[alexaId].UserContext.State.Remove("UserInput", out object value6);


                return await ResponseBuilderWithState.Ask($"Ok I have saved your reward named {title} that costs {cost} channel points." +
                    $" You will now get notifications for redemptions if you have granted me permission to notify you in the " +
                    $"alexa app on your phone.  Lets keep this party going whats up next?  Or you could say cancel and I can go back to crying in my corner",
                    new Reprompt("What else do you need, my friend?"),
                    information.SkillRequest.Session);
            }


            StateCache.Cache.Remove(alexaId, out State valueState);
            StateCache.Cache.TryAdd(alexaId, new State(alexaId, twitchId, information.SkillRequest.Session, information.SkillRequest));

            return await ResponseBuilderWithState.Tell("I totally messed up, again. My bad. You get to start over.  Congrats", information.SkillRequest.Session);
        }
    }
}