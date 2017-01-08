using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using Newtonsoft.Json;
using Microsoft.Win32;
using System.Runtime.InteropServices;

namespace BestWallpaper
{
    class BestWallpaper
    {
        public static string HttpGet(string Url, string postDataStr)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Url + (postDataStr == "" ? "" : "?") + postDataStr);
            request.Method = "GET";
            request.ContentType = "text/html;charset=UTF-8";

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            Stream myResponseStream = response.GetResponseStream();
            StreamReader myStreamReader = new StreamReader(myResponseStream, Encoding.UTF8);
            string retString = myStreamReader.ReadToEnd();
            myStreamReader.Close();
            myResponseStream.Close();

            return retString;
        }
        public static string HttpDownloadFile(string url, string path)
        {
            // 设置参数
            HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;
            //发送请求并获取相应回应数据
            HttpWebResponse response = request.GetResponse() as HttpWebResponse;
            //直到request.GetResponse()程序才开始向目标网页发送Post请求
            Stream responseStream = response.GetResponseStream();
            //创建本地文件写入流
            Stream stream = new FileStream(path, FileMode.Create);
            byte[] bArr = new byte[1024];
            int size = responseStream.Read(bArr, 0, (int)bArr.Length);
            while (size > 0)
            {
                stream.Write(bArr, 0, size);
                size = responseStream.Read(bArr, 0, (int)bArr.Length);
            }
            stream.Close();
            responseStream.Close();
            return path;
        }

        
        const int SPI_SETDESKWALLPAPER = 0X0014;
        const int SPIF_UPDATEINIFILE = 0x01;
        const int SPIF_SENDWININICHANGE = 0x02;
        [DllImport("user32.dll", EntryPoint = "SystemParametersInfo")]
        public static extern bool SystemParametersInfo(int uiAction, int uiParam, String pvParam, int fWinIni);
        static void Main(string[] args)
        {
            RegistryKey key = Registry.CurrentUser.OpenSubKey(@"Control Panel\Desktop", true);
            key.SetValue(@"WallpaperStyle", 10.ToString());
            //key.SetValue(@"TileWallpaper", 0.ToString());
            SystemParametersInfo(SPI_SETDESKWALLPAPER, 0, Environment.CurrentDirectory+@"\JlPPDDD666ggg443.jpg", SPIF_UPDATEINIFILE | SPIF_SENDWININICHANGE);
        }
        public delegate string FuncHandle(string url, string path);
        static FuncHandle fh = new FuncHandle(HttpDownloadFile);
        public static void AsyncCallbackImpl(IAsyncResult ar)
        {
            string re = fh.EndInvoke(ar);
        }
        static AsyncCallback callback = new AsyncCallback(AsyncCallbackImpl);

        static void Download(string[] args)
        {
            string url = "https://api.unsplash.com/photos/?client_id=064937caaac57ac9c253c26c242231b17a4f0a145b0051bc4be554c6f8093a71";
            string data = "";
            string result = HttpGet(url, data);
            //Console.WriteLine(result);
            //Console.ReadLine();
            
            List<Photos> photos = JsonConvert.DeserializeObject<List<Photos>>(result);
            foreach (Photos photo in photos)
            {
                Console.WriteLine(photo.id + "\t" + photo.user.name + "(" + photo.user.username + ")" + "\t" + photo.urls.regular);
                fh.BeginInvoke(photo.urls.regular, photo.id + "_rg.jpg", callback, null);
                //HttpDownloadFile(photo.urls.thumb, photo.id + ".jpg");
                
            }
        }
    }
}
