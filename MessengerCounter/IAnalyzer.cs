using MessengerCounter.Dto.Messenger;

namespace MessengerCounter
{
    interface IAnalyzer
    {
        
        public Conversation? Conversation { get; set; }
        public string ConversationName { get; set; }
        public string InputPath { get; set; }
        public string OutputPath { get; set; }
        
        public void GetMessages();
        public void Analyze();
        public void PrettyPrint();
        public void SaveOutput();
    }
}