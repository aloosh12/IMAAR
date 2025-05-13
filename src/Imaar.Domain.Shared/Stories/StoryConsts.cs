namespace Imaar.Stories
{
    public static class StoryConsts
    {
        private const string DefaultSorting = "{0}Title asc";

        public static string GetDefaultSorting(bool withEntityName)
        {
            return string.Format(DefaultSorting, withEntityName ? "Story." : string.Empty);
        }

    }
}