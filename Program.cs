using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.IO;
using System.Threading;
using Newtonsoft.Json.Linq;

namespace CorpthingPls
{
    class Program
    {
        private static string nightcorpHtmlNew { get; set; }
        private static string nightcorpHtml { get; set; }        
        private static string commentsJson { get; set; }
        private static string postsJson { get; set; }
        private static int oldcomments = 0;
        private static int oldposts = 0;
        private static int newcomments = 0;
        private static int newposts = 0;
        private const string nightcorpUrl = "https://nightcorp.net/";
        private const string commentsUrl = "https://www.reddit.com/user/corpthing/comments.json";
        private const string postsUrl = "https://www.reddit.com/user/corpthing/submitted.json";
        private const string logFile = "log.txt";


        static void Main(string[] args)
        {
            nightcorpHtml = GetHtml(nightcorpUrl, null);
            commentsJson = GetHtml(commentsUrl, null);
            postsJson = GetHtml(postsUrl, null);
            Console.WriteLine(nightcorpHtml);
            Console.WriteLine(commentsJson);
            Console.WriteLine(postsJson);

            var json = JObject.Parse(commentsJson);
            oldcomments = GetChildrenCount(json);
            json = JObject.Parse(postsJson);
            oldposts = GetChildrenCount(json);

            Console.WriteLine("============================================");
            Console.WriteLine("Oh I sure hope CorpThing does something now");
            Console.WriteLine("=============================================");

            while (true)
            {
                Thread.Sleep(15000);
                Console.WriteLine("[{0}] Requesting... ({1} comments {2} posts) ", DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss"), oldcomments, oldposts);
                nightcorpHtmlNew = GetHtml(nightcorpUrl, nightcorpHtml);
                if (!nightcorpHtmlNew.Equals(nightcorpHtml))
                {
                    Ding();
                    Log("[{0}] Nightcorp site changed\nNew html:\n{1}\n", DateTime.UtcNow.ToString("YYYY-MM-dd hh:mm:ss"), nightcorpHtmlNew);
                    Console.WriteLine("==========================");
                    Console.WriteLine("=====Nightcorp site change=====");
                    Console.WriteLine("==========================");
                    Console.WriteLine(nightcorpHtmlNew);
                    Console.WriteLine("==========================");
                    Console.WriteLine("=====Nightcorp site change=====");
                    Console.WriteLine("==========================");
                    nightcorpHtml = nightcorpHtmlNew;
                }

                commentsJson = GetHtml(commentsUrl, null);
                if (!string.IsNullOrEmpty(commentsJson))
                {
                    json = JObject.Parse(commentsJson);
                    newcomments = ((JArray)(json["data"]["children"])).Count;
                    if (newcomments != oldcomments)
                    {
                        Ding();
                        Log("[{0}] Comments count changed\nNew json:\n{1}\n", DateTime.UtcNow.ToString("YYYY-MM-dd hh:mm:ss"), commentsJson);
                        Console.WriteLine("==========================");
                        Console.WriteLine("=====Reddit comments change===");
                        Console.WriteLine("==========================");
                        Console.WriteLine(commentsJson);
                        Console.WriteLine("==========================");
                        Console.WriteLine("=====Reddit comments change===");
                        Console.WriteLine("==========================");
                        oldcomments = newcomments;
                    }
                }

                postsJson = GetHtml(postsUrl, "");
                if (!string.IsNullOrEmpty(postsJson))
                {
                    json = JObject.Parse(postsJson);
                    newposts = ((JArray)(json["data"]["children"])).Count;

                    if (newposts != oldposts)
                    {
                        Ding();
                        Log("[{0}] Posts count changed\nNew json:\n{1}\n", DateTime.UtcNow.ToString("YYYY-MM-dd hh:mm:ss"), postsJson);
                        Console.WriteLine("==========================");
                        Console.WriteLine("=====Reddit submitted change===");
                        Console.WriteLine("==========================");
                        Console.WriteLine(postsJson);
                        Console.WriteLine("==========================");
                        Console.WriteLine("=====Reddit submitted change===");
                        Console.WriteLine("==========================");
                    }
                    oldposts = newposts;
                }

            }
        }

        public static int GetChildrenCount(JObject json)
        {
            return ((JArray)(json["data"]["children"])).Count;
        }

        public static void Log(string format, params string[] args)
        {
            File.AppendAllText(logFile, string.Format(format, args));
        }

        public static void Ding()
        {
            System.Media.SoundPlayer player = new System.Media.SoundPlayer(@"264594_65641-lq.wav");
            player.Play();
        }

        static string GetHtml(string url, string defaultHtml)
        {
            var request = (HttpWebRequest)WebRequest.Create(url);
            request.UserAgent = "Windows:cpargchecker:1.0"; // to make reddit less angery
            var html = defaultHtml;
            try {
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                using (Stream stream = response.GetResponseStream())
                using (StreamReader reader = new StreamReader(stream))
                {
                    html = reader.ReadToEnd();
                }
            } catch (WebException ex)
            {
                Console.WriteLine("Exception requesting {0}: {1}", url, ex.Message);
            }

            return html;
        }
    }
}
