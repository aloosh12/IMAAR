namespace Imaar.Evalauations
{
    public static class EvalauationConsts
    {
        private const string DefaultSorting = "{0}SpeedOfCompletion asc";

        public static string GetDefaultSorting(bool withEntityName)
        {
            return string.Format(DefaultSorting, withEntityName ? "Evalauation." : string.Empty);
        }

    }
}