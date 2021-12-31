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

            if (files.Length == 0)
            {
                var directories = Directory.GetDirectories(InputPath, $"{ConversationName}*", SearchOption.TopDirectoryOnly);

                if (directories.Any())
                    files = Directory.GetFiles($"{InputPath}\\ConversationName", "message_*", SearchOption.TopDirectoryOnly);
                else
                    throw new Exception("No conversation found!");
            }
            
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

            // Conversation.Participants = Conversation.Participants.Distinct();
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

            outputResult += GetOutputForPeriod(Period.Daily);
            outputResult += GetOutputForPeriod(Period.Weekly);
            outputResult += GetOutputForPeriod(Period.Monthly);
            outputResult += GetOutputForPeriod(Period.Yearly);
            outputResult += GetOutputForPeriod(Period.Full);

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

            return result;
        }

        private string GetOutputForMessageType(MessageType messageType, IEnumerable<PeriodicalResult> periodicalResults)
        {
            var result = string.Empty;

            result += messageType + Environment.NewLine;
            result += "-------" + Environment.NewLine;

            foreach (var periodicalResult in periodicalResults.Where(x => x.MessageType == messageType).OrderByDescending(x => x.Count))
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
            var currentResult = new List<PeriodicalResult>();
            
            if (Result.PeriodicalResults != null)
            {
                currentResult = Result.PeriodicalResults.ToList();    
            }
            
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
                    }
                    catch (Exception ex)
                    {
                        //ignore
                        if (ex.Message != "No messages")
                            throw;
                    }
                    try
                    {
                        //media
                        periodicals.Add(CountByTypeForPeriod(MessageType.Media, dateFrom, dateTo, participant, period));
                    }
                    catch (Exception ex)
                    {
                        //ignore
                        if (ex.Message != "No messages")
                            throw;
                    }
                    try
                    {
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

                dateFrom = dateTo;
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
                        Count = Math.Round(periodicals
                            .Where(x => x.Sender == participant.Name && x.MessageType == MessageType.Text &&
                                        x.Period == period).Average(x => x.Count), 1)
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
                        Count = Math.Round(periodicals
                            .Where(x => x.Sender == participant.Name && x.MessageType == MessageType.Media &&
                                        x.Period == period).Average(x => x.Count), 1)
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
                        Count = Math.Round(periodicals
                            .Where(x => x.Sender == participant.Name && x.MessageType == MessageType.Reaction &&
                                        x.Period == period).Average(x => x.Count), 1)
                    });    
                }
                
            }

            currentResult.AddRange(periodicalResults);
            Result.PeriodicalResults = currentResult;

        }

        private PeriodicalResult CountByTypeForPeriod(MessageType messageType, DateTime dateFrom, DateTime dateTo,
            Participant participant, Period period)
        {
            var messages = Conversation.Messages.Where(x =>
                x.SenderName == participant.Name && x.Timestamp >= dateFrom && x.Timestamp < dateTo);

            var messagesList = messages.ToList();
            
            if (!messagesList.Any())
            {
                throw new Exception("No messages");
            }
            
            messagesList = messageType switch
            {
                MessageType.Text => messagesList.Where(x => !string.IsNullOrWhiteSpace(x.Content)).ToList(),
                MessageType.Reaction => messagesList,
                MessageType.Media => messagesList.Where(x => x.Photos != null).ToList(),
                _ => throw new ArgumentOutOfRangeException(nameof(messageType), messageType, null)
            };

            if (messageType is MessageType.Media or MessageType.Text)
            {
                return new PeriodicalResult
                {
                    Sender = participant.Name,
                    Period = period,
                    MessageType = messageType,
                    Count = messagesList.Count
                };
            }

            var reactions = new List<Reaction>();

            foreach (var message in messagesList)
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
                Count = reactions.Count
            };
        }
    }
}