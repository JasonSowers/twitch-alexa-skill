using Microsoft.Azure.Cosmos.Table;
using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using twitch_alexa_skill.StateUtils;

namespace twitch_alexa_skill.Entities
{
    public class StateEntity : TableEntity
    {
        public StateEntity() { }

        public StateEntity(UserState state)
        {
            RowKey = state.UserAlexaId;
            PartitionKey = state.UserTwitchId;
        }

        public string PersistentUserState { get; set; }
    }
}
