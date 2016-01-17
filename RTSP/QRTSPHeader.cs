using System;
using System.Collections;

namespace RTSP
{
	public class QRTSPHeader : Hashtable
	{

		public QRTSPHeader(string rawheader="")
		{
			if (rawheader != "") {
				string[] split = rawheader.Split ('\r');


				foreach (string line in split) {
					string[] a = line.Split (':');	
					this.Add (a [0], a [1]);
				}
			}
		}

		public override string ToString()
		{
			string ret = "";

			foreach (DictionaryEntry entry in this)
			{
				ret += "\t"+entry.Key+": "+entry.Value + "\r\n";
			}

			ret += "\n\t\t\t\t\r\n";

			return ret;
		}

		public string Get(object key)
		{
			

			foreach (DictionaryEntry entry in this)
			{
				if (key == entry.Key) {
					return entry.Value as string;
				}

			}



			return null;
		}
	}


}

