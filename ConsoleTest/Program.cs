using BmecatDatasourceReader;
using BmecatDatasourceReader.Model;
using DbAccessor;
using EtimDatasourceReader;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml.Linq;
using static BmecatDatasourceReader.BmecatDatasource;

namespace ConsoleTest
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Parsing Etim file...");
            EtimDatasource etimDatasource = EtimDatasource.ParseFile(@"C:\Users\Tobias\Desktop\Onlineshop Klaus\E6 - ETIM INTERNATIONAL - PINNED.xml");
            Console.WriteLine(etimDatasource.Units.Count + " Units");
            Console.WriteLine(etimDatasource.Features.Count + " Features");
            Console.WriteLine(etimDatasource.Values.Count + " Values");
            Console.WriteLine(etimDatasource.Groups.Count + " Groups");
            Console.WriteLine(etimDatasource.Classes.Count + " Classes");
            Console.WriteLine();

            Console.WriteLine("Parsing Bmecat file...");
            BmecatParser bmecatParser = new BmecatParser(etimDatasource, GroupMode.NAME_AND_DESCRIPTION);
            BmecatDatasource bmecatDatasource = bmecatParser.Parse(@"C:\Users\Tobias\Desktop\Onlineshop Klaus\ENLITE Trade 2018 DE - 04072018[416].xml");
            Console.WriteLine(bmecatDatasource.Products.Count + " Products");
            Console.WriteLine(bmecatDatasource.AllUsedEtimFeatures.Count + " different EtimFeatures");
            Console.WriteLine(bmecatDatasource.AllUsedEtimGroups.Count + " different EtimGroups");
            Console.WriteLine(bmecatDatasource.AllUsedEtimClasses.Count + " different EtimClasses");
            Console.WriteLine();

            GambioDbAccessor dbAccessor = new GambioDbAccessor("mysql04.manitu.net", "db22682", "u22682", "kycDfmzD33Nq");

            // Do diagnostics
            int groups = 0;
            int products = 0;
            StringBuilder bld = new StringBuilder();
            foreach (KeyValuePair<string, List<Product>> groupedProducts in bmecatDatasource.GetGroupedProducts())
            {
                groups++;
                List<EtimFeature> differingFeatures = bmecatDatasource.GetDifferingFeaturesFromGroupedProducts(groupedProducts);

                bld.Append(groupedProducts.Key).Append(": ");
                
                for (int i = 0; i < differingFeatures.Count; i++)
                {
                    EtimFeature differingFeature = differingFeatures[i];

                    if (i > 0)
                    {
                        bld.Append(" ; ");
                    }
                    bld.Append(differingFeature.Translations["de-DE"].Description);
                }

                bld.AppendLine();
                foreach (Product product in groupedProducts.Value)
                {
                    products++;
                    bld.Append("    ").AppendLine(product.SupplierPid);
                }
            }
            Console.WriteLine("Groups: " + groups);
            Console.WriteLine("Products: " + products);
            File.WriteAllText("C:/Users/Tobias/Desktop/Onlineshop Klaus/" + "GroupedProducts.txt", bld.ToString());

            // Datei für Klaus
            bld = new StringBuilder();
            foreach (KeyValuePair<string, string> groupedProducts in bmecatDatasource.GetProductGroupNamesWithKeywords())
            {
                bld.Append(groupedProducts.Value).Append("\t").Append(groupedProducts.Key).AppendLine("\t");
            }
            File.WriteAllText("C:/Users/Tobias/Desktop/Onlineshop Klaus/" + "KlausKategorienUndVorzüge.csv", bld.ToString());

            Dictionary<EtimFeature, List<ProductFeature>> allDifferentFeaturesWithPossibleValues = bmecatDatasource.GetAllDifferingFeaturesWithPossibleValues();
            Console.WriteLine(allDifferentFeaturesWithPossibleValues.Keys.Count);
            bld = new StringBuilder();

            int featuresCount = 0;
            foreach (KeyValuePair<EtimFeature, List<ProductFeature>> featureWithPossibleValues in allDifferentFeaturesWithPossibleValues)
            {
                featuresCount++;

                EtimFeature etimFeature = featureWithPossibleValues.Key;
                List<ProductFeature> possibleValues = featureWithPossibleValues.Value;

                bld.Append(featuresCount).Append(": ").AppendLine(etimFeature.Translations["de-DE"].Description);
                foreach (ProductFeature possibleValue in possibleValues)
                {
                    bld.Append("    ").AppendLine(possibleValue.ToPropertyValue());
                }
            }
            File.WriteAllText("C:/Users/Tobias/Desktop/Onlineshop Klaus/" + "AllDifferentFeaturesWithPossibleValues.txt", bld.ToString());

            List<ProductFeature> featuresWithTwoValues = bmecatDatasource.GetDistinctFeaturesWithTwoValues();

            bld = new StringBuilder();
            for (int i = 0; i < featuresWithTwoValues.Count; i++)
            {
                ProductFeature feature = featuresWithTwoValues[i];
                bld.Append(feature.EtimFeature.Translations["de-DE"].Description).AppendLine(": ");
                bld.Append("    Value1: ").AppendLine(feature.RawValue1);
                bld.Append("    Value2: ").AppendLine(feature.RawValue2);
                bld.Append("    Unit: ").AppendLine(feature.Unit.Translations["de-DE"].Description);
                bld.Append("    Details: ").AppendLine(feature.ValueDetails);
            }
            File.WriteAllText("C:/Users/Tobias/Desktop/Onlineshop Klaus/" + "FeaturesWithTwoValues.txt", bld.ToString());

            // Handle DB
            HandleDb(dbAccessor, allDifferentFeaturesWithPossibleValues);

            // Build xml file (debugging)
            BuildXml(bmecatDatasource);

            // Build csv file
            BuildCsv(bmecatDatasource, dbAccessor);

            Console.WriteLine("Finished.");
            Console.ReadLine();
        }

        public static void BuildCsv(BmecatDatasource bmecatDatasource, GambioDbAccessor gambioDbAccessor)
        {
            //CsvBuilder csvBuilder = new CsvBuilder("\"", "|");
            CsvBuilder csvBuilder = new CsvBuilder("", "\t");

            csvBuilder.AddData(bmecatDatasource);

            String csvData = csvBuilder.Build(bmecatDatasource, gambioDbAccessor);
            
            DateTime now = DateTime.Now;
            string filename = "Generated--" + now.ToString("yyyy-MM-dd--HH-mm-ssZ") + ".csv";
            File.WriteAllText("C:/Users/Tobias/Desktop/Onlineshop Klaus/imports/" + filename, csvData);
        }

        public static void BuildXml(BmecatDatasource bmecatDatasource)
        {
            XmlCreator xmlCreator = new XmlCreator(bmecatDatasource);

            XElement root = xmlCreator.BuildXml();

            File.WriteAllText("C:/Users/Tobias/Desktop/Onlineshop Klaus/" + "Debug.xml", root.ToString());
        }

        public static void HandleDb(GambioDbAccessor dbAccessor, Dictionary<EtimFeature, List<ProductFeature>> featuresWithPossibleValues)
        {
            Console.WriteLine("Reading properties...");
            //List<GambioProperty> properties = dbAccessor.GetProperties();

            //StringBuilder bld = new StringBuilder();
            //foreach (GambioProperty property in properties)
            //{
            //    bld.AppendLine(property.GermanName + " [" + property.PropertyId + "]");
            //}
            //File.WriteAllText("C:/Users/Tobias/Desktop/Onlineshop Klaus/" + "GambioProperties.txt", bld.ToString());

            dbAccessor.InsertMissingPropertiesAndValues(featuresWithPossibleValues);

            Console.WriteLine("Done.");
        }
    }
}
