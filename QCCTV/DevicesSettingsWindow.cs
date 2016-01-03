using System;
using Gtk;
using CQCCTV.Drivers;
using QCCTVDefs;


namespace QCCTV
{
	public partial class DevicesSettingsWindow : Gtk.Window
	{
		private MainWindow Main=null;
		private ListStore driverlistStore = null;
		private ListStore devicelistStore = null;
		private ListStore hardwarelistStore = null;
		
		private IDevice currentEditDevice = null;
		
		private Gtk.TreeViewColumn deviceColumn = null;
		private Gtk.TreeViewColumn ipColumn = null;
		private Gtk.TreeViewColumn statusColumn=null;
		private bool dontchange;
		
		public DevicesSettingsWindow (MainWindow parent) : 
				base(Gtk.WindowType.Toplevel)
		{
			this.Build ();
			this.Main = parent;
			
			
			deviceColumn = new Gtk.TreeViewColumn ();
			deviceColumn.Title = "Device Name";
			
			ipColumn = new Gtk.TreeViewColumn ();
			ipColumn.Title = "URI";

			statusColumn = new Gtk.TreeViewColumn ();
			statusColumn.Title = "Driver";
			
			deviceListsBox.AppendColumn (deviceColumn);
			deviceListsBox.AppendColumn (ipColumn);
			deviceListsBox.AppendColumn (statusColumn);
			
			devicelistStore = new Gtk.ListStore (typeof(string), typeof(string), typeof(string), typeof(IDevice));
			deviceListsBox.Model = devicelistStore;
							
			Gtk.CellRendererText nameCell = new Gtk.CellRendererText ();
			deviceColumn.PackStart (nameCell, true);
				
			Gtk.CellRendererText uriCell = new Gtk.CellRendererText ();
			ipColumn.PackStart (uriCell, true);
								
			Gtk.CellRendererText statusCell = new Gtk.CellRendererText ();
			statusColumn.PackStart (statusCell, true);
				
			deviceColumn.AddAttribute (nameCell, "text", 0);
			ipColumn.AddAttribute (uriCell, "text", 1);
			statusColumn.AddAttribute (statusCell, "text", 2);

			//Driver combo box
		
			
			driverlistStore = new Gtk.ListStore (typeof(string), typeof(ICameraDriver)); 
			CellRendererText ct = new CellRendererText (); 
			driverListBox.PackStart (ct, false); 
			driverListBox.AddAttribute (ct, "text", 0);
			driverListBox.Model = driverlistStore;
			
	
			
			//Autodiscover combo box
		
			
			hardwarelistStore = new Gtk.ListStore (typeof(string), typeof(string)); 
			CellRendererText ct1 = new CellRendererText (); 
			hardwareListBox.PackStart (ct1, false); 
			hardwareListBox.AddAttribute (ct1, "text", 0);
			hardwareListBox.Model = hardwarelistStore;
		}
		
		protected void OnDeleteEvent (object sender, DeleteEventArgs a)
		{
			
			a.RetVal = false;
		}
		
		
		protected void OnShowWindow (object sender, EventArgs a)
		{
			refreshDevicesList ();
			
		}
		
		protected void OnCloseButton (object sender, EventArgs a)
		{
			this.Visible = false;		
			Main.loadSettings ();
			
		}
		
