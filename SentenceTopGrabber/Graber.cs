namespace SentenceTopGrabber
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Text.RegularExpressions;

    /// <summary>
    /// Grab some text and work with it
    /// </summary>
    internal class Graber
    {
        /// <summary>
        /// Result string
        /// </summary>
        private string resultString;

        /// <summary>
        /// Target text
        /// </summary>
        private string targetText;

        /// <summary>
        /// Collection for wordsList
        /// </summary>
        private List<string> wordsList;

        /// <summary>
        /// Collection for sentencesList
        /// </summary>
        private List<string> sentencesList;

        /// <summary>
        /// Free coefficient
        /// </summary>
        private double coefficientK = 2.0;

        /// <summary>
        /// Free coefficient
        /// </summary>
        private double coefficientB = 0.75;

        /// <summary>
        /// The average document length
        /// </summary>
        private double avgdl;

        /// <summary>
        /// Run of grab
        /// </summary>
        public void Run()
        {
            // Load text
            while (true)
            {
                string someAddress = ConsoleDisplay.AskAddress();

                if (this.LoadText(someAddress))
                {
                    break;
                }
                else
                {
                    Console.WriteLine("Wrong address");
                    Console.ReadKey();
                    Console.Clear();
                }
            }

            Console.WriteLine(new string('=', 30));
            Console.WriteLine("Start calculating");

            // Work loop
            foreach (var item in this.Rank(this.targetText).Take(10))
            {
                this.resultString += string.Format("{0} - {1}", item.Value, item.Score) +
                    Environment.NewLine;
            }

            Console.WriteLine(new string('=', 30));
            Console.WriteLine("Saving result");

            this.SaveText("test.txt", this.resultString);

            Console.WriteLine(new string('=', 30));
            Console.WriteLine("All done");
        }

        /// <summary>
        /// Load target text
        /// </summary>
        /// <param name="filePath">file path</param>
        /// <returns>flag of good load</returns>
        private bool LoadText(string filePath)
        {
            try
            {
                this.targetText = File.ReadAllText(filePath, Encoding.Unicode);
                return true;
            }
            catch (FileNotFoundException excep)
            {
                Console.WriteLine(excep.Message);
            }

            return false;
        }

        /// <summary>
        /// The method set start parameters ,calculate rank for every sentence 
        /// and return sorted by rank result as collection
        /// </summary>
        /// <param name="inputText">Target text</param>
        /// <returns>Sorted by rank collection</returns>
        private IEnumerable<GrabberNode> Rank(string inputText)
        {
            this.SetStartStatement(inputText);
            var result = this.SortResult(this.CalcScore());

            return result;
        }

        /// <summary>
        /// The method calculate rank for every sentence and return result as collection
        /// </summary>
        /// <returns>Calculated rank for every sentence like collection</returns>
        private List<GrabberNode> CalcScore()
        {
            List<GrabberNode> result = new List<GrabberNode>();

            foreach (var sentence in this.sentencesList)
            {
                double score = 0;

                foreach (var word in this.wordsList)
                {
                    score += this.CaclLeftPart(word, sentence) * this.CalculateteIDF(word);
                }

                result.Add(new GrabberNode(sentence, score));
            }

            return result;
        }

        /// <summary>
        /// The method calculate left part of function for current word and current sentence
        /// </summary>
        /// <param name="word">Some word</param>
        /// <param name="sentence">Some sentence</param>
        /// <returns>Double result</returns>
        private double CaclLeftPart(string word, string sentence)
        {
            double wordFrequency = this.GetTermFrequency(word, sentence);
            int sentenceCount = this.GetWords(sentence).Count;

            // Calculate left part of function
            double result = 
                (wordFrequency * (this.coefficientK + 1)) / 
                (wordFrequency + this.coefficientK * (1 - this.coefficientB +
                this.coefficientB * sentenceCount / this.avgdl));

            return result;
        }

        /// <summary>
        /// Calculate inverse document frequency for current word
        /// </summary>
        /// <param name="word">Some word</param>
        /// <returns>IDF value</returns>
        private double CalculateteIDF(string word)
        {
            var sentenceCount = this.sentencesList.Count;

            var wordMatchCount = this.sentencesList
                .Count(s => s.ToLower().Contains(word));

            var result = 
                Math.Log10((sentenceCount * 1.0 - wordMatchCount + 0.5) / (wordMatchCount + 0.5));

            return result > 0 ? result : 0;
        }

        /// <summary>
        /// Calculate word frequency in the current sentence
        /// </summary>
        /// <param name="word">some word</param>
        /// <param name="text">some text</param>
        /// <returns>Frequency value</returns>
        private double GetTermFrequency(string word, string text)
        {
            var currentWords = this.GetWords(text);
            if (currentWords.Contains(word))
            {
                var count = currentWords
                    .Select(w => w.Equals(word))
                    .Count();

                return count * 1.0 / currentWords.Count;
            }
            else
            {
                return 0;
            }
        }

        /// <summary>
        /// Sort result collection by score
        /// </summary>
        /// <param name="result">result collection</param>
        /// <returns>Sorted by score result</returns>
        private List<GrabberNode> SortResult(List<GrabberNode> result)
        {
            return result
                .OrderByDescending(r => r.Score)
                .ToList();
        }

        /// <summary>
        /// Take start text
        /// </summary>
        /// <param name="inputText">start text</param>
        private void SetStartStatement(string inputText)
        {
            this.wordsList = this.GetWords(inputText);
            this.sentencesList = this.GetSentences(inputText);

            this.avgdl = inputText.Length * 1.0 / this.sentencesList.Count;
        }

        /// <summary>
        /// Grab all wordsList in text
        /// </summary>
        /// <param name="text">target text</param>
        /// <returns>Collection of wordsList</returns>
        private List<string> GetWords(string text)
        {
            List<string> result = new List<string>();
            var regex = new Regex("\\w+?\\b");

            result = regex.Matches(text)
                .OfType<Match>()
                .Select(s => s.Value)
                .Select(s => s.Trim())
                .Select(s => s.ToLower())
                .GroupBy(s => s)
                .Select(s => s.Key)
                .ToList();

            return result;
        }

        /// <summary>
        /// Grab all sentence in text
        /// </summary>
        /// <param name="text">Target text</param>
        /// <returns>Collection of sentence</returns>
        private List<string> GetSentences(string text)
        {
            List<string> result = new List<string>();

            // All of Latin and сyrillic letters with some  class [.!?]
            var regex = new Regex("[A-Z А-Я].*?(?=[.!?])");

            result = regex.Matches(text)
                .OfType<Match>()
                .Select(m => m.Value)
                .ToList();

            return result;
        }

        /// <summary>
        /// Write result from out file
        /// </summary>
        /// <param name="path">File path</param>
        /// <param name="text">Result string</param>
        private void SaveText(string path, string text)
        {
            File.WriteAllText(path, text);
        }
    }
}
