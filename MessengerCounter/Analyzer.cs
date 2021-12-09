using System;

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

        public string ConversationName { get; set; }
        public string InputPath { get; set; }
        public string OutputPath { get; set; }
        
        
        public void GetMessages()
        {
            throw new System.NotImplementedException(); 
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