		private void refreshEditList (bool driverList=true)
		{
			dontchange = true;
			nameField.Text = "";
			uriField.Text = "";
			loginField.Text = "";
			passwordField.Text = "";
			
			//Get drivers settings
			
			driverlistStore.Clear ();
		
			TreeIter titer;
			Gtk.TreeIter iter;
			driverlistStore.GetIterFirst (out iter);

			foreach (ICameraDriver driver in Main.drivers) {
				
				titer = driverlistStore.AppendValues (driver.getDriverName (), driver);
				if (currentEditDevice != null && currentEditDevice.driver != null && currentEditDevice.driver.getDriverName () == driver.getDriverName ()) {
					iter = titer;
				}
			}
			
			
			if (currentEditDevice == null || currentEditDevice.driver == null) {
				driverlistStore.GetIterFirst (out iter);
			}
			
			driverListBox.SetActiveIter (iter);
			
			if (currentEditDevice != null) {
				nameField.Text = currentEditDevice.getDeviceName ();
				Console.WriteLine ("Device: " + currentEditDevice.getDeviceName ());
				
			} else {
				Console.WriteLine ("No device");
			}
			
			if (currentEditDevice.driver != null) {
				Console.WriteLine ("Read data driver: " + currentEditDevice.driver.DriverName);
				
				uriBox.Sensitive = currentEditDevice.driver.isNetworkCamera;
				loginBox.Sensitive = currentEditDevice.driver.isLoginReqired;
				passwordBox.Sensitive = currentEditDevice.driver.isPasswordRequired;
				hardwareBox.Sensitive = currentEditDevice.driver.isAutoDiscover;
				//Put data to box
				
				//driverListBox.SetActiveIter (selectIter);
				
				uriField.Text = currentEditDevice.driver.URI;
				loginField.Text = currentEditDevice.driver.Login;
				passwordField.Text = currentEditDevice.driver.Password;
				
			} else {
				uriBox.Sensitive = false;
				loginBox.Sensitive = false;
				passwordBox.Sensitive = false;
				hardwareBox.Sensitive = false;
				Console.WriteLine ("No driver");
			}
				
			
			if (currentEditDevice != null) {
			
				refreshLog ();
			}
			
			dontchange = false;
		}
		
		private void refreshDevicesList ()
		{
			Console.WriteLine ("Device refresh");
			devicelistStore.Clear ();
			
			string driverName = "";
			
			foreach (IDevice device in Main.devices) {
				if (device != null) {
					
					if (device.driver != null) {
						driverName = device.driver.getDriverName ();
					} else {
						driverName = "Error";
					}

				
					string ip = device.getDeviceURI ();
					try {
						Uri address = new Uri(device.getDeviceURI ());
						ip = address.Host;
					} catch (Exception e) {
					}

					devicelistStore.AppendValues (device.getDeviceName (),ip, driverName,device);
				}

			}
		}
		
		private void refreshLog ()
		{
			logView.Buffer.Text = "";
			if (currentEditDevice != null) {
				logView.Buffer.Text = currentEditDevice.GetLog ();
			}
			
			//TUTAJ SCROOL DO BOOTTOM;
		}
		
		protected void OnAddDeviceButton (object sender, EventArgs a)
		{
			
			currentEditDevice = new CDevice ();
			refreshDevicesList ();
			refreshEditList ();
			
			SettingsBox.Sensitive = true;
		}
		
		
		protected void OnEditDeviceButton (object sender, EventArgs a)
		{
			Console.WriteLine ("OnEditDeviceButton " + sender.GetType ());
			
			
			TreePath path;
			TreeViewColumn column;
			TreeIter iter;
			try {
				Console.WriteLine ("OnEditDeviceButton get cursor");
				deviceListsBox.GetCursor (out path, out column);
				if (path == null)
					return;
				
				IDevice device = null;
				int i = 0;
				foreach (IDevice devicein in Main.devices) {
					
					if (path.ToString () == i.ToString ()) {
						device = devicein;
						break;
					}
					i++;
				}
				
				Console.WriteLine ("SELECT: " + path.ToString () + "==" + i.ToString ());
				//devicelistStore.GetIter (out iter, path);
				//IDevice device = (IDevice)devicelistStore.GetValue (iter, 3);
				
				if (device != null && device.getDeviceName() != "") {
					currentEditDevice = null;
					currentEditDevice = device;
					Console.WriteLine ("EDIT DEV" + device.getDeviceName ());
					
					refreshEditList ();
					
					SettingsBox.Sensitive = true;
				} else {
					//currentEditDevice = null;
					//refreshDevicesList ();
					//refreshEditList ();
					SettingsBox.Sensitive = false;
				} 
			
			} catch (Exception e) {
				Console.WriteLine ("Error " + e.Message);
			}	
		}
		
