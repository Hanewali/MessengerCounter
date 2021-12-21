using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using MessengerCounter.Dto.Messenger;
using MessengerCounter.Dto.Statistics;
using MessengerCounter.Enum;

namespace MessengerCounter
{
    class Analyzer : IAnalyzer
    {
        private Analyzer(string conversationName, string inputPath, string outputPath)
        {
            ConversationName = conversationName;
            InputPath = inputPath;
            OutputPath = outputPath;
            Result = new Result();
        }

        public static IAnalyzer CreateInstance(string conversationName, string inputPath, string outputPath)
        {
            if (string.IsNullOrWhiteSpace(inputPath)) inputPath = Environment.CurrentDirectory;
            if (string.IsNullOrWhiteSpace(outputPath)) outputPath = Environment.CurrentDirectory;

            return new Analyzer(conversationName, inputPath, outputPath);
        }

        public Conversation? Conversation { get; set; }
        public string ConversationName { get; set; }
        public string InputPath { get; set; }
        public string OutputPath { get; set; }

        public Result Result { get; set; }


        public void GetMessages()
        {
            var files = Directory.GetFiles(InputPath, "message_*", SearchOption.TopDirectoryOnly);

            var filesQueue = new Queue<string>(files);

            filesQueue.TryDequeue(out var firstFilePath);

            if (filesQueue == null) throw new Exception("No conversation found!");

            Conversation = JsonSerializer.Deserialize<Conversation>(File.ReadAllText(firstFilePath));

            foreach (var file in filesQueue)
            {
                var conversation = JsonSerializer.Deserialize<Conversation>(File.ReadAllText(file));

                var messages = Conversation.Messages.ToList();

                messages.AddRange(conversation.Messages);

                Conversation.Messages = messages;
            }
        }

        public void Analyze()
        {
            AnalyzePeriod(Period.Daily);
            AnalyzePeriod(Period.Weekly);
            AnalyzePeriod(Period.Monthly);
            AnalyzePeriod(Period.Yearly);
            AnalyzePeriod(Period.Full);
        }

        public void PrettyPrint()
        {
            var outputResult = string.Empty;

            outputResult += GetOutputForPeriod(Period.Daily) + Environment.NewLine;
            outputResult += GetOutputForPeriod(Period.Weekly) + Environment.NewLine;
            outputResult += GetOutputForPeriod(Period.Monthly) + Environment.NewLine;
            outputResult += GetOutputForPeriod(Period.Yearly) + Environment.NewLine;
            outputResult += GetOutputForPeriod(Period.Full) + Environment.NewLine;

            Console.Write(outputResult);
        }

        private string GetOutputForPeriod(Period period)
        {
            var result = string.Empty;
            var periodicals = Result.PeriodicalResults.Where(x => x.Period == period).ToList();


            result += period + Environment.NewLine;
            result += "************" + Environment.NewLine;

            result += GetOutputForMessageType(MessageType.Text, periodicals);
            result += GetOutputForMessageType(MessageType.Media, periodicals);
            result += GetOutputForMessageType(MessageType.Reaction, periodicals);

            result += Environment.NewLine;

            return result;
        }

        private string GetOutputForMessageType(MessageType messageType, IEnumerable<PeriodicalResult> periodicalResults)
        {
            var result = string.Empty;

            result += MessageType.Text + Environment.NewLine;
            result += "-------" + Environment.NewLine;

            foreach (var periodicalResult in periodicalResults.OrderByDescending(x => x.Count))
            {
                result += $"{periodicalResult.Sender}: {periodicalResult.Count}" + Environment.NewLine;
            }

            result += Environment.NewLine;
            return result;
        }

        public void SaveOutput()
        {
            throw new System.NotImplementedException();
        }

