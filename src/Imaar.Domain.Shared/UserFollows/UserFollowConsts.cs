namespace Imaar.UserFollows
{
    public static class UserFollowConsts
    {
        private const string DefaultSorting = "{0}Id asc";

        public static string GetDefaultSorting(bool withEntityName)
        {
            return string.Format(DefaultSorting, withEntityName ? "UserFollow." : string.Empty);
        }

    }
}