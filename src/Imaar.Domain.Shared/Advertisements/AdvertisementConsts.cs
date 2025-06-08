namespace Imaar.Advertisements
{
    public static class AdvertisementConsts
    {
        private const string DefaultSorting = "{0}Order asc";

        public static string GetDefaultSorting(bool withEntityName)
        {
            return string.Format(DefaultSorting, withEntityName ? "Advertisement." : string.Empty);
        }

    }
}