using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EtimDatasourceReader.Model
{
    public class ClassFeature
    {
        public string ChangeCode { get; set; }
        public EtimFeature Feature { get; set; }
        public EtimUnit Unit { get; set; }
        public int OrderNumber { get; set; }
        public List<ClassValue> Values { get; set; }

        public ClassFeature()
        {
            Values = new List<ClassValue>();
        }

        public ClassFeature(string changeCode, EtimFeature feature, EtimUnit unit, int orderNumber, List<ClassValue> values)
        {
            ChangeCode = changeCode;
            Feature = feature;
            Unit = unit;
            OrderNumber = orderNumber;
            Values = values;
        }
    }
}
