using System;
using System.Threading;
using System.Net;
using System.IO;
using System.Text;
using Gdk;
using QCCTVDefs;
using System.IO.Compression;
using Ozeki.Camera;

namespace CQCCTV
{
	namespace Drivers
	{

		public class OzekiDriver : ICameraDriver
		{
			private Thread thread;
			private bool connect;
			private bool run;
			private bool dontRun;

			IIPCamera camera=null;

			public OzekiDriver ()
			{
				DriverName = "Ozeki Camera Driver";
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

					try {
					camera = IPCameraFactory.GetCamera (URI, Login, Password);
					camera.Start ();
					} catch(Exception e) {
						Console.Error.WriteLine (e.Message);
					}

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
					camera.Stop ();
					camera.Dispose ();
					camera = null;
				}
			}




			// Thread entry point
			public void WorkerThread ()
			{
				
			}
		}


	}
}

