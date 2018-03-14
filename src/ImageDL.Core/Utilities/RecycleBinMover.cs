using System;
using System.IO;
using System.Runtime.InteropServices;

namespace ImageDL.Core.Utilities
{
	/// <summary>
	/// Sends a file to the recycle bin on Windows. Source: https://stackoverflow.com/a/3282450
	/// </summary>
	internal static class RecycleBinMover
	{
		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto, Pack = 1)]
		public struct SHFILEOPSTRUCT
		{
			public IntPtr hwnd;
			[MarshalAs(UnmanagedType.U4)]
			public int wFunc;
			public string pFrom;
			public string pTo;
			public short fFlags;
			[MarshalAs(UnmanagedType.Bool)]
			public bool fAnyOperationsAborted;
			public IntPtr hNameMappings;
			public string lpszProgressTitle;
		}

		[DllImport("shell32.dll", CharSet = CharSet.Auto)]
		public static extern int SHFileOperation(ref SHFILEOPSTRUCT FileOp);

		public const int FO_DELETE = 3;
		public const int FOF_ALLOWUNDO = 0x40;
		public const int FOF_NOCONFIRMATION = 0x10; //Don't prompt the user

		public static void MoveFile(FileInfo file)
		{
			var shf = new SHFILEOPSTRUCT
			{
				wFunc = FO_DELETE,
				fFlags = FOF_ALLOWUNDO | FOF_NOCONFIRMATION,
				pFrom = file.FullName
			};
			SHFileOperation(ref shf);
		}
	}
}
