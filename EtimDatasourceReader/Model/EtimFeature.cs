using EtimDatasourceReader.Model;
using System.Collections.Generic;

namespace EtimDatasourceReader
{
    public class EtimFeature
    {
        public const string TYPE_NUMERIC = "Numeric";
        public const string TYPE_ALPHANUMERIC = "Alphanumeric";
        public const string TYPE_LOGICAL = "Logical";
        public const string TYPE_RANGE = "Range";

        public string Code { get; set; }
        public string Type { get; set; }
        public Dictionary<string, Translation> Translations { get; set; }
    }
}