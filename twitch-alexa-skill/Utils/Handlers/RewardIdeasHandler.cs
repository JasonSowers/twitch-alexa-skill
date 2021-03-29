using System;
using Alexa.NET;
using Alexa.NET.Request;
using Alexa.NET.RequestHandlers;
using Alexa.NET.Response;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using twitch_alexa_skill.Entities;
using twitch_alexa_skill.Services;
using twitch_alexa_skill.StateUtils;

namespace twitch_alexa_skill.Utils.Handlers
{
    public class RewardIdeasHandler : IAlexaRequestHandler
    {

        public bool CanHandle(AlexaRequestInformation<SkillRequest> information)
        {
            var intentRequest = (Alexa.NET.Request.Type.IntentRequest)information.SkillRequest.Request;
            return intentRequest.Intent.Name == "RewardIdeas";
        }

        public async Task<SkillResponse> Handle(AlexaRequestInformation<SkillRequest> information)
        {
            var randoArray = new string[10];

            for (int i = 0; i < 10; i++)
            {
                randoArray[i] = ideaList.ElementAt(rando.Next(0,ideaList.Count));
            }
            var alexaId = information.SkillRequest.Context.System.User.UserId;
            var intentRequest = (Alexa.NET.Request.Type.IntentRequest)information.SkillRequest.Request;
            var hasLastIntent = StateCache.Cache[alexaId].UserContext.State.TryGetValue("LastIntent", out object lastIntentValue);
            var lastIntentName = new string(lastIntentValue?.ToString());
            StateCache.Cache[alexaId].UserContext.State["LastIntent"] = "RewardIdeas";

            var text = string.Join(". ", randoArray);

            Reprompt rePrompt = new Reprompt("If you want to hear more just say, Show me reward ideas");
            
           return await ResponseBuilderWithState.Ask($"I'm glad you asked! Here are some ideas I picked at random for you. {text}",
                                        rePrompt,
                                        information.SkillRequest.Session);
        }
        

            public static List<string> ideaList = new List<string>()
            {
                "Add a sound or quote to your soundboard",
                "Make me sing to you",
                "VIP badge for a day",
                "Random fact",
                "Random joke",
                "Say a tongue twister",
                "Switch in game role",
                "Switch in game weapon",
                "A reward for absolutely nothing at all",
                "Make me say what you type",
                "Pet cam on for a period of time",
                "Use a voice modulator",
                "Make me dab",
                "Make me wink",
                "Make me wave" +
                "Make me blow kisses",
                "Make me draw something you pick",
                "Compliment a viewer",
                "Time out the viewer",
                "Time out another viewer of your choice",
                "Turn on off sub only mode",
                "Mod for a period of time",
                "Emote only mode",
                "Vip discord role",
                "Change your clothes",
                "Turn the lights off",
                "Do a dance",
                "Do a freestyle dance",
                "Do a dance of their choice",
                "Secret reward no one knows what it is",
                "Go offline for a period of time",
                "Channel point garbage can that does nothing",
                "Run adds",
                "Pick next game",
                "Play next game with me",
                "Play next match with me",
                "Do some pushups",
                "I'll do a dare",
                "Add time to my stream",
                "Make me drink water",
                "Make a chat command",
                "Unlock giveaway",
                "No talking for some amount of time",
                "No saying some word",
                "Song request",
                "Remove a ban from a user",
                "Choose my phone background",
                "Choose my desktop background",
                "Learn a tik tok dance",
                "Follow on social media",
                "Raid my stream",
                "Raid a stream of my choice",
                "Raid a random 1 viewer stream",
                "Choose a new emote",
                "Choose green screen background",
                "Choose my profile pic on twitch or social media",
                "pick next map",
                "Choose what character or hero I play next",
                "Choose an item for me to use or buy or build",
                "Let the viewer roast you",
                "Make me do an AMA for an amount of time"
            };

            public static Random rando = new Random(ideaList.Count);



    }
}