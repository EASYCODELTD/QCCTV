using System;
using Gtk;
using System.Reflection;
using System.IO;

namespace QCCTV
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			if (Environment.OSVersion.Platform != PlatformID.Unix) {
				Windows.kernel32.AttachConsole (Windows.kernel32.ATTACH_PARENT_PROCESS);
			}
			try {
				
			Application.Init ();

			MainWindow win = new MainWindow ();
			win.Resize (1024, 768);
			
			win.Show ();
			Application.Run ();
			} catch (FileNotFoundException ex) {
				Console.Error.WriteLine (ex.Message);

			}
		}
		
	
	}
}
