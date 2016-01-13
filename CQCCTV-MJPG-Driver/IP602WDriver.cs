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

		public class IP602WDriver : MJPGDriver
		{

			
			public IP602WDriver ()
			{
				DriverName = "IP602W";
				isNetworkCamera = true;
				isLoginReqired = true;
				isPasswordRequired = true;

			}

		}
	}
}

