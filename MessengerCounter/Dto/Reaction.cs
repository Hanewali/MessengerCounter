using System.Text.Json.Serialization;

namespace MessengerCounter.Dto
{
    /// <summary>
    /// Reaction object
    /// </summary>
    public class Reaction
    {
        /// <summary>
        /// Type of a reaction
        /// </summary>
        [JsonPropertyName("reaction")]
        public string ReactionType { get; set; }
        
        /// <summary>
        /// Who gave a reaction
        /// </summary>
        [JsonPropertyName("actor")]
        public string Actor { get; set; }
    }
}