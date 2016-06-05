namespace SentenceTopGrabber
{
    /// <summary>
    /// Some grabber node
    /// </summary>
    internal class GrabberNode
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GrabberNode"/> class
        /// </summary>
        /// <param name="value">some value</param>
        /// <param name="score">some score</param>
        public GrabberNode(string value, double score)
        {
            this.Value = value;
            this.Score = score;
        }

        /// <summary>
        /// Gets value of node
        /// </summary>
        public string Value { get; private set; }

        /// <summary>
        /// Gets score of node
        /// </summary>
        public double Score { get; private set; }
    }
}
