using Alexa.NET;
using Alexa.NET.Request;
using Alexa.NET.Response;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using twitch_alexa_skill.Entities;
using twitch_alexa_skill.Services;
using twitch_alexa_skill.StateUtils;
using TwitchLib.Api.Core.Enums;

namespace twitch_alexa_skill.Utils.Handlers
{
    public static class HandlerUtils
    {
        public static async Task<SkillResponse> HandleManageRedemptions(string alexaId, string accessToken, Session session)
        {
            var reply = string.Empty;
            var redemptions = (List<RedemptionEntity>)StateCache.Cache[alexaId].UserContext.State["Redemptions"];
            var count = 0;
            var redemptionState = CustomRewardRedemptionStatus.FULFILLED;
            var action = "Approved";

            if (StateCache.Cache[alexaId].UserContext.State["LastIntent"].ToString() == "AMAZON.NoIntent")
            {
                redemptionState = CustomRewardRedemptionStatus.CANCELED;

                action =  "Rejected";
            }

            if (StateCache.Cache[alexaId].UserContext.State["Manage"].ToString() == "all")
            {
                var rewardIds = redemptions.Select(a => a.reward_id).ToHashSet();

                count = await Twitch.ManageRedemptions(accessToken, redemptions.FirstOrDefault().twitch_id, rewardIds.ToList(), redemptions.Select(a => a.redemption_id).ToList(), redemptionState);
                if (count == redemptions.Count)
                {
                    
                    var remove = await Tables.RemoveRedemptionsAsync(redemptions);
                    if (remove)
                    {
                        var rejected = "You rule your channel with and iron fist, my friend. I like it.";
                        var accepted = "Doesn't it feel good to do nice things for others?";
                        var actionText = action == "Approved" ? accepted : rejected;
 

                        StateCache.Cache[alexaId].UserContext.State.Remove("Manage", out object value);
                       return await ResponseBuilderWithState.Ask($"All of your channel point redemptions have been {action}. {actionText}  What should we do next?",
                           new Reprompt("Let's keep going what do you want to do next?") , 
                           session);
                    }
                }

                StateCache.Cache[alexaId].UserContext.State.Remove("Manage", out object manageValue);
                return await ResponseBuilderWithState.Tell($"You redemptions were not {action} because I am a catastrophic failure, let's start over friend?", session);

            }
            StateCache.Cache[alexaId].UserContext.State["CurrentRedemption"] = redemptions.FirstOrDefault();
            var redemption = (RedemptionEntity)StateCache.Cache[alexaId].UserContext.State["CurrentRedemption"];
            count = await Twitch.ManageRedemptions(accessToken, redemption.twitch_id, new List<string>{redemption.reward_id}, new List<string>{ redemption.redemption_id }, redemptionState);
            

            
            if (count > 0)
            {
                Reprompt rePrompt = null;
                redemptions.Remove((RedemptionEntity) redemption);
                if (redemptions.Any())
                {

                    StateCache.Cache[alexaId].UserContext.State["CurrentRedemption"] = redemptions.FirstOrDefault();
                    var redemptionName = redemptions.FirstOrDefault().redemption_name;
                    var userName = redemptions.FirstOrDefault().twitch_username;
                    var userInputMessage = redemptions.FirstOrDefault().user_input;
                    if (userInputMessage != string.Empty)
                    {
                        userInputMessage = $"{userName} sent the message, {userInputMessage}, when they redeemed the reward";
                    }

                    rePrompt = new Reprompt($"Accept redemption from {userName} for {redemptionName}");
                    return await ResponseBuilderWithState.Ask($"{userName} would like to redeem {redemptionName}. {userInputMessage}.  Do you want to accept this? Say yes to accept,  no to reject, or cancel to do nothing", rePrompt, session);
                }

                StateCache.Cache[alexaId].UserContext.State.Remove("CurrentRedemption", out object currentredemption);
                rePrompt = new Reprompt($"You have no more redemptions to manage, you're all caught up.");
                StateCache.Cache[alexaId].UserContext.State.Remove("Manage", out object manageValue2);
                return await ResponseBuilderWithState.Ask($"You're a champ!  You knocked out all your pending redemptions.  What else can I do for you?", rePrompt, session);


            }
            StateCache.Cache[alexaId].UserContext.State.Remove("Manage", out object manageValue3);
            return await ResponseBuilderWithState.Tell($"Something went wrong while trying to {action}, do you want to try again?", session);
        }



    }
}
