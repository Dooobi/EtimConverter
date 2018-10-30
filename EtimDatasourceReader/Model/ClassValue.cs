using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EtimDatasourceReader.Model
{
    public class ClassValue
    {
        public string ChangeCode { get; set; }
        public EtimValue Value { get; set; }
        public int OrderNumber { get; set; }
    }
}
