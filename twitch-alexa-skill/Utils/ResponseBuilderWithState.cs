using Alexa.NET;
using Alexa.NET.Request;
using Alexa.NET.Response;
using Alexa.NET.Response.Directive;
using Alexa.NET.Response.Ssml;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace twitch_alexa_skill.Utils
{
    public class ResponseBuilderWithState: ResponseBuilder
    {

        #region Tell Responses
        //   public new static async Task<SkillResponse> Tell(IOutputSpeech speechResponse)
        // {
        //     return await BuildResponse(speechResponse, true, null, null, null);
        // }
        //
        //  public new static async Task<SkillResponse> Tell(string speechResponse)
        // {
        //     return await  Tell(new PlainTextOutputSpeech { Text = speechResponse });
        // }

        //  public new static async Task<SkillResponse> Tell(Speech speechResponse)
        // {
        //     return await  Tell(new SsmlOutputSpeech { Ssml = speechResponse.ToXml() });
        // }
        //
        //  public new static async Task<SkillResponse> TellWithReprompt(IOutputSpeech speechResponse, Reprompt reprompt)
        // {
        //     return await BuildResponse(speechResponse, true, null, reprompt, null);
        // }
        //
        //  public new static async Task<SkillResponse> TellWithReprompt(string speechResponse, Reprompt reprompt)
        // {
        //     return await  TellWithReprompt(new PlainTextOutputSpeech { Text = speechResponse }, reprompt);
        // }
        //
        //  public new static async Task<SkillResponse> TellWithReprompt(Speech speechResponse, Reprompt reprompt)
        // {
        //     return await  TellWithReprompt(new SsmlOutputSpeech { Ssml = speechResponse.ToXml() }, reprompt);
        // }

         public new static async Task<SkillResponse> Tell(IOutputSpeech speechResponse, Session sessionAttributes)
        {
            return await BuildResponse(speechResponse, true, sessionAttributes, null, null);
        }

         public new static async Task<SkillResponse> Tell(string speechResponse, Session sessionAttributes)
        {
            return await  Tell(new PlainTextOutputSpeech { Text = speechResponse }, sessionAttributes);
        }

         public new static async Task<SkillResponse> Tell(Speech speechResponse, Session sessionAttributes)
        {
            return await  Tell(new SsmlOutputSpeech { Ssml = speechResponse.ToXml() }, sessionAttributes);
        }

         public new static async Task<SkillResponse> TellWithReprompt(IOutputSpeech speechResponse, Reprompt reprompt, Session sessionAttributes)
        {
            return await BuildResponse(speechResponse, true, sessionAttributes, reprompt, null);
        }

         public new static async Task<SkillResponse> TellWithReprompt(string speechResponse, Reprompt reprompt, Session sessionAttributes)
        {
            return await  TellWithReprompt(new PlainTextOutputSpeech { Text = speechResponse }, reprompt, sessionAttributes);
        }

         public new static async Task<SkillResponse> TellWithReprompt(Speech speechResponse, Reprompt reprompt, Session sessionAttributes)
        {
            return await  TellWithReprompt(new SsmlOutputSpeech { Ssml = speechResponse.ToXml() }, reprompt, sessionAttributes);
        }

        //  public new static async Task<SkillResponse> TellWithCard(IOutputSpeech speechResponse, string title, string content)
        // {
        //     var card = new SimpleCard();
        //     card.Content = content;
        //     card.Title = title;
        //
        //     return await BuildResponse(speechResponse, true, null, null, card);
        // }
        //
        //  public new static async Task<SkillResponse> TellWithCard(string speechResponse, string title, string content)
        // {
        //     return await  TellWithCard(new PlainTextOutputSpeech { Text = speechResponse }, title, content);
        // }
        //
        //  public new static async Task<SkillResponse> TellWithCard(Speech speechResponse, string title, string content)
        // {
        //     return await  TellWithCard(new SsmlOutputSpeech { Ssml = speechResponse.ToXml() }, title, content);
        // }

         public new static async Task<SkillResponse> TellWithCard(IOutputSpeech speechResponse, string title, string content, Session sessionAttributes)
        {
            var card = new SimpleCard
            {
                Content = content,
                Title = title
            };

            return await BuildResponse(speechResponse, true, sessionAttributes, null, card);
        }

         public new static async Task<SkillResponse> TellWithCard(string speechResponse, string title, string content, Session sessionAttributes)
        {
            return await  TellWithCard(new PlainTextOutputSpeech { Text = speechResponse }, title, content, sessionAttributes);
        }

         public new static async Task<SkillResponse> TellWithCard(Speech speechResponse, string title, string content, Session sessionAttributes)
        {
            return await  TellWithCard(new SsmlOutputSpeech { Ssml = speechResponse.ToXml() }, title, content, sessionAttributes);
        }

        //  public new static async Task<SkillResponse> TellWithLinkAccountCard(IOutputSpeech speechResponse)
        // {
        //     var card = new LinkAccountCard();
        //
        //     return await BuildResponse(speechResponse, true, null, null, card);
        // }
        //
        //  public new static async Task<SkillResponse> TellWithLinkAccountCard(string speechResponse)
        // {
        //     return await  TellWithLinkAccountCard(new PlainTextOutputSpeech { Text = speechResponse });
        // }
        //
        //  public new static async Task<SkillResponse> TellWithLinkAccountCard(Speech speechResponse)
        // {
        //     return await  TellWithLinkAccountCard(new SsmlOutputSpeech { Ssml = speechResponse.ToXml() });
        // }

         public new static async Task<SkillResponse> TellWithLinkAccountCard(IOutputSpeech speechResponse, Session sessionAttributes)
        {
            var card = new LinkAccountCard();

            return await BuildResponse(speechResponse, true, sessionAttributes, null, card);
        }

         public new static async Task<SkillResponse> TellWithLinkAccountCard(string speechResponse, Session sessionAttributes)
        {
            return await  TellWithLinkAccountCard(new PlainTextOutputSpeech { Text = speechResponse }, sessionAttributes);
        }

         public new static async Task<SkillResponse> TellWithLinkAccountCard(Speech speechResponse, Session sessionAttributes)
        {
            return await  TellWithLinkAccountCard(new SsmlOutputSpeech { Ssml = speechResponse.ToXml() }, sessionAttributes);
        }

        //  public new static async Task<SkillResponse> TellWithAskForPermissionsConsentCard(IOutputSpeech speechResponse, IEnumerable<string> permissions)
        // {
        //     var card = new AskForPermissionsConsentCard { Permissions = permissions.ToList() };
        //     return await BuildResponse(speechResponse, true, null, null, card);
        // }
        //
        //  public new static async Task<SkillResponse> TellWithAskForPermissionConsentCard(string speechResponse, IEnumerable<string> permissions)
        // {
        //     return await  TellWithAskForPermissionsConsentCard(new PlainTextOutputSpeech { Text = speechResponse }, permissions);
        // }
        //
        //  public new static async Task<SkillResponse> TellWithAskForPermissionConsentCard(Speech speechResponse, IEnumerable<string> permissions)
        // {
        //     return await  TellWithAskForPermissionsConsentCard(new SsmlOutputSpeech { Ssml = speechResponse.ToXml() }, permissions);
        // }

         public new static async Task<SkillResponse> TellWithAskForPermissionsConsentCard(IOutputSpeech speechResponse, IEnumerable<string> permissions, Session sessionAttributes)
        {
            var card = new AskForPermissionsConsentCard { Permissions = permissions.ToList() };
            return await BuildResponse(speechResponse, true, sessionAttributes, null, card);
        }

         public new static async Task<SkillResponse> TellWithAskForPermissionConsentCard(string speechResponse, IEnumerable<string> permissions, Session sessionAttributes)
        {
            return await  TellWithAskForPermissionsConsentCard(new PlainTextOutputSpeech { Text = speechResponse }, permissions, sessionAttributes);
        }

         public new static async Task<SkillResponse> TellWithAskForPermissionConsentCard(Speech speechResponse, IEnumerable<string> permissions, Session sessionAttributes)
        {
            return await  TellWithAskForPermissionsConsentCard(new SsmlOutputSpeech { Ssml = speechResponse.ToXml() }, permissions, sessionAttributes);
        }

        #endregion

        #region Ask Responses
        //  public new static async Task<SkillResponse> Ask(IOutputSpeech speechResponse, Reprompt reprompt)
        // {
        //     return await BuildResponse(speechResponse, false, null, reprompt, null);
        // }
        //
        //  public new static async Task<SkillResponse> Ask(string speechResponse, Reprompt reprompt)
        // {
        //     return await  Ask(new PlainTextOutputSpeech { Text = speechResponse }, reprompt);
        // }
        //
        //  public new static async Task<SkillResponse> Ask(Speech speechResponse, Reprompt reprompt)
        // {
        //     return await  Ask(new SsmlOutputSpeech { Ssml = speechResponse.ToXml() }, reprompt);
        // }

         public new static async Task<SkillResponse> Ask(IOutputSpeech speechResponse, Reprompt reprompt, Session sessionAttributes)
        {
            return await BuildResponse(speechResponse, false, sessionAttributes, reprompt, null);
        }

         public new static async Task<SkillResponse> Ask(string speechResponse, Reprompt reprompt, Session sessionAttributes)
        {
            return await  Ask(new PlainTextOutputSpeech { Text = speechResponse }, reprompt, sessionAttributes);
        }

         public new static async Task<SkillResponse> Ask(Speech speechResponse, Reprompt reprompt, Session sessionAttributes)
        {
            return await  Ask(new SsmlOutputSpeech { Ssml = speechResponse.ToXml() }, reprompt, sessionAttributes);
        }

        //  public new static async Task<SkillResponse> AskWithCard(IOutputSpeech speechResponse, string title, string content, Reprompt reprompt)
        // {
        //     return await  AskWithCard(speechResponse, title, content, reprompt, null);
        // }
        //
        //  public new static async Task<SkillResponse> AskWithCard(string speechResponse, string title, string content, Reprompt reprompt)
        // {
        //     return await  AskWithCard(new PlainTextOutputSpeech { Text = speechResponse }, title, content, reprompt);
        // }
        //
        //  public new static async Task<SkillResponse> AskWithCard(Speech speechResponse, string title, string content, Reprompt reprompt)
        // {
        //     return await  AskWithCard(new SsmlOutputSpeech { Ssml = speechResponse.ToXml() }, title, content, reprompt);
        // }

         public new static async Task<SkillResponse> AskWithCard(IOutputSpeech speechResponse, string title, string content, Reprompt reprompt, Session sessionAttributes)
        {
            var card = new SimpleCard
            {
                Content = content,
                Title = title
            };

            return await BuildResponse(speechResponse, false, sessionAttributes, reprompt, card);
        }

         public new static async Task<SkillResponse> AskWithCard(string speechResponse, string title, string content, Reprompt reprompt, Session sessionAttributes)
        {
            return await  AskWithCard(new PlainTextOutputSpeech { Text = speechResponse }, title, content, reprompt, sessionAttributes);
        }

         public new static async Task<SkillResponse> AskWithCard(Speech speechResponse, string title, string content, Reprompt reprompt, Session sessionAttributes)
        {
            return await  AskWithCard(new SsmlOutputSpeech { Ssml = speechResponse.ToXml() }, title, content, reprompt, sessionAttributes);
        }

        #endregion

        #region AudioPlayer Response
         public new static async Task<SkillResponse> AudioPlayerPlay(PlayBehavior playBehavior, string url, string token)
        {
            return await AudioPlayerPlay(playBehavior, url, token, 0);
        }

         public new static async Task<SkillResponse> AudioPlayerPlay(PlayBehavior playBehavior, string url, string token, int offsetInMilliseconds)
        {
            return await AudioPlayerPlay(playBehavior, url, token, null, offsetInMilliseconds);
        }

         public new static async Task<SkillResponse> AudioPlayerPlay(PlayBehavior playBehavior, string url, string token, string expectedPreviousToken, int offsetInMilliseconds)
        {
            var response = await BuildResponse(null, true, null, null, null);
            response.Response.Directives.Add(new AudioPlayerPlayDirective()
            {
                PlayBehavior = playBehavior,
                AudioItem = new AudioItem()
                {
                    Stream = new AudioItemStream()
                    {
                        Url = url,
                        Token = token,
                        ExpectedPreviousToken = expectedPreviousToken,
                        OffsetInMilliseconds = offsetInMilliseconds
                    }
                }
            });

            return response;
        }

         public new static async Task<SkillResponse> AudioPlayerStop()
        {
            var response = await BuildResponse(null, true, null, null, null);
            response.Response.Directives.Add(new StopDirective());
            return response;
        }

         public new static async Task<SkillResponse> AudioPlayerClearQueue(ClearBehavior clearBehavior)
        {
            var response = await BuildResponse(null, true, null, null, null);
            response.Response.Directives.Add(new ClearQueueDirective()
            {
                ClearBehavior = clearBehavior
            });
            return response;
        }
        #endregion

        #region Dialog Response

        //  public new static async Task<SkillResponse> DialogDelegate(Intent updatedIntent = null)
        // {
        //     return await  DialogDelegate(null, updatedIntent);
        // }

         public new static async Task<SkillResponse> DialogDelegate(Session attributes, Intent updatedIntent = null)
        {
            var response = await BuildResponse(null, false, attributes, null, null);
            response.Response.Directives.Add(new DialogDelegate { UpdatedIntent = updatedIntent });
            return response;
        }

        //  public new static async Task<SkillResponse> DialogElicitSlot(IOutputSpeech outputSpeech, string slotName, Intent updatedIntent = null)
        // {
        //     return await DialogElicitSlot(outputSpeech, slotName, null, updatedIntent);
        // }

         public new static async Task<SkillResponse> DialogElicitSlot(IOutputSpeech outputSpeech, string slotName, Session attributes, Intent updatedIntent = null)
        {
            var response = await BuildResponse(outputSpeech, false, attributes, null, null);
            response.Response.Directives.Add(new DialogElicitSlot(slotName) { UpdatedIntent = updatedIntent });
            return response;
        }

        //  public new static async Task<SkillResponse> DialogConfirmSlot(IOutputSpeech outputSpeech, string slotName,
        //     Intent updatedIntent = null)
        // {
        //     return await  DialogConfirmSlot(outputSpeech, slotName, null, updatedIntent);
        // }

         public new static async Task<SkillResponse> DialogConfirmSlot(IOutputSpeech outputSpeech, string slotName, Session attributes, Intent updatedIntent = null)
        {
            var response = await BuildResponse(outputSpeech, false, attributes, null, null);
            response.Response.Directives.Add(new DialogConfirmSlot(slotName) { UpdatedIntent = updatedIntent });
            return response;
        }

        //  public new static async Task<SkillResponse> DialogConfirmIntent(IOutputSpeech outputSpeech, Intent updatedIntent = null)
        // {
        //     return await DialogConfirmIntent(outputSpeech, null, updatedIntent);
        // }

         public new static async Task<SkillResponse> DialogConfirmIntent(IOutputSpeech outputSpeech, Session attributes, Intent updatedIntent = null)
        {
            var response = await BuildResponse(outputSpeech, false, attributes, null, null);
            response.Response.Directives.Add(new DialogConfirmIntent { UpdatedIntent = updatedIntent });
            return response;
        }

        #endregion
        //
        //  public new static async Task<SkillResponse> Empty()
        // {
        //     return await BuildResponse(null, true, null, null, null);
        // }

        #region Main Response Builder
        private static async  Task<SkillResponse> BuildResponse(IOutputSpeech outputSpeech, bool shouldEndSession, Session sessionAttributes, Reprompt reprompt, ICard card)
        {
            await StateUtils.StateCache.UpdateState(sessionAttributes.User.UserId);
            
            var response = new SkillResponse { Version = "1.0" };
            if (sessionAttributes != null) response.SessionAttributes = sessionAttributes.Attributes;

            var body = new ResponseBody
            {
                ShouldEndSession = shouldEndSession,
                OutputSpeech = outputSpeech
            };

            if (reprompt != null) body.Reprompt = reprompt;
            if (card != null) body.Card = card;

            response.Response = body;

            return response;
        }

#endregion
    }
}
