using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace TransparenzportalDownload
{
    public static class JsonParser
    {

        public static IList<Baugenehmigung> ParseJson(string jsonText)
        {
            var result = new List<Baugenehmigung>();

            dynamic json = JsonConvert.DeserializeObject(jsonText);

            dynamic packages = json.result.results;

            foreach (dynamic package in packages)
            {
                var b = new Baugenehmigung
                {
                    Title          = (package.title.ToString()).Replace(Environment.NewLine, " "),
                    PublishingDate = GetValue(package, "exact_publishing_date").Substring(0, 10), // e.g.  "2016-04-29T20:14:48",
                    Number         = GetValue(package, "number"),
                    FileReference  = GetValue(package, "file_reference_digital"),
                    Author         = package.author,
                    Id             = package.id,
                    Tags           = GetTagsFromPackage(package)
                };

                result.Add(b);
            }

            return result;
        }

        public static IEnumerable<string> GetTagsFromPackage(dynamic package)
        {
            var result = new List<string>();

            dynamic tags = package.tags;

            foreach (dynamic tag in tags)
            {
                result.Add(tag.name.ToString());
            }

            return result;
        }

        public static string GetValue(dynamic package, string key)
        {
            dynamic extras = package.extras;

            // e.g.
            //"extras": [
            //        {
            //            "state": "active",
            //            "value": "[{\"date\": \"2016-05-18T00:00:00\", \"role\": \"veroeffentlicht\"}]",
            //            "revision_timestamp": "2017-01-27T10:27:10.094623",
            //            "package_id": "09cea53e-940f-4e0e-b3e6-38baa731e951",
            //            "key": "dates",
            //            "revision_id": "d9d32ef3-0f61-404e-b618-e83f597f3d5d",
            //            "id": "94ae9a86-df81-41a9-895d-204caceb8925"
            //        },
            //        {
            //        ...
            // so both "key" and "value" are key-value-pairs:

            foreach (dynamic extra in extras)
            {
                if (extra.key == key)
                {
                    return extra.value;
                }
            }

            return "";
        }
    }
}
