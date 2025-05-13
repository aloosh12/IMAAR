namespace Imaar.StoryLovers
{
    public static class StoryLoverConsts
    {
        private const string DefaultSorting = "{0}Id asc";

        public static string GetDefaultSorting(bool withEntityName)
        {
            return string.Format(DefaultSorting, withEntityName ? "StoryLover." : string.Empty);
        }

    }
}