using System.Threading.Tasks;
using ImageDL.Classes.ImageComparing.Implementations;

namespace ImageDL
{
	public class Program
	{
		public static Task Main(string[] args)
			=> new ImageDL().RunFromArguments(ImageDL.CreateServices<ImageSharpImageComparer>(), args);
	}
}