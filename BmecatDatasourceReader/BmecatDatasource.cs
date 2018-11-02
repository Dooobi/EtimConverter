using System;
using System.Collections.Generic;
using System.Linq;
using BmecatDatasourceReader.Model;
using System.Xml;
using EtimDatasourceReader;

namespace BmecatDatasourceReader
{
    public class BmecatDatasource
    {
        public List<EtimFeature> AllUsedEtimFeatures { get; set; }
        public List<EtimClass> AllUsedEtimClasses { get; set; }
        public List<EtimGroup> AllUsedEtimGroups { get; set; }

        private Dictionary<EtimFeature, List<ProductFeature>> allDifferingFeaturesWithValues;

        public List<Product> Products { get; set; }

        public Dictionary<string, List<Product>> productsGroupedByParent;

        private BmecatDatasource()
        {
            AllUsedEtimFeatures = new List<EtimFeature>();
            AllUsedEtimClasses = new List<EtimClass>();
            AllUsedEtimGroups = new List<EtimGroup>();
            Products = new List<Product>();
        }

        public List<ProductFeature> GetDistinctFeaturesWithTwoValues()
        {
            List<ProductFeature> featuresWithTwoValues = new List<ProductFeature>();
            foreach (Product product in Products)
            {
                foreach (ProductFeature feature in product.Features)
                {
                    if (feature.Value1 != null && feature.Value2 != null)
                    {
                        if (!IsProductFeatureInList(feature, featuresWithTwoValues))
                        {
                            featuresWithTwoValues.Add(feature);
                        }
                    }
                }
            }
            return featuresWithTwoValues;
        }

        public Dictionary<string, string> GetProductGroupNamesWithKeywords()
        {
            Dictionary<string, string> groupNameWithKeyword = new Dictionary<string, string>();

            foreach (Product product in Products)
            {
                if (!groupNameWithKeyword.ContainsKey(product.DescriptionShort))
                {
                    groupNameWithKeyword[product.DescriptionShort] = product.ShortestKeyword;
                    continue;
                }
                if (groupNameWithKeyword[product.DescriptionShort] != product.ShortestKeyword)
                {
                    Console.WriteLine("Grouping conflict for Name '" + product.DescriptionShort + "' and keyword '" + product.ShortestKeyword + "' (old keyword: '" + groupNameWithKeyword[product.DescriptionShort] + "')");
                }
            }

            return groupNameWithKeyword;
        }

        public Dictionary<string, List<Product>> GetProductsGroupedByParentName()
        {
            if (productsGroupedByParent == null)
            {
                productsGroupedByParent = new Dictionary<string, List<Product>>();

                foreach (Product product in Products)
                {
                    if (!productsGroupedByParent.ContainsKey(product.DescriptionShort))
                    {
                        productsGroupedByParent[product.DescriptionShort] = new List<Product>();
                    }
                    productsGroupedByParent[product.DescriptionShort].Add(product);
                }
            }
            return productsGroupedByParent;
        }

        public Dictionary<string, List<Product>> GetProductsGroupedByParentNameAndDescription()
        {
            if (productsGroupedByParent == null)
            {
                productsGroupedByParent = new Dictionary<string, List<Product>>();

                foreach (Product product in Products)
                {
                    string key = product.DescriptionShort + product.DescriptionLong;

                    if (!productsGroupedByParent.ContainsKey(key))
                    {
                        productsGroupedByParent[key] = new List<Product>();
                    }
                    productsGroupedByParent[key].Add(product);
                }
            }
            return productsGroupedByParent;
        }

        public Dictionary<string, List<Product>> GetProductsGroupedByParentKeyword()
        {
            if (productsGroupedByParent == null)
            {
                productsGroupedByParent = new Dictionary<string, List<Product>>();

                foreach (Product product in Products)
                {
                    if (!productsGroupedByParent.ContainsKey(product.ShortestKeyword))
                    {
                        productsGroupedByParent[product.ShortestKeyword] = new List<Product>();
                    }
                    productsGroupedByParent[product.ShortestKeyword].Add(product);
                }
            }
            return productsGroupedByParent;
        }

