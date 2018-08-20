using System.Threading.Tasks;
using ImageDL.Classes.ImageComparing.Implementations;

namespace ImageDL
{
	public class Program
	{
		public static async Task Main(string[] args)
		{
			await new ImageDL().RunFromArguments(ImageDL.CreateServices<ImageSharpImageComparer>(), args);
		}
	}
}