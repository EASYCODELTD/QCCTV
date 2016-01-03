using System;
using CQCCTV.Drivers;


namespace CQCCTV
{
	namespace Drivers {
		
		public class EmptyDriver : ICameraDriver
		{
			public EmptyDriver ()
			{
				DriverName = "Empty Driver";
				isEmptyDriver = true;
			}
			
			public override bool TestConnection ()
			{ 
				device.AddLog ("Can`t connect to device");
				return false;
			}	
		}
	}
}

