namespace Imaar.Notifications
{
    public static class NotificationConsts
    {
        private const string DefaultSorting = "{0}Title asc";

        public static string GetDefaultSorting(bool withEntityName)
        {
            return string.Format(DefaultSorting, withEntityName ? "Notification." : string.Empty);
        }

    }
}