        public Dictionary<EtimFeature, Dictionary<Product, ProductFeature>> GetFeatureMatrixForGroupedProducts(List<Product> groupedProducts)
        {
            Dictionary<EtimFeature, Dictionary<Product, ProductFeature>> featureMatrix = new Dictionary<EtimFeature, Dictionary<Product, ProductFeature>>();

            foreach (Product product in groupedProducts)
            {
                foreach (ProductFeature productFeature in product.Features)
                {
                    if (!featureMatrix.ContainsKey(productFeature.EtimFeature))
                    {
                        featureMatrix[productFeature.EtimFeature] = new Dictionary<Product, ProductFeature>();
                    }
                    featureMatrix[productFeature.EtimFeature][product] = productFeature;
                }
            }
            foreach (KeyValuePair<EtimFeature, Dictionary<Product, ProductFeature>> featureMatrixRow in featureMatrix)
            {
                Dictionary<Product, ProductFeature> featureMatrixColumns = featureMatrixRow.Value;
                foreach (Product product in groupedProducts)
                {
                    if (!featureMatrixColumns.ContainsKey(product))
                    {
                        featureMatrixColumns[product] = null;
                    }
                }
            }


            return featureMatrix;
        }

        public List<EtimFeature> GetDifferingFeaturesFromFeatureMatrix(Dictionary<EtimFeature, Dictionary<Product, ProductFeature>> featureMatrix)
        {
            List<EtimFeature> differentFeatures = new List<EtimFeature>();

            foreach (KeyValuePair<EtimFeature, Dictionary<Product, ProductFeature>> featureMatrixRow in featureMatrix)
            {
                Dictionary<Product, ProductFeature> featureMatrixColumns = featureMatrixRow.Value;

                ProductFeature firstFoundProductFeature = null;
                foreach (KeyValuePair<Product, ProductFeature> featureMatrixColumn in featureMatrixColumns)
                {
                    if (featureMatrixColumn.Value != null)
                    {
                        firstFoundProductFeature = featureMatrixColumn.Value;
                    }
                }

                foreach (KeyValuePair<Product, ProductFeature> featureMatrixColumn in featureMatrixColumns)
                {
                    ProductFeature currentProductFeature = featureMatrixColumn.Value;

                    //if (currentProductFeature == null
                    //    || currentProductFeature.Value1 != firstFoundProductFeature.Value1
                    //    || currentProductFeature.Value2 != firstFoundProductFeature.Value2)
                    if (!Equals(currentProductFeature, firstFoundProductFeature))
                    {
                        differentFeatures.Add(currentProductFeature.EtimFeature);
                        break;
                    }
                }
            }

            return differentFeatures;
        }

        public List<EtimFeature> GetDifferingFeaturesFromGroupedProducts(List<Product> groupedProducts)
        {
            return GetDifferingFeaturesFromFeatureMatrix(GetFeatureMatrixForGroupedProducts(groupedProducts));
        }

        public Dictionary<EtimFeature, List<ProductFeature>> GetAllDifferingFeaturesWithPossibleValues()
        {
            if (allDifferingFeaturesWithValues == null)
            {
                allDifferingFeaturesWithValues = new Dictionary<EtimFeature, List<ProductFeature>>();

                foreach (KeyValuePair<string, List<Product>> groupedProducts in GetProductsGroupedByParentKeyword())
                {
                    Dictionary<EtimFeature, Dictionary<Product, ProductFeature>> featureMatrix = GetFeatureMatrixForGroupedProducts(groupedProducts.Value);
                    List<EtimFeature> featuresWithDifferentValues = GetDifferingFeaturesFromFeatureMatrix(featureMatrix);

                    foreach (EtimFeature etimFeature in featuresWithDifferentValues)
                    {
                        if (!allDifferingFeaturesWithValues.ContainsKey(etimFeature))
                        {
                            allDifferingFeaturesWithValues[etimFeature] = new List<ProductFeature>();
                        }
                    }

                    foreach (Product product in groupedProducts.Value)
                    {
                        foreach (EtimFeature etimFeature in featuresWithDifferentValues)
                        {
                            List<ProductFeature> currentValues = allDifferingFeaturesWithValues[etimFeature];

                            foreach (ProductFeature feature in product.Features)
                            {
                                if (feature.EtimFeature == etimFeature
                                    && !IsProductFeatureInList(feature, currentValues))
                                {
                                    currentValues.Add(feature);
                                    break;
                                }
                            }
                        }
                    }
                }
            }
            return allDifferingFeaturesWithValues;
        }

