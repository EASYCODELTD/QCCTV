using System;

namespace QRTSP
{
	public class RtspEventArgs : System.EventArgs { }

	public class ConnectEvenArgs : RtspEventArgs { public ConnectEvenArgs () { } }
	public class DisconnectEvenArgs : RtspEventArgs { public DisconnectEvenArgs () { } }

	/**************************************
	 * Event handlers
	 */ 
	public delegate void ConnectEventHandler(object sender, ConnectEvenArgs e);
	public delegate void DisconnectEventHandler(object sender, DisconnectEvenArgs e);
}

