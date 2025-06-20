namespace Imaar.SecondaryAmenities
{
    public static class SecondaryAmenityConsts
    {
        private const string DefaultSorting = "{0}Name asc";

        public static string GetDefaultSorting(bool withEntityName)
        {
            return string.Format(DefaultSorting, withEntityName ? "SecondaryAmenity." : string.Empty);
        }

    }
}