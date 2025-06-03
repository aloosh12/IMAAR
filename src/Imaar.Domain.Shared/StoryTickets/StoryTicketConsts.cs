namespace Imaar.StoryTickets
{
    public static class StoryTicketConsts
    {
        private const string DefaultSorting = "{0}Description asc";

        public static string GetDefaultSorting(bool withEntityName)
        {
            return string.Format(DefaultSorting, withEntityName ? "StoryTicket." : string.Empty);
        }

    }
}