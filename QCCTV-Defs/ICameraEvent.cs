using System;
using CQCCTV.Drivers;


namespace QCCTVDefs
{
	public class ICameraEventArgs : EventArgs
	{
		public ICameraDriver driver=null;
		
	}
	
	public class ICameraNewFrameEventArgs : ICameraEventArgs
	{
		public ICameraNewFrameEventArgs (ICameraDriver driver, Gtk.Image frame)
		{
			this.driver = driver; this.frame = frame; }
		public Gtk.Image frame=null;
	}
	
	
}

