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
    class Program
    {
        public class Photos
        {
            public string id;
            //public string created_at;
            //public int width;
            //public int height;
            //public string color;
            public int likes;
            //public bool liked_by_user;
            public User user;
            public class User
            {
                //public string id;
                public string username;
                public string name;
                //public string first_name;
                //public string last_name;
                //public string portfolio_url;
                //public string bio;
                //public string location;
                //public int total_likes;
                //public int total_photos;
                //public int total_collections;
                public ProfileImage profileImage;
                public class ProfileImage
                {
                    public string small;
                    //public string medium;
                    //public string large;
                }
                //public Links links;
                //public class Links
                //{
                //    public string self;
                //    public string html;
                //    public string photos;
                //    public string likes;
                //    public string portfolio;
                //    public string following;
                //    public string followers;
                //}
            }
            public Urls urls;
            public class Urls
            {
                public string raw;
                public string full;
                public string regular;
                public string small;
                public string thumb;
            }
            //public Links links;
            //public class Links
            //{
            //    public string self;
            //    public string html;
            //    public string download;
            //    public string download_location;
            //}
        }
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
                Console.WriteLine(photo.id + "\t" + photo.user.name + "(" + photo.user.username + ")" + "\t" + photo.urls.thumb);
                HttpDownloadFile(photo.urls.thumb, photo.id + ".jpg");
            }
        }

    }
}
