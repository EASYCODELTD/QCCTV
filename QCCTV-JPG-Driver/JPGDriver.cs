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
			private bool connect;
			private bool run;
			private bool dontRun;

			
			public JPGDriver ()
			{
				DriverName = "JPG Camera Driver";
				isNetworkCamera = true;
				isLoginReqired = true;
				isPasswordRequired = true;
				
			
				
				
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
			
			
	
			
			// Thread entry point
			public void WorkerThread ()
			{
				HttpWebRequest req = null;
				WebResponse resp = null;
				Stream stream = null;
				Pixbuf pixbuffer = null;
				int read = 0, pos = 0;
				string url = "";
				byte[] buffer = new byte[4096];
				
				while (run) {
					if (device != null) {
						
						url = URI;
						url = url.Replace ("[LOGIN]", Login);
						url = url.Replace ("[PASSWORD]", Password);
									

						
						if (connect) {
							
							try {

								req = (HttpWebRequest)WebRequest.Create (url);
								req.Headers.Add ("Accept-Encoding", "gzip,deflate");
								req.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
								req.UseDefaultCredentials = true;
								req.Credentials = new NetworkCredential (Login, Password);		
								req.Proxy = null;
								req.Timeout = 100;							
								resp = null;
								req.KeepAlive = false;
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
											read = stream.Read (buffer, 0, 4096);

											// make sure we read some data
											if (read != 0) {
												ms.Write (buffer, 0, read);
											}
										} while (read > 0); // any more data to read?
																				
										if (ms.Length > 0) {
											
											ms.Position = 0;
											
												Gtk.Image frame = new Gtk.Image ();
												frame.Pixbuf = new Gdk.Pixbuf (ms);
												if (frame != null && frame.Pixbuf != null) {
													
													device.frameReady (frame);
													frame.Dispose ();
												} 
											
												
																
										} 
									} catch (Exception e) {
										device.AddLog (e.Message);
										Thread.Sleep (1000);
									
									}
								}
															
								
								resp.Close ();
								resp = null;
								req = null;
								
							} catch (Exception e) { 
								//device.AddLog (e.Message + " " + url + " type:" + e.GetType ());
							}
							
							Thread.Sleep (1000 / 30);
						}
						
						//device.AddLog ("idle " + url);
					} else {
						//Console.WriteLine ("idle 2 " + url);
					}
					
					Thread.Sleep (1000/20);
				}
				
				device.AddLog ("Shutdown");
			}
		}
		
		
	}
}

