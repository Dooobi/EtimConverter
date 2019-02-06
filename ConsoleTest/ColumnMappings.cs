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
            columns.Add(new Column("XTSOL", (groupId, productGroup, product) => "XTSOL"));
            columns.Add(new Column("p_id", (groupId, productGroup, product) => Convert.ToString(groupId)));
            columns.Add(new Column("p_model", (groupId, productGroup, product) =>
            {
                //if (productGroup.Value.Count <= 1)
                //{
                //    return product.SupplierPid;
                //}
                return "";
            }));
            //columns.Add(new Column("p_model", (groupId, productGroup, product) => product.ShortestKeyword));
            columns.Add(new Column("p_stock", (groupId, productGroup, product) => "9999"));
            columns.Add(new Column("p_sorting", (groupId, productGroup, product) => "0"));
            columns.Add(new Column("p_startpage", (groupId, productGroup, product) => "0"));
            columns.Add(new Column("p_startpage_sort", (groupId, productGroup, product) => "0"));
            columns.Add(new Column("p_shipping", (groupId, productGroup, product) => "1"));
            columns.Add(new Column("p_tpl", (groupId, productGroup, product) => "default"));
            columns.Add(new Column("p_opttpl", (groupId, productGroup, product) => "product_options_dropdown.html"));
            columns.Add(new Column("p_manufacturer", (groupId, productGroup, product) => ""));
            columns.Add(new Column("p_fsk18", (groupId, productGroup, product) => "0"));
            //columns.Add(new Column("p_priceNoTax", (groupId, productGroup, product) => ""));
            columns.Add(new Column("p_priceNoTax", (groupId, productGroup, product) =>
            {
                //if (productGroup.Value.Count <= 1)
                //{
                //    if (product.Prices.ContainsKey("net_list"))
                //    {
                //        return product.Prices["net_list"].PriceAmount;
                //    }
                //    return "9999.99";
                //}
                return "";
            }));
            columns.Add(new Column("p_priceNoTax.1", (groupId, productGroup, product) => ""));
            columns.Add(new Column("p_priceNoTax.2", (groupId, productGroup, product) => ""));
            columns.Add(new Column("p_priceNoTax.3", (groupId, productGroup, product) => ""));
            columns.Add(new Column("p_tax", (groupId, productGroup, product) => "0"));
            columns.Add(new Column("p_status", (groupId, productGroup, product) => "1"));
            columns.Add(new Column("p_weight", (groupId, productGroup, product) => "0"));
            columns.Add(new Column("p_ean", (groupId, productGroup, product) => ""));
            //columns.Add(new Column("p_ean", (groupId, productGroup, product) => product.InternationalPid));
            columns.Add(new Column("code_isbn", (groupId, productGroup, product) => ""));
            columns.Add(new Column("code_upc", (groupId, productGroup, product) => ""));
            columns.Add(new Column("code_mpn", (groupId, productGroup, product) => ""));
            columns.Add(new Column("code_jan", (groupId, productGroup, product) => ""));
            columns.Add(new Column("brand_name", (groupId, productGroup, product) => product.ManufacturerName));
            columns.Add(new Column("p_disc", (groupId, productGroup, product) => "0.00"));
            //columns.Add(new Column("p_date_added", (groupId, productGroup, product) => "0000-00-00 00:00:00"));
            //columns.Add(new Column("p_last_modified", (groupId, productGroup, product) => "0000-00-00 00:00:00"));
            //columns.Add(new Column("p_date_available", (groupId, productGroup, product) => "0000-00-00 00:00:00"));
            columns.Add(new Column("p_date_added", (groupId, productGroup, product) => ""));
            columns.Add(new Column("p_last_modified", (groupId, productGroup, product) => ""));
            columns.Add(new Column("p_date_available", (groupId, productGroup, product) => ""));
            columns.Add(new Column("p_ordered", (groupId, productGroup, product) => ""));
            columns.Add(new Column("nc_ultra_shipping_costs", (groupId, productGroup, product) => ""));
            columns.Add(new Column("gm_show_date_added", (groupId, productGroup, product) => "0"));
            columns.Add(new Column("gm_show_price_offer", (groupId, productGroup, product) => "0"));
            columns.Add(new Column("gm_show_weight", (groupId, productGroup, product) => "0"));
            columns.Add(new Column("gm_show_qty_info", (groupId, productGroup, product) => "0"));
            columns.Add(new Column("gm_price_status", (groupId, productGroup, product) => "0"));
            columns.Add(new Column("gm_min_order", (groupId, productGroup, product) => "1"));
            columns.Add(new Column("gm_graduated_qty", (groupId, productGroup, product) => ""));
            columns.Add(new Column("gm_options_template", (groupId, productGroup, product) => "default"));
            columns.Add(new Column("p_vpe", (groupId, productGroup, product) => "0"));
            columns.Add(new Column("p_vpe_status", (groupId, productGroup, product) => "0"));
            columns.Add(new Column("p_vpe_value", (groupId, productGroup, product) => "0"));
            //columns.Add(new Column("p_image.1", (groupId, productGroup, product) => ""));
            //columns.Add(new Column("p_image.2", (groupId, productGroup, product) => ""));
            //columns.Add(new Column("p_image.3", (groupId, productGroup, product) => ""));
            //columns.Add(new Column("p_image", (groupId, productGroup, product) => ""));
            columns.Add(new Column("p_image.1", (groupId, productGroup, product) =>
            {
                if (product.Mimes.ContainsKey("image/jpeg")
                    && product.Mimes["image/jpeg"].Count > 1)
                {
                    return product.Mimes["image/jpeg"][1].Source;
                }
                return "";
            }));
            columns.Add(new Column("p_image.2", (groupId, productGroup, product) =>
            {
                if (product.Mimes.ContainsKey("image/jpeg")
                    && product.Mimes["image/jpeg"].Count > 2)
                {
                    return product.Mimes["image/jpeg"][2].Source;
                }
                return "";
            }));
            columns.Add(new Column("p_image.3", (groupId, productGroup, product) =>
            {
                if (productGroup.Value.Count <= 1)
                {
                    return product.ManufacturerPid.Replace("/", "_") + "_EL.png";
                }
                if (product.Mimes.ContainsKey("image/jpeg")
                    && product.Mimes["image/jpeg"].Count > 3)
                {
                    return product.Mimes["image/jpeg"][3].Source;
                }
                return "";
            }));
            columns.Add(new Column("p_image", (groupId, productGroup, product) =>
            {
                if (product.Mimes.ContainsKey("image/jpeg")
                    && product.Mimes["image/jpeg"].Count > 0)
                {
                    return product.Mimes["image/jpeg"][0].Source;
                }
                return "";
            }));
            columns.Add(new Column("p_name.en", (groupId, productGroup, product) => product.DescriptionShort));
            columns.Add(new Column("p_desc.en", (groupId, productGroup, product) => product.GetFullDescription()));
            columns.Add(new Column("p_shortdesc.en", (groupId, productGroup, product) => ""));
            columns.Add(new Column("p_checkout_information.en", (groupId, productGroup, product) => ""));
            columns.Add(new Column("p_meta_title.en", (groupId, productGroup, product) => ""));
            columns.Add(new Column("p_meta_desc.en", (groupId, productGroup, product) => ""));
            columns.Add(new Column("p_meta_key.en", (groupId, productGroup, product) => ""));
            columns.Add(new Column("p_meta_desc.de", (groupId, productGroup, product) => ""));
            columns.Add(new Column("p_keywords.en", (groupId, productGroup, product) =>
            {
                StringBuilder sb = new StringBuilder();
                sb.Append(product.DescriptionShort).Append(" ");
                product.Keywords.ForEach(keyword => sb.Append(keyword).Append(" "));
                sb.Append(product.Category);
                return sb.ToString();
            }));
            //columns.Add(new Column("p_url.en", (groupId, productGroup, product) => ""));
            columns.Add(new Column("p_url.en", (groupId, productGroup, product) =>
            {
                string url = product.GetUrl();
                if (url != null)
                {
                    return url;
                }
                return "";
            }));
            columns.Add(new Column("gm_url_keywords.en", (groupId, productGroup, product) => ""));
            columns.Add(new Column("rewrite_url.en", (groupId, productGroup, product) => ""));
            columns.Add(new Column("p_name.de", (groupId, productGroup, product) => product.DescriptionShort));
            columns.Add(new Column("p_desc.de", (groupId, productGroup, product) => product.GetFullDescription()));
            columns.Add(new Column("p_shortdesc.de", (groupId, productGroup, product) => ""));
            columns.Add(new Column("p_checkout_information.de", (groupId, productGroup, product) => ""));
            columns.Add(new Column("p_meta_title.de", (groupId, productGroup, product) => ""));
            columns.Add(new Column("p_meta_key.de", (groupId, productGroup, product) => ""));
            columns.Add(new Column("p_keywords.de", (groupId, productGroup, product) =>
            {
                StringBuilder sb = new StringBuilder();
                sb.Append(product.DescriptionShort).Append(" ");
                product.Keywords.ForEach(keyword => sb.Append(keyword).Append(" "));
                sb.Append(product.Category);
                return sb.ToString();
            }));
            //columns.Add(new Column("p_url.de", (groupId, productGroup, product) => ""));
            columns.Add(new Column("p_url.de", (groupId, productGroup, product) =>
            {
                string url = product.GetUrl();
                if (url != null)
                {
                    return url;
                }
                return "";
            }));
            columns.Add(new Column("gm_url_keywords.de", (groupId, productGroup, product) => ""));
            columns.Add(new Column("rewrite_url.de", (groupId, productGroup, product) => ""));
            //columns.Add(new Column("p_cat.en", (groupId, productGroup, product) => product.ReferenceFeatureGroup.Group.Translations["en-GB"].Description));
            //columns.Add(new Column("p_cat.de", (groupId, productGroup, product) => product.ReferenceFeatureGroup.Group.Translations["de-DE"].Description));
            columns.Add(new Column("p_cat.en", (groupId, productGroup, product) =>
            {
                if (product.Category != null)
                {
                    return product.Category.GetCategoryPath();
                }
                return "";
            }));
            columns.Add(new Column("p_cat.de", (groupId, productGroup, product) =>
            {
                if (product.Category != null)
                {
                    return product.Category.GetCategoryPath();
                }
                return "";
            }));
            //columns.Add(new Column("google_export_availability", (groupId, productGroup, product) => "auf lager"));
            columns.Add(new Column("google_export_availability", (groupId, productGroup, product) => ""));
            //columns.Add(new Column("google_export_condition", (groupId, productGroup, product) => "neu"));
            columns.Add(new Column("google_export_condition", (groupId, productGroup, product) => ""));
            columns.Add(new Column("google_category", (groupId, productGroup, product) => ""));
            columns.Add(new Column("p_img_alt_text.en", (groupId, productGroup, product) => ""));
            columns.Add(new Column("p_img_alt_text.1.en", (groupId, productGroup, product) => ""));
            columns.Add(new Column("p_img_alt_text.2.en", (groupId, productGroup, product) => ""));
            columns.Add(new Column("p_img_alt_text.3.en", (groupId, productGroup, product) => ""));
            columns.Add(new Column("p_img_alt_text.de", (groupId, productGroup, product) => ""));
            columns.Add(new Column("p_img_alt_text.1.de", (groupId, productGroup, product) => ""));
            columns.Add(new Column("p_img_alt_text.2.de", (groupId, productGroup, product) => ""));
            columns.Add(new Column("p_img_alt_text.3.de", (groupId, productGroup, product) => ""));
            columns.Add(new Column("p_group_permission.0", (groupId, productGroup, product) => "1"));
            columns.Add(new Column("p_group_permission.1", (groupId, productGroup, product) => "1"));
            columns.Add(new Column("p_group_permission.2", (groupId, productGroup, product) => "1"));
            columns.Add(new Column("p_group_permission.3", (groupId, productGroup, product) => "1"));
            columns.Add(new Column("specials_qty", (groupId, productGroup, product) => "0"));
            columns.Add(new Column("specials_new_products_price", (groupId, productGroup, product) => ""));
            columns.Add(new Column("expires_date", (groupId, productGroup, product) => ""));
            columns.Add(new Column("specials_status", (groupId, productGroup, product) => "0"));
            columns.Add(new Column("gm_priority", (groupId, productGroup, product) => "0"));
            columns.Add(new Column("gm_changefreq", (groupId, productGroup, product) => "always"));
            columns.Add(new Column("gm_sitemap_entry", (groupId, productGroup, product) => "0"));
            columns.Add(new Column("p_qty_unit_id", (groupId, productGroup, product) => ""));
            columns.Add(new Column("p_type", (groupId, productGroup, product) => "1"));
            columns.Add(new Column("products_properties_combis_id", (groupId, productGroup, product) => Convert.ToString(Settings.nextPropertiesId++)));
            columns.Add(new Column("combi_sort_order", (groupId, productGroup, product) => ""));
            columns.Add(new Column("combi_model", (groupId, productGroup, product) => product.SupplierPid));
            columns.Add(new Column("combi_ean", (groupId, productGroup, product) => ""));
            columns.Add(new Column("combi_quantity", (groupId, productGroup, product) => "9999"));
            columns.Add(new Column("combi_shipping_status_id", (groupId, productGroup, product) => "1"));
            columns.Add(new Column("combi_weight", (groupId, productGroup, product) => ""));
            columns.Add(new Column("combi_price", (groupId, productGroup, product) =>
            {
                if (product.Prices.ContainsKey("net_list"))
                {
                    return product.Prices["net_list"].PriceAmount;
                }
                return "9999.99";
            }));
            columns.Add(new Column("combi_price_type", (groupId, productGroup, product) => "fix"));
            columns.Add(new Column("combi_image", (groupId, productGroup, product) => product.ManufacturerPid.Replace("/", "_") + "_EL.png"));
            columns.Add(new Column("combi_vpe_id", (groupId, productGroup, product) => "0"));
            columns.Add(new Column("combi_vpe_value", (groupId, productGroup, product) => "0"));

            return columns;
        }
    }
}
