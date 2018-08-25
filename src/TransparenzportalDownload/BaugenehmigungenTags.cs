using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace TransparenzportalDownload
{
    class BaugenehmigungenTags
    {
        /// <summary>
        /// Returns a fixed list of tags to query documents with Baugenehmigungen.
        /// </summary>
        /// <remarks>
        /// These are the tags we found to be relevant for our use case.
        /// </remarks>
        public static IEnumerable<string> GetBaugenehmigungTags()
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
        public static IEnumerable<string> GetBaugenehmigungTagsFromPortal()
        {
            string s;
            using (var webClient = new WebClient())
            {
                s = webClient.DownloadString("http://suche.transparenz.hamburg.de/api/3/action/tag_list");
            }

            dynamic json = JsonConvert.DeserializeObject(s);

            dynamic tagsInJson = json.result;

            var unfilteredTags = new List<string>();

            foreach (var tagInJson in tagsInJson)
            {
                string tag = tagInJson;
                unfilteredTags.Add(tag);
            }

            Console.WriteLine($"Found {unfilteredTags.Count()} tags, filtering for 'Baugenehmigung'...");

            var baugenehmigungTags = unfilteredTags.Where(t => t.Trim().ToLower().StartsWith("baugenehmi"));

            foreach (var tag in baugenehmigungTags)
            {
                Console.WriteLine(tag);
            }

            return baugenehmigungTags;
        }
    }
}
