namespace TextTools.CommandBuilders
{
    using System.CommandLine;

    /// <summary>
    /// Factory class to build the root command
    /// </summary>
    public class RootCommandBuilder
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RootCommandBuilder"/> class.
        /// </summary>
        public RootCommandBuilder()
        {
        }

        /// <summary>
        /// Builds the command-line options, returning them as a  <see cref="System.CommandLine.RootCommand" /> object.
        /// </summary>
        /// <returns>The <see cref="System.CommandLine.RootCommand" /> object containing the command-line options for this program.</returns>
        public RootCommand BuildRootCommand()
        {
            var rootCommand = new RootCommand("TextTools");
            rootCommand.Add(new ReportCommandBuilder().BuildMatchListSubCommand());
            return rootCommand;
        }
    }
}