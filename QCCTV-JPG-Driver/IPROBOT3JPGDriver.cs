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

		public class IPROBOT3JPGDriver : JPGDriver
		{


			public IPROBOT3JPGDriver ()
			{
				DriverName = "IPROBOT3 (JPG)";
				isNetworkCamera = true;
				isLoginReqired = true;
				isPasswordRequired = true;
				pause = false;

				_requiredFps = 20;
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
					
		}
		
		
	}
}

