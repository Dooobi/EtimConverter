using EtimDatasourceReader.Model;
using System.Collections.Generic;

namespace EtimDatasourceReader
{
    public class EtimValue
    {
        public string Code { get; set; }
        public Dictionary<string, Translation> Translations { get; set; }
    }
}