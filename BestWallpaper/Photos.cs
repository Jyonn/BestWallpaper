using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BestWallpaper
{
    public class SimplePhoto
    {
        public string path;
        //SimplePhoto(string path, DateTime datetime) { this.path = path; this.datetime = datetime; }
    }
    class Photo
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
}
