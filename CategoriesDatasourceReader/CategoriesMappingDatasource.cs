using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CategoriesDatasourceReader
{
    public class CategoriesMappingDatasource
    {
        public Dictionary<string, Category> CategoriesMapping { get; set; }

        public CategoriesMappingDatasource()
        {
            CategoriesMapping = new Dictionary<string, Category>();
        }

        public class CategoriesMappingParser
        {
            //public const string COLUMN_HEADER_ARTICLE_NUMBER = "Artikelnummer";
            public const string COLUMN_HEADER_MAIN_CATEGORY = "Hauptkategorie";
            public const string COLUMN_HEADER_SUB_CATEGORY = "Unterkategorie";
            public const string COLUMN_HEADER_NAME = "Name";

            //public int IndexArticleNumber { get; set; }
            public int IndexMainCategory { get; set; }
            public int IndexSubCategory { get; set; }
            public int IndexName { get; set; }

            public char Separator { get; set; }

            public CategoriesMappingDatasource CategoriesMappingDatasource { get; set; }

            public CategoriesMappingParser(char separator)
            {
                CategoriesMappingDatasource = new CategoriesMappingDatasource();

                Separator = separator;
            }

            public CategoriesMappingDatasource Parse(string filepath)
            {
                string line;
                string[] lineParts;

                StreamReader file = new StreamReader(filepath);

                int i = 0;
                while ((line = file.ReadLine()) != null)
                {
                    lineParts = line.Split(Separator);

                    if (i == 0)
                    {
                        ParseColumnHeaders(lineParts);
                    }
                    else
                    {
                        ParseDataLine(lineParts);
                    }

                    i++;
                }

                return CategoriesMappingDatasource;
            }

            public void ParseColumnHeaders(string[] lineParts)
            {
                for (int i = 0; i < lineParts.Length; i++)
                {
                    switch (lineParts[i])
                    {
                        //case COLUMN_HEADER_ARTICLE_NUMBER:
                        //    IndexArticleNumber = i;
                        //    break;
                        case COLUMN_HEADER_NAME:
                            IndexName = i;
                            break;
                        case COLUMN_HEADER_MAIN_CATEGORY:
                            IndexMainCategory = i;
                            break;
                        case COLUMN_HEADER_SUB_CATEGORY:
                            IndexSubCategory = i;
                            break;
                    }
                }
            }

            public void ParseDataLine(string[] lineParts)
            {
                Category category = new Category();
                category.MainCategory = lineParts[IndexMainCategory].Replace("-", " ").Trim();
                category.SubCategory = lineParts[IndexSubCategory].Replace("-", " ").Trim();
                //string articleNumber = lineParts[IndexArticleNumber].Trim();
                string name = lineParts[IndexName].Trim();
                
                if (!CategoriesMappingDatasource.CategoriesMapping.ContainsKey(name))
                {
                    CategoriesMappingDatasource.CategoriesMapping[name] = category;
                }
            }
        }
    }
}
