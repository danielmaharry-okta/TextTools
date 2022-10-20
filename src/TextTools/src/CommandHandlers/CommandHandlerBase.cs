namespace TextTools.CommandHandlers
{
	/// <summary>
   /// Abstract base class for all command handlers
   /// </summary>
	public abstract class CommandHandlerBase
	{
		/// <summary>
      /// Initializes a new instance of the <see cref="CommandHandlerBase" /> class.
      /// </summary>
		protected CommandHandlerBase()
		{}

        /// <summary>
        /// Runs the handler
        /// </summary>
        public void Go()
        {
            Console.OutputEncoding = System.Text.Encoding.Unicode;
            if (!Initialize())
            {
                return;
            }

            RunCommand();
        }

        /// <summary>
        /// Runs the handler
        /// </summary>
        protected abstract void RunCommand();

        /// <summary>
        /// Parses and validates as necessary the values of any command parameters not described in core options
        /// </summary>
        /// <returns><c>true</c> if all parameters are valid, <c>false</c> otherwise</returns>
        protected abstract bool ValidateOptions();

        /// <summary>
        /// Writes a string to the console in a given color on a new line
        /// </summary>
        /// <param name="text">The text to write to the console</param>
        /// <param name="color">The color it should be</param>
        protected void SendToConsole(string text, ConsoleColor color)
        {
            Console.ForegroundColor = color;
            Console.WriteLine();
            Console.WriteLine(text);
            Console.ResetColor();
        }

        private bool Initialize()
        {
            return ValidateOptions();
        }
	}
}