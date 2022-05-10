using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackUpDesktop
{

    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
    public class Package
    {
        public string PackageIdentifier { get; set; }
    }

    public class Root
    {
        [JsonProperty("$schema")]
        public string Schema { get; set; }
        public DateTime CreationDate { get; set; }
        public List<Source> Sources { get; set; }
        public string WinGetVersion { get; set; }
    }

    public class Source
    {
        public List<Package> Packages { get; set; }
        public SourceDetails SourceDetails { get; set; }
    }

    public class SourceDetails
    {
        public string Argument { get; set; }
        public string Identifier { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
    }
}
