using System.Threading.Tasks;

using ImageDL.Classes.ImageComparing.Implementations;

namespace ImageDL
{
	public static class Program
	{
		public static Task Main(string[] args)
		{
			var services = ImageDL.CreateServices<ImageSharpImageComparer>();
			return new ImageDL().RunFromArguments(services, args);
		}
	}
}