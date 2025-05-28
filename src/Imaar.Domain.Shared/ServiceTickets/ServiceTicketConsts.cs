namespace Imaar.ServiceTickets
{
    public static class ServiceTicketConsts
    {
        private const string DefaultSorting = "{0}Description  asc";

        public static string GetDefaultSorting(bool withEntityName)
        {
            return string.Format(DefaultSorting, withEntityName ? "ServiceTicket." : string.Empty);
        }

    }
}