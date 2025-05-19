namespace Imaar.UserEvalauations
{
    public static class UserEvalauationConsts
    {
        private const string DefaultSorting = "{0}SpeedOfCompletion asc";

        public static string GetDefaultSorting(bool withEntityName)
        {
            return string.Format(DefaultSorting, withEntityName ? "UserEvalauation." : string.Empty);
        }

    }
}