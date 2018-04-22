using System;
using System.Threading.Tasks;

namespace ImageDL.Interfaces
{
	/// <summary>
	/// Attempts to do a method relating to downloading images.
	/// </summary>
	public interface IMethodRunner
	{
		/// <summary>
		/// Runs the specified method.
		/// </summary>
		/// <param name="services"></param>
		/// <returns></returns>
		Task RunAsync(IServiceProvider services);
	}
}