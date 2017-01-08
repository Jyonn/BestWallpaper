using System;
using System.Collections.Generic;
using System.IO;

namespace BestWallpaper
{
    public class PicStore
    {
        static string filename = "data.dat";
        public static int iMaxPicNum = 30;
        public static List<SimplePhoto> simplePhotos = new List<SimplePhoto>();
        public static void Save()
        {
            string absPath = Environment.CurrentDirectory + "\\" + filename;
            System.IO.StreamWriter fWrite = new System.IO.StreamWriter(absPath, false);
            fWrite.Write(iMaxPicNum);
            fWrite.Write(simplePhotos);
            fWrite.Close();
            fWrite.Dispose();
        }
        public static void Remove(int index)
        {
            if (File.Exists(simplePhotos[index].path))
                File.Delete(simplePhotos[index].path);
            simplePhotos.Remove(simplePhotos[index]);
        }
        public static Boolean Touch(int index)
        {
            if (File.Exists(simplePhotos[index].path))
                return true;
            simplePhotos.Remove(simplePhotos[index]);
            return false;
        }
    }
}
