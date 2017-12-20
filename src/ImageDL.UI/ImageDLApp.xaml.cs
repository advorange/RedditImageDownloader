using ImageDL.UI.Classes;
using System;
using System.IO;
using System.Windows;
using System.Windows.Threading;

namespace ImageDL.UI
{
	/// <summary>
	/// Interaction logic for ImageDLApp.xaml
	/// </summary>
	public partial class ImageDLApp : Application
	{
		private BindingListener _Listener = new BindingListener();

		public ImageDLApp()
		{
			InitializeComponent();
		}

		public void OnStartup(object sender, StartupEventArgs e)
			=> DispatcherUnhandledException += OnDispatcherUnhandledException;

		private void OnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
		{
			//Display to the user what happened and also log it
			MessageBox.Show($"UNHANDLED EXCEPTION:\n\n{e.Exception}", "UNHANDLED EXCEPTION", MessageBoxButton.OK, MessageBoxImage.Error);
			var appdata = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
			var path = Path.Combine(appdata, "ImageDL", "Exceptions.txt");
			using (var writer = new FileInfo(path).AppendText())
			{
				writer.WriteLine(e.Exception.ToString() + Environment.NewLine);
			}
			e.Handled = true;
			Shutdown();
		}
	}
}
