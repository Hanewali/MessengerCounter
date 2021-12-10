using System.Text.Json.Serialization;

namespace MessengerCounter.Dto.Messenger
{
    /// <summary>
    /// Conversation image
    /// </summary>
    public class Image
    {
        /// <summary>
        /// Image source URI
        /// </summary>
        [JsonPropertyName("uri")]
        public string Uri { get; set; }

        /// <summary>
        /// Timestamp of image creation
        /// </summary>
        [JsonPropertyName("creation_timestamp")]
        public long CreationTimestamp { get; set; }
    }
}