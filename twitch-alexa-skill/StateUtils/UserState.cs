using Alexa.NET.Request;
using Alexa.NET.StateManagement;
using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using twitch_alexa_skill.Entities;
using twitch_alexa_skill.Services;

namespace twitch_alexa_skill.StateUtils
{
    public class UserState
    {
        public UserState(string id, string twitchId)
        {
            UserTwitchId = twitchId;
            UserAlexaId = id;
            State = new Dictionary<string, object>();
            State.Add("Created", "true");
        }

        public string UserAlexaId { get; }
        public Dictionary<string, object> State { get; set; }
        public string UserTwitchId { get; }


        public string GetKey()
        {
            return UserAlexaId;
        }

        public override bool Equals(object userstate)
        {
            if (userstate == null || userstate.GetType() != this.GetType())
            {
                return false;
            }

            var userState2 = userstate as UserState;

            if (string.IsNullOrWhiteSpace(userState2.UserAlexaId) || userState2.State == null || !this.UserAlexaId.Equals(userState2.UserAlexaId))
            {
                return false;
            }

            var sortedDictionary = this.State.OrderBy(kvp => kvp.Key);
            var sortedUserState2 = userState2.State.OrderBy(kvp => kvp.Key);            

            if (sortedDictionary.Count() != sortedUserState2.Count())
            {
                return false;
            }

            foreach (var kvp in sortedDictionary)
            {
                var current = sortedUserState2.GetEnumerator().Current;

                if (kvp.Key != current.Key || kvp.Value != current.Value)
                {
                    return false;
                }
            }

            return true;
        }

        public override int GetHashCode()
        {
            var hash = 13;
            var sortedDictionary = this.State.OrderBy(kvp => kvp.Key);

            hash = (hash * 7) + this.UserAlexaId.GetHashCode();

            foreach (var kvp in sortedDictionary)
            {
                hash = (hash * 7) + kvp.Key.GetHashCode();
                hash = (hash * 7) + kvp.Value.GetHashCode();
            }
            return hash;
        }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }

        public UserState CreateStateFromJson(string json)
        {
            return JsonConvert.DeserializeObject<UserState>(json);
        }

    }

    public static class DictionaryExtensionClass
    {
        public static  Task Get<T>(this Dictionary<string, object> dictionary, string key)
        {
            return Task.FromResult(dictionary[key]);
        }
    }
    
}
