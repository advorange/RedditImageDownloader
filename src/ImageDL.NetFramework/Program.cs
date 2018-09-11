using System.Threading.Tasks;

namespace ImageDL
{
	public class Program
	{
		public static async Task Main(string[] args) => await new ImageDL().RunFromArguments(ImageDL.CreateServices<NetFrameworkImageComparer>(), args);
	}
}