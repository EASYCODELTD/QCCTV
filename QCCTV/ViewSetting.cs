using System;
using System.Xml;
using System.Xml.Serialization;

namespace QCCTV
{
	[XmlRoot("ViewSetting")]
	public class ViewSetting
	{
		public string DeviceName = null;
		public bool motionDetect = false;
		public bool record = false;
		
		public ViewSetting ()
		{
		}
	}
}

