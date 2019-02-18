using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace VorzuegeDatasourceReader
{
    public class VorzuegeDatasource
    {
        XmlDocument xmlCache;

        Dictionary<string, string> VorzuegeByUrl;

        private string cacheFilepath;

        public VorzuegeDatasource(string cacheFilepath)
        {
            VorzuegeByUrl = new Dictionary<string, string>();
            this.cacheFilepath = cacheFilepath;

            InitializeXmlCache(cacheFilepath);

            LoadCacheData();
        }
        
        public void PersistCache()
        {
            XElement xmlDataFromWebsite = new XElement("DataFromWebsite");

            foreach (KeyValuePair<string, string> vorzuege in VorzuegeByUrl)
            {
                XElement xmlProductData = new XElement("ProductData");
                xmlProductData.Add(new XElement("Url", vorzuege.Key));
                xmlProductData.Add(new XElement("Vorzuege", vorzuege.Value));

                xmlDataFromWebsite.Add(xmlProductData);
            }

            File.WriteAllText(cacheFilepath, xmlDataFromWebsite.ToString());
        }

        public string GetVorzuegeByUrl(string url)
        {
            if (!VorzuegeByUrl.ContainsKey(url))
            {
                string vorzuege = LoadVorzuegeFromUrl(url);
                VorzuegeByUrl[url] = vorzuege;
            }
            return VorzuegeByUrl[url].Replace("\r\n", "<br/>").Replace("\r", "<br/>").Replace("\n", "<br/>");
        }

        private string LoadVorzuegeFromUrl(string url)
        {
            try
            {
                Console.WriteLine("Loading Vorzuege from " + url);

                HtmlWeb web = new HtmlWeb();
                HtmlDocument doc = web.Load(url);

                HtmlNodeCollection htmlVorzuegeCollection = doc.DocumentNode.SelectNodes("//*[@id=\"features\"]//div[@class=\"feature_bullet\"]");
                StringBuilder bld = new StringBuilder();
                foreach (HtmlNode htmlVorzuege in htmlVorzuegeCollection)
                {
                    string rawText = htmlVorzuege.InnerText;
                    bld.Append("• ").AppendLine(rawText.Trim().Replace("\r\n", "<br/>").Replace("\r", "<br/>").Replace("\n", "<br/>"));
                }

                return bld.ToString();
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
                XmlNode xmlVorzuege = xmlProductData.SelectSingleNode("./Vorzuege");

                VorzuegeByUrl[xmlUrl.InnerText] = xmlVorzuege.InnerText;
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
