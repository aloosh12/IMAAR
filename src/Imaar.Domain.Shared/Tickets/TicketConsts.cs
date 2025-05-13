namespace Imaar.Tickets
{
    public static class TicketConsts
    {
        private const string DefaultSorting = "{0}Description asc";

        public static string GetDefaultSorting(bool withEntityName)
        {
            return string.Format(DefaultSorting, withEntityName ? "Ticket." : string.Empty);
        }

    }
}