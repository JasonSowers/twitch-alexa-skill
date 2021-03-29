using Google.Api.Gax.ResourceNames;
using Google.Cloud.Translate.V3;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace twitch_alexa_skill.Utils
{
    public static class Language
    {
        public static List<string> GetSupportedLanguages(string projectId) 
        {

            TranslationServiceClient translationServiceClient = TranslationServiceClient.Create();
            GetSupportedLanguagesRequest request = new GetSupportedLanguagesRequest
            {
                ParentAsLocationName = new LocationName(projectId, "global"),
            };
            SupportedLanguages response = translationServiceClient.GetSupportedLanguages(request);
            // List language codes of supported languages
            foreach (SupportedLanguage language in response.Languages)
            {
                Console.WriteLine($"Language Code: {language.LanguageCode}");
            }
            return response.Languages.Select(l => l.LanguageCode).ToList();
        }
    

        public static string DetectLanguage(string text, string projectId)
        {
            TranslationServiceClient translationServiceClient = TranslationServiceClient.Create();
            DetectLanguageRequest request = new DetectLanguageRequest
            {
                ParentAsLocationName = new LocationName(projectId, "global"),
                Content = text,
                MimeType = "text/plain",
            };
            DetectLanguageResponse response = translationServiceClient.DetectLanguage(request);
            // Display list of detected languages sorted by detection confidence.
            // The most probable language is first.
             var code = response.Languages.FirstOrDefault().LanguageCode;

            return code;
        }

        public static string TranslateText(string text, string targetLanguage, string projectId)
        {
            TranslationServiceClient translationServiceClient = TranslationServiceClient.Create();
            TranslateTextRequest request = new TranslateTextRequest
            {
                Contents =
                {                    
                    text,
                },
                TargetLanguageCode = targetLanguage,
                ParentAsLocationName = new LocationName(projectId, "global"),
            };
            TranslateTextResponse response = translationServiceClient.TranslateText(request);
            // Display the translation for each input text provided
            var traslatedText = response.Translations.FirstOrDefault().TranslatedText;

            return traslatedText;
        }
    }
}
