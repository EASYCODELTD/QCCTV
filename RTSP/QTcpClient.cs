using System;
using System.Net.Sockets;
using System.Threading;

namespace RTSP
{
	public delegate void OnConnectHandler(object sender, EventArgs e);
	public delegate void OnDisconnectHandler(object sender, EventArgs e);

	public class QTcpClient : IDisposable
	{
		private TcpClient client;
		private Thread thread;

		private int Port;
		private string Host;
		private NetworkStream stream;
		public bool Connected;
		public int Timeout;
		public bool run;

		public event OnConnectHandler OnConnectEvent=null;
		public event OnDisconnectHandler OnDisconnectEvent=null;



		public QTcpClient (string host,int port)
		{
			this.Host = host;
			this.Port = port;
			thread = null;
			client = new TcpClient ();
			Timeout = 30000;
		}

		public void Connect()
		{
			if (thread != null)
				return;
			
			this.run = true;
			thread = new Thread (() => connectionThread ());
			thread.Start ();				
		}

		public void Disconnect(bool wait=false)
		{
			if (thread == null)
				return;
			
			this.run = false;
			if (wait) {
				Console.WriteLine("Waiting for disconect: "+Host+":"+Port);
				thread.Abort ();
				thread.Join();
				thread = null;
			}
		}

		public void Dispose()
		{
			Disconnect ();
		}

		public NetworkStream GetStream()
		{
			return stream;
		}

		private void connectionThread()
		{
			try{
				Console.WriteLine("Connecting to: "+Host+":"+Port);

				client.ConnectAsync(Host, Port);
				while(Timeout>0)
				{
					if(client.Connected) break;
					Timeout--;
					Thread.Sleep(1);
				}

				if(!client.Connected) {
					throw new Exception("Cant connect to: "+Host+":"+Port+" Timeout");
				}

				Console.WriteLine("Connected to: "+Host+":"+Port);	

				stream = client.GetStream();
				if(stream == null) throw new Exception("Cant get stream for: "+Host+":"+Port+" ");

				if(OnConnectEvent!=null) OnConnectEvent(this,new EventArgs());

				while(run)
				{
					//Tutaj tylko testujemy
					if (!client.Connected) {
						//Jakis ewvent

						run = false;
					}

					Thread.Sleep(1000);
				}


			} catch(Exception e) {
				Console.WriteLine ("Error: "+e.Message);
				return;
			}

			Console.WriteLine("Disconecting: "+Host+":"+Port);
			if (client.Connected) {
				if (stream != null) {
					stream.Close ();
					stream.Dispose ();
					stream = null;
				}
				client.Close ();
				client = null;
			}

			Console.WriteLine("Disconected: "+Host+":"+Port);
			if(OnDisconnectEvent!=null) OnDisconnectEvent(this,new EventArgs());
		}
	}
}

