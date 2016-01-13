using System;
using QCCTVDefs;


namespace QCCTV
{
	public class CDevice : IDevice
	{
		public override event NewFrameEventHandler NewFrameEvent=null;
		public override event ConnectEventHandler ConnectEvent=null;
		public override event DisconnectEventHandler DisconnectEvent=null;

		public CDevice ()
		{
			this.AddLog ("Device created");
			process = false;

		}
		
		public override void startRecording ()
		{

		}

		public override void stopRecording ()
		{

		}

		public override string getDeviceName () { return DeviceName; }
		public override string getDeviceURI ()
		{ 
			if (driver == null) return "";
			return driver.URI; 
		}


		public override void Connect() { 
			if (this.driver != null) {
				this.driver.Connect ();
				this.startRecording ();
			}
		}
		public override void Disconnect() { 
			if (this.driver != null) {
				this.stopRecording ();
				this.driver.Disconnect ();
			}
		}

		public override bool hasControls() {
			if (this.driver != null) {
				return this.driver.hasControls ();
			}
			return false;
		}

		public override void control(int what,int how)
		{
			if (this.driver != null) {
				this.driver.control (what,how);
			}
		}

		public override bool TestConnection ()
		{ 
			if (this.driver == null) {

				this.AddLog ("Device: No driver");

				return false;
			}

			this.AddLog ("Device: \"" + DeviceName+"\"");
			this.AddLog ("Driver: \"" + driver.getDriverName ()+"\"");
			return this.driver.TestConnection ();
		}

		public override void shutdown ()
		{
			if (driver != null) {
				driver.shutdown ();
			}
		}		

		public override string GetLog () { return Log; }
		public override void AddLog (string text)
		{ 
			DateTime now = DateTime.UtcNow;

			Log += now.ToString () + ": " + text + "\n"; 
			Console.WriteLine (text);
		}

		public override void OnFrameReady (byte[] frame) {
			Console.WriteLine ("got");
		}

		public override void frameReady (byte[] frame)
		{
			
			if (process) {
				return;
			}
			process = true;


			Gtk.Application.Invoke(delegate(object sender, EventArgs e) {

				if(NewFrameEvent!=null && driver != null && frame != null )
				{
					NewFrameEvent(this,new ICameraNewFrameEventArgs(driver,frame));
				}
				process = false;
			});				


		}
	}
}

