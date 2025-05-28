namespace Imaar.ServiceTicketTypes
{
    public static class ServiceTicketTypeConsts
    {
        private const string DefaultSorting = "{0}Title asc";

        public static string GetDefaultSorting(bool withEntityName)
        {
            return string.Format(DefaultSorting, withEntityName ? "ServiceTicketType." : string.Empty);
        }

    }
}