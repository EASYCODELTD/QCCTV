using System;
using System.Runtime.InteropServices;
using CQCCTV.Drivers;
using System.Collections.Generic;

namespace CQCCTV.Drivers
{
	public class Export : IExportDriver
	{
		public List<ICameraDriver> Init() {
			List<ICameraDriver> drivers = new List<ICameraDriver>();

			drivers.Add (new JPGDriver ());
			drivers.Add (new IPROBOT3JPGDriver ());
			drivers.Add (new JPT3815WJPGDriver ());

			return drivers;
		}

	}
}

