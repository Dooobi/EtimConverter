using EtimDatasourceReader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml;

namespace EtimConverter
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private String pathToXml = @"C:\Users\Tobias\Desktop\Onlineshop Klaus\ENLITE Trade 2018 DE - 04072018[416].xml";

        private EtimDatasource etimDatasource;

        public MainWindow()
        {
            InitializeComponent();
            Test();
        }

        private void Test()
        {
            etimDatasource = new EtimDatasource(@"C:\Users\Tobias\Desktop\Onlineshop Klaus\E6 - ETIM INTERNATIONAL - PINNED.xml");
            etimDatasource.ParseFile();

            Console.WriteLine(etimDatasource);

            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(pathToXml);

            int count = HandleProductGroups(xmlDoc);

            Console.WriteLine(count);
        }

        private int HandleProductGroups(XmlDocument xmlDoc)
        {
            List<String> uniques = new List<String>();
            XmlNodeList featureGroups = xmlDoc.GetElementsByTagName("REFERENCE_FEATURE_GROUP_ID");

            foreach (XmlNode node in featureGroups)
            {
                if (!uniques.Contains(node.InnerText))
                {
                    uniques.Add(node.InnerText);
                    Console.WriteLine(node.InnerText);
                }
            }

            return uniques.Count;
        }
    }
}
