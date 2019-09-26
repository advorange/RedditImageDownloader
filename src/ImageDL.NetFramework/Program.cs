using System.Threading.Tasks;

namespace ImageDL
{
	public class Program
	{
		public static Task Main(string[] args)
		{
			var services = ImageDL.CreateServices<NetFrameworkImageComparer>();
			return new ImageDL().RunFromArguments(services, args);
		}
	}
}