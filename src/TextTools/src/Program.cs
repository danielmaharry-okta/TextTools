using System.CommandLine;
using System.Threading.Tasks;
using TextTools.CommandBuilders;

internal class Program
{
   private static async Task<int> Main(string[] args)
   {
      var rootCommand = new RootCommandBuilder().BuildRootCommand();
		return await rootCommand.InvokeAsync(args);
	}
}