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
            StreamWriter fWrite = new StreamWriter(filename, false);
            fWrite.WriteLine(iMaxPicNum);
            fWrite.WriteLine(simplePhotos.Count);
            foreach (SimplePhoto simplePhoto in simplePhotos)
                fWrite.WriteLine(simplePhoto.path);
            fWrite.Close();
            fWrite.Dispose();
        }
        public static void Load()
        {
            if (!File.Exists(filename))
                return;
            StreamReader fRead = new StreamReader(filename, false);
            iMaxPicNum = int.Parse(fRead.ReadLine());
            int count = int.Parse(fRead.ReadLine());
            for (int i = 0; i < count; i++)
            {
                SimplePhoto item = new SimplePhoto();
                item.path = fRead.ReadLine();
                simplePhotos.Add(item);
            }
            fRead.Close();
            fRead.Dispose();
        }
        public static void Remove(int index)
        {
            if (File.Exists(@"pic\" + simplePhotos[index].path))
                File.Delete(@"pic\" + simplePhotos[index].path);
            simplePhotos.Remove(simplePhotos[index]);
        }
        public static Boolean Touch(int index)
        {
            if (File.Exists(@"pic\"+simplePhotos[index].path))
                return true;
            simplePhotos.Remove(simplePhotos[index]);
            return false;
        }
    }
}
