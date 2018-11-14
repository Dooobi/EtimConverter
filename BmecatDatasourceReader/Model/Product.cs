using CategoriesDatasourceReader;
using EtimDatasourceReader;
using System.Collections.Generic;
using System.Text;

namespace BmecatDatasourceReader.Model
{
    public class Product
    {
        public Product()
        {
            Keywords = new List<string>();
            Features = new List<ProductFeature>();
            Prices = new Dictionary<string, ProductPrice>();
            Mimes = new Dictionary<string, List<ProductMime>>();
            References = new Dictionary<string, List<ProductReference>>();
        }

        public ProductFeature GetProductFeatureByEtimFeature(EtimFeature etimFeature)
        {
            foreach (ProductFeature feature in Features)
            {
                if (feature.EtimFeature == etimFeature)
                {
                    return feature;
                }
            }
            return null;
        }

        public string GetUrl()
        {
            if (Mimes.ContainsKey("url")
                    && Mimes["url"].Count > 0)
            {
                ProductMime productMime = Mimes["url"].Find((mime) => mime.Description == "MD04");
                if (productMime != null)
                {
                    return productMime.Source;
                }
            }
            return null;
        }

        public string GetFullDescription()
        {
            string descriptionLong = "";
            string vorzuege = "";

            if (DescriptionLong != null)
            {
                descriptionLong = DescriptionLong.Replace("\n", "<br/>");
            }
            if (Vorzuege != null)
            {
                vorzuege = Vorzuege;
            }

            StringBuilder bld = new StringBuilder();
            bld.Append(descriptionLong);
            if (descriptionLong != "" && vorzuege != "")
            {
                bld.Append("<br/><br/>");
            }
            bld.Append(vorzuege);

            return bld.ToString();
        }

        // Category (from CategoriesMappingDatasource)
        public Category Category { get; set; }

        // Vorzuege (from VorzuegeDatasource)
        public string Vorzuege { get; set; }

        // Header
        public string Mode { get; set; }
        public string SupplierPid { get; set; }

        // Product Details
        public string DescriptionShort { get; set; }
        public string DescriptionLong { get; set; }
        public string InternationalPid { get; set; }
        public string InternationalPidType { get; set; }
        public string ManufacturerPid { get; set; }
        public string ManufacturerName { get; set; }
        public string ManufacturerTypeDescr { get; set; }
        public string SpecialTreatmentClass { get; set; }
        public string SpecialTreatmentClassType { get; set; }
        public List<string> Keywords { get; set; }
        public string ShortestKeyword { get; set; }

        // Product Features
        public string ReferenceFeatureSystemName { get; set; }
        public EtimClass ReferenceFeatureGroup { get; set; }
        public List<ProductFeature> Features { get; set; }

        // Product Order Details
        public string OrderUnit { get; set; }
        public string ContentUnit { get; set; }
        public string NoCuPerOu { get; set; }
        public string PriceQuantity { get; set; }
        public string QuantityMin { get; set; }
        public string QuantityInterval { get; set; }

        // Product Price Details
        public string ValidStartDate { get; set; }
        public string DailyPrice { get; set; }
        public Dictionary<string, ProductPrice> Prices { get; set; }

        // Mime Infos
        public Dictionary<string, List<ProductMime>> Mimes { get; set; }

        // User Defined Extensions
        // TODO

        // Product References
        public Dictionary<string, List<ProductReference>> References { get; set; }

        // Product Logistic Details
        public string CustomsNumber { get; set; }
        public string CountryOfOrigin { get; set; }
    }
}
