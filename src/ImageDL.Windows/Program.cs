using System.Threading.Tasks;

namespace ImageDL.Windows
{
	public class Program
	{
		public static async Task Main(string[] args)
		{
			await new ImageDL().RunFromArguments(ImageDL.CreateServices<WindowsImageComparer>(), args);
		}
	}
}