		protected void OnNameChanged (object sender, EventArgs a)
		{
			if (dontchange || currentEditDevice == null)
				return;
			currentEditDevice.DeviceName = nameField.Text;
		}

		protected void OnUriChanged (object sender, EventArgs a)
		{
			if (dontchange || currentEditDevice == null)
				return;
			
			if (currentEditDevice.driver.isNetworkCamera) {
				currentEditDevice.driver.URI = uriField.Text;
			}
		}
		
		protected void OnLoginChanged (object sender, EventArgs a)
		{
			if (dontchange || currentEditDevice == null)
				return;
			
			if (currentEditDevice.driver.isLoginReqired) {
				currentEditDevice.driver.Login = loginField.Text;
			}
		}

		protected void OnPasswordChanged (object sender, EventArgs a)
		{
			if (dontchange || currentEditDevice == null)
				return;
			
			if (currentEditDevice.driver.isPasswordRequired) {
				currentEditDevice.driver.Password = passwordField.Text;
			}
		}
		
		protected void OnSelectDeviceChanged (object sender, EventArgs a)
		{
		
			editButton.Sensitive = true;
			

			
		}

		protected void OnDeviceEditChanged (object sender, EventArgs a)
		{
			
		}
		
		protected void OnDriverChanged (object sender, EventArgs a)
		{
			if (dontchange || currentEditDevice == null)
				return;
			
			Console.WriteLine ("Changed");

			TreeIter iter;
					
			if (driverListBox.GetActiveIter (out iter)) {
				
				if (driverlistStore.GetValue (iter, 0) != null) {
					ICameraDriver driver = (ICameraDriver)driverlistStore.GetValue (iter, 1);
					if (currentEditDevice.driver != driver) {
						if (driver != null) {
							currentEditDevice.driver = driver.Clone ();
							currentEditDevice.driver.device = currentEditDevice;
						} else {
							currentEditDevice.driver = null;		
						}
						
			

					}
					
				} else {
					currentEditDevice.driver = null;
				}
			} else {
				currentEditDevice.driver = null;
			}

			
			if (currentEditDevice.driver != null && !currentEditDevice.driver.isEmptyDriver) {
				
				uriBox.Sensitive = currentEditDevice.driver.isNetworkCamera;
				loginBox.Sensitive = currentEditDevice.driver.isLoginReqired;
				passwordBox.Sensitive = currentEditDevice.driver.isPasswordRequired;
				hardwareBox.Sensitive = currentEditDevice.driver.isAutoDiscover;
				
				if (currentEditDevice.driver.isNetworkCamera) {
					currentEditDevice.driver.URI = uriField.Text;
				}
					
				if (currentEditDevice.driver.isLoginReqired) {
					currentEditDevice.driver.Login = loginField.Text;
				}
					
				if (currentEditDevice.driver.isPasswordRequired) {
					currentEditDevice.driver.Password = passwordField.Text;
				}
					
				testButton.Sensitive = saveButton.Sensitive = true;
				
				
				
			} else {
				hardwareBox.Sensitive = uriBox.Sensitive = loginBox.Sensitive = passwordBox.Sensitive = testButton.Sensitive = saveButton.Sensitive = false;
			}
			
			Console.WriteLine(sender.GetType ().ToString());
		}
		
		protected void OnTestDevice (object sender, EventArgs a)
		{
			if (currentEditDevice == null)
				return;
			
			currentEditDevice.TestConnection ();
			refreshLog ();
				
		}
		
		protected void OnSaveDeviceNow (object sender, EventArgs a)
		{
			SettingsBox.Sensitive = false;
			bool bAdd = true;
			foreach (IDevice device in Main.devices) {
				if (device == currentEditDevice) {
					bAdd = false;
					break;
				}
			}
			if(bAdd) Main.devices.Add (currentEditDevice);	
			refreshDevicesList ();
			currentEditDevice = null;	
			Main.saveSettings ();
		}
	}
}