        private void AnalyzePeriod(Period period)
        {
            var periodicals = new List<PeriodicalResult>();

            var dateFrom = Conversation.Messages.Select(x => x.Timestamp).Min();

            var maxDate = Conversation.Messages.Select(x => x.Timestamp).Max();

            var dateTo = dateFrom;

            do
            {
                dateTo = period switch
                {
                    Period.Daily => dateTo.AddDays(1),
                    Period.Weekly => dateTo.AddDays(7),
                    Period.Monthly => dateTo.AddMonths(1),
                    Period.Yearly => dateTo.AddYears(1),
                    Period.Full => maxDate,
                    _ => throw new ArgumentOutOfRangeException(nameof(period), period, null)
                };

                foreach (var participant in Conversation.Participants)
                {
                    try
                    {
                        //text
                        periodicals.Add(CountByTypeForPeriod(MessageType.Text, dateFrom, dateTo, participant, period));
                        //media
                        periodicals.Add(CountByTypeForPeriod(MessageType.Media, dateFrom, dateTo, participant, period));
                        //reactions
                        periodicals.Add(CountByTypeForPeriod(MessageType.Reaction, dateFrom, dateTo, participant,
                            period));
                    }
                    catch (Exception ex)
                    {
                        //ignore
                        if (ex.Message != "No messages")
                            throw;
                    }
                }
            } while (dateTo < maxDate);

            var periodicalResults = new List<PeriodicalResult>();
            
            foreach (var participant in Conversation.Participants)
            {

                if (periodicals.Any(x => x.Sender == participant.Name && x.MessageType == MessageType.Text && x.Period == period))
                {
                    periodicalResults.Add(new PeriodicalResult
                    {
                        Sender = participant.Name,
                        MessageType = MessageType.Text,
                        Period = period,
                        Count = periodicals
                            .Where(x => x.Sender == participant.Name && x.MessageType == MessageType.Text &&
                                        x.Period == period).Average(x => long.Parse(x.Count)).ToString()
                    });    
                }

                if (periodicals.Any(x =>
                    x.Sender == participant.Name && x.MessageType == MessageType.Media && x.Period == period))
                {
                    periodicalResults.Add(new PeriodicalResult
                    {
                        Sender = participant.Name,
                        MessageType = MessageType.Media,
                        Period = period,
                        Count = periodicals
                            .Where(x => x.Sender == participant.Name && x.MessageType == MessageType.Media &&
                                        x.Period == period).Average(x => long.Parse(x.Count)).ToString()
                    });    
                }

                if (periodicals.Any(x =>
                    x.Sender == participant.Name && x.MessageType == MessageType.Reaction && x.Period == period))
                {
                    periodicalResults.Add(new PeriodicalResult
                    {
                        Sender = participant.Name,
                        MessageType = MessageType.Reaction,
                        Period = period,
                        Count = periodicals
                            .Where(x => x.Sender == participant.Name && x.MessageType == MessageType.Reaction &&
                                        x.Period == period).Average(x => long.Parse(x.Count)).ToString()
                    });    
                }
                
                Result.PeriodicalResults = periodicalResults;
            }
        }

        private PeriodicalResult CountByTypeForPeriod(MessageType messageType, DateTime dateFrom, DateTime dateTo,
            Participant participant, Period period)
        {
            var messages = Conversation.Messages.Where(x =>
                x.SenderName == participant.Name && x.Timestamp >= dateFrom && x.Timestamp < dateTo);

            messages = messageType switch
            {
                MessageType.Text => messages.Where(x => !string.IsNullOrWhiteSpace(x.Content)),
                MessageType.Reaction => messages,
                MessageType.Media => messages.Where(x => x.Photos.Any()),
                _ => throw new ArgumentOutOfRangeException(nameof(messageType), messageType, null)
            };

            try
            {
                messages.Count();
            }
            catch
            {
                throw new Exception("No messages");
            }

            if (messageType is MessageType.Media or MessageType.Text)
            {
                return new PeriodicalResult
                {
                    Sender = participant.Name,
                    Period = period,
                    MessageType = messageType,
                    Count = messages.Count().ToString()
                };
            }

            var reactions = new List<Reaction>();

            foreach (var message in messages)
            {
                try
                {
                    if (message.Reactions.Any())
                        reactions.AddRange(message.Reactions.Where(x => x.Actor == participant.Name));
                }
                catch
                {
                    throw new Exception("No messages");
                }
            }

            return new PeriodicalResult
            {
                Sender = participant.Name,
                Period = period,
                MessageType = messageType,
                Count = reactions.Count.ToString()
            };
        }
    }
}