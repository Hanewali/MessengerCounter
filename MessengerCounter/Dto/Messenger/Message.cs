using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace MessengerCounter.Dto.Messenger
{
    /// <summary>
    /// Message object
    /// </summary>
    public class Message
    {
        /// <summary>
        /// Name of message sender
        /// </summary>
        [JsonPropertyName("sender_name")]
        public string SenderName { get; set; }

        /// <summary>
        /// Timestampt of message sent
        /// </summary>
        [JsonPropertyName("timestamp_ms")]
        public long TimestampMiliseconds { get; set; }

        /// <summary>
        /// Timestamp converted to datetime
        /// </summary>
        public DateTime Timestamp => DateTimeOffset.FromUnixTimeMilliseconds(TimestampMiliseconds).DateTime;

        /// <summary>
        /// Content of a messange
        /// </summary>
        [JsonPropertyName("content")]
        public string Content { get; set; }

        /// <summary>
        /// Type of a message
        /// </summary>
        [JsonPropertyName("type")]
        public string Type { get; set; }

        /// <summary>
        /// Is message unsent by a sender
        /// </summary>
        [JsonPropertyName("is_unsent")]
        public bool IsUnsent { get; set; }

        /// <summary>
        /// Photos attached to a message
        /// </summary>
        [JsonPropertyName("photos")]
        public IEnumerable<Photo> Photos { get; set; }

        /// <summary>
        /// Reactions to a message
        /// </summary>
        [JsonPropertyName("reactions")]
        public IEnumerable<Reaction> Reactions { get; set; }

        /// <summary>
        /// Videos attached to a message
        /// </summary>
        [JsonPropertyName("videos")]
        public IEnumerable<Video> Videos { get; set; }
    }
}