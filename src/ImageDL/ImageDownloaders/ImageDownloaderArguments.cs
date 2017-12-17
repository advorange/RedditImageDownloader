using ImageDL.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;

namespace ImageDL.ImageDownloaders
{
	/// <summary>
	/// The arguments that <see cref="ImageDownloader{T}"/> uses.
	/// </summary>
	public abstract class ImageDownloaderArguments
	{
		protected static Dictionary<Type, Func<string, object>> _TryParses = new Dictionary<Type, Func<string, object>>
		{
			{ typeof(bool), (value) => bool.TryParse(value, out var result) ? result : false },
			{ typeof(int), (value) => int.TryParse(value, out var result) ? result : default },
			{ typeof(int?), (value) => int.TryParse(value, out var result) ? result as int? : null },
			{ typeof(uint), (value) => uint.TryParse(value, out var result) ? result : default },
			{ typeof(uint?), (value) => uint.TryParse(value, out var result) ? result as uint? : null },
			{ typeof(long), (value) => long.TryParse(value, out var result) ? result : default },
			{ typeof(long?), (value) => long.TryParse(value, out var result) ? result as long? : null },
			{ typeof(ulong), (value) => ulong.TryParse(value, out var result) ? result : default },
			{ typeof(ulong?), (value) => ulong.TryParse(value, out var result) ? result as ulong? : null },
		};

		protected PropertyInfo[] _Arguments = new PropertyInfo[0];
		protected PropertyInfo[] _UnsetArguments => _Arguments.Where(x => !_SetArguments.Contains(x.Name)).ToArray();
		protected List<string> _SetArguments = new List<string>();
		public bool IsReady
		{
			get
			{
				if (!String.IsNullOrWhiteSpace(Directory) && !System.IO.Directory.Exists(Directory))
				{
					Console.WriteLine($"{Directory} does not exist as a directory.");
					return false;
				}
				return !_UnsetArguments.Any();
			}
		}

		private string _Directory;
		public string Directory
		{
			get => _Directory;
			set
			{
				_Directory = value;
				AddArgumentToSetArguments();
			}
		}
		private int _AmountToDownload;
		public int AmountToDownload
		{
			get => Math.Max(1, _AmountToDownload);
			set
			{
				_AmountToDownload = value;
				AddArgumentToSetArguments();
			}
		}
		private int _MinWidth;
		public int MinWidth
		{
			get => _MinWidth;
			set
			{
				_MinWidth = value;
				AddArgumentToSetArguments();
			}
		}
		private int _MinHeight;
		public int MinHeight
		{
			get => _MinHeight;
			set
			{
				_MinHeight = value;
				AddArgumentToSetArguments();
			}
		}

		public ImageDownloaderArguments(string[] args, Type type)
		{
			if (!typeof(ImageDownloaderArguments).IsAssignableFrom(type))
			{
				throw new ArgumentException($"{nameof(type)} must be derived from {nameof(ImageDownloaderArguments)}.");
			}

			_Arguments = type.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.FlattenHierarchy)
				.Where(x => (x.PropertyType.IsPrimitive || x.PropertyType == typeof(string)) && x.SetMethod != null)
				.OrderByNonComparable(x => x.PropertyType)
				.ToArray();
			SetArguments(args);
		}

		/// <summary>
		/// Sets the arguments that can be gathered from <paramref name="args"/>.
		/// </summary>
		/// <param name="args">The supplied arguments.</param>
		public void SetArguments(string[] args)
		{
			foreach (var argument in args)
			{
				//Split, left side is the arg name, right is value
				var split = argument.Split(new[] { ':' }, 2);
				if (split.Length != 2)
				{
					Console.WriteLine($"Unable to split \"{argument}\" to the correct length.");
					continue;
				}

				//See if any arguments have the supplied name
				var property = _Arguments.SingleOrDefault(x => x.Name.CaseInsEquals(split[0]));
				if (property == null)
				{
					Console.WriteLine($"{split[0]} is not a valid argument name.");
					continue;
				}

				//If number then use the tryparses, if string just set, if neither then nothing
				if (_TryParses.TryGetValue(property.PropertyType, out var f))
				{
					property.SetValue(this, f(split[1]));
				}
				else if (property.PropertyType == typeof(string))
				{
					property.SetValue(this, split[1]);
				}
				else
				{
					Console.WriteLine($"Unable to set the value for {property.Name}.");
					continue;
				}

				Console.WriteLine($"Successfully set {property.Name} to {property.GetValue(this)}.");
			}
		}
		/// <summary>
		/// Prints out to the console what arguments are still needed.
		/// </summary>
		public void AskForArguments()
		{
			if (!_UnsetArguments.Any())
			{
				return;
			}

			var sb = new StringBuilder("The following arguments need to be set:" + Environment.NewLine);
			foreach (var argument in _UnsetArguments)
			{
				sb.AppendLine($"\t{argument.Name} ({argument.PropertyType.Name})");
			}
			Console.WriteLine(sb.ToString().Trim());
		}
		protected void AddArgumentToSetArguments([CallerMemberName] string name = "")
		{
			if (!_SetArguments.Contains(name))
			{
				_SetArguments.Add(name);
			}
		}
	}
}
