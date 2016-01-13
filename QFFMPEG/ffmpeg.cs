using System;
using System.Diagnostics;
using System.Threading;
using System.IO;
using System.Collections.Generic;

namespace QFFMPEG
{
	public class ffmpeg
	{
		public int frameid = 0;
		public int savedFrame = 0;
		private Thread thread;
		private Thread thread2;
		public bool running=true;
		public string name;
		public string tmpdir;
		public string output;
		public int minute;
		private Queue<Gtk.Image> myQueue = new Queue<Gtk.Image>();

		public ffmpeg (string name="",string output="",string tmpdir="")
		{
			this.tmpdir = tmpdir;
			this.name = name;
			this.output = output;

			DateTime date = DateTime.Now;
			minute = date.Minute;
				
			thread2 = new Thread (() => appendThread ());
			thread2.Start ();	

		}

		public void appendFrameToVideo(Gtk.Image frame)
		{
			lock (myQueue)
			{
				lock (frame) {
					Gtk.Image newframe = new Gtk.Image ();
					newframe.Pixbuf = (Gdk.Pixbuf)frame.Pixbuf.Clone ();
					myQueue.Enqueue (newframe);
				}
			}
		}

		private void appendThread()
		{
			Gtk.Image frame = null;


			while (true) {
				

				while (myQueue.Count > 0)
				{
					

					lock (myQueue)
					{
						if (frame != null) {
							frame.Dispose ();
							frame = null;
						}

						frame = myQueue.Dequeue();
					}
											

					
				} 

				if(frame!=null) append(frame);

				Thread.Sleep (1000 / 25);
			}
		}

		private void append(Gtk.Image frame)
		{
			DateTime date = DateTime.Now;


			string dirpath = tmpdir + "" + date.Day + "-" + date.Month + "-" + date.Year;
			if (!Directory.Exists (dirpath)) { Directory.CreateDirectory (dirpath); }
			dirpath += "/" + name;

			if (!Directory.Exists (dirpath)) { Directory.CreateDirectory (dirpath); }
			dirpath += "/" + minute;
			if (!Directory.Exists (dirpath)) { Directory.CreateDirectory (dirpath); }

			if (minute != date.Minute) {
				string target = output + "" + date.Day + "-" + date.Month + "-" + date.Year;
				if (!Directory.Exists (target)) { Directory.CreateDirectory (target); }
				target += "/"+name;
				if (!Directory.Exists (target)) { Directory.CreateDirectory (target); }
				target += "/"+date.Hour + ":" + date.Minute + ":" + date.Second;
				target += ".mp4";

				GenerateVideo (dirpath+"/", target,Convert.ToUInt32(frameid/60));

				minute = date.Minute;
				frameid = 0;
			}

			dirpath = tmpdir + "" + date.Day + "-" + date.Month + "-" + date.Year;
			if (!Directory.Exists (dirpath)) { Directory.CreateDirectory (dirpath); }
			dirpath += "/" + name;
			if (!Directory.Exists (dirpath)) { Directory.CreateDirectory (dirpath); }
			dirpath += "/" + minute;
			if (!Directory.Exists (dirpath)) { Directory.CreateDirectory (dirpath); }

			string tmpname = dirpath+"/";
			tmpname += frameid;
			tmpname += ".png";
			//Console.WriteLine (tmpname);

			try {
				frame.Pixbuf.Save (tmpname, "png");
				frameid++;
			} catch(Exception e) {
				
			}

		}

		public void GenerateVideo(string path,string target,uint fps)
		{
			thread = new Thread (() => WorkerThread (path,target,fps));
			thread.Start ();	
		}

		public void RemoveTmpFileDirectory(string path)
		{


			System.IO.DirectoryInfo di = new DirectoryInfo(path);

			foreach (FileInfo file in di.GetFiles())
			{
				file.Delete(); 
			}
			foreach (DirectoryInfo dir in di.GetDirectories())
			{
				dir.Delete(true); 
			}

			Directory.Delete (path);
		}

		public void WorkerThread (string path,string target,uint fps)
		{
			if (fps < 1) {
				Console.WriteLine ("Too low fps");		
				RemoveTmpFileDirectory (path);
				return;
			}
			
				Console.WriteLine ("Generate:"+path+" > "+target);					
			//return;

					Console.WriteLine("ffmpeg -start_number 0 -i \""+path+"%d.png\" -c:v libx264 -vf fps="+fps+" -r "+fps+" -pix_fmt yuv420p \""+target+"\"");
					
					Process proc = new Process {
						StartInfo = new ProcessStartInfo {
							FileName = "ffmpeg",
							Arguments = "-start_number 0 -i \""+path+"%d.png\"  -c:v libx264 -vf fps=25 -pix_fmt yuv420p \""+target+"\"",
							//Arguments = "-start_number 0 -framerate 1/5 -i \""+path+"%d.png\"  -c:v libx264 -vf \"fps=25,format=yuv420p\" \""+target+"\"",
							UseShellExecute = false,
							RedirectStandardOutput = true,
							CreateNoWindow = true
						}
					};

					proc.Start ();
				
					proc.Dispose ();
					proc = null;

					
				RemoveTmpFileDirectory (path);


		}
	}
}

