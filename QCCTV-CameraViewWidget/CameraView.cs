using System;
using Gdk;
using Gtk;
using System.Collections.Generic;
using QCCTVDefs;
using System.Net;
using System.Diagnostics;

	
namespace QCCTVCameraViewWidget
{
	[System.ComponentModel.ToolboxItem(true)]
	public partial class CameraView : Gtk.Bin
	{
		
		
		private int windowWidth = 340;
		private int windowHeight = 190;
		private Gdk.Pixbuf internalBuffer = null;
		private bool inResize=false;
		private bool controlsVisible=true;
		
		private List<IDevice> _devices=null;
		private ListStore devicelistStore = null;
		private IDevice currentDevice = null;
		private bool dontrefresh;
		private Gtk.Image offImage;

		Gdk.Color textColor;
		Gdk.Color bgColor;

		Stopwatch timer;

		int currentFps=0;
		int _fps=0;

		public CameraView ()
		{
			this.Build ();

			devicelistStore = new Gtk.ListStore (typeof(string), typeof(IDevice)); 
			CellRendererText ct = new CellRendererText ();
			
			
			source_box.PackStart (ct, false); 
			source_box.AddAttribute (ct, "text", 0);
			source_box.Model = devicelistStore;

			textColor = new Gdk.Color();
			bgColor = new Gdk.Color();

			Gdk.Color.Parse("white", ref textColor);
			Gdk.Color.Parse("black", ref bgColor);

			bgstats.ModifyBg (StateType.Normal,bgColor);
			bgstats.ModifyBase(StateType.Normal,bgColor);

			IPaddresLabel.ModifyBase(StateType.Normal,bgColor);
			IPaddresLabel.ModifyBg(StateType.Normal,bgColor);
			IPaddresLabel.ModifyFg (StateType.Normal,textColor);

			NameLabel.ModifyFg (StateType.Normal,textColor);
			FpsLabel.ModifyFg (StateType.Normal,textColor);
			DriverLabel.ModifyFg (StateType.Normal,textColor);

			timer = new Stopwatch ();
			timer.Start ();
		}

		~CameraView ()
		{
			
		}



		public void setOffImage(Gtk.Image offImage)
		{
			this.offImage = offImage;
			this.buffer = offImage.Pixbuf;
		}

		public List<IDevice> devicelist {
			get { return _devices;  } 		
			set {
				_devices = value;
				refreshDeviceList ();
			}
		}
	
		public int fps {
			get { return _fps; } 		
			set {
				_fps = value;
				FpsLabel.Text = "Fps: " + _fps;
				fpslabel.Text = ""+_fps;
			}
		}
		public bool controlVisible {
			get { return controlsVisible;  } 		
			set {
				controlsVisible = value; 
				optionbox.Visible = value;
				topbox.Visible = value;
				bottombox.Visible = value;
				
				RecordButton.Visible = value;
				source_box.Visible = value;
				MotionButton.Visible = value;
				
				activity.Visible = value;
				sensitivity.Visible = value;
				fpslabel.Visible = value;
				detect.Visible = value;
				record.Visible = value;
				
				_Resize ();
			} 
		}
		
		public Gdk.Pixbuf buffer {
			get { return internalBuffer;  } 		
			set {
				internalBuffer = value;
				//	_refreshOutput (); 
				output.Pixbuf = internalBuffer;
				_refreshOutput (); 
			} 
		}

		public int width {
			get { return windowWidth;  } 		
			set {
				windowWidth = value; 
				_Resize ();
			} 
		}

		public int height {
			get { return windowHeight;  } 		
			set { windowHeight = value;
				_Resize ();
			} 
		}
		
		private void _Resize ()
		{
			output.SetSizeRequest (windowWidth, windowHeight);
			_refreshOutput ();
		}
		
		private void refreshDeviceList ()
		{
			dontrefresh = true;
			
			Gtk.TreeIter iter;
			Gtk.TreeIter citer;
									
			devicelistStore.Clear ();
			devicelistStore.AppendValues ("Select camera", null);
			devicelistStore.GetIterFirst (out iter);	
			
					
			foreach (IDevice device in _devices) {
				citer = devicelistStore.AppendValues (device.getDeviceName (), device);
				if (currentDevice != null) {
					if (device.getDeviceName () == currentDevice.getDeviceName ()) {
						iter = citer;
						
						
					} 
				}
			}
				
			
			
			source_box.SetActiveIter (iter);
			dontrefresh = false;
		}
		
				
		protected void OnSelectDeviceChanged (object sender, EventArgs a)
		{

			if (dontrefresh)
				return;
			
			IDevice device = null;
			
			TreeIter iter;
					
			if (source_box.GetActiveIter (out iter)) {
				
				if (devicelistStore.GetValue (iter, 0) != null) {
					device = (IDevice)devicelistStore.GetValue (iter, 1);
				}
		
			}
		
			
			this.setCurrentDevice (device);
			
		}

		
		public void ConnectEventHandler (object sender, ICameraEventArgs e)
		{
			Console.WriteLine ("CONNECT");
		}
		
		
		public void DisconnectEventHandler (object sender, ICameraEventArgs e)
		{
			Console.WriteLine ("DISCONNECT");
		}
		
