namespace Imaar.StoryTicketTypes
{
    public static class StoryTicketTypeConsts
    {
        private const string DefaultSorting = "{0}Title asc";

        public static string GetDefaultSorting(bool withEntityName)
        {
            return string.Format(DefaultSorting, withEntityName ? "StoryTicketType." : string.Empty);
        }

    }
}