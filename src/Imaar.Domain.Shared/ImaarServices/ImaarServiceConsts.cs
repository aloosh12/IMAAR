namespace Imaar.ImaarServices
{
    public static class ImaarServiceConsts
    {
        private const string DefaultSorting = "{0}Title asc";

        public static string GetDefaultSorting(bool withEntityName)
        {
            return string.Format(DefaultSorting, withEntityName ? "ImaarService." : string.Empty);
        }

    }
}