        private bool IsProductFeatureInList(ProductFeature feature, List<ProductFeature> features)
        {
            foreach (ProductFeature featureInList in features)
            {
                if (Equals(feature, featureInList))
                {
                    return true;
                }
            }
            return false;
        }

        public class BmecatParser
        {
            public EtimDatasource EtimDatasource { get; set; }

            public BmecatDatasource BmecatDatasource { get; set; }

            public BmecatParser(EtimDatasource etimDatasource)
            {
                BmecatDatasource = new BmecatDatasource();
                EtimDatasource = etimDatasource;
            }

            public BmecatDatasource Parse(string bmecatFilepath)
            {
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load(bmecatFilepath);

                ParseProducts(xmlDoc);

                return BmecatDatasource;
            }

            //public void ParseCatalog(string bmecatFilepath)
            //{
            //    Console.WriteLine("");
            //    Console.WriteLine("Used EtimClasses: " + AllUsedEtimClasses.Count);
            //    foreach (EtimClass etimClass in AllUsedEtimClasses)
            //    {
            //        Console.WriteLine(
            //            etimClass.Translations["de-DE"].Description
            //            + " (" + etimClass.Group.Translations["de-DE"].Description + ")");
            //    }

            //    Console.WriteLine("");
            //    Console.WriteLine("Used EtimGroups: " + AllUsedEtimGroups.Count);
            //    foreach (EtimGroup etimGroup in AllUsedEtimGroups)
            //    {
            //        Console.WriteLine(etimGroup.Translations["de-DE"].Description);
            //    }

            //    Console.WriteLine("");
            //    Console.WriteLine("Used EtimFeatures: " + AllUsedEtimFeatures.Count);
            //    foreach (EtimFeature etimFeature in AllUsedEtimFeatures)
            //    {
            //        Console.WriteLine(etimFeature.Translations["de-DE"].Description);
            //    }
            //}

            private void ParseProducts(XmlDocument xmlDoc)
            {
                XmlNodeList xmlProducts = xmlDoc.DocumentElement.SelectNodes("/BMECAT/T_NEW_CATALOG/PRODUCT");

                int maxCount = xmlProducts.Count;
                int currentCount = 0;
                foreach (XmlNode xmlProduct in xmlProducts)
                {
                    Product product = new Product();

                    XmlAttribute xmlMode = xmlProduct.Attributes["mode"];
                    XmlNode xmlSupplierPid = xmlProduct.SelectSingleNode("./SUPPLIER_PID");

                    // Header
                    product.Mode = xmlMode.InnerText;
                    product.SupplierPid = xmlSupplierPid.InnerText;

                    // Product Details
                    ParseProductDetails(product, xmlProduct);

                    // Product Features
                    ParseProductFeatures(product, xmlProduct);

                    // Product Order Details
                    ParseProductOrderDetails(product, xmlProduct);

                    // Product Price Details
                    ParseProductPriceDetails(product, xmlProduct);

                    // Product Mime Infos
                    ParseMimeInfos(product, xmlProduct);

                    // Product References
                    ParseProductReferences(product, xmlProduct);

                    // Product Logistic Details
                    ParseProductLogisticDetails(product, xmlProduct);

                    BmecatDatasource.Products.Add(product);

                    currentCount++;

                    if (currentCount == (int)(maxCount * 0.25))
                    {
                        Console.WriteLine("25%..");
                    }
                    if (currentCount == (int)(maxCount * 0.5))
                    {
                        Console.WriteLine("50%..");
                    }
                    if (currentCount == (int)(maxCount * 0.75))
                    {
                        Console.WriteLine("75%..");
                    }
                }
            }

