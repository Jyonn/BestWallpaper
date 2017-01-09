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
        public static List<SimplePhoto>[] photoAlbum = new List<SimplePhoto>[5];    // store total photos
        public static List<SimplePhoto> currentPhotos; // store current photo path
        /*
         * Save: save date from memory to file
         */
        public static void Save()
        {
            StreamWriter fWrite = new StreamWriter(filename, false);    // file writer
            int count = 0;
            foreach (List<SimplePhoto> simplePhoto in photoAlbum)
                count += simplePhoto.Count;
            fWrite.WriteLine(count);   // write number of total photos
            foreach (List<SimplePhoto> currentPhotos in photoAlbum)
                foreach (SimplePhoto simplePhoto in currentPhotos)
                    fWrite.WriteLine(simplePhoto.path); // write each path of photo
            fWrite.Close();
            fWrite.Dispose();
        }
        /*
         * Load: load date from file to memory
         */
        public static void Load()
        {
            for (int i = 0; i < 5; i++)
                photoAlbum[i] = new List<SimplePhoto>();
            if (!File.Exists(filename)) // check data.dat if exist
                return;
            StreamReader fRead = new StreamReader(filename, false); // file reader
            int count = int.Parse(fRead.ReadLine());    // read number of current photo
            for (int i = 0; i < count; i++)
            {
                // add photos into list
                SimplePhoto item = new SimplePhoto();
                item.path = fRead.ReadLine();
                for (int j = 0; j < Setting.picModeStr.Length; j++)
                    if (Setting.picModeStr[j].Equals(item.path.Substring(0, 2)))
                        photoAlbum[j].Add(item);
            }
            currentPhotos = photoAlbum[Setting.picMode];
            fRead.Close();
            fRead.Dispose();
        }
        /*
         * Remove: delete image file and remove it from list, when total number is more than iMaxPicNum
         */
        public static void Remove(int index)
        {
            if (File.Exists(@"pic\" + currentPhotos[index].path))    // check
                File.Delete(@"pic\" + currentPhotos[index].path);    // delete file
            currentPhotos.Remove(currentPhotos[index]);   // remove from list
            Save();
        }
        /*
         * Touch: check if exist, or remove it from list
         */
        public static Boolean Touch(int index)
        {
            if (File.Exists(@"pic\"+currentPhotos[index].path))
                return true;    //exist

            currentPhotos.Remove(currentPhotos[index]);   // remove from list
            Save();
            return false;
        }
    }
}
