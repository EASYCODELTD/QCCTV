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
		
			public RTSPDriver ()
			{
				DriverName = "RTSP Camera Driver";
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
				try {

				while (run) {

					Thread.Sleep (1000);
					Console.WriteLine("CONNECT LOOP");
				}

				} catch(Exception ex) {
					Console.Error.WriteLine (ex.Message);
				}
			}






			/** end **/
		}
	}
}

