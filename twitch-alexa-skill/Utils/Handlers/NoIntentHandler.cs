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
using Alexa.NET.Profile;
using twitch_alexa_skill.StateUtils;
using twitch_alexa_skill.Entities;
using twitch_alexa_skill.Services;
using TwitchLib.Api.Core.Enums;
using Newtonsoft.Json.Linq;

namespace twitch_alexa_skill.Utils.Handlers
{
    public class NoIntentHandler : IAlexaRequestHandler
    {
        public bool CanHandle(AlexaRequestInformation<SkillRequest> information)
        {
            var intentRequest = (IntentRequest)information.SkillRequest.Request;
            return intentRequest.Intent.Name == "AMAZON.NoIntent";
        }

        public async Task<SkillResponse> Handle(AlexaRequestInformation<SkillRequest> information)
        {
            var alexaId = new string(information.SkillRequest.Session.User.UserId);
            var accessToken = new string(information.SkillRequest.Session.User.AccessToken);
            var twitchId = new string(StateCache.Cache[alexaId].UserContext.UserTwitchId);

            var hasLastIntent = StateCache.Cache[alexaId].UserContext.State.TryGetValue("LastIntent", out object lastIntentValue);
            var lastIntentName = new string(lastIntentValue?.ToString());
            StateCache.Cache[alexaId].UserContext.State["LastIntent"] = "AMAZON.NoIntent";


            if (lastIntentName == "DeleteReward")
            {
                StateCache.Cache[alexaId].UserContext.State.Remove("Match");
                return await ResponseBuilderWithState.Ask("Guess who gets to start over.  I'll give you 3 guesses. Just say Delete reward to start again, or cancel if you give up.", new Reprompt("Say delete reward to try again"), information.SkillRequest.Session);
            }

            var userInput = StateCache.Cache[alexaId].UserContext.State.TryGetValue("UserInput", out object inputValue);
            if (userInput && inputValue.ToString() == string.Empty)
            {
                StateCache.Cache[alexaId].UserContext.State["UserInput"] = "no";
            }

            var customMessage = StateCache.Cache[alexaId].UserContext.State.TryGetValue("CustomMessage", out object customMessageValue);
            if (customMessage && customMessageValue.ToString() == string.Empty)
            {
                StateCache.Cache[alexaId].UserContext.State["CustomMessage"] = "no";
            }


            if (StateCache.Cache[alexaId].UserContext.State.TryGetValue("Match", out object match) && match.ToString() != string.Empty)
            {
                StateCache.Cache[alexaId].UserContext.State.Remove("Match", out object value);
                return ResponseBuilder.Tell("Well, it's confirmed I'm a failure.  I guess you have to start over.");
            }

            if ((!StateCache.Cache[alexaId].UserContext.State.TryGetValue("Redemptions", out object redeemCheck)
                 || redeemCheck.ToString() == string.Empty) &&
                StateCache.Cache[alexaId].UserContext.State.TryGetValue("CurrentRedemption", out object current))
            {
                var rePrompt = new Reprompt($"Is there anything else?");

               return await ResponseBuilderWithState.Ask($"You do not have any redemptions at this time. Do you need anything else? If not you can just say cancel, and I'll go back to my corner", rePrompt, information.SkillRequest.Session);

            }

            if (StateCache.Cache[alexaId].UserContext.State.TryGetValue("Redemptions", out object redeem) 
                && redeem.ToString() != string.Empty
                && redeem.GetType() != typeof(JArray)
                && hasLastIntent
                && (lastIntentName == "ManageRedemptions" || lastIntentName == "AMAZON.YesIntent" || lastIntentName == "AMAZON.NoIntent")
                && StateCache.Cache[alexaId].UserContext.State.TryGetValue("Manage", out object manage) 
                && manage.ToString() != string.Empty)
            
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

            if ((hasLastIntent && lastIntentName == "CreateReward")
                || customMessage || userInput)
            {

                if (StateCache.Cache[alexaId].UserContext.State["Approval"].ToString() == string.Empty)
                {
                    StateCache.Cache[alexaId].UserContext.State["Approval"] = "no";
                }

                if (!userInput)
                {
                    StateCache.Cache[alexaId].UserContext.State["UserInput"] = string.Empty;
                    return await ResponseBuilderWithState.Ask($"Would you like to require the user to enter a message when they redeem this reward?" +
                                                              $"  Say yes to require it, no to skip, cancel to abort mission",
                        new Reprompt("Require user to enter a message on redemption?"),
                        information.SkillRequest.Session);
                }

                if (userInput && StateCache.Cache[alexaId].UserContext.State["UserInput"].ToString() == "yes" && !customMessage)
                {
                    StateCache.Cache[alexaId].UserContext.State["CustomMessage"] = string.Empty;
                    return await ResponseBuilderWithState.Ask($"Do you want to have a custom message display when the user redeems this reward and enters their message?" +
                                                              $" yes to accept, no to reject, or cancel to stop creating this reward",
                        new Reprompt("Display a custom message upon redemption?"),
                        information.SkillRequest.Session);
                }

                if (customMessage && StateCache.Cache[alexaId].UserContext.State["CustomMessage"].ToString() == "yes")
                {
                    return await ResponseBuilderWithState.Ask($"What would you like that custom message to say?  When you respond please begin you response with the word. Message." +
                                                              $"  Followed by the message you would like to display.",
                        new Reprompt("What should the custom message say?"),
                        information.SkillRequest.Session);
                }

                var title = StateCache.Cache[alexaId].UserContext.State["Title"].ToString();
                var cost = Convert.ToInt32((string)StateCache.Cache[alexaId].UserContext.State["Cost"]);
                bool needsApproval = StateCache.Cache[alexaId].UserContext.State["Approval"].ToString() != "yes";
                bool messageRequired = StateCache.Cache[alexaId].UserContext.State["UserInput"].ToString() == "yes";

                var twichReward = await Twitch.CreateCustomReward(accessToken,
                    StateCache.Cache[alexaId].UserContext.UserTwitchId,
                    title,
                    cost,
                    needsApproval,
                    messageRequired);

                //await Tables.InsertRewardsAsync(new List<RewardEntity>() { new RewardEntity(twichReward.FirstOrDefault(), alexaId) });

                StateCache.Cache[alexaId].UserContext.State.Remove("Cost", out object value);
                StateCache.Cache[alexaId].UserContext.State.Remove("Title", out object value2);
                StateCache.Cache[alexaId].UserContext.State.Remove("Approval", out object value3);
                StateCache.Cache[alexaId].UserContext.State.Remove("UserInput", out object value4);
                StateCache.Cache[alexaId].UserContext.State.Remove("CustomMessage", out object value5);
                StateCache.Cache[alexaId].UserContext.State.Remove("UserInput", out object value6);
                return await ResponseBuilderWithState.Ask($"I have created the reward wither the title {title} and a cost of {cost}" +
                                                          $" channel points at your request. Do you need anything else, since I have nothing to do now? Maybe create a new reward, or you can say cancel to severely dissapoint me.",
                    new Reprompt("Can I do anything else for you?"),
                    information.SkillRequest.Session);

            }

            if ((hasLastIntent) && (lastIntentName.ToString() == "AMAZON.YesIntent" ||
                                    lastIntentName.ToString() == "AMAZON.NoIntent"))
            {
                return await ResponseBuilderWithState.Ask($"Great your leaving me again, I thought we were just getting started. maybe try asking me to manage redemption or provide reward ideas",
                    new Reprompt("What else do you need? say cancel to finish"),
                    information.SkillRequest.Session);
            }

            StateCache.Cache.Remove(alexaId, out State valueState);
            StateCache.Cache.TryAdd(alexaId, new State(alexaId, twitchId, information.SkillRequest.Session, information.SkillRequest));
            StateCache.Cache[alexaId].UserContext.State["LastIntent"] = "AMAZON.NoIntent";
            return await ResponseBuilderWithState.Tell("I totally messed up, again. My bad. You're going to have to start over", information.SkillRequest.Session);
        }
    }
}