            private void ParseProductDetails(Product product, XmlNode xmlProduct)
            {
                XmlNode xmlDescriptionShort = xmlProduct.SelectSingleNode("./PRODUCT_DETAILS/DESCRIPTION_SHORT");
                XmlNode xmlDescriptionLong = xmlProduct.SelectSingleNode("./PRODUCT_DETAILS/DESCRIPTION_LONG");
                XmlNode xmlInternationalPid = xmlProduct.SelectSingleNode("./PRODUCT_DETAILS/INTERNATIONAL_PID");
                XmlAttribute xmlInternationalPidType = xmlInternationalPid.Attributes["type"];
                XmlNode xmlManufacturerPid = xmlProduct.SelectSingleNode("./PRODUCT_DETAILS/MANUFACTURER_PID");
                XmlNode xmlManufacturerName = xmlProduct.SelectSingleNode("./PRODUCT_DETAILS/MANUFACTURER_NAME");
                XmlNode xmlManufacturerTypeDescr = xmlProduct.SelectSingleNode("./PRODUCT_DETAILS/MANUFACTURER_TYPE_DESCR");
                XmlNode xmlSpecialTreatmentClass = xmlProduct.SelectSingleNode("./PRODUCT_DETAILS/SPECIAL_TREATMENT_CLASS");
                XmlAttribute xmlSpecialTreatmentClassType = xmlSpecialTreatmentClass.Attributes["type"];
                XmlNodeList xmlKeywords = xmlProduct.SelectNodes("./PRODUCT_DETAILS/KEYWORD");

                product.DescriptionShort = xmlDescriptionShort.InnerText;
                product.DescriptionLong = xmlDescriptionLong.InnerText;
                product.InternationalPid = xmlInternationalPid.InnerText;
                product.InternationalPidType = xmlInternationalPidType.InnerText;
                product.ManufacturerPid = xmlManufacturerPid.InnerText;
                product.ManufacturerName = xmlManufacturerName.InnerText;
                product.ManufacturerTypeDescr = xmlManufacturerTypeDescr.InnerText;
                product.SpecialTreatmentClass = xmlSpecialTreatmentClass.InnerText;
                product.SpecialTreatmentClassType = xmlSpecialTreatmentClassType.InnerText;
                foreach (XmlNode xmlKeyword in xmlKeywords)
                {
                    product.Keywords.Add(xmlKeyword.InnerText);
                    if (product.ShortestKeyword == null
                        || xmlKeyword.InnerText.Length < product.ShortestKeyword.Length)
                    {
                        product.ShortestKeyword = xmlKeyword.InnerText;
                    }
                }
            }

            public void ParseProductFeatures(Product product, XmlNode xmlProduct)
            {
                XmlNode xmlReferenceFeatureSystemName = xmlProduct.SelectSingleNode("./PRODUCT_FEATURES/REFERENCE_FEATURE_SYSTEM_NAME");
                XmlNode xmlReferenceFeatureGroupId = xmlProduct.SelectSingleNode("./PRODUCT_FEATURES/REFERENCE_FEATURE_GROUP_ID");
                XmlNodeList xmlFeatures = xmlProduct.SelectNodes("./PRODUCT_FEATURES/FEATURE");

                EtimClass etimClass = EtimDatasource.Classes[xmlReferenceFeatureGroupId.InnerText];

                product.ReferenceFeatureSystemName = xmlReferenceFeatureSystemName.InnerText;
                product.ReferenceFeatureGroup = etimClass;
                foreach (XmlNode xmlFeature in xmlFeatures)
                {
                    ParseProductFeature(product, xmlFeature);
                }

                if (!BmecatDatasource.AllUsedEtimClasses.Contains(etimClass))
                {
                    BmecatDatasource.AllUsedEtimClasses.Add(etimClass);
                }
                if (!BmecatDatasource.AllUsedEtimGroups.Contains(etimClass.Group))
                {
                    BmecatDatasource.AllUsedEtimGroups.Add(etimClass.Group);
                }
            }

