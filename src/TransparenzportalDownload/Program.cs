using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

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

            var tags = BaugenehmigungenTags.GetBaugenehmigungTags();

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
    }
}
