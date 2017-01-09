using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Microsoft.Win32;
using System.Runtime.InteropServices;
using System.Threading;


namespace BestWallpaper
{
    class BestWallpaper
    {
        static Mutex mutex = new Mutex();
        static int SPI_SETDESKWALLPAPER = 0X0014;
        static int SPIF_UPDATEINIFILE = 0x01;
        static int SPIF_SENDWININICHANGE = 0x02;
        [DllImport("user32.dll", EntryPoint = "SystemParametersInfo")]
        public static extern bool SystemParametersInfo(int uiAction, int uiParam, String pvParam, int fWinIni);
        //[DllImport("user32.dll", EntryPoint = "ShowWindow", SetLastError = true)]
        //private static extern bool ShowWindow(IntPtr hWnd, uint nCmdShow);
        //[DllImport("user32.dll", EntryPoint = "FindWindow", SetLastError = true)]
        //private static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        //public static void WindowHide(string consoleTitle)
        //{
        //    IntPtr a = FindWindow("ConsoleWindowClass", consoleTitle);
        //    if (a != IntPtr.Zero)
        //        ShowWindow(a, 0);//hide window
        //    else
        //        throw new Exception("can't hide console window");
        //}

        static void Main(string[] args)
        {
            //WindowHide(System.Console.Title);
            Setting.Load();
            PicStore.Load();
            RegistryKey key = Registry.CurrentUser.OpenSubKey(@"Control Panel\Desktop", true);
            key.SetValue(@"WallpaperStyle", 10.ToString()); // fill style

            new Thread(Download).Start();
            new Thread(changeWallpaper).Start();
        }
        /*
         * changeWallpaper: timed change wallpaper from photo list
         */
        static void changeWallpaper()
        {
            while (true)
            {
                int oldPhotoIndex = PicStore.photoIndex;    // last photo index

                mutex.WaitOne();    // get mutex of PicStore.simplePhoto
                if (PicStore.currentPhotos.Count <= 0)   // no photo in lsit
                {
                    mutex.ReleaseMutex();
                    Thread.Sleep(Setting.changeFrequent);
                    continue;
                }
                if (PicStore.currentPhotos.Count <= PicStore.photoIndex) // photoIndex is too large
                    PicStore.photoIndex = 0;
                else
                    // get next photo index
                    PicStore.photoIndex = (PicStore.photoIndex + PicStore.currentPhotos.Count - 1) % PicStore.currentPhotos.Count;
                // check if exist
                while (PicStore.Touch(PicStore.photoIndex) == false)
                {
                    // not exist, change to another
                    if (PicStore.currentPhotos.Count <= 0)
                    {
                        Thread.Sleep(Setting.changeFrequent);
                        continue;
                    }
                    if (PicStore.currentPhotos.Count <= PicStore.photoIndex)
                        PicStore.photoIndex = PicStore.currentPhotos.Count - 1;
                }
                // check if list is null
                if (PicStore.currentPhotos.Count <= 0)
                {
                    mutex.ReleaseMutex();
                    Thread.Sleep(Setting.changeFrequent);
                    continue;
                }
                // absolute path of image
                String absPath = Environment.CurrentDirectory + @"\pic\" + PicStore.currentPhotos[PicStore.photoIndex].path;
                mutex.ReleaseMutex();   // release mutex of PicStore.simplePhotos

                // if index is same, no need change wallpaper
                if (PicStore.photoIndex != oldPhotoIndex)
                {
                    Console.WriteLine("ChangeWallpaper() change To " + absPath.Substring(absPath.LastIndexOf('\\')+1));
                    SystemParametersInfo(SPI_SETDESKWALLPAPER, 0, absPath, SPIF_UPDATEINIFILE | SPIF_SENDWININICHANGE);
                    }

                // sleep interval time
                Thread.Sleep(Setting.changeFrequent);
            }
        }
        public delegate string FuncHandle(string url, string path);
        static FuncHandle fh = new FuncHandle(NetConnect.HttpDownloadFile);
        public static void AsyncCallbackImpl(IAsyncResult ar)
        {
            string path;
            try
            {
                path = fh.EndInvoke(ar);
                if (path == null)
                {
                    Console.WriteLine("Callback() failed to download...");
                    return;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Callback() failed to download...");
                return;
            }
            Console.WriteLine(path + "\t Done...");
            SimplePhoto item = new SimplePhoto();
            item.path = path;

            mutex.WaitOne();
            if (PicStore.currentPhotos.Count > Setting.iMaxPicNum)
                PicStore.Remove(0);
            PicStore.currentPhotos.Add(item);
            PicStore.Save();
            mutex.ReleaseMutex();
        }
        static AsyncCallback callback = new AsyncCallback(AsyncCallbackImpl);

        static string url = "https://api.unsplash.com/photos/?client_id=064937caaac57ac9c253c26c242231b17a4f0a145b0051bc4be554c6f8093a71";
        static string data = "";
        static void Download()
        {
            while (true)
            {
                Console.WriteLine("Download() try to download...");
                string result;
                try
                {
                    result = NetConnect.HttpGet(url, data);
                }
                catch(Exception e)
                {
                    Console.WriteLine("Download() fail to get response...");
                    Thread.Sleep(Setting.downloadFailFrequent);
                    continue;
                }
                //Console.WriteLine(result);
                //Console.ReadLine();

                List<Photo> photos = JsonConvert.DeserializeObject<List<Photo>>(result);
                int i = 0;
                foreach (Photo photo in photos)
                {
                    Console.WriteLine("Download() " + i++ + "\t" + photo.id + "\t Downloading...");
                    string path = Setting.picModeStr[Setting.picMode] + @"\" + photo.id + ".jpg";
                    string url;
                    if (Setting.picMode == Setting.PICMODE_RAW) url = photo.urls.raw;
                    else if (Setting.picMode == Setting.PICMODE_FULL) url = photo.urls.full;
                    else if (Setting.picMode == Setting.PICMODE_REGULAR) url = photo.urls.regular;
                    else if (Setting.picMode == Setting.PICMODE_SMALL) url = photo.urls.small;
                    else url = photo.urls.thumb;

                    mutex.WaitOne();
                    SimplePhoto r = PicStore.currentPhotos.Find(
                        delegate (SimplePhoto simplePhoto)
                        {
                            return simplePhoto.path.Equals(path);
                        });
                    mutex.ReleaseMutex();
                    if (r != null)
                        continue;
                    fh.BeginInvoke(url, path, callback, null);
                }
                Console.WriteLine("Download wait for next download...");
                Thread.Sleep(Setting.downloadFrequent);
            }
        }
    }
}
