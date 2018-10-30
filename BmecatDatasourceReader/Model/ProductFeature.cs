using EtimDatasourceReader;
using System;
using System.Text;

namespace BmecatDatasourceReader.Model
{
    public class ProductFeature
    {
        public EtimFeature EtimFeature { get; set; }
        public string RawValue1 { get; set; }
        public string RawValue2 { get; set; }
        public EtimValue Value1 { get; set; }
        public EtimValue Value2 { get; set; }
        public EtimUnit Unit { get; set; }
        public string ValueDetails { get; set; }

        public string ToPropertyValue()
        {
            string value1 = GetActualValueForLanguage(RawValue1, Value1, "de-DE");
            string value2 = GetActualValueForLanguage(RawValue2, Value2, "de-DE");

            StringBuilder bld = new StringBuilder();
            bld.Append(FormatValue(value1, EtimFeature));
            if (Value2 != null)
            {
                bld.Append("-").Append(FormatValue(value2, EtimFeature));
            }
            if (Unit != null)
            {
                bld.Append(" ").Append(Unit.Translations["de-DE"].Description);
            }
            return bld.ToString();
        }

        private string GetActualValueForLanguage(string rawValue, EtimValue value, string language)
        {
            if (value == null)
            {
                return rawValue;
            }
            return value.Translations[language].Description;
        }

        private string FormatValue(string value, EtimFeature etimFeature)
        {
            switch (etimFeature.Type)
            {
                case EtimFeature.TYPE_LOGICAL:
                    if (value == "true")
                    {
                        return "Ja";
                    }
                    else
                    {
                        return "Nein";
                    }
            }
            return value;
        }

        public override bool Equals(object obj)
        {
            try
            {
                ProductFeature other = (ProductFeature)obj;

                return ToPropertyValue() == other.ToPropertyValue();
            }
            catch (Exception)
            {
                return false;
            }
        }

        //public override bool Equals(object obj)
        //{
        //    try
        //    {
        //        ProductFeature other = (ProductFeature)obj;
        //        return Value1 == other.Value1
        //            && Value2 == other.Value2
        //            && ValueDetails == other.ValueDetails
        //            && EtimFeature.Code == other.EtimFeature.Code
        //            && Unit.Code == other.Unit.Code;

        //    } catch (Exception)
        //    {
        //        return false;
        //    }
        //}
    }
}
