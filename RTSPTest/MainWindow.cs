using System;
using Gtk;
using RTSP;


public partial class MainWindow: Gtk.Window
{
	QRtspClient client;

	public MainWindow () : base (Gtk.WindowType.Toplevel)
	{
		Build ();

		try {
			client = new QRtspClient ("192.168.1.7",554,"/","admin","Abra0906");
			client.Connect ();


		} catch (Exception e ) {
			Console.WriteLine ("Error: "+e.Message);
		}
	}

	protected void OnDeleteEvent (object sender, DeleteEventArgs a)
	{
		client.Disconnect ();

		Application.Quit ();
		a.RetVal = true;
	}
}
