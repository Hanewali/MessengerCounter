using MessengerCounter.Enum;

namespace MessengerCounter.Dto.Statistics
{
    public class PeriodicalResult
    {
        public Period Period { get; set; }
        public string Sender { get; set; }
        public string Count { get; set; }
        public MessageType MessageType { get; set; }
    }
}