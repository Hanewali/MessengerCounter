using System.Text.Json.Serialization;

namespace MessengerCounter.Dto
{
    /// <summary>
    /// Photo object
    /// </summary>
    public class Photo
    {
        /// <summary>
        /// Image source URI
        /// </summary>
        [JsonPropertyName("uri")]
        public string Uri { get; set; }
        
        /// <summary>
        /// Creation timestamp of an image
        /// </summary>
        [JsonPropertyName("creation_timestamp")]
        public long CreationTimestamp { get; set; }
    }
}