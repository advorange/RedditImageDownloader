using System.Threading.Tasks;

namespace ImageDL
{
	public class Program
	{
		public static Task Main(string[] args)
			=> new ImageDL().RunFromArguments(ImageDL.CreateServices<NetFrameworkImageComparer>(), args);
	}
}