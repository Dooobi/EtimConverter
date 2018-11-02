using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DbAccessor
{
    public class GambioPropertyValue
    {
        public long PropertyValueId { get; set; }
        public long PropertyId { get; set; }
        public string GermanName { get; set; }
        public string EnglishName { get; set; }
    }
}
