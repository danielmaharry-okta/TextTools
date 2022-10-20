using System.CommandLine;
using System.Threading.Tasks;
using TextTools.CommandBuilders;

internal class Program
{
   private static async Task<int> Main(string[] args)
   {
      var rootCommand = new RootCommand("TextTools");
      rootCommand.AddCommand(new RootCommandBuilder().BuildRootCommand());
		return await rootCommand.InvokeAsync(args);
	}
}