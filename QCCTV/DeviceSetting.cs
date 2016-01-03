using System;
using System.Xml;
using System.Xml.Serialization;

namespace QCCTV
{
	[XmlRoot("DeviceSetting")]
	public class DeviceSetting
	{
		public string DeviceName = "Empty Device";
		public string DriverName = "Empty Driver";
		public string DeviceURI = "";
		public string DeviceLogin = "";
		public string DevicePassword = "";
		public bool   hasControls = false;

		public string controlsUP = "";
		public string controlsDOWN = "";
		public string controlsLEFT = "";
		public string controlsRIGHT = "";

		public DeviceSetting ()
		{
		}
	}
}

