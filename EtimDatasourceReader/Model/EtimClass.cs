using EtimDatasourceReader.Model;
using System.Collections.Generic;

namespace EtimDatasourceReader
{
    public class EtimClass
    {
        public string Code { get; set; }
        public string Version { get; set; }
        public EtimGroup Group { get; set; }
        public Dictionary<string, Translation> Translations { get; set; }
        public List<ClassFeature> Features { get; set; }
        public string Status { get; set; }
    }
}