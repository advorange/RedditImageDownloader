using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ImageDL.Interfaces
{
	/// <summary>
	/// Interface for something that can gather posts.
	/// </summary>
	public interface IPostGatherer
	{
		/// <summary>
		/// Gathers the posts which match the supplied settings.
		/// </summary>
		/// <param name="services">The client to gather posts with.</param>
		/// <param name="token">Cancels gathering.</param>
		/// <returns></returns>
		Task<List<IPost>> GatherAsync(IServiceProvider services, CancellationToken token = default);
	}
}