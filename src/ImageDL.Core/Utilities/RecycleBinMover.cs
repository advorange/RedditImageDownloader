using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;

namespace ImageDL.Core.Utilities
{
	/// <summary>
	/// Sends a file to the recycle bin on Windows. Source: https://stackoverflow.com/a/3282450
	/// </summary>
	public static class RecycleBinMover
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

		/// <summary>
		/// Utilizes <see cref="SHFileOperation(ref SHFILEOPSTRUCT)"/> to move a file to the recycle bin with undo preservation and no confirmation.
		/// </summary>
		/// <param name="file"></param>
		public static int MoveFile(FileInfo file)
		{
			return Move(file.FullName);
		}
		/// <summary>
		/// Utilizes <see cref="SHFileOperation(ref SHFILEOPSTRUCT)"/> to move multiple files to the recycle bin with undo preservation and no confirmation.
		/// </summary>
		/// <param name="files"></param>
		public static int MoveFiles(IEnumerable<FileInfo> files)
		{
			//Files need to be joined with null char and entire string needs to end with it too
			return Move(String.Join("\0", files.Select(x => x.FullName)) + "\0");
		}
		private static int Move(string input)
		{
			var shf = new SHFILEOPSTRUCT
			{
				wFunc = FO_DELETE,
				fFlags = FOF_ALLOWUNDO | FOF_NOCONFIRMATION,
				pFrom = input
			};
			return SHFileOperation(ref shf);
		}
	}
}
