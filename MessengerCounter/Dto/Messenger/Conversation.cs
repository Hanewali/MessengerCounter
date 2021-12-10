using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace MessengerCounter.Dto.Messenger
{
    /// <summary>
    /// Main class for conversation
    /// </summary>
    public class Conversation
    {
        /// <summary>
        /// Collection of messages in conversation
        /// </summary>
        [JsonPropertyName("messages")]
        public IEnumerable<Message> Messages { get; set; }

        /// <summary>
        /// Participants of conversation
        /// </summary>
        [JsonPropertyName("participants")]
        public IEnumerable<Participant> Participants { get; set; }

        /// <summary>
        /// Conversation title
        /// </summary>
        [JsonPropertyName("title")]
        public string Title { get; set; }
        
        /// <summary>
        /// Is a user who downloaded the data still a participant of this conversation
        /// </summary>
        [JsonPropertyName("is_still_participant")]
        public bool IsStillParticipant { get; set; }
        
        /// <summary>
        /// Type of a group
        /// </summary>
        [JsonPropertyName("thread_type")]
        public string ThreadType { get; set; }
        
        /// <summary>
        /// URL path to the group
        /// </summary>
        [JsonPropertyName("thread_path")]
        public string ThreadPath { get; set; }
        
        /// <summary>
        /// Conversation image
        /// </summary>
        [JsonPropertyName("image")]
        public Image Image { get; set; }
    }
}