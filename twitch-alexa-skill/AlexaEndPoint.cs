using Alexa.NET;
using Alexa.NET.Conversations;
using Alexa.NET.Request;
using Alexa.NET.Response.Converters;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.IO;
using System.Threading.Tasks;
using twitch_alexa_skill.Utils;

namespace twitch_alexa_skill
{
    public static class AlexaEndPoint
    {
        [FunctionName("AlexaEndPoint")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            DialogApiInvokedRequest.AddToRequestConverter();
            if (!DirectiveConverter.TypeFactories.ContainsKey("Dialog.DelegateRequest"))
            {
                DialogDelegateRequestDirective.AddSupport();
            }
            

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var skillRequest = JsonConvert.DeserializeObject<SkillRequest>(requestBody);


            var reply = string.Empty;
            if (string.IsNullOrWhiteSpace(skillRequest.Context.System.User.AccessToken)) 
            {
                reply =  "I have sent a card to the alexa app on your phone to link your twitch account";
                return new OkObjectResult(ResponseBuilder.TellWithLinkAccountCard(reply, skillRequest.Session));                
            }
            
            AlexaRequestProcessor alexaRequestProcessor = new AlexaRequestProcessor();
            var skillResponse =  await alexaRequestProcessor.ProcessAsync(skillRequest);           


            return new OkObjectResult(skillResponse);
        }



    }

}
