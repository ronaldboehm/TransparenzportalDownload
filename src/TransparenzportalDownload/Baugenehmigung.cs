using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TransparenzportalDownload
{
    /// <summary>
    /// Contains only the data relevant for so-called Baugenehmigungen.
    /// </summary>
    public class Baugenehmigung
    {
        private string number;
        private Dictionary<string, string> numbers = new Dictionary<string, string>();

        public Baugenehmigung()
        {

        }
        public string Id                { get; set; }
        public string FileReference     { get; set; }
        public string Title             { get; set; }
        public string PublishingDate    { get; set; }
        public string Author            { get; set; }
        public IEnumerable<string> Tags { get; set; }
        public string Flurstueck        { get { return GetNumberValue("Flurstück"); } }
        public string Gemarkung         { get { return GetNumberValue("Gemarkung"); } }
        public string Baublock          { get { return GetNumberValue("Baublock"); } }
        public string Bebauungsplan     { get { return GetNumberValue("Bebauungsplan"); } } 
        public string Baustufenplan     { get { return GetNumberValue("Baustufenplan"); } }
        public string Number
        {
            get { return number; }
            set
            {
                number = value;
                SplitNumber();
            }
        }

        public static string Header = "nummer;title;publishingdate;author;tags;id;filereference;flurstueck;gemarkung;baublock;bebauungsplan;baustufenplan";

        public override string ToString()
        {
            var sb = new StringBuilder();

            sb.Append(Quoted(Number));
            sb.Append(Quoted(Title));
            sb.Append(Quoted(PublishingDate));
            sb.Append(Quoted(Author));
            sb.Append(Quoted(TagsAsString));
            sb.Append(Quoted(Id));
            sb.Append(Quoted(FileReference));
            sb.Append(Quoted(Flurstueck));
            sb.Append(Quoted(Gemarkung));
            sb.Append(Quoted(Baublock));
            sb.Append(Quoted(Bebauungsplan));
            sb.Append(Quoted(Baustufenplan, insertTrailingDelimiter: false));

            return sb.ToString();
        }

        private const string Delimiter = ";";

        private string Quoted(string value, bool insertTrailingDelimiter = true)
        {
            value = value.Replace(Environment.NewLine, " ");
            value = value.Replace("\n", " ");
            value = value.Replace("\"", "\"\""); // Excel will interpret all pairs of double-quotes ("") with single double-quotes(").

            return insertTrailingDelimiter
                ? $"\"{value}\"{Delimiter}"
                : $"\"{value}\"";
        }

        private string TagsAsString
        {
            get
            {
                return string.Join(",", Tags);
            }
        }

        private string GetNumberValue(string key)
        {
            if (numbers.ContainsKey(key))
                return numbers[key];

            return "";
        }

        private void SplitNumber()
        {
            // e.g. "5696 (Flurstück), Harburg (Gemarkung), 702016 (Baublock), Harburg 59 (Bebauungsplan)"
            var parts = Number.Split(',');

            foreach (var n in parts)
            {
                // e.g. "5696 (Flurstück)"
                var valueAndKey = n.Trim().Split(' ');

                if (valueAndKey.Count() != 2)
                    continue;

                var value = valueAndKey[0];
                var key    = valueAndKey[1].Trim('(', ')', ' ');

                // the numbers text may contain some keys multiple times, e.g. Flurstück
                if (this.numbers.ContainsKey(key))
                {
                    this.numbers[key] += "|" + value;
                }
                else
                { 
                    this.numbers.Add(key, value);
                }
            }
        }
    }
}
