using EtimDatasourceReader.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace EtimDatasourceReader
{
    public class EtimDatasource
    {        
        public Dictionary<String, EtimGroup> Groups { get; set; }
        public Dictionary<String, EtimClass> Classes { get; set; }
        public Dictionary<String, EtimFeature> Features { get; set; }
        public Dictionary<String, EtimValue> Values { get; set; }
        public Dictionary<String, EtimUnit> Units { get; set; }

        private EtimDatasource()
        {
            Groups = new Dictionary<string, EtimGroup>();
            Classes = new Dictionary<string, EtimClass>();
            Features = new Dictionary<string, EtimFeature>();
            Values = new Dictionary<string, EtimValue>();
            Units = new Dictionary<string, EtimUnit>();
        }

        public static EtimDatasource ParseFile(string filepath)
        {
            EtimDatasource etimDatasource = new EtimDatasource();

            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(filepath);

            XmlNamespaceManager manager = new XmlNamespaceManager(new NameTable());
            manager.AddNamespace("ns", "http://www.etim-international.com/etimixf/1");
            manager.AddNamespace("xsi", "http://www.w3.org/2001/XMLSchema-instance");
            manager.AddNamespace("xsd", "http://www.w3.org/2001/XMLSchema");

            etimDatasource.Units = ParseUnits(xmlDoc, manager);
            etimDatasource.Features = ParseFeatures(xmlDoc, manager);
            etimDatasource.Values = ParseValues(xmlDoc, manager);
            etimDatasource.Groups = ParseGroups(xmlDoc, manager);
            etimDatasource.Classes = ParseClasses(xmlDoc, manager, etimDatasource.Groups, etimDatasource.Features, etimDatasource.Units, etimDatasource.Values);

            // Add SKU feature (custom feature)
            EtimFeature skuFeature = new EtimFeature();
            skuFeature.Code = "SKU_FEATURE";
            skuFeature.Type = "Alphanumeric";

            Translation skuFeatureTranslation = new Translation();
            skuFeatureTranslation.Language = "de-DE";
            skuFeatureTranslation.Abbreviation = "Artikelnummer";
            skuFeatureTranslation.Description = "Artikelnummer";
            skuFeature.Translations.Add("de-DE", skuFeatureTranslation);

            skuFeatureTranslation = new Translation();
            skuFeatureTranslation.Language = "en-GB";
            skuFeatureTranslation.Abbreviation = "Product Number";
            skuFeatureTranslation.Description = "Product Number";
            skuFeature.Translations.Add("en-GB", skuFeatureTranslation);

            etimDatasource.Features.Add(skuFeature.Code, skuFeature);

            return etimDatasource;
        }

        private static Dictionary<string, EtimUnit> ParseUnits(XmlDocument xmlDoc, XmlNamespaceManager manager)
        {
            Dictionary<string, EtimUnit> units = new Dictionary<string, EtimUnit>();

            XmlNodeList xmlUnits = xmlDoc.DocumentElement.SelectNodes("/ns:IXF/ns:Units/ns:Unit", manager);
            // Console.WriteLine("Units: " + xmlUnits.Count);
                
            foreach (XmlNode xmlUnit in xmlUnits)
            {
                EtimUnit unit = new EtimUnit();

                XmlNode xmlCode = xmlUnit.SelectSingleNode("ns:Code", manager);

                unit.Code = xmlCode.InnerText;
                unit.Translations = ParseTranslations(xmlUnit, manager);

                units.Add(unit.Code, unit);
            }

            return units;
        }

        private static Dictionary<String, EtimFeature> ParseFeatures(XmlDocument xmlDoc, XmlNamespaceManager manager)
        {
            Dictionary<String, EtimFeature> features = new Dictionary<String, EtimFeature>();

            XmlNodeList xmlFeatures = xmlDoc.DocumentElement.SelectNodes("/ns:IXF/ns:Features/ns:Feature", manager);
            // Console.WriteLine("Features: " + xmlFeatures.Count);

            foreach (XmlNode xmlFeature in xmlFeatures)
            {
                EtimFeature feature = new EtimFeature();

                XmlNode xmlCode = xmlFeature.SelectSingleNode("ns:Code", manager);
                XmlNode xmlType = xmlFeature.SelectSingleNode("ns:Type", manager);

                feature.Code = xmlCode.InnerText;
                feature.Type = xmlType.InnerText;
                feature.Translations = ParseTranslations(xmlFeature, manager);

                features.Add(feature.Code, feature);
            }

            return features;
        }

        private static Dictionary<String, EtimValue> ParseValues(XmlDocument xmlDoc, XmlNamespaceManager manager)
        {
            Dictionary<String, EtimValue> values = new Dictionary<String, EtimValue>();

            XmlNodeList xmlValues = xmlDoc.DocumentElement.SelectNodes("/ns:IXF/ns:Values/ns:Value", manager);
            // Console.WriteLine("Values: " + xmlValues.Count);

            foreach (XmlNode xmlValue in xmlValues)
            {
                EtimValue value = new EtimValue();

                XmlNode xmlCode = xmlValue.SelectSingleNode("ns:Code", manager);

                value.Code = xmlCode.InnerText;
                value.Translations = ParseTranslations(xmlValue, manager);

                values.Add(value.Code, value);
            }

            return values;
        }

        private static Dictionary<String, EtimGroup> ParseGroups(XmlDocument xmlDoc, XmlNamespaceManager manager)
        {
            Dictionary<String, EtimGroup> groups = new Dictionary<String, EtimGroup>();

            XmlNodeList xmlGroups = xmlDoc.DocumentElement.SelectNodes("/ns:IXF/ns:Groups/ns:Group", manager);
            // Console.WriteLine("Groups: " + xmlGroups.Count);

            foreach (XmlNode xmlGroup in xmlGroups)
            {
                EtimGroup group = new EtimGroup();

                XmlNode xmlCode = xmlGroup.SelectSingleNode("ns:Code", manager);

                group.Code = xmlCode.InnerText;
                group.Translations = ParseTranslations(xmlGroup, manager);

                groups.Add(group.Code, group);
            }

            return groups;
        }

        private static Dictionary<String, EtimClass> ParseClasses(XmlDocument xmlDoc, XmlNamespaceManager manager, Dictionary<String, EtimGroup> groups, Dictionary<String, EtimFeature> features, Dictionary<String, EtimUnit> units, Dictionary<String, EtimValue> values)
        {
            Dictionary<String, EtimClass> classes = new Dictionary<String, EtimClass>();

            XmlNodeList xmlClasses = xmlDoc.DocumentElement.SelectNodes("/ns:IXF/ns:Classes/ns:Class", manager);
            // Console.WriteLine("Classes: " + xmlClasses.Count);

            foreach (XmlNode xmlClass in xmlClasses)
            {
                EtimClass etimClass = new EtimClass();

                XmlNode xmlCode = xmlClass.SelectSingleNode("ns:Code", manager);
                XmlNode xmlVersion = xmlClass.SelectSingleNode("ns:Version", manager);
                XmlNode xmlStatus = xmlClass.SelectSingleNode("ns:Status", manager);
                XmlNode xmlGroupCode = xmlClass.SelectSingleNode("ns:GroupCode", manager);

                etimClass.Code = xmlCode.InnerText;
                etimClass.Version = xmlVersion.InnerText;
                etimClass.Status = xmlStatus.InnerText;
                etimClass.Group = groups[xmlGroupCode.InnerText];
                etimClass.Translations = ParseTranslations(xmlClass, manager);
                etimClass.Features = ParseClassFeatures(xmlClass, manager, features, units, values);

                classes.Add(etimClass.Code, etimClass);
            }

            return classes;
        }

        private static Dictionary<string, Translation> ParseTranslations(XmlNode rootNode, XmlNamespaceManager manager)
        {
            Dictionary<string, Translation> translations = new Dictionary<string, Translation>();

            XmlNodeList xmlTranslations = rootNode.SelectNodes("ns:Translations/ns:Translation", manager);

            foreach (XmlNode xmlTranslation in xmlTranslations)
            {
                Translation translation = new Translation();

                translation.Language = xmlTranslation.Attributes["language"].InnerText;
                translation.Description = xmlTranslation.SelectSingleNode("ns:Description", manager)?.InnerText;
                translation.Abbreviation = xmlTranslation.SelectSingleNode("ns:Abbreviation", manager)?.InnerText;

                XmlNodeList xmlSynonyms = xmlTranslation.SelectNodes("ns:Synonyms/ns:Synonym", manager);
                foreach (XmlNode xmlSynonym in xmlSynonyms)
                {
                    translation.Synonyms.Add(xmlSynonym.InnerText);
                }

                translations.Add(translation.Language, translation);
            }

            return translations;
        }

        private static List<ClassFeature> ParseClassFeatures(XmlNode rootNode, XmlNamespaceManager manager, Dictionary<String, EtimFeature> features, Dictionary<String, EtimUnit> units, Dictionary<String, EtimValue> values)
        {
            List<ClassFeature> classFeatures = new List<ClassFeature>();

            XmlNodeList xmlClassFeatures = rootNode.SelectNodes("ns:Features/ns:Feature", manager);

            foreach (XmlNode xmlClassFeature in xmlClassFeatures)
            {
                ClassFeature classFeature = new ClassFeature();

                XmlAttribute xmlChangeCode = xmlClassFeature.Attributes["changeCode"];
                XmlNode xmlFeatureCode = xmlClassFeature.SelectSingleNode("ns:FeatureCode", manager);
                XmlNode xmlUnitCode = xmlClassFeature.SelectSingleNode("ns:UnitCode", manager);
                XmlNode xmlOrderNumber = xmlClassFeature.SelectSingleNode("ns:OrderNumber", manager);
                
                classFeature.ChangeCode = xmlChangeCode.InnerText;
                classFeature.Feature = features[xmlFeatureCode.InnerText];
                classFeature.Unit = (xmlUnitCode != null ? units[xmlUnitCode.InnerText] : null);
                classFeature.OrderNumber = Int32.Parse(xmlOrderNumber.InnerText);
                classFeature.Values = ParseClassFeatureValues(xmlClassFeature, manager, values);
                
                classFeatures.Add(classFeature);
            }

            return classFeatures;
        }

        private static List<ClassValue> ParseClassFeatureValues(XmlNode rootNode, XmlNamespaceManager manager, Dictionary<String, EtimValue> values)
        {
            List<ClassValue> classValues = new List<ClassValue>();

            XmlNodeList xmlClassValues = rootNode.SelectNodes("ns:Values/ns:Value", manager);

            foreach (XmlNode xmlClassValue in xmlClassValues)
            {
                ClassValue classValue = new ClassValue();

                XmlAttribute xmlChangeCode = xmlClassValue.Attributes["changeCode"];
                XmlNode xmlValueCode = xmlClassValue.SelectSingleNode("ns:ValueCode", manager);
                XmlNode xmlOrderNumber = xmlClassValue.SelectSingleNode("ns:OrderNumber", manager);

                classValue.ChangeCode = xmlChangeCode.InnerText;
                classValue.Value = values[xmlValueCode.InnerText];
                classValue.OrderNumber = Int32.Parse(xmlOrderNumber.InnerText);

                classValues.Add(classValue);
            }

            return classValues;
        }

        public override string ToString()
        {
            return "Units: " + Units.Count + "\n" +
                    "Features: " + Features.Count + "\n" +
                    "Values: " + Values.Count + "\n" +
                    "Groups:" + Groups.Count + "\n" +
                    "Classes: " + Classes.Count;
        }

    }
}
