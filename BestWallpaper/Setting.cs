using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BestWallpaper
{
    class Setting
    {
        public static int changeFrequent = 10000;   // change interval time
        public static int downloadFrequent = 120000; // download interval time
        public static int downloadFailFrequent = 10000; // download failed intveral time
        public static int PICMODE_RAW = 0;
        public static int PICMODE_FULL = 1;
        public static int PICMODE_REGULAR = 2;
        public static int PICMODE_SMALL = 3;
        public static int PICMODE_THUMB = 4;
        public static string clientID = "064937caaac57ac9c253c26c242231b17a4f0a145b0051bc4be554c6f8093a71";
        public static int iMaxPicNum = 100;  // number of max images saved, default 30
        public static int picMode = PICMODE_REGULAR;
        public static string[] picModeStr = { "rw", "fl", "rg", "sm", "th" };
        static string filename = "setting.conf";    // local config file
        static void Translate(string s)
        {
            if (s.IndexOf('=') == -1)
                return;
            string head = s.Substring(0, s.IndexOf('='));
            string body = s.Substring(s.IndexOf('=') + 1);
            if (head.Equals("ChangeFrequent"))
                changeFrequent = int.Parse(body);
            else if (head.Equals("DownloadFrequent"))
                downloadFrequent = int.Parse(body);
            else if (head.Equals("DownloadFailFrequent"))
                downloadFailFrequent = int.Parse(body);
            else if (head.Equals("ClientID"))
                clientID = body;
            else if (head.Equals("MaxPictureNumber"))
                iMaxPicNum = int.Parse(body);
            else if (head.Equals("PictureMode"))
            {
                if (body.Equals("raw"))
                    picMode = PICMODE_RAW;
                else if (body.Equals("full"))
                    picMode = PICMODE_FULL;
                else if (body.Equals("regular"))
                    picMode = PICMODE_REGULAR;
                else if (body.Equals("small"))
                    picMode = PICMODE_SMALL;
                else
                    picMode = PICMODE_THUMB;
            }
        }
        public static void Load()
        {
            string line;
            // check data.dat if exist
            if (!Directory.Exists("pic"))
                Directory.CreateDirectory("pic");
            foreach (string dir in picModeStr)
                if (!Directory.Exists(@"pic\" + dir))
                    Directory.CreateDirectory(@"pic\" + dir);
            if (!File.Exists(filename))
                return;
            StreamReader fRead = new StreamReader(filename, false); // file reader
            while ((line = fRead.ReadLine()) != null)
                Translate(line);
        }
    }
}
