using System.Text.Json.Serialization;

namespace MessengerCounter.Dto.Messenger
{
    /// <summary>
    /// Video object
    /// </summary>
    public class Video
    {
        /// <summary>
        /// Video source URI
        /// </summary>
        [JsonPropertyName("uri")]
        public string Uri { get; set; }
        
        /// <summary>
        /// Timestamp of video creation
        /// </summary>
        [JsonPropertyName("creation_timestamp")]
        public string CreationTimestamp { get; set; }
        
        /// <summary>
        /// Video thumbnail
        /// </summary>
        [JsonPropertyName("thumbnail")]
        public VideoThumbnail VideoThumbnail { get; set; }
    }
}