            public void ParseProductFeature(Product product, XmlNode xmlFeature)
            {
                XmlNode xmlFName = xmlFeature.SelectSingleNode("./FNAME");
                XmlNodeList xmlFValues = xmlFeature.SelectNodes("./FVALUE");
                XmlNode xmlFUnit = xmlFeature.SelectSingleNode("./FUNIT");
                XmlNode xmlFValueDetails = xmlFeature.SelectSingleNode("./FVALUE_DETAILS");

                EtimFeature etimFeature = EtimDatasource.Features[xmlFName.InnerText];
                EtimUnit etimUnit = xmlFUnit == null ? null : EtimDatasource.Units[xmlFUnit.InnerText];

                ProductFeature feature = new ProductFeature();

                feature.EtimFeature = etimFeature;
                feature.Unit = (etimUnit == null ? null : etimUnit);
                feature.ValueDetails = (xmlFValueDetails == null ? null : xmlFValueDetails.InnerText);
                if (xmlFValues.Count > 0)
                {
                    feature.RawValue1 = xmlFValues[0].InnerText;
                    try
                    {
                        feature.Value1 = EtimDatasource.Values[xmlFValues[0].InnerText];
                    }
                    catch (Exception)
                    {
                    }
                }
                if (xmlFValues.Count > 1)
                {
                    if (xmlFValues[1].InnerText != feature.RawValue1)
                    {
                        feature.RawValue2 = xmlFValues[1].InnerText;
                        try
                        {
                            feature.Value2 = EtimDatasource.Values[xmlFValues[1].InnerText];
                        }
                        catch (Exception)
                        {
                        }
                    }
                }
                product.Features.Add(feature);

                if (!BmecatDatasource.AllUsedEtimFeatures.Contains(etimFeature))
                {
                    BmecatDatasource.AllUsedEtimFeatures.Add(etimFeature);
                }

                if (xmlFValues.Count == 0)
                {
                    throw new Exception("There are Features without a value. Is this allowed?");
                }
                if (xmlFValues.Count > 2)
                {
                    throw new Exception("There are Features with more than two values. Is this allowed?");
                }
            }

            public void ParseProductOrderDetails(Product product, XmlNode xmlProduct)
            {
                XmlNode xmlOrderUnit = xmlProduct.SelectSingleNode("./PRODUCT_ORDER_DETAILS/ORDER_UNIT");
                XmlNode xmlContentUnit = xmlProduct.SelectSingleNode("./PRODUCT_ORDER_DETAILS/CONTENT_UNIT");
                XmlNode xmlNoCuPerOu = xmlProduct.SelectSingleNode("./PRODUCT_ORDER_DETAILS/NO_CU_PER_OU");
                XmlNode xmlPriceQuantity = xmlProduct.SelectSingleNode("./PRODUCT_ORDER_DETAILS/PRICE_QUANTITY");
                XmlNode xmlQuantityMin = xmlProduct.SelectSingleNode("./PRODUCT_ORDER_DETAILS/QUANTITY_MIN");
                XmlNode xmlQuantityInterval = xmlProduct.SelectSingleNode("./PRODUCT_ORDER_DETAILS/QUANTITY_INTERVAL");

                product.OrderUnit = xmlOrderUnit.InnerText;
                product.ContentUnit = xmlContentUnit.InnerText;
                product.NoCuPerOu = xmlNoCuPerOu.InnerText;
                product.PriceQuantity = xmlPriceQuantity.InnerText;
                product.QuantityMin = xmlQuantityMin.InnerText;
                product.QuantityInterval = xmlQuantityInterval.InnerText;
            }

            public void ParseProductPriceDetails(Product product, XmlNode xmlProduct)
            {
                XmlNode xmlValidStartDate = xmlProduct.SelectSingleNode("./PRODUCT_PRICE_DETAILS/DATETIME/DATE");
                XmlNode xmlDailyPrice = xmlProduct.SelectSingleNode("./PRODUCT_PRICE_DETAILS/DAILY_PRICE");
                XmlNodeList xmlProductPrices = xmlProduct.SelectNodes("./PRODUCT_PRICE_DETAILS/PRODUCT_PRICE");

                product.ValidStartDate = xmlValidStartDate.InnerText;
                product.DailyPrice = xmlDailyPrice.InnerText;
                foreach (XmlNode xmlProductPrice in xmlProductPrices)
                {
                    ParseProductPrice(product, xmlProductPrice);
                }
            }

