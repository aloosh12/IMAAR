namespace Imaar.UserSavedItems
{
    public static class UserSavedItemConsts
    {
        private const string DefaultSorting = "{0}SourceId asc";

        public static string GetDefaultSorting(bool withEntityName)
        {
            return string.Format(DefaultSorting, withEntityName ? "UserSavedItem." : string.Empty);
        }

    }
}