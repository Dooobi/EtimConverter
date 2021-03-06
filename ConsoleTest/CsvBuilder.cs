﻿using BmecatDatasourceReader;
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
        public const int WITH_PROPERTIES = 0;
        public const int WITHOUT_PROPERTIES = 1;

        private List<Column> columns;

        private StringBuilder builder;

        private string textSeparator;
        private string columnSeparator;

        private int type;

        public CsvBuilder(string textSeparator, string columnSeparator, int type)
        {
            builder = new StringBuilder();
            columns = ColumnMappings.GetColumns();

            this.textSeparator = textSeparator;
            this.columnSeparator = columnSeparator;
            this.type = type;
        }

        public void AddData(BmecatDatasource bmecatDatasource, bool skipGroupsWithOnlyOneProduct, bool onlyGroupsWithOneProduct, bool filterDuplicateFeatureCombinations)
        {
            if (onlyGroupsWithOneProduct && skipGroupsWithOnlyOneProduct)
            {
                throw new Exception("Das widerspricht sich.");
            }
            List<EtimFeature> allDifferingFeatures = bmecatDatasource.GetAllDifferingFeatures();
            
            foreach (KeyValuePair<string, List<Product>> groupedProducts in bmecatDatasource.GetGroupedProducts(filterDuplicateFeatureCombinations))
            {
                if (skipGroupsWithOnlyOneProduct && groupedProducts.Value.Count == 1)
                {
                    Console.WriteLine("Skipped group (only one product): " + groupedProducts.Key);
                    continue;
                }
                if (onlyGroupsWithOneProduct && groupedProducts.Value.Count != 1)
                {
                    Console.WriteLine("Skipped group (multiple products): " + groupedProducts.Key);
                    continue;
                }
                Dictionary<EtimFeature, Dictionary<Product, ProductFeature>> featureMatrix = bmecatDatasource.GetFeatureMatrixForGroupedProducts(groupedProducts.Key, groupedProducts.Value, true);
                List<EtimFeature> differingFeaturesForGroup = bmecatDatasource.GetDifferingFeaturesFromFeatureMatrix(featureMatrix);

                foreach (Product product in groupedProducts.Value)
                {
                    for (int i = 0; i < columns.Count; i++)
                    {
                        Column column = columns[i];

                        if (column.Name == "products_properties_combis_id" && type == WITHOUT_PROPERTIES)
                        {
                            break;
                        }

                        string value = column.DelegateGetValue(groupedProducts.Key, groupedProducts, product);

                        if (i != 0)
                        {
                            builder.Append(columnSeparator);
                        }
                        builder.Append(textSeparator + value + textSeparator);
                    }

                    // Append properties
                    if (type == WITH_PROPERTIES)
                    {
                        foreach (EtimFeature feature in allDifferingFeatures)
                        {
                            string value = "";
                            if (feature.Code == "SKU_FEATURE")
                            {
                                value = "<placeholder>";
                            }
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
                    }

                    builder.AppendLine();
                }
            }
        }

        public string Build(BmecatDatasource bmecatDatasource, GambioDbAccessor gambioDbAccessor)
        {
            List<EtimFeature> allDifferingFeatures = bmecatDatasource.GetAllDifferingFeatures();
            List<GambioProperty> gambioProperties = gambioDbAccessor.GetProperties();

            StringBuilder headerBuilder = new StringBuilder();
            for (int i = 0; i < columns.Count; i++)
            {
                Column column = columns[i];

                if (column.Name == "products_properties_combis_id" && type == WITHOUT_PROPERTIES)
                {
                    break;
                }

                if (i != 0)
                {
                    headerBuilder.Append(columnSeparator);
                }
                headerBuilder.Append(textSeparator + column.Name + textSeparator);
            }

            if (type == WITH_PROPERTIES)
            {
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
