namespace SentenceTopGrabber
{
    using System;

    /// <summary>
    /// Display on console some info
    /// </summary>
    internal static class ConsoleDisplay
    {
        /// <summary>
        /// Ask address for local file
        /// </summary>
        /// <returns>Some address</returns>
        public static string AskAddress()
        {
            string result = string.Empty;

            Console.Write("Input local parth for txt file in unicode ");
            string address = Console.ReadLine();

            return address;
        }
    }
}
