using System;
using System.Runtime.InteropServices;

namespace Windows {

	public static class libdl
	{
		[DllImport("libdl.so")]
		public static extern IntPtr dlopen(string filename, int flags);

		[DllImport("libdl.so")]
		public static extern IntPtr dlsym(IntPtr handle, string symbol);

		const int RTLD_NOW = 2; // for dlopen's flags 

	}
}
