using System;
using System.Diagnostics;
using System.Reflection;

namespace ImageDL.UI.Classes
{
	//Source: http://www.jasonbock.net/jb/News/Item/0f221e047de740ee90722b248933a28d
	internal sealed class BindingListener : DefaultTraceListener
	{
		private const TraceOptions OPTIONS = 0
			| TraceOptions.Callstack
			| TraceOptions.DateTime
			| TraceOptions.LogicalOperationStack
			| TraceOptions.ProcessId
			| TraceOptions.ThreadId
			| TraceOptions.Timestamp;
		private const BindingFlags PROPERTY_FLAGS = 0
			| BindingFlags.IgnoreCase
			| BindingFlags.NonPublic
			| BindingFlags.Instance;

		private int InformationPropertyCount { get; set; }
		private string Callstack { get; set; }
		private string DateTime { get; set; }
		private string LogicalOperationStack { get; set; }
		private string Message { get; set; }
		private string ProcessId { get; set; }
		private string ThreadId { get; set; }
		private string Timestamp { get; set; }

		public BindingListener() : base()
		{
			PresentationTraceSources.Refresh();
			PresentationTraceSources.DataBindingSource.Listeners.Add(this);
			PresentationTraceSources.DataBindingSource.Switch.Level = SourceLevels.Error;

			TraceOutputOptions = OPTIONS;
			foreach (TraceOptions traceOption in Enum.GetValues(typeof(TraceOptions)))
			{
				if (traceOption == TraceOptions.None)
				{
					continue;
				}
				InformationPropertyCount += (TraceOutputOptions.HasFlag(traceOption) ? 1 : 0);
			}
		}

		public override void WriteLine(string message)
		{
			if (Message == null)
			{
				Message = message;
			}
			else
			{
				var propertyInformation = message.Split(new string[] { "=" }, StringSplitOptions.None);

				if (propertyInformation.Length == 1)
				{
					LogicalOperationStack = propertyInformation[0];
				}
				else
				{
					GetType().GetProperty(propertyInformation[0], PROPERTY_FLAGS).SetValue(this, propertyInformation[1], null);
				}

				--InformationPropertyCount;
			}

			Flush();
			if (InformationPropertyCount == 0)
			{
				PresentationTraceSources.DataBindingSource.Listeners.Remove(this);

				var exceptionMessage = $"{Message}\n" +
					$"Time: {DateTime}\n" +
					$"Logical Operation Stack: {LogicalOperationStack}\n" +
					$"Process Id: {ProcessId}\n" +
					$"Thread Id: {ThreadId}\n" +
					$"Timestamp: {Timestamp}\n" +
					$"Callstack: {Callstack}";
				throw new Exception(exceptionMessage);
			}
		}
	}
}
