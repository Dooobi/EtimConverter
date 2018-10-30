using BmecatDatasourceReader;
using BmecatDatasourceReader.Model;
using DbAccessor;
using EtimDatasourceReader;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
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
            BmecatParser bmecatParser = new BmecatParser(etimDatasource);
            BmecatDatasource bmecatDatasource = bmecatParser.Parse(@"C:\Users\Tobias\Desktop\Onlineshop Klaus\ENLITE Trade 2018 DE - 04072018[416].xml");
            Console.WriteLine(bmecatDatasource.Products.Count + " Products");
            Console.WriteLine(bmecatDatasource.AllUsedEtimFeatures.Count + " different EtimFeatures");
            Console.WriteLine(bmecatDatasource.AllUsedEtimGroups.Count + " different EtimGroups");
            Console.WriteLine(bmecatDatasource.AllUsedEtimClasses.Count + " different EtimClasses");
            Console.WriteLine();

            // Do diagnostics
            StringBuilder bld = new StringBuilder();
            foreach (KeyValuePair<string, List<Product>> groupedProducts in bmecatDatasource.GetProductsGroupedByParent())
            {
                List<EtimFeature> differingFeatures = bmecatDatasource.GetDifferingFeaturesFromGroupedProducts(groupedProducts.Value);

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
                    bld.Append("    ").AppendLine(product.SupplierPid);
                }
            }
            File.WriteAllText("C:/Users/Tobias/Desktop/Onlineshop Klaus/" + "GroupedProducts.txt", bld.ToString());


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

            // Build csv file
            BuildCsv(bmecatDatasource);

            //HandleDb(allDifferentFeatures[0]);

            Console.WriteLine("Finished.");
            Console.ReadLine();
        }

        public static void BuildCsv(BmecatDatasource bmecatDatasource)
        {
            CsvBuilder csvBuilder = new CsvBuilder("\"", "|");

            for (int i = 0; i < bmecatDatasource.Products.Count; i++)
            {
                //if (i > 0)
                //{
                //    break;
                //}
                Product product = bmecatDatasource.Products[i];

                csvBuilder.AddLine(product);
            }

            DateTime now = DateTime.Now;
            string filename = "Generated--" + now.ToString("yyyy-MM-dd--HH-mm-ssZ") + ".csv";
            File.WriteAllText("C:/Users/Tobias/Desktop/Onlineshop Klaus/imports/" + filename, csvBuilder.Build());
        }

        public static void HandleDb(EtimFeature feature)
        {
            GambioDbAccessor dbAccessor = new GambioDbAccessor("mysql04.manitu.net", "db22682", "u22682", "kycDfmzD33Nq");

            Console.WriteLine("Reading properties...");
            List<GambioProperty> properties = dbAccessor.GetProperties();

            StringBuilder bld = new StringBuilder();
            foreach (GambioProperty property in properties)
            {
                bld.AppendLine(property.GermanName + " [" + property.PropertyId + "]");
            }
            File.WriteAllText("C:/Users/Tobias/Desktop/Onlineshop Klaus/" + "GambioProperties.txt", bld.ToString());

            dbAccessor.InsertProperty(feature);

            Console.WriteLine("Done.");
        }
    }
}
