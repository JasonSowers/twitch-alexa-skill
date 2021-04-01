using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using twitch_alexa_skill.Entities;
using TwitchLib.Api;
using TwitchLib.Api.Core.Enums;
using TwitchLib.Api.Helix.Models.ChannelPoints;
using TwitchLib.Api.Helix.Models.ChannelPoints.CreateCustomReward;
using TwitchLib.Api.Helix.Models.ChannelPoints.GetCustomRewardRedemption;
using TwitchLib.Api.Helix.Models.ChannelPoints.UpdateCustomRewardRedemptionStatus;
using TwitchLib.Api.Helix.Models.Users.GetUsers;

namespace twitch_alexa_skill.Services
{
    public class Twitch
    {
        public static TwitchAPI TwitchApiClient(string accessToken)
        {
            var client = new TwitchAPI();
            client.Settings.ClientId = Environment.GetEnvironmentVariable("TWITCH_CLIENT_ID");
            client.Settings.AccessToken = accessToken;
            return client;
        }

        public static async Task<TwitchLib.Api.Helix.Models.Users.GetUsers.User> GetTwitchUserAsync(string accessToken)
        {
            var twitchApiClient = TwitchApiClient(accessToken);
            var usersResponse = await twitchApiClient.Helix.Users.GetUsersAsync();
            return usersResponse.Users.ToList().FirstOrDefault();
        }

        public static async Task<List<CustomReward>> GetUserRewards(string accessToken, string twitchId, bool onlyManageable = true) 
        {
            var twitchApiClient = TwitchApiClient(accessToken);
            var rewardResponse =  await twitchApiClient.Helix.ChannelPoints.GetCustomReward(twitchId, onlyManageableRewards: onlyManageable);
            return rewardResponse.Data.ToList();
        }

        public static async Task<List<CustomReward>> CreateCustomReward(string accessToken, string twitchId, string title, int cost, bool autoApproval = false, bool inputRequired = false, string message = null)
        {
            var reward = new CreateCustomRewardsRequest();
            reward.Title = title;
            reward.Cost = cost;
            reward.IsEnabled = true;
            reward.ShouldRedemptionsSkipRequestQueue = autoApproval;
            reward.IsUserInputRequired = inputRequired;
            reward.Prompt = message?? string.Empty;
            

            var twitchApiClient = TwitchApiClient(accessToken);
            var rewardResponse = await twitchApiClient.Helix.ChannelPoints.CreateCustomRewards(twitchId, reward);
            return rewardResponse.Data.ToList();
        }

        public static async Task<bool> DeleteCustomReward(string accessToken, string twitchId, string rewardId)
        {
            var twitchApiClient = TwitchApiClient(accessToken);
            await twitchApiClient.Helix.ChannelPoints.DeleteCustomReward(twitchId, rewardId);
            return true;
        }
        public static async Task<int> ManageRedemptions(string accessToken, string broadcasterId, List<string> rewardIds, List<string> redemptionIds, CustomRewardRedemptionStatus status)
        {
            try
            {
                var redemptionUpdate = new UpdateCustomRewardRedemptionStatusRequest();
                redemptionUpdate.Status = status;
                var twitchApiClient = TwitchApiClient(accessToken);
                int count = 0;
                foreach (var rewardId in rewardIds)
                {
                    var response = await twitchApiClient.Helix.ChannelPoints.UpdateCustomRewardRedemptionStatus(broadcasterId, rewardId, redemptionIds, redemptionUpdate);
                    var ids = response.Data.Select(a => a.Id).ToList();
                    redemptionIds = redemptionIds.Except(ids).ToList();
                    count += response.Data.Count();
                }



                return count;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return 0;
            }
        }

        //<List<RedemptionEntity>>
        public static async Task<List<RedemptionEntity>> GetRedemptionsForUser(string accessToken, string alexaId,  string broadcasterId, List<string> rewardIds)
        {
            var twitchApiClient =  TwitchApiClient(accessToken);
            var tasks = new List<Task<IRestResponse>>();
            var redemptionsList = new List<RewardRedemption>();
            foreach (var id in rewardIds)
            {
                var client = new RestClient($"https://api.twitch.tv/helix/channel_points/custom_rewards/redemptions?broadcaster_id={broadcasterId}&reward_id={id}&status=UNFULFILLED");
                client.Timeout = -1;
                var request = new RestRequest(Method.GET);
                request.AddHeader("Client-Id", Environment.GetEnvironmentVariable("TWITCH_CLIENT_ID"));
                request.AddHeader("Authorization", $"Bearer {accessToken}");
                
                 tasks.Add(client.ExecuteAsync(request));
                //var result = await twitchApiClient.Helix.ChannelPoints.GetCustomRewardRedemption(broadcasterId, id, null, "UNFULFILLED");
                
            }

            var redemptions = await Task.WhenAll(tasks);
            foreach (var item in redemptions)
            {
                var response = JsonConvert.DeserializeObject<GetCustomRewardRedemptionResponse>(item.Content);
                var data = response.Data;
                redemptionsList.AddRange(data);
            }           
            
            var entities = redemptionsList.Select(a => new RedemptionEntity(alexaId, a.BroadcasterId, a.Reward.Id, a.Id, a.Reward.Title, a.UserName, a.UserInput));

            return entities.ToList();
        }

    }
}
           
        
            
