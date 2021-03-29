using Alexa.NET.Request;
using Alexa.NET.StateManagement;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using twitch_alexa_skill.Entities;

namespace twitch_alexa_skill.StateUtils
{


    public class State 
    {


        public Session Session { get; set; }
        public SkillRequest Request { get; set; }
        public Entities.UserEntity User {get; set;}
        public UserState UserContext { get; set; }
        public ConversationState ConversationContext { get; set; }
        

        public State(string alexaId, string twitchId, Session session, SkillRequest request) 
        {
            Session = session;
            Request = request;
            UserContext = new UserState(alexaId, twitchId);
            ConversationContext = new ConversationState(alexaId);

        }

    }
}
