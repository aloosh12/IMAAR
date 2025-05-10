namespace Imaar.TicketTypes
{
    public static class TicketTypeConsts
    {
        private const string DefaultSorting = "{0}Title asc";

        public static string GetDefaultSorting(bool withEntityName)
        {
            return string.Format(DefaultSorting, withEntityName ? "TicketType." : string.Empty);
        }

    }
}