using Alexa.NET.Request;
using System;
using System.Collections.Generic;
using System.Text;

namespace twitch_alexa_skill.StateUtils
{
    public class ConversationState
    {
        public ConversationState(string id) 
        {
            UserAlexaId = id;
            State = new Dictionary<string, object>();
            State.Add("Created", "True");
        }
        public string UserAlexaId { get; }
        public Dictionary<string, object> State { get; set; }

        public string SessionId { get; set; }

        public void CheckSession(Session session)
        {
            if (SessionId != session.SessionId)
            {
                SessionId = session.SessionId;
                State = new Dictionary<string, object>();                
            }
        }

    }
}
