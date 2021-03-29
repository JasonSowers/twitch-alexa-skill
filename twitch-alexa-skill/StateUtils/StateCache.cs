using Alexa.NET;
using Alexa.NET.Request;
using Alexa.NET.Response;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using twitch_alexa_skill.Entities;
using twitch_alexa_skill.Services;
using twitch_alexa_skill.Utils;
using TwitchLib.Api.Helix.Models.ChannelPoints;

namespace twitch_alexa_skill.StateUtils
{
    public static class StateCache
    {
        public static ConcurrentDictionary<string, State> Cache = new ConcurrentDictionary<string, State>();
        
        public static async Task<SkillResponse> CacheStateForUser(SkillRequest skillRequest)
        {

            UserEntity user = null;
            try
            {  
                var alexaId = skillRequest.Context.System.User.UserId;
                var accessToken = string.Empty;
                var twitchId = string.Empty;
                var twitchName = string.Empty;

                await CheckAccessToken(skillRequest);
                accessToken = skillRequest.Context.System.User.AccessToken;

                user = await GetUser(alexaId);
                var twitchUser = await Twitch.GetTwitchUserAsync(accessToken);

                if (user == null)
                {
                    twitchId = twitchUser.Id;
                    twitchName = twitchUser.Login;                    
                    user = await Tables.InsertUserAsync(new UserEntity(alexaId, twitchId, accessToken, twitchName));
                }
                else                 
                {
                    twitchId = user.twitch_id;
                    twitchName = user.twitch_usename;
                    if (user.access_token != accessToken || user.twitch_usename != twitchName) 
                    {
                        user.access_token = accessToken;
                        user.twitch_usename = twitchName;
                        user = await Tables.UpdateUser(user);
                    }                    
                }

                if (!CheckCache(alexaId))
                {
                    Cache.TryAdd(alexaId,  new State(alexaId, twitchId, skillRequest.Session, skillRequest));
                }

                Cache[alexaId].UserContext.State["User"] = user;
                Cache[alexaId].ConversationContext.CheckSession(skillRequest.Session);

                if (!await Tables.GetUserState(alexaId, twitchId))
                {
                    await UpdateState(alexaId);
                }

                await SyncRewards(alexaId, twitchId, accessToken);
                await SyncRedemptions(accessToken, twitchId, alexaId);
                await UpdateState(alexaId);
            }
            catch (Exception e)
            {
                string reply = string.Empty;
                var errorInserted = await Tables.InsertErrorAsync(new ErrorEntity(e));
                if (errorInserted)
                {
                    reply = "Oops, I did it again. I played with your heart.";
                   return await ResponseBuilderWithState.TellWithLinkAccountCard(reply, skillRequest.Session);
                }
                reply = "There is a problem with my data, I'm failing miserably, please send help.";
               return await ResponseBuilderWithState.TellWithLinkAccountCard(reply, skillRequest.Session);
            }

            return null;

        }

        public static async Task<SkillResponse> CheckAccessToken(SkillRequest skillRequest)
        {
            if (skillRequest.Session.User.AccessToken == null)
            {
                var reply = "In order to use this skill you must link your  twitch account. I have tried to send a card to youe alexa app on your phone," +
                    " but if you do not have notifications enabled from me you will have to go to your skills in the app and grant permissions for me to do epic shhhh it for you";
               return await ResponseBuilderWithState.TellWithLinkAccountCard(reply, skillRequest.Session);
            }           

            return null;
        }

        public static bool CheckCache(string alexaId)
        {
            return Cache.TryGetValue(alexaId, out State state);
        }

        public static async Task<UserEntity> GetUser(string alexaId)
        {
            return await Tables.GetUserByAlexaId(alexaId);
        }

        public static async Task SyncRewards(string alexaId, string twitchId, string accessToken)
        {
            try
            {
                var customRewards = await Twitch.GetUserRewards(accessToken, twitchId);
                var userRewards = await Tables.GetRewardsByTwitchUser(twitchId);
                await Tables.RemoveRewardsAsync(userRewards);
                if (!customRewards.Any())
                {
                    Cache[alexaId].UserContext.State.Remove("Redmptions", out object value3);
                    Cache[alexaId].UserContext.State.Remove("CurrentRedemption", out object value4);
                    return;
                }



                var newRewards = new List<RewardEntity>();
                newRewards.AddRange(customRewards.Select(a => new RewardEntity(a, alexaId)));
                await Tables.InsertRewardsAsync(newRewards);
                Cache[alexaId].UserContext.State["Rewards"] = newRewards;
            }
            catch (Exception e)
            {
                
                throw e;
            }
        }
        public static async Task SyncRedemptions(string accessToken, string twitchId, string alexaId) 
        {
            if (Cache[alexaId].UserContext.State.TryGetValue("Rewards", out object rewards1))
            {
                List<string> rewardIds = new List<string>();
                var removed = false;
                rewardIds = ((List<RewardEntity>)Cache[alexaId].UserContext.State["Rewards"]).Select(a => a.reward_id).ToList();
             
                var redemptions = await Tables.GetRedemptionByTwitchUser(twitchId);
                
                if (!rewardIds.Any())
                {
                    Cache[alexaId].UserContext.State.Remove("Rewards", out object rewards);
                    return;
                }

                if (redemptions.Any()) 
                {
                    removed = await Tables.RemoveRedemptionsAsync(redemptions);
                }
                if (rewardIds.Any())
                {
                    var twitchRedemptions = await Twitch.GetRedemptionsForUser(accessToken, alexaId, twitchId, rewardIds);
                    if (twitchRedemptions.Any())
                    {
                        var count = await Tables.InsertRedemptionsAsync(twitchRedemptions);
                        if (count > 0)
                        {
                            Cache[alexaId].UserContext.State["Redemptions"] = twitchRedemptions;
                            return;
                        }
                    }
                }

                Cache[alexaId].UserContext.State.Remove("Redmptions", out object value);
                Cache[alexaId].UserContext.State.Remove("CurrentRedemption", out object value2);
                return;

            }
            Cache[alexaId].UserContext.State.Remove("Redmptions", out object value3);
            Cache[alexaId].UserContext.State.Remove("CurrentRedemption", out object value4);
        }

        public static async Task UpdateState(string alexaId)
        {
            await Tables.UpdateUserState(alexaId, Cache[alexaId].UserContext.UserTwitchId);
        }
    }
}
