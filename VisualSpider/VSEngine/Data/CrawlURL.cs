using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace VSEngine.Data
{
    public class CrawlURL
    {
        public int URLIndex { get; set; }
        public int URLResolvedIndex { get; set; }
        public int URLCollectedIndex { get; set; }
        public int[] URLLinkIndexs { get; set; }

        public string URL { get; set; }
        public string ResolvedURL { get; set; }
        public string URLHash { get; set; }
        public string ResolvedHash { get; set; }
        List<string> FoundURLs { get; set; }

        public DateTime DateFound { get; set; }
        public DateTime DateScrapped { get; set; }
        public DateTime DateResolved { get; set; }

        private CrawlURL()
        {
            FoundURLs = new List<string>();
        }

        /// <summary>
        /// Creates a url data point
        /// </summary>
        /// <param name="url"></param>
        public CrawlURL(string url) : this()
        {
            URL = url;
            URLHash = ComputeHash(url);
        }

        public void StoreURL(int index)
        {
            URLIndex = index;
            DateFound = DateTime.Now;
        }

        public void AddScrappedURL(string url)
        {
            FoundURLs.Add(url);
        }

        public void StoreURLForNav(int index)
        {
            URLCollectedIndex = index;
            DateScrapped = DateTime.Now;

        }

        public void StoreResolved(int index, string resolvedURL)
        {
            URLResolvedIndex = index;
            ResolvedURL = resolvedURL;
            ResolvedHash = ComputeHash(resolvedURL);
            DateResolved = DateTime.Now;
        }

        private string ComputeHash(string input)
        {
            MD5 hashTranslate = MD5.Create();
            byte[] stringByteValue = new UTF8Encoding().GetBytes(input);
            byte[] hashByteValue = hashTranslate.ComputeHash(stringByteValue);
            return BitConverter.ToString(hashByteValue).Replace("-", string.Empty).ToLower();
        }

    }
}
