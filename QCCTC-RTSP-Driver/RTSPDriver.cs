using System;
using System.Threading;
using System.Net;
using System.IO;
using System.Text;
using Gdk;
using QCCTVDefs;

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace CQCCTV
{
	namespace Drivers
	{

	

		public class RTSPDriver : ICameraDriver
		{
			private Thread thread;
			private bool connect=false;
			private bool run;
			private bool dontRun;
			private Media.Rtsp.RtspServer server;
			private Media.Rtsp.Server.MediaTypes.RtspSource  source;
			private Media.Rtsp.RtspClient client;
			private Media.Common.Loggers.ConsoleLogger loger;

			public RTSPDriver ()
			{
				DriverName = "RTSP Camera Driver";
				isNetworkCamera = true;
				isLoginReqired = true;
				isPasswordRequired = true;

				loger = new Media.Common.Loggers.ConsoleLogger();
				loger.Log ("Testujemy LOG");
				Console.Error.WriteLine ("TUTAJ TEST");
			}

			public override void shutdown ()
			{
				dontRun = false;
				connect = false;
				run = false;
				Console.WriteLine ("Wait for join");
				thread.Abort ();			
				thread = null;
				Console.WriteLine ("Done");

			

			}

			public override bool TestConnection ()
			{ 
				if (device == null)
					return false;



				if (Password.Length == 0) {
					device.AddLog ("Driver: No password");	
					return false;
				}

				if (Login.Length == 0) {
					device.AddLog ("Driver: No login");	
					return false;
				}

				if (Login.Length == 0) {
					device.AddLog ("Driver: No URI");	
					return false;
				}




				return false; }

			public override void Connect ()
			{
				if (dontRun)
					return;

				device.AddLog ("Camera connect: " + device.getDeviceName () + " to " + URI);

				if (run == false) {
					thread = new Thread (() => WorkerThread ());
					thread.Start ();
					run = true;
					//server = new Media.Rtsp.RtspServer (System.Net.Sockets.AddressFamily.InterNetwork,1500);
					//source = new Media.Rtsp.Server.MediaTypes.RtspSource("2", "rtsp://192.168.1.2/2,",new System.Net.NetworkCredential("admin", "Abra0906"));
					//server.TryAddMedia(source);
					//server.Start();
					connect = true;
		


				}

				connect = true;
			}

	
			public override void Disconnect ()
			{ 
				device.AddLog ("Camera disconnect: " + device.getDeviceName () + " to " + URI);
				if (run == true) {
					connect = false;
					run = false;
					thread.Abort ();
					thread = null;

				}
			}


			public void WorkerThread ()
			{
				
				//Create a client
				string url="";
				url = "rtsp://192.168.1.2:554/";
				//url = "rtsp://wowzaec2demo.streamlock.net/vod/mp4:BigBuckBunny_115k.mov";

				Media.Rtsp.RtspClient client = new Media.Rtsp.RtspClient(url);
				client.Client.Logger = loger;
				client.Logger = loger;

				client.SocketReadTimeout = 1000;
				client.SocketWriteTimeout = 1000;

				client.Credential = new System.Net.NetworkCredential ("admin", "Abra0906");

				///The client has a Client Property which is used to access the RtpClient

				client.OnConnect += delegate(Media.Rtsp.RtspClient sender, object args) {
					Console.WriteLine("CONNECTED?");
					Console.WriteLine("Try play");
					//sender.Play();
				};

				client.OnPlay += delegate(Media.Rtsp.RtspClient sender, object args) {
					Console.WriteLine("START PLAY?");
				};

				client.OnDisconnect += delegate(Media.Rtsp.RtspClient sender, object args) {
					Console.WriteLine("DISCONNECTED?");
				};



				///Attach events at the packet level
				client.Client.RtcpPacketReceieved +=  delegate(object sender, Media.Rtcp.RtcpPacket packet, Media.Rtp.RtpClient.TransportContext tc ) {
					Console.WriteLine("GOT RTCP PACKET");
				};

				client.Client.RtpPacketReceieved +=   delegate(object sender, Media.Rtp.RtpPacket packet , Media.Rtp.RtpClient.TransportContext tc ) {
					Console.WriteLine("GOT RTP PACKET");
				};

				//Attach events at the frame level
				client.Client.RtpFrameChanged +=  delegate(object sender, Media.Rtp.RtpFrame frame , Media.Rtp.RtpClient.TransportContext tc , bool final ) {
					Console.WriteLine("GOT RTP FRAME");
				};

				client.OnConnect += delegate(Media.Rtsp.RtspClient sender, object args) {
					Console.WriteLine("CONNECTED 2 ?");
					sender.StartPlaying();
				};

				client.OnRequest += delegate(Media.Rtsp.RtspClient sender, Media.Rtsp.RtspMessage request) {
					Console.WriteLine("OnRequest ? "+request.Body+":"+request.RtspStatusCode+":"+request.ToString());

				};

				client.OnResponse += delegate(Media.Rtsp.RtspClient sender, Media.Rtsp.RtspMessage request, Media.Rtsp.RtspMessage response) {
					Console.WriteLine("OnResponse ? "+response.HttpStatusCode+":"+response.RtspStatusCode+":"+response.ToString());


				};

				client.OnDisconnect += delegate(Media.Rtsp.RtspClient sender, object args) {
					Console.WriteLine("DISCONNECTED 2?");
				};

				//Performs the Options, Describe, Setup and Play Request
				Console.WriteLine("Connect and back");
				client.Connect ();

				Console.WriteLine("Connect and back OK");
				while (run) {

					Thread.Sleep (1000);
					Console.WriteLine("CONNECT LOOP");
				}
			}






			/** end **/
		}
	}
}

