namespace Imaar.UserWorksExhibitions
{
    public static class UserWorksExhibitionConsts
    {
        private const string DefaultSorting = "{0}Title asc";

        public static string GetDefaultSorting(bool withEntityName)
        {
            return string.Format(DefaultSorting, withEntityName ? "UserWorksExhibition." : string.Empty);
        }

    }
}