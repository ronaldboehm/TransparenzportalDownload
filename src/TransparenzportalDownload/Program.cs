using System;
using System.Linq;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Web.Script.Serialization;
using System.IO;

namespace TransparenzportalDownload
{
    class Program
    {
        static void Main(string[] args)
        {
            // for the first run, download all data from the portal:
            List<Baugenehmigung> results = QueryFromTransparenzportal();

            // after the first download, you can just read the cached data instead and process as needed:
            // List<Baugenehmigung> results = QueryByPackageSearch.ReadBaugenehmigungenFromFiles();

            results = RemoveDuplicates(results);

            Dump(results);

            Console.WriteLine("Done.");
            Console.ReadLine();
        }

        private static List<Baugenehmigung> QueryFromTransparenzportal()
        {
            var results = new List<Baugenehmigung>();

            var tags = GetBaugenehmigungTags();

            foreach (var tag in tags)
            {
                results.AddRange(QueryByPackageSearch.GetBaugenehmigungenFor(tag));
            }

            return results;
        }

        private static List<Baugenehmigung> RemoveDuplicates(List<Baugenehmigung> results)
        {
            // Baugenehmigungen may have multiple tags (i.e. one may have both the tag
            // "Baugenehmigung" and " Baugenehmigungsverfahren") and would be returned
            // multiple times when querying for different tags, so form groups of 
            // documents with the same id ...
            var groups = results.GroupBy(b => b.Id);

            // ... and keep only one of each:
            return groups.Select(group => group.First()).ToList();
        }

        private static void Dump(List<Baugenehmigung> results)
        {
            var filename = Path.Combine(Environment.CurrentDirectory, "baugenehmigungen.csv");

            Console.WriteLine("Writing to file " + filename);

            using (var stream = new FileStream(filename, FileMode.Create))
            {
                using (var writer = new StreamWriter(stream, Encoding.UTF8))
                {
                    writer.WriteLine(Baugenehmigung.Header);
                    foreach (var result in results)
                        writer.WriteLine(result);
                }
            }
        }

        /// <summary>
        /// Returns a fixed list of tags to query documents with Baugenehmigungen.
        /// </summary>
        /// <remarks>
        /// These are the tags we found to be relevant for our use case.
        /// </remarks>
        private static IEnumerable<string> GetBaugenehmigungTags()
        {
            return new List<string>
            {
                "Baugenehmigung",
                // " Baugenehmigung",                   // --> Globalrichtlinie GR 1/2006 (BSU), Anlage 1, 1a und 2
                // "Baugenehmigungen",                  // --> Baugenehmigungen pro Bezirk, Wohnungsbauprojekte, ...
                // "Baugenehmigungen in Hamburg",       // --> Baugenehmigungen in Hamburg im <Monat>/<Jahr> / im <Quartal>
                // "Baugenehmigungen und Bautätigkeit", // --> Baugenehmigungen in Hamburg im 3. Vierteljahr 2014, ...
                // " Baugenehmigungsverfahren",         // --> Bauprüfdienst (BPD) 2017-3 Baugenehmigungsverfahren mit Konzentrationswirkung nach § 62 HBauO
                // "Baugenehmigungsverfahren nach § 62 HBauO"
            };
        }

        /// <summary>
        /// Queries tags from the portal and filters for the text 'baugenehmi'.
        /// </summary>
        /// <remarks>
        /// Use this to search for documents with other tags - modify as needed.
        /// </remarks>
        private static IEnumerable<string> GetBaugenehmigungTagsFromPortal()
        { 
            const string url = "http://suche.transparenz.hamburg.de/api/3/action/tag_list";

            IEnumerable<string> tags;

            var webClient = new WebClient();

            var s = webClient.DownloadString(url);

            var deserializer = new JavaScriptSerializer();
            dynamic json = deserializer.DeserializeObject(s);

            tags = (json["result"] as IEnumerable<object>).Select(o => o.ToString());

            Console.WriteLine($"Found {tags.Count()} tags, filtering for 'Baugenehmigung'...");

            var baugenehmigungTags = tags.Where(t => t.Trim().ToLower().StartsWith("baugenehmi"));

            foreach (var tag in baugenehmigungTags)
            {
                Console.WriteLine(tag);
            }

            return baugenehmigungTags;
        }
    }
}
