using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace TransparenzportalDownload
{
    public static class QueryByPackageSearch
    {
        private const string detailUrl =
             "http://suche.transparenz.hamburg.de/api/3/action/package_search?rows={0}&start={1}&fq=tags:{2}";

        private const int RowsPerPage = 1000;

        /// <summary>
        /// Read from file after first downloading with GetBaugenehmigungenFor().
        /// </summary>
        public static List<Baugenehmigung> ReadBaugenehmigungenFromFiles()
        {
            var result = new List<Baugenehmigung>();

            foreach (var file in Directory.GetFiles(DownloadDirectory))
            {
                Console.WriteLine("Reading file " + file);

                var s = ReadFromFile(file);
                var packages = JsonParser.ParseJson(s);

                Console.WriteLine($"Adding {packages.Count} results");

                result.AddRange(packages);
            }

            return result;
        }

        public static IList<Baugenehmigung> GetBaugenehmigungenFor(string tag)
        {
            EnsureDownloadDirectoryExists();

            Console.WriteLine($"Getting Baugenehmigungen for tag \"{tag}\".");

            var result = new List<Baugenehmigung>();

            using (var webClient = new WebClient())
            { 

                int lastRowCount = 0;
                int currentPage = 0;

                do
                {
                    var url = string.Format(detailUrl, RowsPerPage, currentPage * RowsPerPage, tag);
                    var file = Path.Combine(DownloadDirectory,
                        string.Format("{0}_{1}.json", DateTime.Now.ToString("yyyy_MM_dd_hhmmss"), currentPage));

                    // instead of webClient.DownloadString(), make local copies so that the 
                    // same data can be used again without downloading:
                    webClient.DownloadFile(url, file);
                    var s = ReadFromFile(file);

                    var packages = JsonParser.ParseJson(s);
                    result.AddRange(packages);

                    lastRowCount = packages.Count();
                    currentPage++;

                    Console.WriteLine($"Found {lastRowCount} more results.");

                } while (lastRowCount == RowsPerPage);
            }

            Console.WriteLine($"Found a total {result.Count} results for tag \"{tag}\".");

            return result;
        }

        private static string ReadFromFile(string file)
        {
            using (var stream = new FileStream(file, FileMode.Open))
            {
                using (var reader = new StreamReader(stream))
                {
                    return reader.ReadToEnd();
                }
            }
        }

        private static string DownloadDirectory
        {
            get
            {
                return Path.Combine(Environment.CurrentDirectory, "Downloads");
            }
        }

        private static void EnsureDownloadDirectoryExists()
        {
            if (!Directory.Exists(DownloadDirectory))
            {
                Directory.CreateDirectory(DownloadDirectory);
            }
        }
    }
}
