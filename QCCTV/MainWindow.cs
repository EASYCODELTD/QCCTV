using System;
using Gtk;
using System.Collections.Generic;
using QCCTVCameraViewWidget;
using CQCCTV.Drivers;
using QCCTV;
using QCCTVDefs;


public partial class MainWindow: Gtk.Window
{	
	private Image offImage;
	public int windowWidth;
	public int windowHeight;
	public List<ICameraDriver> drivers=null;
	
	public List<CameraView> views=null;
	public List<IDevice> devices=null;
	
	public AppSettings settings = null;
	
	public bool toolboxVisible { 
		get{ return ViewTolboxAction.Active; }
		set {
			
			settings.toolboxVisible = value;
			ViewTolboxAction.Active = value;
	
			foreach (CameraView view in views) {
				view.controlVisible = value;
			}
			
			
		}
	}
	
	/*****
	 * WINDOWS
	 */
	private DevicesSettingsWindow DevicesSettings;
	
	public MainWindow (): base (Gtk.WindowType.Toplevel)
	{
		Build ();

		offImage = Image.LoadFromResource ("QCCTV.Resources.Images.nosignal.png");

		menubar.Visible = false;

		DevicesSettings = new DevicesSettingsWindow (this);
		prepareDrivers ();
		prepareDevices ();
		prepareOutputView ();
		
		settings = null;
		loadSettings ();
	}

	
	
	
	/**
	 * 
	 */
	private void prepareDevices ()
	{
		devices = new List<IDevice> ();
		// tutaj ladujemy ustawiena urzadzen
	}
	
	/**
	 * 
	 */
	private void prepareDrivers ()
	{
		
		drivers = new List<ICameraDriver> ();
		drivers.Add (new CQCCTV.Drivers.EmptyDriver ());
		drivers.Add (new CQCCTV.Drivers.MJPGDriver ());
		drivers.Add (new CQCCTV.Drivers.JPGDriver ());
		drivers.Add (new CQCCTV.Drivers.USBDriver ());
		drivers.Add (new CQCCTV.Drivers.RTSPDriver ());


		drivers.Add (new CQCCTV.Drivers.IPROBOT3JPGDriver ());
		drivers.Add (new CQCCTV.Drivers.IP602WDriver ());
	
	}
	
	
	/**
	 * 
	 */
	private void prepareOutputView ()
	{
		
		views = new List<CameraView> ();
		
		
		
		views.Add (VIEW_1);
		views.Add (VIEW_2);
		views.Add (VIEW_3);
		views.Add (VIEW_4);
		views.Add (VIEW_5);
		views.Add (VIEW_6);
		views.Add (VIEW_7);
		views.Add (VIEW_8);
		views.Add (VIEW_9);
		
		foreach (CameraView view in views) {
			view.setOffImage(offImage);
		}
		
	
	}
	
	public IDevice getDeviceFromName (string name)
	{
		foreach (IDevice device in devices) {
			if (device.getDeviceName () == name)
				return device;
		}
		
		return null;
	}

	public CameraView getViewFromID (int id)
	{
		var i = 0;
		foreach (CameraView view in views) {
			if (i == id)
				return view;
			i++;
		}
		
		return null;
	}


	void EntryKeyPressEvent(object o, KeyPressEventArgs args)
	{
		if (args.Event.KeyValue == 65513) {
			menubar.Visible = !menubar.Visible;
		}
	}
	
	void EntryKeyUpEvent(object o, Gtk.KeyReleaseEventArgs args)
	{
		

	}

	public ICameraDriver createDriverFromName (string name)
	{
		foreach (ICameraDriver driver in drivers) {
			if (driver.getDriverName () == name)
				return driver.Clone ();
		}
		
		return new EmptyDriver ();
	}
	
