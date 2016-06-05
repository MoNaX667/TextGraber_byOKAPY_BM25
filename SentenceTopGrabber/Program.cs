namespace SentenceTopGrabber
{
    using System;

    /// <summary>
    /// Find some general sentence by Okapi_BM25
    /// </summary>
    internal class Program
    {
        /// <summary>
        /// The main method
        /// </summary>
        private static void Main()
        {
            // Сustomize console
            Console.Title = "Text Grabbbbbber";

            Console.WindowHeight = 40;
            Console.WindowWidth = 100;

            var grabber = new Graber();
            grabber.Run();

            Console.ReadKey();
        }
    }
}
