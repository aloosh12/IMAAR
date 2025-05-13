namespace Imaar.Medias
{
    public static class MediaConsts
    {
        private const string DefaultSorting = "{0}Title asc";

        public static string GetDefaultSorting(bool withEntityName)
        {
            return string.Format(DefaultSorting, withEntityName ? "Media." : string.Empty);
        }

    }
}