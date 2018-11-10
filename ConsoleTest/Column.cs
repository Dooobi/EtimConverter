using BmecatDatasourceReader.Model;
using System.Collections.Generic;

namespace ConsoleTest
{
    public class Column
    {
        public delegate string GetValue(int groupId, KeyValuePair<string, List<Product>> productGroup, Product product);

        public string Name { get; set; }
        public GetValue DelegateGetValue { get; set; }

        public Column(string name, GetValue getValue)
        {
            Name = name;
            DelegateGetValue = getValue;
        }
    }
}