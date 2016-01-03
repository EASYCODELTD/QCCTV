using System;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using System.Text;
using System.Collections.Generic;
using QCCTVCameraViewWidget;
using QCCTVDefs;

namespace QCCTV
{
	[XmlRoot("AppSetting")]
	public class AppSettings
	{
		
		public bool toolboxVisible = false;
		public List<DeviceSetting> deviceSettings = null;
		public List<ViewSetting> viewSettings = null;
	
		
		public AppSettings ()
		{
			toolboxVisible = false;
			deviceSettings = new  List<DeviceSetting>();
			
		}
		
		public void SaveToFile (string path)
		{
			try {
				var serializer = new XmlSerializer (typeof(AppSettings));

		
				using (var file = new FileStream(path, FileMode.Create)) {
					using (StreamWriter stream = new StreamWriter(file, Encoding.UTF8)) {
						Console.WriteLine (stream.ToString ());
						serializer.Serialize (stream, this);
				
					}

					file.Close ();
				}
				
	
				
				Console.WriteLine ("Settings save to: " + path + " ");
				
			} catch (Exception e) {
				Console.WriteLine ("Can`t save settings: " + path + " " + e.Message);	
				
			}
		}
		
		
		public static AppSettings LoadFromFile (string path)
		{
			try {
				var serializer = new XmlSerializer (typeof(AppSettings));
				AppSettings settings = null;
				
				using (var file = new FileStream(path, FileMode.Open)) {
					using (StreamReader stream = new StreamReader(file, Encoding.UTF8)) {
						settings = serializer.Deserialize (stream) as AppSettings;
					}
				}
				
				Console.WriteLine ("Settings loaded from: " + path + " ");
				
				return settings;
				
			} catch (Exception e) {
				Console.WriteLine ("Can`t load settings: " + path + " " + e.Message);	
				
			}
			return new AppSettings();
		}
	}
}

