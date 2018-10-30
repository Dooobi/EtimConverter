using BmecatDatasourceReader.Model;

namespace ConsoleTest
{
    public class Column
    {
        public delegate string GetValue(Product product);

        public string Name { get; set; }
        public GetValue DelegateGetValue { get; set; }

        public Column(string name, GetValue getValue)
        {
            Name = name;
            DelegateGetValue = getValue;
        }
    }
}