	public void loadSettings ()
	{
		settings = AppSettings.LoadFromFile ("settings.xml");
		toolboxVisible = settings.toolboxVisible;
		
		devices.Clear ();
		foreach (DeviceSetting devsettings in settings.deviceSettings) {
			CDevice device = new CDevice ();
			device.DeviceName = devsettings.DeviceName;
			Console.WriteLine ("Load settings for: " + devsettings.DeviceName);
			device.driver = createDriverFromName (devsettings.DriverName);
			if (device.driver == null) {
				device.driver = new EmptyDriver ();
			}
			device.driver.device = device;
			
			device.driver.URI = devsettings.DeviceURI;
			device.driver.Login = devsettings.DeviceLogin;
			device.driver.Password = devsettings.DevicePassword;
			devices.Add (device);
			device.driver.Connect ();
			
		}

		refreshDeviceListInViewWindow ();
		
		IDevice dev = null;
		CameraView view = null;
		int i = 0;
		if(settings.viewSettings!=null)
		{
			foreach (ViewSetting viewsettings in settings.viewSettings) {
				view = getViewFromID (i);
			
				dev = getDeviceFromName (viewsettings.DeviceName);
				
					view.setCurrentDevice (dev);
				
				i++;
			}		
		}
	}

	public void refreshDeviceListInViewWindow ()
	{
		foreach (CameraView view in views) {
			view.devicelist = devices;
		}	
	}
	
	public void saveSettings ()
	{
		if (settings == null) {
			Console.WriteLine ("NO SETTINGS??");
			return;
		}
		
		/***************************
		 * SAVE DEVICE
		 */ 
		settings.deviceSettings.Clear ();
		foreach (IDevice device in devices) {
			DeviceSetting devsettings = new DeviceSetting ();
			
			//device
			devsettings.DeviceName = device.getDeviceName ();
			if (device.driver == null) {
				device.driver = new EmptyDriver ();
			}
			devsettings.DriverName = device.driver.getDriverName ();
			
			//driver
			devsettings.DeviceLogin = device.driver.Login;
			devsettings.DevicePassword = device.driver.Password;
			devsettings.DeviceURI = device.driver.URI;
			
			settings.deviceSettings.Add (devsettings);
		}
		
		/***************************
		 * SAVE VIEW
		 */
		settings.viewSettings.Clear ();
		foreach (CameraView view in views) {
			ViewSetting viewsettings = new ViewSetting ();
			
			//device
			if (view.getCurrentDevice () != null) {
				viewsettings.DeviceName = view.getCurrentDevice ().getDeviceName ();
			} else {
				viewsettings.DeviceName = "";
			}
			settings.viewSettings.Add (viewsettings);
		}
		
	
		
		settings.SaveToFile ("settings.xml");
	}
	
	
	public void shutdown ()
	{
		saveSettings ();
		
		Console.WriteLine ("Shutdown all");
		foreach (IDevice device in devices) {
			device.shutdown ();
		}
		
		Console.WriteLine ("Shutdown done");
	}
	
	/**
	 * events
	 */
	protected void OnDeleteEvent (object sender, DeleteEventArgs a)
	{
		
		
		shutdown ();
		
		Gtk.Application.Quit ();
		a.RetVal = true;
	}
	
	protected void OnMenuActionExit (object sender, EventArgs a)
	{

		
		shutdown ();
		
		Gtk.Application.Quit ();

	}

	protected void OnMenuActionShowDevicesSettings (object sender, EventArgs a)
	{
		DevicesSettings.Visible = true;
	}
	
	 private void OnSizeAllocated (object o, Gtk.SizeAllocatedArgs e)
	{ 
		
		windowWidth = e.Allocation.Width; 
		windowHeight = e.Allocation.Height; 
   
		
		//table.Resize ((uint)windowWidth, (uint)windowHeight);
		uint width = (uint)(windowWidth/3);
		uint height = (uint)(windowHeight/3);
		
		foreach (CameraView view in views) {
			view.Resize (width,height);
		}
		
		//Console.WriteLine ("Resize " + windowWidth + "px " + windowHeight+" px");
		
		             
    }
	
	public bool isFullscreen {
		get { return FullscreenAction.Active;  } 		
		set {
			FullscreenAction.Active = value; 
			if (isFullscreen) {
				this.Fullscreen ();
			
			} else {
				this.Unfullscreen ();
			}
		} 
		}
	
	protected void OnMenuActionFullScreenTogle (object sender, EventArgs a)
	{
		
		if (isFullscreen) {
			this.Fullscreen ();
			
		} else {
			this.Unfullscreen ();
		}

	}
	
	protected void OnMenuActionTooboxVisibleTogle (object sender, EventArgs a)
	{
		
		
		Console.WriteLine ("change");
		
		if (ViewTolboxAction.Active) {
			toolboxVisible = true;
			
		} else {
			toolboxVisible = false;
		}
		

	}
}
