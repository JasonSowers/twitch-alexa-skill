using twitch_alexa_skill.Utils.Id.Utils;

namespace twitch_alexa_skill.Utils.Id.Configuration
{
    /// <summary>
    /// Provides programmatic configuration for the shortid library.
    /// </summary>
    public class GenerationOptions
    {
        /// <summary>
        /// Determines whether numbers are used in generating the id.
        /// Default: false.
        /// </summary>
        public bool UseNumbers { get; set; }

        /// <summary>
        /// Determines whether special characters are used in generating the id.
        /// Default: true.
        /// </summary>
        public bool UseSpecialCharacters { get; set; } = true;

        public bool UseAlphabet { get; set; }

        public bool UseAlphabetBig { get; set; }

        public bool UseAlphabetSmall { get; set; }

        /// <summary>
        /// Determines the length of the generated id.
        /// Default: a random length between 7 and 15.
        /// </summary>
        public int Length { get; set; } =
            RandomUtils.GenerateNumberInRange(Constants.MinimumAutoLength, Constants.MaximumAutoLength);


        public void SetAlpha() 
        {
            if (UseAlphabetBig || UseAlphabetSmall)
            {
                UseAlphabet = false;
            }
        }
    }
}
