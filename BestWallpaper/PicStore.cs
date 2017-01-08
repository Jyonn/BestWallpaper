using System;
using System.Collections.Generic;
using System.IO;

namespace BestWallpaper
{
    /*
     * List of photos, main class
     */
    public class PicStore
    {
        public static int photoIndex = 0;
        static string filename = "data.dat";    // local saved file
        public static int iMaxPicNum = 30;  // number of max images saved, default 30
        public static List<SimplePhoto> simplePhotos = new List<SimplePhoto>(); // store photo path
        /*
         * Save: save date from memory to file
         */
        public static void Save()
        {
            StreamWriter fWrite = new StreamWriter(filename, false);    // file writer
            fWrite.WriteLine(iMaxPicNum);   // write iMaxPicNum
            fWrite.WriteLine(simplePhotos.Count);   // write number of current photo
            foreach (SimplePhoto simplePhoto in simplePhotos)
                fWrite.WriteLine(simplePhoto.path); // write each path of photo
            fWrite.Close();
            fWrite.Dispose();
        }
        /*
         * Load: load date from file to memory
         */
        public static void Load()
        {
            if (!File.Exists(filename)) // check data.dat if exist
                return;
            StreamReader fRead = new StreamReader(filename, false); // file reader
            iMaxPicNum = int.Parse(fRead.ReadLine());   // read iMaxPicNum
            int count = int.Parse(fRead.ReadLine());    // read number of current photo
            for (int i = 0; i < count; i++)
            {
                // add photos into list
                SimplePhoto item = new SimplePhoto();
                item.path = fRead.ReadLine();
                simplePhotos.Add(item);
            }
            fRead.Close();
            fRead.Dispose();
        }
        /*
         * Remove: delete image file and remove it from list, when total number is more than iMaxPicNum
         */
        public static void Remove(int index)
        {
            if (File.Exists(@"pic\" + simplePhotos[index].path))    // check
                File.Delete(@"pic\" + simplePhotos[index].path);    // delete file
            simplePhotos.Remove(simplePhotos[index]);   // remove from list
        }
        /*
         * Touch: check if exist, or remove it from list
         */
        public static Boolean Touch(int index)
        {
            if (File.Exists(@"pic\"+simplePhotos[index].path))
                return true;    //exist

            simplePhotos.Remove(simplePhotos[index]);   // remove from list
            return false;
        }
    }
}
