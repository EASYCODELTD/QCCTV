using System;
namespace CQCCTV
{
	namespace Drivers
	{

		public class USBDriver : ICameraDriver
		{
			
			public USBDriver ()
			{
				DriverName = "USB Camera Driver";
				isNetworkCamera = false;
				isLoginReqired = false;
				isPasswordRequired = false;
				isAutoDiscover = true;
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
			
		}
	}
}

