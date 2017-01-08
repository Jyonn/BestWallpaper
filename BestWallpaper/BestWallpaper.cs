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
            int changeTimeInt = 1000 * 10;  // change interval time
            while (true)
            {
                int oldPhotoIndex = PicStore.photoIndex;    // last photo index

                mutex.WaitOne();    // get mutex of PicStore.simplePhoto
                if (PicStore.simplePhotos.Count <= 0)   // no photo in lsit
                {
                    mutex.ReleaseMutex();
                    Thread.Sleep(changeTimeInt);
                    continue;
                }
                if (PicStore.simplePhotos.Count <= PicStore.photoIndex) // photoIndex is too large
                    PicStore.photoIndex = 0;
                else
                    // get next photo index
                    PicStore.photoIndex = (PicStore.photoIndex + PicStore.simplePhotos.Count - 1) % PicStore.simplePhotos.Count;
                // check if exist
                while (PicStore.Touch(PicStore.photoIndex) == false)
                {
                    // not exist, change to another
                    if (PicStore.simplePhotos.Count <= 0)
                    {
                        Thread.Sleep(changeTimeInt);
                        continue;
                    }
                    if (PicStore.simplePhotos.Count <= PicStore.photoIndex)
                        PicStore.photoIndex = 0;
                    if (PicStore.simplePhotos.Count <= 0)
                        break;
                }
                // check if list is null
                if (PicStore.simplePhotos.Count <= 0)
                {
                    mutex.ReleaseMutex();
                    Thread.Sleep(changeTimeInt);
                    continue;
                }
                // absolute path of image
                String absPath = Environment.CurrentDirectory + @"\pic\" + PicStore.simplePhotos[PicStore.photoIndex].path;
                mutex.ReleaseMutex();   // release mutex of PicStore.simplePhotos

                // if index is same, no need change wallpaper
                if (PicStore.photoIndex != oldPhotoIndex)
                {
                    Console.WriteLine("ChangeWallpaper() change To " + absPath.Substring(absPath.LastIndexOf('\\')+1));
                    SystemParametersInfo(SPI_SETDESKWALLPAPER, 0, absPath, SPIF_UPDATEINIFILE | SPIF_SENDWININICHANGE);
                }

                // sleep interval time
                Thread.Sleep(changeTimeInt);
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
            //Console.WriteLine("Callback() get mutex...");
            SimplePhoto r = PicStore.simplePhotos.Find(
                delegate (SimplePhoto simplePhoto)
                {
                    return simplePhoto.path.Equals(path);
                });
            if (r == null)
            {
                if (PicStore.simplePhotos.Count > PicStore.iMaxPicNum)
                    PicStore.Remove(0);
                PicStore.simplePhotos.Add(item);
            }
            PicStore.Save();
            mutex.ReleaseMutex();
            //Console.WriteLine("Callback() release mutex...");
        }
        static AsyncCallback callback = new AsyncCallback(AsyncCallbackImpl);

        static string url = "https://api.unsplash.com/photos/?client_id=064937caaac57ac9c253c26c242231b17a4f0a145b0051bc4be554c6f8093a71";
        static string data = "";
        static void Download()
        {
            int downloadTimeInt = 1000 * 60 * 20;
            int downloadFailedTimeInt = 1000 * 10;
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
                    Thread.Sleep(downloadFailedTimeInt);
                    continue;
                }
                //Console.WriteLine(result);
                //Console.ReadLine();

                List<Photo> photos = JsonConvert.DeserializeObject<List<Photo>>(result);
                int i = 0;
                foreach (Photo photo in photos)
                {
                    Console.WriteLine("Download() " + i++ + "\t" + photo.id + "\t Downloading...");
                    fh.BeginInvoke(photo.urls.regular, photo.id + "_rg.jpg", callback, null);
                    //HttpDownloadFile(photo.urls.thumb, photo.id + ".jpg");
                }
                Console.WriteLine("Download wait for next download...");
                Thread.Sleep(downloadTimeInt);
            }
        }
    }
}
