using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EtimDatasourceReader.Model
{
    public class Translation
    {
        public string Language { get; set; }
        public string Description { get; set; }
        public string Abbreviation { get; set; }
        public List<string> Synonyms { get; set; }

        public Translation()
        {
            Synonyms = new List<string>();
        }
    }
}
