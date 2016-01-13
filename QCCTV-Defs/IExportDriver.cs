using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace CQCCTV.Drivers
{
	public class IExportDriver
	{

		public List<ICameraDriver> Init(){ throw new EntryPointNotFoundException (); }
	}
}

