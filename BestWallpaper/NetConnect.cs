using System.IO;
using System.Net;
using System.Text;

namespace BestWallpaper
{
    /*
     * NetConnect: Provide functions based on HTTP
     */
    class NetConnect
    {
        /*
         * HttpGet: Implement GET method in HTTP
         * @Param url: website url
         * @Param dateStr: get parameters
         */
        public static string HttpGet(string url, string dateStr)
        {
            // set url
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url + (dateStr == "" ? "" : "?") + dateStr);
            request.Method = "GET"; // set method
            request.ContentType = "text/html;charset=UTF-8";    // set contentType
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();  // send request
            Stream myResponseStream = response.GetResponseStream();
            StreamReader myStreamReader = new StreamReader(myResponseStream, Encoding.UTF8);
            string retString = myStreamReader.ReadToEnd();
            myStreamReader.Close();
            myResponseStream.Close();

            return retString;
        }

        /*
         * HttpDownloadFile: file download function
         * @Param url: file url
         * @Param path: save file path
         */
        public static string HttpDownloadFile(string url, string path)
        {
            HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;  // set url
            HttpWebResponse response = request.GetResponse() as HttpWebResponse;    // send request
            Stream responseStream = response.GetResponseStream();
            Stream stream = new FileStream(@"pic\"+path, FileMode.Create);  // create local file
            byte[] bArr = new byte[1024];   // string buffer
            int size = responseStream.Read(bArr, 0, (int)bArr.Length);  // read from response
            while (size > 0)
            {
                stream.Write(bArr, 0, size);    // write to file
                size = responseStream.Read(bArr, 0, (int)bArr.Length);
            }
            stream.Close();
            responseStream.Close();
            return path;    // return file absolute path
        }
    }
}
