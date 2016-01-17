using System;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;



namespace RTSP
{

	public enum Method
	{
		SETUP,
		DESCRIBE
	}

	public delegate void OnRestConnectHandler(object sender, EventArgs e);
	public delegate void OnRestDisconnectHandler(object sender, EventArgs e);

	public class QRtspClient : IDisposable
	{
		
		private QTcpClient client;
		private int Port;
		private string Host;
		private string Res;
		private string Login;
		private string Password;
		private NetworkStream stream;
		private UInt32 cseq=0;

		public event OnRestConnectHandler OnConnectEvent=null;
		public event OnRestDisconnectHandler OnDisconnectEvent=null;

		public QRtspClient (string host,int port=554,string res="",string login="",string password="")
		{

			this.Host = host;
			this.Port = port;
			this.Res = res;
			this.Login = login;
			this.Password = password;

			client = new QTcpClient(Host,Port);
		}

		public void Dispose()
		{
		}

		public bool Connect()
		{
			Console.WriteLine ("Connect to: " + Host + " on port: " + Port);

			client.OnConnectEvent += delegate(object sender, EventArgs e) {
				stream = client.GetStream();
				if(Login+Password!="") Auhorize(Login,Password);
			};

			client.OnDisconnectEvent += delegate(object sender, EventArgs e) {
				
			};

			client.Connect ();



			return false;
		}

		public bool Disconnect()
		{
			client.Disconnect ();
			client.Dispose ();
			client = null;
			return true;
		}

		private string CalculateMD5Hash(string input)
		{
			// step 1, calculate MD5 hash from input
			MD5 md5 = MD5.Create();
			byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(input);
			byte[] hash = md5.ComputeHash(inputBytes);

			// step 2, convert byte array to hex string
			StringBuilder sb = new StringBuilder();
			for (int i = 0; i < hash.Length; i++)
			{
				sb.Append(hash[i].ToString("X2"));
			}
			return sb.ToString().ToLower();
		}

		public void Auhorize(string login,string password)
		{
			//SETUP rtsp://192.168.201.113 RTSP/1.0
			//CSeq: 1
			//Transport: RTP/AVP;unicast;client_port 4588-4589
			//Authorization: Basic YWRtaW46NDQxOWI2M2Y1ZTUxOjEyMzQ=
			//User-Agent: VLC media player (LIVE555 Streaming Media v2010.02.10)
			QRTSPHeader header = new QRTSPHeader();
			QRTSPHeader responseHeader = new QRTSPHeader();
			string body = "";
			int code = 0;
			header.Add ("Transport","RTP/AVP;unicast;client_port 4588-4589");
			header.Add ("Authorization","Digest "+CalculateMD5Hash(login+":"+password));
			header.Add ("User-Agent","VLC media player (LIVE555 Streaming Media v2010.02.10)");

			if (sendMessage (RTSP.Method.DESCRIBE, header, "/", "")) {

				if(getMessage(ref responseHeader,ref  body,ref code))
				{

				}
			}


			string WWWAuthenticate = responseHeader.Get("WWW-Authenticate");
				
			Console.WriteLine (">----------------------------------------------------");
			Console.WriteLine (WWWAuthenticate.ToString());
			Console.WriteLine (">----------------------------------------------------");


			string nonce = (WWWAuthenticate.Split (',')) [1];
			Console.WriteLine ("NONCE IS: [" + nonce+"]");
			header.Add ("Transport","RTP/AVP;unicast;client_port 4588-4589");
			header.Add ("Authorization","Digest "+CalculateMD5Hash(login+":"+nonce+":"+password));
			header.Add ("User-Agent","VLC media player (LIVE555 Streaming Media v2010.02.10)");

			if (sendMessage (RTSP.Method.DESCRIBE, header, "/", "")) {

				if(getMessage(ref responseHeader,ref  body,ref code))
				{

				}
			}
		}

		public void RTSPDescribe()
		{
			//string message = "DESCRIBE rtsp://"+Host+":"+Port+"/"+Res+" RTSP/1.0\\r\\n\n\t\t\t\tCSeq: 1\\r\\n\n\t\t\t\t\\r\\n";
			//sendRawMessage (message);
			//getRawMessage ();
		}

		public bool getMessage(ref QRTSPHeader header,ref string body,ref int code)
		{
			
			string message = "";
			message = getRawMessage ();	

			//Parsowanie odpowiedzi

			return true;
		}

		public bool sendMessage(RTSP.Method method,QRTSPHeader header,string res,string body)
		{
			string head = "";
			string methode = "";
			string response = "";

			switch (method) {
				case RTSP.Method.SETUP: methode = "SETUP"; break;
				case RTSP.Method.DESCRIBE: methode = "DESCRIBE"; break;
				
			}

			cseq++;
			header.Add ("CSeq", cseq);

			head = methode + " rtsp://" + Host + ":" + Port + Res + " RTSP/1.0\r\n";
			head += header.ToString ();


			sendRawMessage(head,body);


			return true;
		}

		public bool sendRawMessage(string header,string body)
		{

			Console.WriteLine ("-----------------------------------------------------");
			Console.WriteLine (header);
			Console.WriteLine ("-----------------------------------------------------");
			Console.WriteLine (body);
			Console.WriteLine ("-----------------------------------------------------");

			byte[] bytes = System.Text.Encoding.ASCII.GetBytes (header+body);
			return sendData (ref bytes);
		}

		public string getRawMessage()
		{
			byte[] bytes = new byte[2048];

			Int32 size = reciveData (ref bytes);
			string message = System.Text.Encoding.ASCII.GetString(bytes, 0, size);
			Console.WriteLine ("-----------------------------------------------------");
			Console.WriteLine (message);
			Console.WriteLine ("-----------------------------------------------------");

			return message;
		}

		public bool sendData(ref byte[] data)
		{
			stream.Write(data, 0, data.Length);
			return true;
		}

		public Int32 reciveData(ref byte[] data)
		{
			return stream.Read(data, 0, data.Length);
		}
	}
}

