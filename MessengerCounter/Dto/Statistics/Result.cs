using System.Collections.Generic;

namespace MessengerCounter.Dto.Statistics
{
    public class Result
    {
        public IEnumerable<PeriodicalResult> PeriodicalResults { get; set; }
    }
}