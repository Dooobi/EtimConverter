using EtimDatasourceReader.Model;
using System.Collections.Generic;

namespace EtimDatasourceReader
{
    public class EtimGroup
    {
        public string Code { get; set; }
        public Dictionary<string, Translation> Translations { get; set; }
    }
}