            public void ParseProductPrice(Product product, XmlNode xmlProductPrice)
            {
                XmlAttribute xmlPriceType = xmlProductPrice.Attributes["price_type"];
                XmlNode xmlPriceAmount = xmlProductPrice.SelectSingleNode("./PRICE_AMOUNT");
                XmlNode xmlPriceCurrency = xmlProductPrice.SelectSingleNode("./PRICE_CURRENCY");
                XmlNode xmlTax = xmlProductPrice.SelectSingleNode("./TAX");
                XmlNode xmlLowerBound = xmlProductPrice.SelectSingleNode("./LOWER_BOUND");

                ProductPrice price = new ProductPrice();

                price.PriceType = xmlPriceType.InnerText;
                price.PriceAmount = xmlPriceAmount.InnerText;
                price.PriceCurrency = xmlPriceCurrency.InnerText;
                price.Tax = xmlTax.InnerText;
                price.LowerBound = xmlLowerBound.InnerText;

                product.Prices[price.PriceType] = price;
            }

            public void ParseMimeInfos(Product product, XmlNode xmlProduct)
            {
                XmlNodeList xmlMimes = xmlProduct.SelectNodes("./MIME_INFO/MIME");

                foreach (XmlNode xmlMime in xmlMimes)
                {
                    XmlNode xmlMimeType = xmlMime.SelectSingleNode("./MIME_TYPE");
                    XmlNode xmlMimeSource = xmlMime.SelectSingleNode("./MIME_SOURCE");
                    XmlNode xmlMimeDescription = xmlMime.SelectSingleNode("./MIME_DESCR");
                    XmlNode xmlMimePurpose = xmlMime.SelectSingleNode("./MIME_PURPOSE");

                    ProductMime mime = new ProductMime();

                    mime.Type = xmlMimeType.InnerText;
                    mime.Source = xmlMimeSource.InnerText;
                    mime.Description = xmlMimeDescription.InnerText;
                    mime.Purpose = xmlMimePurpose.InnerText;

                    if (!product.Mimes.ContainsKey(mime.Type))
                    {
                        product.Mimes[mime.Type] = new List<ProductMime>();
                    }

                    product.Mimes[mime.Type].Add(mime);
                }
            }

            public void ParseProductReferences(Product product, XmlNode xmlProduct)
            {
                XmlNodeList xmlProductReferences = xmlProduct.SelectNodes("./PRODUCT_REFERENCE");

                foreach (XmlNode xmlProductReference in xmlProductReferences)
                {
                    XmlAttribute xmlType = xmlProductReference.Attributes["type"];
                    XmlNode xmlProdIdTo = xmlProductReference.SelectSingleNode("./PROD_ID_TO");

                    ProductReference reference = new ProductReference();

                    reference.Type = xmlType.InnerText;
                    reference.ProdIdTo = xmlProdIdTo.InnerText;

                    if (!product.References.ContainsKey(reference.Type))
                    {
                        product.References[reference.Type] = new List<ProductReference>();
                    }

                    product.References[reference.Type].Add(reference);
                }
            }

            public void ParseProductLogisticDetails(Product product, XmlNode xmlProduct)
            {
                XmlNode xmlCustomsNumber = xmlProduct.SelectSingleNode("./PRODUCT_LOGISTIC_DETAILS/CUSTOMS_TARIFF_NUMBER/CUSTOMS_NUMBER");
                XmlNode xmlCountryOfOrigin = xmlProduct.SelectSingleNode("./PRODUCT_LOGISTIC_DETAILS/COUNTRY_OF_ORIGIN");

                product.CustomsNumber = xmlCustomsNumber.InnerText;
                product.CountryOfOrigin = xmlCountryOfOrigin.InnerText;
            }
        }
    }
}
