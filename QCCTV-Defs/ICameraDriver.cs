using System;
using QCCTVDefs;


namespace CQCCTV
{
	namespace Drivers 
	{


		
		public class ICameraDriver : ICloneable
		{

				
			public string DriverName = "Empty Driver";
			public string URI = "";
			public string Login = "";
			public string Password = "";
			public IDevice device=null;
			
			public bool isEmptyDriver = false;
			public bool isNetworkCamera = false;
			public bool isPasswordRequired = false;
			public bool isLoginReqired = false;
			public bool isAutoDiscover = false;
			
			 object ICloneable.Clone()
		    {
		        return this.Clone();
		    }
			
		    public ICameraDriver Clone ()
			{
				return (ICameraDriver)this.MemberwiseClone();
		    }
			
			public virtual string getDriverName ()
			{
				return DriverName; }
			
			
			public virtual bool hasControls() {
						return false;
			}

			public virtual void control(int what,int how)
			{
				
			}

			public virtual void shutdown ()
			{
			
			}			
			
			
			public virtual bool TestConnection() { return false; }
			public virtual void Connect() { }
			public virtual void Disconnect() { }
			
			
		}
	}
}
