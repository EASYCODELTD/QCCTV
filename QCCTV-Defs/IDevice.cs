using System;
using CQCCTV.Drivers;
using System.Threading;


namespace QCCTVDefs
{
	
	public delegate void NewFrameEventHandler(object sender, ICameraNewFrameEventArgs e);
	public delegate void ConnectEventHandler(object sender, ICameraEventArgs e);
	public delegate void DisconnectEventHandler(object sender, ICameraEventArgs e);
	
	public class IDevice
	{
			public  event NewFrameEventHandler NewFrameEvent=null;
			public  event ConnectEventHandler ConnectEvent=null;
			public  event DisconnectEventHandler DisconnectEvent=null;
		
			public string DeviceName = "Empty Device";
			
			public string Log = "";
			public ICameraDriver driver;
			public Gtk.Image currentFrame;
		
			private bool process;
		
			public virtual string getDeviceName () { return DeviceName; }
			public virtual string getDeviceURI ()
			{ 
				if (driver == null) return "";
				return driver.URI; 
			}
			
			
			public virtual void Connect() { }
			public virtual void Disconnect() { }

			public virtual bool hasControls() {
				if (this.driver != null) {
					return this.driver.hasControls ();
				}
				return false;
			}

			public virtual void control(int what,int how)
			{
				if (this.driver != null) {
				this.driver.control (what,how);
				}
			}

			public virtual bool TestConnection ()
		{ 
			if (this.driver == null) {
					
				this.AddLog ("Device: No driver");
					
				return false;
			}
			
				this.AddLog ("Device: \"" + DeviceName+"\"");
				this.AddLog ("Driver: \"" + driver.getDriverName ()+"\"");
				return this.driver.TestConnection ();
			}

			public virtual void shutdown ()
		{
			if (driver != null) {
				driver.shutdown ();
				}
			}		
		
			public virtual string GetLog () { return Log; }
			public virtual void AddLog (string text)
		{ 
			DateTime now = DateTime.UtcNow;
			
			Log += now.ToString () + ": " + text + "\n"; 
			Console.WriteLine (text);
		}


		
		public void frameReady (Gtk.Image frame)
		{
			if (process)
				return;
			
			process = true;
			Gtk.Application.Invoke (delegate {
			
				
				if (NewFrameEvent != null) {
					
					NewFrameEvent (this, new ICameraNewFrameEventArgs (driver, frame));
				}
		
				process = false;
			}
			);
			
			while (process) {
				Thread.Sleep (1);
			}
		}
	}
}

