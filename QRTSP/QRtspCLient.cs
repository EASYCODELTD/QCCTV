using System;
using System.Net;

namespace QRTSP
{
	public class QRtspCLient
	{
		/****
		 * Evants
		 */

		public event ConnectEventHandler onConnectEvent=null;
		public event DisconnectEventHandler onDisconnectEvent=null;


		private Uri uri;
		private int port;
		private string Login;
		private string Password;

		private HttpWebRequest request;

		public QRtspCLient (string uri,string login,string password)
		{
			this.uri = new Uri(uri);

			if (this.port < 0) {
				this.port = 554;
			}

			this.Login = login;
			this.Password = password;

		}


		public bool Connect()
		{
			

			return false;
		}
	}
}

