namespace CategoriesDatasourceReader
{
    public class Category
    {
        public string MainCategory { get; set; }
        public string SubCategory { get; set; }

        public string GetCategoryPath()
        {
            if (MainCategory != null && MainCategory.Length > 0)
            {
                if (SubCategory != null && SubCategory.Length > 0)
                {
                    return MainCategory + " > " + SubCategory;
                }
                else
                {
                    return MainCategory;
                }
            }

            if (SubCategory != null && SubCategory.Length > 0)
            {
                return SubCategory;
            }
            
            return "";
        }
    }
}