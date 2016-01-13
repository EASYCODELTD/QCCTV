using System;
using System.Threading;
using System.Net;
using System.IO;
using System.Text;
using Gdk;
using QCCTVDefs;

namespace CQCCTV
{
	namespace Drivers
	{

		public class MJPGDriver : ICameraDriver
		{
			private Thread thread;
			private bool connect;
			private bool run;
			private bool dontRun;
			
			private int		bytesReceived;
			private const int	bufSize = 512 * 1024;	// buffer size
			private const int	readSize = 1024;		// portion size to read
			
			public MJPGDriver ()
			{
				DriverName = "MJPG Camera";
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
			
			
			public void WorkerThread ()
			{
				
			
				byte[] buffer = new byte[bufSize];	// buffer to read stream
				
				string url;
			
				url = URI;
				url = url.Replace ("[LOGIN]", Login);
				url = url.Replace ("[PASSWORD]", Password);
			
				while (true) {
				
				
					HttpWebRequest req = null;
					WebResponse resp = null;
					Stream stream = null;
					byte[] delimiter = null;
					byte[] delimiter2 = null;
					byte[] boundary = null;
					int boundaryLen, delimiterLen = 0, delimiter2Len = 0;
					int read, todo = 0, total = 0, pos = 0, align = 1;
					int start = 0, stop = 0;
					
					// align
					//  1 = searching for image start
					//  2 = searching for image end
					try {
						
					
						
						
						// create request
						Console.WriteLine ("MAKE REQUEST TO " + url);
						req = (HttpWebRequest)WebRequest.Create (url);
						req.Headers.Add ("Accept-Encoding", "gzip,deflate");
						req.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
						Console.WriteLine ("SET CREDITAILS  ");
						
						req.Credentials = new NetworkCredential (Login, Password);
						
						Console.WriteLine ("MAKED REQUEST TO " + url);
						
						// get response
						resp = req.GetResponse ();
	
						Console.WriteLine ("GOT RESPONSE " + url + "[" + resp.ContentType + "]");
						
						// check content type
						string ct = resp.ContentType;
						if (ct.IndexOf ("multipart/x-mixed-replace") == -1) {
							Console.WriteLine ("NOT SUPPORTED MJPEG");
							
							throw new ApplicationException ("Invalid URL");
						}
	
						Console.WriteLine ("FIND 1 BOUNDUARY");
						
						// get boundary
						ASCIIEncoding encoding = new ASCIIEncoding ();
						boundary = encoding.GetBytes (ct.Substring (ct.IndexOf ("boundary=", 0) + 9));
						boundaryLen = boundary.Length;
	
						Console.WriteLine ("FIND 2 BOUNDUARY " + ct.Substring (ct.IndexOf ("boundary=", 0) + 9));
						
						// get response stream
						stream = resp.GetResponseStream ();
	
						
						
						// loop
						while (run) {
							// check total read
							if (total > bufSize - readSize) {
								total = pos = todo = 0;
							}
	
							// read next portion from stream
							if ((read = stream.Read (buffer, total, readSize)) == 0)
								throw new ApplicationException ();
	
							total += read;
							todo += read;
	
							// increment received bytes counter
							bytesReceived += read;
					
							//Console.WriteLine ("DATA STREAM " + bytesReceived);
							
							// does we know the delimiter ?
							if (delimiter == null) {
								// find boundary
								
								pos = ByteArrayUtils.Find (buffer, boundary, pos, todo);
	
								if (pos == -1) {
									// was not found
									todo = boundaryLen - 1;
									pos = total - todo;
									continue;
								}
	
								todo = total - pos;
	
								if (todo < 2)
									continue;
	
								// check new line delimiter type
								if (buffer [pos + boundaryLen] == 10) {
									delimiterLen = 2;
									delimiter = new byte[2] {10, 10};
									delimiter2Len = 1;
									delimiter2 = new byte[1] {10};
								} else {
									delimiterLen = 4;
									delimiter = new byte[4] {13, 10, 13, 10};
									delimiter2Len = 2;
									delimiter2 = new byte[2] {13, 10};
								}
	
								pos += boundaryLen + delimiter2Len;
								todo = total - pos;
							}
	
							// search for image
							if (align == 1) {
								start = ByteArrayUtils.Find (buffer, delimiter, pos, todo);
								if (start != -1) {
									// found delimiter
									start += delimiterLen;
									pos = start;
									todo = total - pos;
									align = 2;
								} else {
									// delimiter not found
									todo = delimiterLen - 1;
									pos = total - todo;
								}
							}
	
							// search for image end
							while ((align == 2) && (todo >= boundaryLen)) {
								stop = ByteArrayUtils.Find (buffer, boundary, pos, todo);
								if (stop != -1) {
									pos = stop;
									todo = total - pos;
	

									var ms = new MemoryStream (buffer, start, stop - start);
									
									if(ms.Length>0)
									{
										device.frameReady (ms.ToArray());
									}
						
									
									
									// shift array
									pos = stop + boundaryLen;
									todo = total - pos;
									Array.Copy (buffer, pos, buffer, 0, todo);
	
									total = todo;
									pos = 0;
									align = 1;
								} else {
									// delimiter not found
									todo = boundaryLen - 1;
									pos = total - todo;
								}
							}
							
							
							}
						} catch (WebException ex) {
							Console.WriteLine ("=============: " + ex.Message);
							// wait for a while before the next try
							Thread.Sleep (250);
						} catch (ApplicationException ex) {
							Console.WriteLine ("=============: " + ex.Message);
							// wait for a while before the next try
							Thread.Sleep (250);
						} catch (Exception ex) {
							Console.WriteLine ("=============: " + ex.Message);
						} finally {
							// abort request
							if (req != null) {
								req.Abort ();
								req = null;
							}
							// close response stream
							if (stream != null) {
								stream.Close ();
								stream = null;
							}
							// close response
							if (resp != null) {
								resp.Close ();
								resp = null;
							}
						}

					Thread.Sleep (1000/30);
					// need to stop ?
					if (!run)
						break;
				}
			
				Console.WriteLine ("Death 2 !!!");
			}
		}
	}
}

