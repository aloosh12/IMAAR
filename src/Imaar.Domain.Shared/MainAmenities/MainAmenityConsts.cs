namespace Imaar.MainAmenities
{
    public static class MainAmenityConsts
    {
        private const string DefaultSorting = "{0}Name asc";

        public static string GetDefaultSorting(bool withEntityName)
        {
            return string.Format(DefaultSorting, withEntityName ? "MainAmenity." : string.Empty);
        }

    }
}