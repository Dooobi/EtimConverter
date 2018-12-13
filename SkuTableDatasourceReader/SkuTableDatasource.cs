using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace SkuTableDatasourceReader
{
    public class SkuTableDatasource
    {
        XmlDocument xmlCache;

        Dictionary<string, string> SkuTablesByUrl;

        private string cacheFilepath;

        public SkuTableDatasource(string cacheFilepath)
        {
            SkuTablesByUrl = new Dictionary<string, string>();
            this.cacheFilepath = cacheFilepath;

            InitializeXmlCache(cacheFilepath);

            LoadCacheData();
        }

        public void PersistCache()
        {
            XElement xmlDataFromWebsite = new XElement("DataFromWebsite");

            foreach (KeyValuePair<string, string> skuTables in SkuTablesByUrl)
            {
                XElement xmlProductData = new XElement("ProductData");
                xmlProductData.Add(new XElement("Url", skuTables.Key));
                xmlProductData.Add(new XElement("SkuTable", skuTables.Value));

                xmlDataFromWebsite.Add(xmlProductData);
            }

            File.WriteAllText(cacheFilepath, xmlDataFromWebsite.ToString());
        }

        public string GetSkuTableByUrl(string url)
        {
            if (!SkuTablesByUrl.ContainsKey(url))
            {
                string skuTable = LoadSkuTableFromUrl(url);
                SkuTablesByUrl[url] = skuTable;
            }
            return SkuTablesByUrl[url];
        }

        private string LoadSkuTableFromUrl(string url)
        {
            try
            {
                Console.WriteLine("Loading SkuTable from " + url);

                HtmlWeb web = new HtmlWeb();
                HtmlDocument doc = web.Load(url);

                HtmlNode htmlSkuTable = doc.DocumentNode.SelectSingleNode("//*[@id=\"skutable\"]/div/div/div/table");

                string rawText = htmlSkuTable.OuterHtml;
                rawText = rawText.Trim().Replace("\r\n", "").Replace("\r", "").Replace("\n", "");

                return rawText;
            }
            catch (Exception)
            {
                return "";
            }
        }

        private void LoadCacheData()
        {
            XmlNodeList xmlProductDatas = xmlCache.DocumentElement.SelectNodes("/DataFromWebsite/ProductData");

            foreach (XmlNode xmlProductData in xmlProductDatas)
            {
                XmlNode xmlUrl = xmlProductData.SelectSingleNode("./Url");
                XmlNode xmlSkuTable = xmlProductData.SelectSingleNode("./SkuTable");

                SkuTablesByUrl[xmlUrl.InnerText] = xmlSkuTable.InnerText;
            }
        }

        private void InitializeXmlCache(string cacheFilepath)
        {
            xmlCache = new XmlDocument();
            try
            {
                xmlCache.Load(cacheFilepath);
            }
            catch (FileNotFoundException)
            {
                PersistCache();
                xmlCache.Load(cacheFilepath);
            }
        }
    }
}
