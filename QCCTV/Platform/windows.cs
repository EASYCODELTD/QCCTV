using System;
using System.Runtime.InteropServices;

namespace Windows {
	
	public static class kernel32
	{
		
		[DllImport("kernel32.dll")]
		public static extern bool AttachConsole(int dwProcessId);
		public const int ATTACH_PARENT_PROCESS = -1;

		[DllImport("kernel32.dll", SetLastError = true)]
		public static extern int FreeConsole();

		[DllImport("kernel32.dll")]
		public static extern IntPtr LoadLibrary(string filename);

		[DllImport("kernel32.dll")]
		public static extern IntPtr GetProcAddress(IntPtr hModule, string procname);
	}
}