using Newtonsoft.Json;

namespace Alexa.NET.ProactiveEvents.ReservationConfirmations
{
    public class ProviderName
    {
        public ProviderName() { }

        public ProviderName(LocaleAttributes name)
        {
            Name = name;
        }

        [JsonProperty("name"), JsonConverter(typeof(LocaleAttributeConverter), "providerName")]
        public LocaleAttributes Name { get; set; }
    }
}