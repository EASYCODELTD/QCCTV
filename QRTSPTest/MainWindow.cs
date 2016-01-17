using System;
using Gtk;
using QRTSP;

public partial class MainWindow: Gtk.Window
{
	public QRTSP.QRtspCLient client;

	public MainWindow () : base (Gtk.WindowType.Toplevel)
	{
		Build ();



		client = new QRtspCLient ("rtsp:\\192.168.1.2","Admin","Abra0906");

		client.onConnectEvent += delegate(object sender, ConnectEvenArgs e) {
			Console.WriteLine("Conected");	
		};

		client.onDisconnectEvent += delegate(object sender, DisconnectEvenArgs e) {
			Console.WriteLine("Disconnected");	
		};

		client.Connect ();

	}

	protected void OnDeleteEvent (object sender, DeleteEventArgs a)
	{
		Application.Quit ();
		a.RetVal = true;
	}
}
