using System;
using CQCCTV.Drivers;
using System.Threading;
using System.IO;


namespace QCCTVDefs
{



	public delegate void NewFrameEventHandler(object sender, ICameraNewFrameEventArgs e);
	public delegate void ConnectEventHandler(object sender, ICameraEventArgs e);
	public delegate void DisconnectEventHandler(object sender, ICameraEventArgs e);
	
	public class IDevice
	{
		public  virtual event NewFrameEventHandler NewFrameEvent=null;
		public  virtual event ConnectEventHandler ConnectEvent=null;
		public  virtual event DisconnectEventHandler DisconnectEvent=null;
	
		public string DeviceName = "Empty Device";
		
		public string videoPath = "";

		public string Log = "";
		public ICameraDriver driver;
		public Gtk.Image currentFrame;
	
		public bool process;

		public byte[] current_frame=null;


		public virtual void startRecording (){ throw new MissingMethodException (); }
		public virtual void stopRecording (){ throw new MissingMethodException ();  }
		public virtual string getDeviceName () { throw new MissingMethodException ();  return ""; }
		public virtual string getDeviceURI (){ throw new MissingMethodException ();  return ""; }
		public virtual void Connect() { throw new MissingMethodException ();  }
		public virtual void Disconnect() { throw new MissingMethodException ();  }
		public virtual bool hasControls() { throw new MissingMethodException ();  return false; }
		public virtual void control(int what,int how){ throw new MissingMethodException (); }
		public virtual bool TestConnection (){ return false;  }
		public virtual void shutdown () {}		
		public virtual string GetLog () { throw new MissingMethodException ();  return ""; }
		public virtual void AddLog (string text) { throw new MissingMethodException ();  }
		public virtual void OnFrameReady (byte[] frame) { throw new MissingMethodException ();  }
		public virtual void frameReady (byte[] frame) { throw new MissingMethodException (); }
	}
}

