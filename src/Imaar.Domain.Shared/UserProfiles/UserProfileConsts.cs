namespace Imaar.UserProfiles
{
    public static class UserProfileConsts
    {
        private const string DefaultSorting = "{0}SecurityNumber asc";

        public static string GetDefaultSorting(bool withEntityName)
        {
            return string.Format(DefaultSorting, withEntityName ? "UserProfile." : string.Empty);
        }

    }
}