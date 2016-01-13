using System;
using System.Threading;
using System.Net;
using System.IO;
using System.Text;
using Gdk;
using QCCTVDefs;
using System.IO.Compression;

namespace CQCCTV
{
	namespace Drivers
	{

		public class JPGDriver : ICameraDriver
		{
			private Thread thread;
			public bool connect;
			public bool run;
			public bool dontRun;
			public bool pause;
			public int _requiredFps;
			public int __requiredFps;

			HttpWebRequest req = null;
			WebResponse resp = null;
			Stream stream = null;
			int read = 0;
			string url = "";
			byte[] buffer = new byte[15000];
			int sleepIfProblem=0;

			int requiredFps {
				get { return _requiredFps; }
				set { _requiredFps = __requiredFps = value; }
			}

			public JPGDriver ()
			{
				DriverName = "JPG Camera";
				isNetworkCamera = true;
				isLoginReqired = true;
				isPasswordRequired = true;
				pause = false;
				requiredFps = 10;
				sleepIfProblem = 0;
				
			}
		
			public override bool hasControls() {
				return true;
			}


			private void disableHUD()
			{
				this.set_option ("cmd=setoverlayattr&-region=0&-show=0");
				this.set_option ("cmd=setoverlayattr&-region=1&-show=0");
			}

			private void set_option(string rel)
			{
				string url = "http://" + URI + "/web/cgi-bin/hi3510/param.cgi?"+rel+"&usr=" + Login + "&pwd=" + Password;

				Console.WriteLine("Call "+url+" - start");
				HttpWebRequest req = (HttpWebRequest)WebRequest.Create (url);
				req.KeepAlive = false;
				req.Headers.Add ("Accept-Encoding", "gzip,deflate");
				req.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
				req.UseDefaultCredentials = true;
				req.Credentials = new NetworkCredential (Login, Password);		
				req.Proxy = null;
				req.Timeout = 1000;							
				WebResponse resp = null;
				req.KeepAlive = false;

				pause = true;
				try{
					resp = req.GetResponse();
					resp.Close();
				} catch(Exception e) {
					Console.WriteLine (e.Message);
				}

				req = null;

				Console.WriteLine("Call "+url+" - end");

			}

			private void sentCtrCmd(string pos,int step=0,int speed=45)
			{
				string url = "http://" + URI + "/web/cgi-bin/param.cgi?cmd=ptzctrl&-step="+step+"&-act="+pos+"&-speed="+speed+"&usr=" + Login + "&pwd=" + Password;



				HttpWebRequest req = (HttpWebRequest)WebRequest.Create (url);
				req.KeepAlive = false;
				req.Headers.Add ("Accept-Encoding", "gzip,deflate");
				req.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
				req.UseDefaultCredentials = true;
				req.Credentials = new NetworkCredential (Login, Password);		
				req.Proxy = null;
				req.Timeout = 100;							
				WebResponse resp = null;
				req.KeepAlive = false;

				pause = true;
				try{
					resp = req.GetResponse();
					resp.Close();
				} catch(Exception e) {
					Console.WriteLine (e.Message);
				}


				req = null;
				//if(resp!=null) resp.Dispose ();
				resp = null;
				pause = false;
			}

			public override void control(int what,int how)
			{

				switch (what) {
				case 0: //Stop
					sentCtrCmd("stop",0);
					break;

				case 1: //up
					sentCtrCmd("up",0);

					break;
				case 2: //left
					sentCtrCmd("left");

					break;
				case 3: //right
					sentCtrCmd("right");

					break;
				case 4: //Down
					sentCtrCmd("down");

					break;
				}
			}

			public override void shutdown ()
			{
				dontRun = false;
				connect = false;
				run = false;
				pause = true;

				Console.WriteLine ("Wait for join "+DriverName);
				thread.Abort ();			
				thread = null;
				Console.WriteLine ("Done "+DriverName);
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

			public void getFrame()
			{

				req = (HttpWebRequest)WebRequest.Create (url);
				req.Headers.Add ("Accept-Encoding", "gzip,deflate");
				req.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
				req.UseDefaultCredentials = true;
				req.Credentials = new NetworkCredential (Login, Password);		
				req.Proxy = null;
				req.Timeout = 1000/_requiredFps;							
				resp = null;
				req.KeepAlive = true;

				try {

					//Prepare connection
					resp = req.GetResponse ();


					string ct = resp.ContentType;
					if (ct.IndexOf ("image/jpeg") == -1) {
						throw new ApplicationException ("Invalid Content Type: " + ct);
					}


					stream = resp.GetResponseStream ();



					if (stream != null) {
						try {

							MemoryStream ms = new MemoryStream ();

							do {
								// fill the buffer with data
								read = stream.Read (buffer, 0, 15000);

								// make sure we read some data
								if (read != 0) {
									ms.Write (buffer, 0, read);
								}
							} while (read > 0); // any more data to read?

							if (ms.Length > 0) {

								ms.Position = 0;
								if(ms.Length>0)
								{
									device.frameReady (ms.ToArray());
								}
							} 

							ms.Dispose();
							ms = null;

							if(_requiredFps < __requiredFps)
							{
								_requiredFps++;
								device.AddLog ("Go up "+_requiredFps);
							}

						} catch (Exception e) {
							//device.AddLog (e.Message);
						}
					}


					resp.Close ();
					resp = null;
					req = null;

				} catch(TimeoutException e) {
					device.AddLog ("Timeout, need slowdown1? "+_requiredFps);
					_requiredFps--;
					if (_requiredFps < 5) {
						sleepIfProblem++;	
					}

				} catch (Exception e) { 
					if (e.Message.Contains ("timed out")) {
						device.AddLog ("Timeout, need slowdown2? "+_requiredFps);
						_requiredFps--;
						if (_requiredFps < 5) {
							_requiredFps = 5;
							sleepIfProblem++;	
						}

					} else {
						//device.AddLog (e.Message + " " + url + " type:" + e.GetType ());
						sleepIfProblem++;
						if (sleepIfProblem > 5) {
					
							device.AddLog (e.Message + " " + url + " type:" + e.GetType ());
						}

					}

				}

				if (sleepIfProblem > 100) {
					device.AddLog ("Camera `"+device.DeviceName+"` go sleep for 10sec, somthing not right");
					Thread.Sleep (10000);
					sleepIfProblem = 0;
				}
			}


			// Thread entry point
			public void WorkerThread ()
			{



				url = "http://"+URI+"/tmpfs/auto.jpg?usr="+Login+"&pwd="+Password;

				while (run) {
					if (device != null && connect && !pause) {
						getFrame ();
					}

					Thread.Sleep (1000/(_requiredFps * 4));
				}

				device.AddLog ("Shutdown");
			}
		}
		
		
	}
}

