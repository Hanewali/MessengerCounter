using System.Text.Json.Serialization;

namespace MessengerCounter.Dto.Messenger
{
    /// <summary>
    /// Participant object
    /// </summary>
    public class Participant
    {
        /// <summary>
        /// Name of a participant
        /// </summary>
        [JsonPropertyName("name")]
        public string Name { get; set; }
    }
}