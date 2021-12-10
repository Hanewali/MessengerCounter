using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using MessengerCounter.Dto.Messenger;

namespace MessengerCounter
{
    class Analyzer : IAnalyzer
    {
        private Analyzer(string conversationName, string inputPath, string outputPath)
        {
            ConversationName = conversationName;
            InputPath = inputPath;
            OutputPath = outputPath;
        }

        public IAnalyzer CreateInstance(string conversationName, string inputPath, string outputPath)
        {
            if (string.IsNullOrWhiteSpace(inputPath)) inputPath = Environment.CurrentDirectory;
            if (string.IsNullOrWhiteSpace(outputPath)) outputPath = Environment.CurrentDirectory;
            
            return new Analyzer(conversationName, inputPath, outputPath);
        }

        public Conversation? Conversation { get; set; }
        public string ConversationName { get; set; }
        public string InputPath { get; set; }
        public string OutputPath { get; set; }
        
        
        public void GetMessages()
        {
            var files = Directory.GetFiles(InputPath, "message_*", SearchOption.TopDirectoryOnly);

            var filesQueue = new Queue<string>(files);

            filesQueue.TryDequeue(out var firstFilePath);
            
            if (filesQueue == null) throw new Exception("No conversation found!");
            
            Conversation = JsonSerializer.Deserialize<Conversation>(File.ReadAllText(firstFilePath));
            
            foreach (var file in files)
            {
                var conversation = JsonSerializer.Deserialize<Conversation>(file);

                var messages = Conversation.Messages.ToList();
                
                messages.AddRange(conversation.Messages);

                Conversation.Messages = messages;
            }
            
        }

        public void Analyze()
        {
            throw new System.NotImplementedException();
        }

        public void PrettyPrint()
        {
            throw new System.NotImplementedException();
        }

        public void SaveOutput()
        {
            throw new System.NotImplementedException();
        }
    }
}