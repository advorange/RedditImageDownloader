using System.Threading.Tasks;
using ImageDL.Utilities;

namespace ImageDL.Windows
{
	public class Program
	{
		public static async Task Main(string[] args)
		{
			await new ImageDL().RunFromArguments(DIUtils.CreateServices<WindowsImageComparer>(), args);
		}
	}
}