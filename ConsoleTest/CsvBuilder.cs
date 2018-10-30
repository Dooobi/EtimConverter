using BmecatDatasourceReader.Model;
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

        public string Build()
        {
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
