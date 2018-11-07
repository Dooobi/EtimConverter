using BmecatDatasourceReader.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleTest
{
    public class ColumnMappings
    {
        public static List<Column> GetColumns()
        {
            List<Column> columns = new List<Column>();
            columns.Add(new Column("XTSOL", (product) => "XTSOL"));
            columns.Add(new Column("p_id", (product) => ""));
            columns.Add(new Column("p_model", (product) => product.ShortestKeyword));
            columns.Add(new Column("p_stock", (product) => "1"));
            columns.Add(new Column("p_sorting", (product) => "0"));
            columns.Add(new Column("p_startpage", (product) => "0"));
            columns.Add(new Column("p_startpage_sort", (product) => "0"));
            columns.Add(new Column("p_shipping", (product) => "1"));
            columns.Add(new Column("p_tpl", (product) => "default"));
            columns.Add(new Column("p_opttpl", (product) => "product_options_dropdown.html"));
            columns.Add(new Column("p_manufacturer", (product) => ""));
            columns.Add(new Column("p_fsk18", (product) => "0"));
            columns.Add(new Column("p_priceNoTax", (product) => ""));
            //columns.Add(new Column("p_priceNoTax", (product) => 
            //{
            //    if (product.Prices.ContainsKey("net_list"))
            //    {
            //        return product.Prices["net_list"].PriceAmount;
            //    }
            //    return "9999.99";
            //}));
            columns.Add(new Column("p_priceNoTax.1", (product) => ""));
            columns.Add(new Column("p_priceNoTax.2", (product) => ""));
            columns.Add(new Column("p_priceNoTax.3", (product) => ""));
            columns.Add(new Column("p_tax", (product) => "0"));
            columns.Add(new Column("p_status", (product) => "1"));
            columns.Add(new Column("p_weight", (product) => "0"));
            columns.Add(new Column("p_ean", (product) => ""));
            //columns.Add(new Column("p_ean", (product) => product.InternationalPid));
            columns.Add(new Column("code_isbn", (product) => ""));
            columns.Add(new Column("code_upc", (product) => ""));
            columns.Add(new Column("code_mpn", (product) => ""));
            columns.Add(new Column("code_jan", (product) => ""));
            columns.Add(new Column("brand_name", (product) => product.ManufacturerName));
            columns.Add(new Column("p_disc", (product) => "0.00"));
            //columns.Add(new Column("p_date_added", (product) => "0000-00-00 00:00:00"));
            //columns.Add(new Column("p_last_modified", (product) => "0000-00-00 00:00:00"));
            //columns.Add(new Column("p_date_available", (product) => "0000-00-00 00:00:00"));
            columns.Add(new Column("p_date_added", (product) => ""));
            columns.Add(new Column("p_last_modified", (product) => ""));
            columns.Add(new Column("p_date_available", (product) => ""));
            columns.Add(new Column("p_ordered", (product) => ""));
            columns.Add(new Column("nc_ultra_shipping_costs", (product) => ""));
            columns.Add(new Column("gm_show_date_added", (product) => "0"));
            columns.Add(new Column("gm_show_price_offer", (product) => "0"));
            columns.Add(new Column("gm_show_weight", (product) => "0"));
            columns.Add(new Column("gm_show_qty_info", (product) => "0"));
            columns.Add(new Column("gm_price_status", (product) => "0"));
            columns.Add(new Column("gm_min_order", (product) => "1"));
            columns.Add(new Column("gm_graduated_qty", (product) => ""));
            columns.Add(new Column("gm_options_template", (product) => "default"));
            columns.Add(new Column("p_vpe", (product) => "0"));
            columns.Add(new Column("p_vpe_status", (product) => "0"));
            columns.Add(new Column("p_vpe_value", (product) => "0"));
            columns.Add(new Column("p_image.1", (product) => ""));
            columns.Add(new Column("p_image.2", (product) => ""));
            columns.Add(new Column("p_image.3", (product) => ""));
            columns.Add(new Column("p_image", (product) => ""));
            //columns.Add(new Column("p_image.1", (product) =>
            //{
            //    if (product.Mimes.ContainsKey("image/jpeg")
            //        && product.Mimes["image/jpeg"].Count > 1)
            //    {
            //        return product.Mimes["image/jpeg"][1].Source;
            //    }
            //    return "";
            //}));
            //columns.Add(new Column("p_image.2", (product) =>
            //{
            //    if (product.Mimes.ContainsKey("image/jpeg")
            //        && product.Mimes["image/jpeg"].Count > 2)
            //    {
            //        return product.Mimes["image/jpeg"][2].Source;
            //    }
            //    return "";
            //}));
            //columns.Add(new Column("p_image.3", (product) =>
            //{
            //    if (product.Mimes.ContainsKey("image/jpeg")
            //        && product.Mimes["image/jpeg"].Count > 3)
            //    {
            //        return product.Mimes["image/jpeg"][3].Source;
            //    }
            //    return "";
            //}));
            //columns.Add(new Column("p_image", (product) =>
            //{
            //    if (product.Mimes.ContainsKey("image/jpeg")
            //        && product.Mimes["image/jpeg"].Count > 0)
            //    {
            //        return product.Mimes["image/jpeg"][0].Source;
            //    }
            //    return "";
            //}));
            columns.Add(new Column("p_name.en", (product) => product.DescriptionShort));
            columns.Add(new Column("p_desc.en", (product) => product.DescriptionLong));
            columns.Add(new Column("p_shortdesc.en", (product) => ""));
            columns.Add(new Column("p_checkout_information.en", (product) => ""));
            columns.Add(new Column("p_meta_title.en", (product) => ""));
            columns.Add(new Column("p_meta_desc.en", (product) => ""));
            columns.Add(new Column("p_meta_key.en", (product) => ""));
            columns.Add(new Column("p_meta_desc.de", (product) => ""));
            columns.Add(new Column("p_keywords.en", (product) => product.ReferenceFeatureGroup.Group.Translations["en-GB"].Description));
            //columns.Add(new Column("p_url.en", (product) => ""));
            columns.Add(new Column("p_url.en", (product) =>
            {
                if (product.Mimes.ContainsKey("url")
                    && product.Mimes["url"].Count > 0)
                {
                    ProductMime productMime = product.Mimes["url"].Find((mime) => mime.Description == "MD04");
                    if (productMime != null)
                    {
                        return productMime.Source;
                    }
                }
                return "";
            }));
            columns.Add(new Column("gm_url_keywords.en", (product) => ""));
            columns.Add(new Column("rewrite_url.en", (product) => ""));
            columns.Add(new Column("p_name.de", (product) => product.DescriptionShort));
            columns.Add(new Column("p_desc.de", (product) => product.DescriptionLong));
            columns.Add(new Column("p_shortdesc.de", (product) => ""));
            columns.Add(new Column("p_checkout_information.de", (product) => ""));
            columns.Add(new Column("p_meta_title.de", (product) => ""));
            columns.Add(new Column("p_meta_key.de", (product) => ""));
            columns.Add(new Column("p_keywords.de", (product) => product.ReferenceFeatureGroup.Group.Translations["de-DE"].Description));
            //columns.Add(new Column("p_url.de", (product) => ""));
            columns.Add(new Column("p_url.de", (product) =>
            {
                if (product.Mimes.ContainsKey("url")
                    && product.Mimes["url"].Count > 0)
                {
                    ProductMime productMime = product.Mimes["url"].Find((mime) => mime.Description == "MD04");
                    if (productMime != null)
                    {
                        return productMime.Source;
                    }
                }
                return "";
            }));
            columns.Add(new Column("gm_url_keywords.de", (product) => ""));
            columns.Add(new Column("rewrite_url.de", (product) => ""));
            columns.Add(new Column("p_cat.en", (product) => product.ReferenceFeatureGroup.Group.Translations["en-GB"].Description));
            columns.Add(new Column("p_cat.de", (product) => product.ReferenceFeatureGroup.Group.Translations["de-DE"].Description));
            //columns.Add(new Column("google_export_availability", (product) => "auf lager"));
            columns.Add(new Column("google_export_availability", (product) => ""));
            //columns.Add(new Column("google_export_condition", (product) => "neu"));
            columns.Add(new Column("google_export_condition", (product) => ""));
            columns.Add(new Column("google_category", (product) => ""));
            columns.Add(new Column("p_img_alt_text.en", (product) => ""));
            columns.Add(new Column("p_img_alt_text.1.en", (product) => ""));
            columns.Add(new Column("p_img_alt_text.2.en", (product) => ""));
            columns.Add(new Column("p_img_alt_text.3.en", (product) => ""));
            columns.Add(new Column("p_img_alt_text.de", (product) => ""));
            columns.Add(new Column("p_img_alt_text.1.de", (product) => ""));
            columns.Add(new Column("p_img_alt_text.2.de", (product) => ""));
            columns.Add(new Column("p_img_alt_text.3.de", (product) => ""));
            columns.Add(new Column("p_group_permission.0", (product) => "1"));
            columns.Add(new Column("p_group_permission.1", (product) => "1"));
            columns.Add(new Column("p_group_permission.2", (product) => "1"));
            columns.Add(new Column("p_group_permission.3", (product) => "1"));
            columns.Add(new Column("specials_qty", (product) => "0"));
            columns.Add(new Column("specials_new_products_price", (product) => ""));
            columns.Add(new Column("expires_date", (product) => ""));
            columns.Add(new Column("specials_status", (product) => "0"));
            columns.Add(new Column("gm_priority", (product) => "0"));
            columns.Add(new Column("gm_changefreq", (product) => "always"));
            columns.Add(new Column("gm_sitemap_entry", (product) => "0"));
            columns.Add(new Column("p_qty_unit_id", (product) => ""));
            columns.Add(new Column("p_type", (product) => "1"));
            columns.Add(new Column("products_properties_combis_id", (product) => Convert.ToString(Settings.nextPropertiesId++)));
            columns.Add(new Column("combi_sort_order", (product) => ""));
            columns.Add(new Column("combi_model", (product) => product.SupplierPid));
            columns.Add(new Column("combi_ean", (product) => ""));
            columns.Add(new Column("combi_quantity", (product) => "1"));
            columns.Add(new Column("combi_shipping_status_id", (product) => "1"));
            columns.Add(new Column("combi_weight", (product) => ""));
            columns.Add(new Column("combi_price", (product) =>
            {
                if (product.Prices.ContainsKey("net_list"))
                {
                    return product.Prices["net_list"].PriceAmount;
                }
                return "9999.99";
            }));
            columns.Add(new Column("combi_price_type", (product) => "fix"));
            columns.Add(new Column("combi_image", (product) => ""));
            columns.Add(new Column("combi_vpe_id", (product) => "0"));
            columns.Add(new Column("combi_vpe_value", (product) => "0"));

            return columns;
        }
    }
}
