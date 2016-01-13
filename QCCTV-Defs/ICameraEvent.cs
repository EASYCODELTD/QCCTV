using System;
using CQCCTV.Drivers;


namespace QCCTVDefs
{
	public class ICameraEventArgs : System.EventArgs
	{
		public ICameraDriver driver=null;
		
	}
	
	public class ICameraNewFrameEventArgs : ICameraEventArgs
	{
		public ICameraNewFrameEventArgs ( ICameraDriver driver,  byte[] frame)
		{
			this.driver = driver; this.frame = frame; }
		public byte[] frame=null;
	}
	
	
}

