using BmecatDatasourceReader;
using BmecatDatasourceReader.Model;
using DbAccessor;
using EtimDatasourceReader;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleTest
{
    public class CsvBuilder
    {
        private List<Column> columns;

        private StringBuilder builder;

        private string textSeparator;
        private string columnSeparator;

        public CsvBuilder(string textSeparator, string columnSeparator)
        {
            builder = new StringBuilder();
            columns = ColumnMappings.GetColumns();

            this.textSeparator = textSeparator;
            this.columnSeparator = columnSeparator;
        }

        public void AddData(BmecatDatasource bmecatDatasource)
        {
            List<EtimFeature> allDifferingFeatures = bmecatDatasource.GetAllDifferingFeatures();

            foreach (KeyValuePair<string, List<Product>> groupedProducts in bmecatDatasource.GetProductsGroupedByParentKeyword())
            {
                if (groupedProducts.Key == "AL-1337")
                {
                    Console.Write("");
                }

                Dictionary<EtimFeature, Dictionary<Product, ProductFeature>> featureMatrix = bmecatDatasource.GetFeatureMatrixForGroupedProducts(groupedProducts.Value);
                List<EtimFeature> differingFeaturesForGroup = bmecatDatasource.GetDifferingFeaturesFromFeatureMatrix(featureMatrix);

                foreach (Product product in groupedProducts.Value)
                {
                    for (int i = 0; i < columns.Count; i++)
                    {
                        Column column = columns[i];
                        string value = column.DelegateGetValue(product);

                        if (i != 0)
                        {
                            builder.Append(columnSeparator);
                        }
                        builder.Append(textSeparator + value + textSeparator);
                    }

                    // Append properties
                    foreach (EtimFeature feature in allDifferingFeatures)
                    {
                        string value = "";
                        if (differingFeaturesForGroup.Contains(feature))
                        {
                            Dictionary<Product, ProductFeature> productFeatures = featureMatrix[feature];
                            ProductFeature productFeature = productFeatures[product];
                            if (productFeature != null)
                            {
                                value = productFeature.ToPropertyValue();
                            }
                        }
                        builder.Append(columnSeparator).Append(textSeparator + value + textSeparator);
                    }

                    builder.AppendLine();
                }
            }
        }

        public void AddLine(Product product)
        {
            for (int i = 0; i < columns.Count; i++)
            {
                Column column = columns[i];
                string value = column.DelegateGetValue(product);

                if (i != 0)
                {
                    builder.Append(columnSeparator);
                }
                builder.Append(textSeparator + value + textSeparator);
            }
            builder.AppendLine();
        }

        public string Build(BmecatDatasource bmecatDatasource, GambioDbAccessor gambioDbAccessor)
        {
            List<EtimFeature> allDifferingFeatures = bmecatDatasource.GetAllDifferingFeatures();
            List<GambioProperty> gambioProperties = gambioDbAccessor.GetProperties();

            StringBuilder headerBuilder = new StringBuilder();
            for (int i = 0; i < columns.Count; i++)
            {
                Column column = columns[i];

                if (i != 0)
                {
                    headerBuilder.Append(columnSeparator);
                }
                headerBuilder.Append(textSeparator + column.Name + textSeparator);
            }
            foreach (EtimFeature etimFeature in allDifferingFeatures)
            {
                string germanPropertyName = etimFeature.Translations["de-DE"].Description;

                GambioProperty gambioProperty = gambioProperties.Find(property => property.GermanName == germanPropertyName);
                if (gambioProperty == null)
                {
                    Console.WriteLine("Für Feature '" + germanPropertyName + "' wurde keine Eigenschaft in der Gambio Datenbank gefunden.");
                }

                headerBuilder.Append(columnSeparator).Append(textSeparator).Append("Eigenschaft: ").Append(germanPropertyName).Append(".de [").Append(gambioProperty.PropertyId).Append("]").Append(textSeparator);
            }

            headerBuilder.AppendLine();

            return headerBuilder.ToString() + builder.ToString();
        }
        
    }

    public enum BmecatField
    {
        [Description("PRODUCT[mode]")]
        ProductMode,
        [Description("PRODUCT/SUPPLIER_PID")]
        Product_SupplierPid,
        [Description("PRODUCT/PRODUCT_DETAILS/DESCRIPTION_SHORT")]
        Product_ProductDetails_DescriptionShort,
        [Description("PRODUCT/PRODUCT_DETAILS/DESCRIPTION_LONG")]
        Product_ProductDetails_DescriptionLong,
        [Description("PRODUCT/PRODUCT_DETAILS/INTERNATIONAL_PID")]
        Product_ProductDetails_InternationalPid,
        [Description("PRODUCT/PRODUCT_DETAILS/INTERNATIONAL_PID[type]")]
        Product_ProductDetails_InternationalPidType,
        [Description("PRODUCT/PRODUCT_DETAILS/MANUFACTURER_PID")]
        Product_ProductDetails_ManufacturerPid,
        [Description("PRODUCT/PRODUCT_DETAILS/MANUFACTURER_NAME")]
        Product_ProductDetails_ManufacturerName,
        [Description("PRODUCT/PRODUCT_DETAILS/MANUFACTURER_TYPE_DESCR")]
        Product_ProductDetails_ManufacturerTypeDescr,
        [Description("PRODUCT/PRODUCT_DETAILS/SPECIAL_TREATMENT_CLASS")]
        Product_ProductDetails_SpecialTreatmentClass,
        [Description("PRODUCT/PRODUCT_DETAILS/SPECIAL_TREATMENT_CLASS[type]")]
        Product_ProductDetails_SpecialTreatmentClassType,
        [Description("PRODUCT/PRODUCT_DETAILS/KEYWORD")]
        Product_ProductDetails_Keyword,

    }

}