		public void NewFrameEventHandler (object sender, ICameraNewFrameEventArgs e)
		{
	

			if (e.frame != null && e.frame.Length > 0) {

				if (buffer != null) {
					buffer.Dispose ();
					buffer = null;
				}

				buffer = new Gdk.Pixbuf(e.frame);

			
				long inmicroseconds = timer.ElapsedTicks / (Stopwatch.Frequency / (1000L * 1000L));
				currentFps++;

				if (inmicroseconds > 1000000) {
					

					fps = currentFps;

					currentFps = 0;
					timer.Restart ();
				}

			} else {
				Console.WriteLine ("EMPTY FRAME");
			}
		}
		
		public void setCurrentDevice (IDevice device)
		{
			
			if (currentDevice != null) {
				
				
				currentDevice.NewFrameEvent -= NewFrameEventHandler;
				currentDevice.ConnectEvent -= ConnectEventHandler;
				currentDevice.DisconnectEvent -= DisconnectEventHandler;	
	
			
			}

			if(offImage!=null) buffer = offImage.Pixbuf;

			currentDevice = device;
			if (currentDevice != null) {
				
				currentDevice.NewFrameEvent += NewFrameEventHandler;
				currentDevice.ConnectEvent += ConnectEventHandler;
				currentDevice.DisconnectEvent += DisconnectEventHandler;		
			
				if (currentDevice.hasControls ()) {
					controls.Show ();

				} else {
					controls.Hide ();

				}

				string ip = currentDevice.getDeviceURI ();
				try {
					Uri address = new Uri(currentDevice.getDeviceURI ());
					ip = address.Host;
				} catch (Exception e) {
				}

				IPaddresLabel.Text = "Address: "+ip;
				NameLabel.Text = "Name: "+currentDevice.getDeviceName ();
				FpsLabel.Text = "Fps: 0";
				if(currentDevice.driver!=null)
				{
					DriverLabel.Text = currentDevice.driver.getDriverName ();
				}
			}
			
			
			refreshDeviceList ();
		}
		
		
		public IDevice getCurrentDevice ()
		{
			return currentDevice;
		}
		
		private void _refreshOutput ()
		{
			if (inResize)
				return;
			
			//Console.WriteLine ("Request resize buffer: " + windowWidth + " " + windowHeight);
			
			if (windowWidth > 0 && windowHeight > 0 && internalBuffer != null && output.Pixbuf != null && (output.Pixbuf.Width != windowWidth || output.Pixbuf.Height != windowHeight)) {
				inResize = true;

				mainBox.SetSizeRequest (windowWidth, windowHeight);
				
				//Console.WriteLine ("Resize buffer: " + windowWidth + " " + windowHeight);
				fixbox.SetSizeRequest(windowWidth, windowHeight);
				fixbox.Move (controls,windowWidth-90,windowHeight-90);


				statsBox.SetSizeRequest(windowWidth, 30);
				fixbox.Move (statsBox, 0, 0);
				bgstats.SetSizeRequest (windowWidth, 20);

				optionbox.SetSizeRequest (windowWidth, 30);

				setBox.SetSizeRequest (windowWidth, 30);
				fixbox.Move (setBox, 5, windowHeight - 40);

				output.Pixbuf = internalBuffer.ScaleSimple (windowWidth, windowHeight, InterpType.Bilinear);
				inResize = false;
			} 
		}
		
		public void Resize (uint width, uint height)
		{
			windowWidth = (int)width;
			windowHeight = (int)height;
			_Resize ();
		}
		
	
		protected void OnMotionButtonClicked (object sender, EventArgs e)
		{
			throw new NotImplementedException ();
		}


		/******************************/
		protected void OnButton2Clicked (object sender, EventArgs e)
		{
			

		}

		protected void OnButton1Clicked (object sender, EventArgs e)
		{
			
		}

		protected void OnButton3Clicked (object sender, EventArgs e)
		{
			
		}

		protected void OnButton4Clicked (object sender, EventArgs e)
		{
			
		}

		protected void OnButton2Released (object sender, EventArgs e)
		{
			if (currentDevice!=null) {
				currentDevice.control (0, 10); //Down
			}
		}

		protected void OnButton1Released (object sender, EventArgs e)
		{
			if (currentDevice!=null) {
				currentDevice.control (0, 10); //Down
			}
		}

		protected void OnButton3Released (object sender, EventArgs e)
		{
			if (currentDevice!=null) {
				currentDevice.control (0, 10); //Down
			}
		}

		protected void OnButton4Released (object sender, EventArgs e)
		{
			if (currentDevice!=null) {
				currentDevice.control (0, 10); //Down
			}
		}

		protected void OnButton2Pressed (object sender, EventArgs e)
		{
			if (currentDevice!=null) {
				currentDevice.control (1, 10); //Down
			}
		}

		protected void OnButton1Pressed (object sender, EventArgs e)
		{
			if (currentDevice!=null) {
				currentDevice.control (2, 10); //Down
			}
		}

		protected void OnButton3Pressed (object sender, EventArgs e)
		{
			if (currentDevice!=null) {
				currentDevice.control (3, 10); //Down
			}
		}

		protected void OnButton4Pressed (object sender, EventArgs e)
		{
			if (currentDevice!=null) {
				currentDevice.control (4, 10); //Down
			}
		}
	}
}

