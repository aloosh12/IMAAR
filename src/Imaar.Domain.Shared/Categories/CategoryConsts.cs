namespace Imaar.Categories
{
    public static class CategoryConsts
    {
        private const string DefaultSorting = "{0}Order asc";

        public static string GetDefaultSorting(bool withEntityName)
        {
            return string.Format(DefaultSorting, withEntityName ? "Category." : string.Empty);
        }

    }
}