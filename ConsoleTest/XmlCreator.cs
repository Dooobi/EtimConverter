using BmecatDatasourceReader;
using BmecatDatasourceReader.Model;
using EtimDatasourceReader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace ConsoleTest
{
    public class XmlCreator
    {
        public BmecatDatasource BmecatDatasource { get; set; }

        public XmlCreator(BmecatDatasource bmecatDatasource)
        {
            BmecatDatasource = bmecatDatasource;
        }

        public XElement BuildXml()
        {
            XElement root = new XElement("Root");

            AddGroups(root);

            return root;
        }

        private void AddGroups(XElement root)
        {
            Dictionary<string, List<Product>> groupedProducts = BmecatDatasource.GetProductsGroupedByParentKeyword();

            XElement xGroups = new XElement("Groups");
            xGroups.SetAttributeValue("count", groupedProducts.Count);

            foreach (KeyValuePair<string, List<Product>> group in groupedProducts)
            {
                AddGroup(xGroups, group);
            }

            root.Add(xGroups);
        }

        private void AddGroup(XElement xGroups, KeyValuePair<string, List<Product>> group)
        {
            List<EtimFeature> differingFeatures = BmecatDatasource.GetDifferingFeaturesFromFeatureMatrix(BmecatDatasource.GetFeatureMatrixForGroupedProducts(group.Value));

            XElement xGroup = new XElement("Group");

            xGroup.Add(new XElement("Key", group.Key));

            // Add differing Features
            XElement xDifferingFeatures = new XElement("DifferingFeatures");
            xDifferingFeatures.SetAttributeValue("count", differingFeatures.Count);

            foreach (EtimFeature feature in differingFeatures)
            {
                AddDifferingFeature(xDifferingFeatures, feature);
            }
            xGroup.Add(xDifferingFeatures);

            // Add Products
            XElement xProducts = new XElement("Products");
            xProducts.SetAttributeValue("count", group.Value.Count);

            foreach (Product product in group.Value)
            {
                AddProduct(xProducts, product);
            }
            xGroup.Add(xProducts);

            xGroups.Add(xGroup);
        }

        private void AddProduct(XElement xProducts, Product product)
        {
            XElement xProduct = new XElement("Product");

            xProduct.Add(new XElement("Id", product.SupplierPid));

            XElement xFeatures = new XElement("Features");
            xFeatures.SetAttributeValue("count", product.Features.Count);

            foreach (ProductFeature feature in product.Features)
            {
                AddFeature(xProduct, feature);
            }

            xProducts.Add(xProduct);
        }

        private void AddFeature(XElement xFeatures, ProductFeature feature)
        {
            XElement xFeature = new XElement("Feature");

            xFeature.Add(new XElement("Name", feature.EtimFeature.Translations["de-DE"].Description));
            xFeature.Add(new XElement("Value", feature.ToPropertyValue()));

            xFeatures.Add(xFeature);
        }

        private void AddDifferingFeature(XElement xDifferingFeatures, EtimFeature feature)
        {
            XElement xFeature = new XElement("Feature");

            xFeature.Add(new XElement("Name", feature.Translations["de-DE"].Description));

            xDifferingFeatures.Add(xFeature);
        }
    }
}
