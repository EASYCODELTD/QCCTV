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
			Application.Init ();
			

			
			MainWindow win = new MainWindow ();
			win.Resize (1024, 768);
			
			win.Show ();
			Application.Run ();
		}
		
	
	}
}
