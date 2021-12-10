using System.Text.Json.Serialization;

namespace MessengerCounter.Dto.Messenger
{
    /// <summary>
    /// Object for a video thumbnail
    /// </summary>
    public class VideoThumbnail
    {
        /// <summary>
        /// Uri of a video thumbnail
        /// </summary>
        [JsonPropertyName("uri")]
        public string Uri { get; set; }
    }
}