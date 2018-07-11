using System;
using ImageDL.Classes.ImageComparing;
using ImageDL.Classes.ImageDownloading;
using ImageDL.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace ImageDL.Utilities
{
	/// <summary>
	/// Utilities for dependency injection.
	/// </summary>
	public static class DIUtils
	{
		/// <summary>
		/// Creates a service provider using the specified image comparer.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		public static IServiceProvider CreateServices<T>() where T : IImageComparer
		{
			//Services used when downloading. Client should be constant, but comparer should be discarded after each use.
			return new DefaultServiceProviderFactory().CreateServiceProvider(new ServiceCollection()
				.AddSingleton<IDownloaderClient, DownloaderClient>()
				.AddSingleton<IImageComparerFactory, ImageComparerFactory<T>>());
		}
	}
}